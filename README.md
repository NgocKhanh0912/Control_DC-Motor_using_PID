This project is the Capstone 1 project that I am working on in this semester 232. The project content is as follows: control the position and velocity of the DC motor JGB37-545 using PID algorithm and PWM technique. WinForm GUI will send parameters: control mode, PID parameters (KP, KI, KD), setpoint, stop_time, and start running to ESP32. 

1. The data received from the GUI comes in 2 forms: "M...S...P...I...D...E...T" and "RUN".

a) The string "M...S...P...I...D...E...T" is used to provide ESP32 with control mode, PID parameters, stop_time, and setpoint.
   - Data ... between 'M' and 'S' represents the control mode: '1' for position control, '2' for velocity control.
   - Data ... between 'S' and 'P' represents the setpoint value.
   - Data ... between 'P' and 'I' represents the KP value.
   - Data ... between 'I' and 'D' represents the KI value.
   - Data ... between 'D' and 'E' represents the KD value.
   - Data ... between 'E' and 'T' represents the stop_time value.
     
   For example: "M2S100P3I5D0E10T" contains:
   - Control mode '2' for velocity control
   - Setpoint value = 100 (RPM as it is velocity control)
   - KP value = 3
   - KI value = 5
   - KD value = 0
   - Stop time value = 10 seconds.

b) The string "RUN" is used to instruct ESP32 to start the motor.

2. The data transmitted from ESP32 to the GUI has the format: "@...&...#", where:
   - The value between '@' and '&' represents the value of the setpoint.
   - The value between '&' and '#' represents the value of the current response.
   - The GUI will parse this frame, extract the data, and draw the response graph 
