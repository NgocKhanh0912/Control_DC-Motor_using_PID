#include <stdio.h>
#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "freertos/queue.h"
#include "driver/gpio.h"
#include "driver/ledc.h"
#include "driver/uart.h"
#include "esp_log.h"
#include "esp_system.h"
#include "esp_timer.h"
#include "stdlib.h"
#include <sys/time.h>
#include <math.h>

#define BUF_SIZE (1024)
#define ENCODER_PIN_A  22
#define ENCODER_PIN_B  23
#define MOTOR_PIN_EN   26
#define MOTOR_PIN_IN1  18
#define MOTOR_PIN_IN2  19

// Tần số PWM
#define PWM_FREQ 1000

// Độ phân giải PWM 8 bit
#define PWM_RESOLUTION 8

// Số xung Encoder mỗi vòng quay
#define ENCODER_RESOLUTION 480.0 

// Giới hạn Integral và Control Signal u
#define INTEGRAL_LIMIT 100.0
#define CONTROL_SIGNAL_LIMIT 255.0

// Đặt cờ lệnh để kiểm soát chế độ hoạt động của motor
bool mode_position_flag = 0;
bool mode_speed_flag = 0;
volatile bool pid_task_running = false; 

// Ba thông số của bộ PID (được nhập vào từ GUI xuống ESP32)
float KP = 0;
float KI = 0;
float KD = 0;

float setpoint = 0; 
volatile int position = 0;
volatile int prev_position = 0;
float prev_error = 0;
float integral = 0;
int stop_time = 0;
volatile unsigned long last_time = 0;

QueueHandle_t uart_queue;


// Hàm ngắt ngoài cạnh lên của kênh A Encoder
// Đếm xung dựa vào chiều quay thuận hay nghịch của motor, sử dụng cả kênh A và kênh B để kết luận
void IRAM_ATTR encoder_isr_handler(void *arg) 
{
    int level_b = gpio_get_level(ENCODER_PIN_B);

    if (level_b == 1) 
    {
        position--;
    } 
    else 
    {
        position++;
    }
}


// Hàm khởi động GPIO của kênh A và kênh B, enable ngắt ngoài cạnh lên kênh A
void encoder_init() 
{
    gpio_config_t io_conf_A;
    io_conf_A.intr_type = GPIO_INTR_POSEDGE;
    io_conf_A.mode = GPIO_MODE_INPUT;
    io_conf_A.pin_bit_mask = (1ULL << ENCODER_PIN_A);
    io_conf_A.pull_down_en = GPIO_PULLDOWN_DISABLE;
    io_conf_A.pull_up_en = GPIO_PULLUP_ENABLE;
    gpio_config(&io_conf_A);

    gpio_config_t io_conf_B;
    io_conf_B.intr_type = GPIO_INTR_DISABLE;
    io_conf_B.mode = GPIO_MODE_INPUT;
    io_conf_B.pin_bit_mask = (1ULL << ENCODER_PIN_B);
    io_conf_B.pull_down_en = GPIO_PULLDOWN_DISABLE;
    io_conf_B.pull_up_en = GPIO_PULLUP_ENABLE;
    gpio_config(&io_conf_B);

    gpio_install_isr_service(0);
    gpio_isr_handler_add(ENCODER_PIN_A, encoder_isr_handler, NULL);
}


// Hàm khởi động PWM: chế độ High Speed với tần số 1000Hz, độ phân giải 8 bit (từ 0 đến 255)
void pwm_init() 
{
    ledc_timer_config_t timer_conf = 
    {
        .duty_resolution = PWM_RESOLUTION,
        .freq_hz = PWM_FREQ,
        .speed_mode = LEDC_HIGH_SPEED_MODE,
        .timer_num = LEDC_TIMER_0
    };
    ledc_timer_config(&timer_conf);

    ledc_channel_config_t ledc_conf = 
    {
        .gpio_num = MOTOR_PIN_EN,
        .speed_mode = LEDC_HIGH_SPEED_MODE,
        .channel = LEDC_CHANNEL_0,
        .intr_type = LEDC_INTR_DISABLE,
        .timer_sel = LEDC_TIMER_0,
        .duty = 0
    };
    ledc_channel_config(&ledc_conf);
}


// Hàm khởi động GPIO của các chân IN1 và IN2 nối đến driver L298N
void motor_init() 
{
    esp_rom_gpio_pad_select_gpio(MOTOR_PIN_IN1);
    gpio_set_direction(MOTOR_PIN_IN1, GPIO_MODE_OUTPUT);
    esp_rom_gpio_pad_select_gpio(MOTOR_PIN_IN2);
    gpio_set_direction(MOTOR_PIN_IN2, GPIO_MODE_OUTPUT);
}


// Hàm set duty cycle cho chân PWM (nối với chân Enable 1 trên driver L298N) và chọn chiều quay dựa trên Control Signal
// Input: tín hiệu Control Signal u
// Output: PWM duty cycle và chiều quay của động cơ
void set_motor_speed(int ControlSignal) 
{
    if (ControlSignal > 0) 
    {
        gpio_set_level(MOTOR_PIN_IN1, 1);
        gpio_set_level(MOTOR_PIN_IN2, 0);
    } 
    else if (ControlSignal < 0) 
    {
        gpio_set_level(MOTOR_PIN_IN1, 0);
        gpio_set_level(MOTOR_PIN_IN2, 1);
    } 
    else 
    {
        gpio_set_level(MOTOR_PIN_IN1, 0);
        gpio_set_level(MOTOR_PIN_IN2, 0);
    }
    
    ledc_set_duty(LEDC_HIGH_SPEED_MODE, LEDC_CHANNEL_0, abs(ControlSignal));
    ledc_update_duty(LEDC_HIGH_SPEED_MODE, LEDC_CHANNEL_0);
}


// Bộ điều khiển PID
// Input: giá trị setpoint
// Output: tín hiệu điều khiển Control Signal u
float compute_pid(float setpoint) 
{
    char buffer[20];
    // Lấy thời gian hiện tại từ hệ thống, tính toán sự chênh lệch thời gian dt giữa 2 thời điểm
    unsigned long now = esp_timer_get_time();
    float deltaT = now - last_time;
    float dt = deltaT / 1000000; 
    printf("Now: %lu, Last time: %lu, deltaT: %.2f, dt: %.9f\n", now, last_time, deltaT, dt);
    float error = 0;
    last_time = now;
    
    // Nếu cờ lệnh đang báo chế độ hoạt động được nhập từ GUI là điều khiển vị trí:
    // Tính toán vị trí hiện tại theo đơn vị độ
    // Tính toán sai số vị trí
    if ((mode_position_flag == 1) && (mode_speed_flag == 0))
    {
        float position_degree = (position * 360)/ENCODER_RESOLUTION;
        error = setpoint - position_degree;

        // Gửi dữ liệu có dạng "@...&...#"
        // Dữ liệu nằm giữa '@' và '&' là giá trị Setpoint
        // Dữ liệu nằm giữa '&' và '#' là giá trị đáp ứng hiện tại
        int write = sprintf(buffer, "@%.2f&%.2f#\r\n", setpoint, position_degree); 
        uart_write_bytes(UART_NUM_2, buffer, write);
    }

    // Nếu cờ lệnh đang báo chế độ hoạt động được nhập từ GUI là điều khiển vận tốc:
    // Tính toán vận tốc hiện tại theo đơn vị RPM bằng phương pháp đếm số xung trong 1 khoảng thời gian dt
    // Tính toán sai số vận tốc
    else if ((mode_position_flag == 0) && (mode_speed_flag == 1))
    {
        // Tính số xung mỗi giây từ encoder
        float encoder_ticks_per_second = (position - prev_position) / dt;
        // Tính tốc độ RPM từ số xung mỗi giây
        float speed_rpm = encoder_ticks_per_second * 60 / ENCODER_RESOLUTION; 
        printf("Speed: %.2f RPM, Encoder tick per second: %.2f Tick, Position: %d, Prev_position: %d\n", speed_rpm, encoder_ticks_per_second, position, prev_position);
        prev_position = position;

        error = setpoint - speed_rpm;

        // Gửi dữ liệu có dạng "@...&...#"
        // Dữ liệu nằm giữa '@' và '&' là giá trị Setpoint
        // Dữ liệu nằm giữa '&' và '#' là giá trị đáp ứng hiện tại
        int write = sprintf(buffer, "@%.2f&%.2f#\r\n", setpoint, speed_rpm); 
        uart_write_bytes(UART_NUM_2, buffer, write);
    }
    
    // Tính phép tích phân
    integral = integral + (error * dt); 

    // giới hạn giá trị tích phân từ -100 đến 100
    if (integral > INTEGRAL_LIMIT) 
    {
        integral = INTEGRAL_LIMIT;
    } 
    else if (integral < -INTEGRAL_LIMIT) 
    {
        integral = -INTEGRAL_LIMIT;
    }

    // Tính phép vi phân
    float derivative = (error - prev_error) / dt;
    prev_error = error;

    // Tính toán tín hiệu điều khiển u
    float ControlSignal = KP * error + KI * integral + KD * derivative;

    // Giới hạn giá trị ControlSignal trong khoảng từ -255 đến 255
    if (ControlSignal > CONTROL_SIGNAL_LIMIT) 
    {
        ControlSignal = CONTROL_SIGNAL_LIMIT;
    } 
    else if (ControlSignal < -CONTROL_SIGNAL_LIMIT) 
    {
        ControlSignal = -CONTROL_SIGNAL_LIMIT;
    }
    
    printf("Error %.2f, Integral: %.2f, Derivate: %.2f, ControlSignal: %.2f\n", error, integral, derivative, ControlSignal);

    return ControlSignal;
}


// Task chạy PID và điều khiển motor
// Task này sẽ dừng (dừng motor) sau stop_time (ms)
void pid_task(void *pvParameter) 
{
    float output;
    unsigned long start_time = esp_timer_get_time() / 1000; 

    while (pid_task_running) 
    { 
        unsigned long current_time = esp_timer_get_time() / 1000; 

        output = compute_pid(setpoint);
        set_motor_speed((int)output);
        printf("Setpoint: %.2f, Position: %d\n", setpoint, position);
        vTaskDelay(pdMS_TO_TICKS(100));

        if (current_time - start_time >= stop_time) 
        {   
            set_motor_speed(0); 
            pid_task_running = false; 
        }
    }
    vTaskDelete(NULL); 
}


/* Hàm bóc tách chuỗi dữ liệu nhận được từ GUI
   Dữ liệu nhận được từ GUI có 2 dạng: "M...S...P...I...D...E...T" và "RUN"

   Chuỗi "M...S...P...I...D...E...T" dùng để đưa cho ESP32 biết chế độ điều khiển, các thông số PID, stop_time và setpoint
   Dữ liệu ... ở giữa 'M' và 'S' là chế độ điều khiển: '1' là chế độ điều khiển vị trí, '2' là chế độ điều khiển vận tốc
   Dữ liệu ... ở giữa 'S' và 'P' là giá trị setpoint
   Dữ liệu ... ở giữa 'P' và 'I' là giá trị KP
   Dữ liệu ... ở giữa 'I' và 'D' là giá trị KI
   Dữ liệu ... ở giữa 'D' và 'E' là giá trị KD
   Dữ liệu ... ở giữa 'E' và 'T' là giá trị stop_time

   Ví dụ: "M2S100P3I5D0E10000T" có:
   + Chế độ điều khiển '2' là điều khiển vận tốc
   + Giá trị setpoint = 100 (vì là điều khiển vận tốc nên đơn vị là RPM)
   + Giá trị KP = 3
   + Giá trị KI = 5
   + Giá trị KD = 0
   + Giá trị stop_time = 10000 ms = 10s

   Chuỗi "RUN" dùng để cho ESP32 biết cần phải bắt đầu chạy motor
*/
void process_uart_data(uint8_t *data, int len) 
{
    // Reset các thông số có tính tích lũy để chuẩn bị cho lần chạy mới
    position = 0;
    prev_position = 0;
    prev_error = 0;
    integral = 0;
    last_time = 0;

    int start_pos = -1;
    int mode_pos = -1;
    int S_pos = -1;
    int P_pos = -1;
    int I_pos = -1;
    int D_pos = -1;
    int E_pos = -1;
    int end_pos = -1;

    for (int i = 0; i < len; i++) 
    {
        if (data[i] == 'M' && start_pos == -1) 
        {
            start_pos = i;
        } 
        else if ((data[i] == '1' || data[i] == '2') && start_pos != -1 && mode_pos == -1) 
        {
            mode_pos = i;
            if (data[i] == '1') 
            {
                mode_position_flag = 1;
                mode_speed_flag = 0;
            } 
            else if (data[i] == '2') 
            {
                mode_position_flag = 0;
                mode_speed_flag = 1;
            }
        } 
        else if (data[i] == 'S' && start_pos != -1 && S_pos == -1) 
        {
            S_pos = i;
        } 
        else if (data[i] == 'P' && start_pos != -1 && P_pos == -1) 
        {
            P_pos = i;
        } 
        else if (data[i] == 'I' && start_pos != -1 && I_pos == -1) 
        {
            I_pos = i;
        } 
        else if (data[i] == 'D' && start_pos != -1 && D_pos == -1) 
        {
            D_pos = i;
        } 
        else if (data[i] == 'E' && start_pos != -1 && E_pos == -1) 
        {
            E_pos = i;
        } 
        else if (data[i] == 'T' && start_pos != -1) 
        {
            end_pos = i;

            char setpoint_str[P_pos - S_pos];
            char kp_str[I_pos - P_pos];
            char ki_str[D_pos - I_pos];
            char kd_str[E_pos - D_pos];
            char stoptime_str[end_pos - E_pos];

            memcpy(setpoint_str, &data[S_pos + 1], P_pos - S_pos - 1);
            setpoint_str[P_pos - S_pos - 1] = '\0';
            setpoint = atof(setpoint_str);

            memcpy(kp_str, &data[P_pos + 1], I_pos - P_pos - 1);
            kp_str[I_pos - P_pos - 1] = '\0';
            KP = atof(kp_str);

            memcpy(ki_str, &data[I_pos + 1], D_pos - I_pos - 1);
            ki_str[D_pos - I_pos - 1] = '\0';
            KI = atof(ki_str);

            memcpy(kd_str, &data[D_pos + 1], E_pos - D_pos - 1);
            kd_str[E_pos - D_pos - 1] = '\0';
            KD = atof(kd_str);

            memcpy(stoptime_str, &data[E_pos + 1], end_pos - E_pos - 1);
            stoptime_str[end_pos - E_pos - 1] = '\0';
            stop_time = atof(stoptime_str);
            break;
        } 
        else if (memcmp(&data[i], "RUN", 3) == 0) 
        {
            pid_task_running = true;
            xTaskCreate(&pid_task, "pid_task", 2048, NULL, 5, NULL);
        }
    }

    //char buffer[200];
    //int write = sprintf(buffer, "Setpoint: %.2f, Mode Pos: %d, Mode Speed: %d, Run flag: %d, KP: %.5f, KI: %.5f, KD: %.5f, Stop time: %d,\r\n", setpoint, mode_position_flag, mode_speed_flag, pid_task_running, KP, KI, KD, stop_time); 
    //uart_write_bytes(UART_NUM_2, buffer, write);

    //write = sprintf(buffer, "Start: %d, Mode: %d, S: %d, P: %d, I: %d, D: %d, E: %d, Stop: %d,\r\n", start_pos, mode_pos, S_pos, P_pos, I_pos, D_pos, E_pos, end_pos); 
    //uart_write_bytes(UART_NUM_2, buffer, write);
}


// Task xử lý khi nhận được dữ liệu từ UART2 từ GUI gửi xuống
void uart_event_task(void *pvParameters) 
{
    uart_event_t event;
    uint8_t *data = (uint8_t *) malloc(BUF_SIZE);
    for (;;) 
    {
        if (xQueueReceive(uart_queue, (void *)&event, (TickType_t)portMAX_DELAY)) 
        {
            memset(data, 0, BUF_SIZE);
            if (event.type == UART_DATA) 
            {
                int len = uart_read_bytes(UART_NUM_2, data, event.size, portMAX_DELAY);
                process_uart_data(data, len);
            }
        }
    }
    free(data);
    vTaskDelete(NULL);
}


// Khởi động UART2
void uart_init(void) 
{
    uart_config_t uart_config = 
    {
        .baud_rate = 9600,
        .data_bits = UART_DATA_8_BITS,
        .parity = UART_PARITY_DISABLE,
        .stop_bits = UART_STOP_BITS_1,
        .flow_ctrl = UART_HW_FLOWCTRL_DISABLE,
        .source_clk = UART_SCLK_APB,
    };

    uart_driver_install(UART_NUM_2, BUF_SIZE * 2, BUF_SIZE * 2, 10, &uart_queue, 0);
    uart_param_config(UART_NUM_2, &uart_config);
    uart_set_pin(UART_NUM_2, GPIO_NUM_17, GPIO_NUM_16, UART_PIN_NO_CHANGE, UART_PIN_NO_CHANGE);

    xTaskCreate(uart_event_task, "uart_event_task", 4096, NULL, 12, NULL);
}


void app_main() 
{
    encoder_init();
    pwm_init();
    motor_init();
    uart_init();
}