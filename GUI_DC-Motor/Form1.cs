using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using ZedGraph;

namespace GUI_DC_Motor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] Baudrate = { "1200", "2400", "3600", "4800", "9600", "115200" };
            cboBaudrate.Items.AddRange(Baudrate);
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboComPort.DataSource = SerialPort.GetPortNames();
            // Chọn mặc định Baudrate là 9600
            cboBaudrate.Text = "9600";

            // Thêm các giá trị vào combo box Control Mode
            string[] controlModes = { "Position", "Speed" };
            cboControlMode.Items.AddRange(controlModes);
            // Chọn mặc định là Position
            cboControlMode.SelectedIndex = 0; 

            // Khởi tạo biểu đồ (lúc ban đầu)
            GraphPane myPanne = zedGraphControl1.GraphPane;
            myPanne.Title.Text = "Response Graph";
            myPanne.XAxis.Title.Text = "Time (ms)";
            myPanne.YAxis.Title.Text = "Response Value";

            RollingPointPairList list1 = new RollingPointPairList(500000);
            RollingPointPairList list2 = new RollingPointPairList(500000);

            LineItem line1 = myPanne.AddCurve("Response", list1, Color.Red, SymbolType.Circle);
            LineItem line2 = myPanne.AddCurve("Setpoint", list2, Color.Black, SymbolType.Circle);

            myPanne.XAxis.Scale.Min = 0;
            myPanne.XAxis.Scale.Max = 5000;
            myPanne.XAxis.Scale.MinorStep = 100; 
            myPanne.XAxis.Scale.MajorStep = 1000; 

            myPanne.YAxis.Scale.Min = 0;
            myPanne.YAxis.Scale.Max = 200;
            myPanne.YAxis.Scale.MinorStep = 10; 
            myPanne.YAxis.Scale.MajorStep = 50;

            zedGraphControl1.AxisChange();
        }

        // Hàm set giá trị max của trục X và Y dựa trên giá trị nhập vào txtStoptime và txtSetpoint
        private void UpdateGraphMaxValues(string modeValue)
        {
            // Cập nhật giá trị tối đa của trục X và Y trên biểu đồ
            GraphPane myPanne = zedGraphControl1.GraphPane;

            int maxXValue = Convert.ToInt32(txtStoptime.Text);
            int maxYValue = Convert.ToInt32(txtSetpoint.Text);

            if (modeValue == "1")
            {
                // Nếu là mode position:
                // Giá trị tối đa của trục Y bằng giá trị Setpoint cộng thêm 300 để dễ quan sát vọt lố
                maxYValue = Convert.ToInt32(txtSetpoint.Text) + 300;
            }
            else if (modeValue == "2")
            {
                // Nếu là mode speed:
                // Giá trị tối đa của trục Y bằng giá trị Setpoint cộng thêm 300 để dễ quan sát vọt lố
                maxYValue = Convert.ToInt32(txtSetpoint.Text) + 100;
            }

            myPanne.XAxis.Scale.Max = maxXValue;
            myPanne.YAxis.Scale.Max = maxYValue;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        int sum = 0;

        public void draw(double line1, double line2)
        {
            LineItem duongline1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            LineItem duongline2 = zedGraphControl1.GraphPane.CurveList[1] as LineItem;

            if (duongline1 == null) return;
            if (duongline2 == null) return;

            IPointListEdit list1 = duongline1.Points as IPointListEdit;
            IPointListEdit list2 = duongline2.Points as IPointListEdit;

            if (list1 == null) return;
            if (list2 == null) return;

            list1.Add(sum, line1);
            list2.Add(sum, line2);

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            sum += 106;
        }

        private void butConnect_Click(object sender, EventArgs e)
        {
            if (!serCOM.IsOpen)
            {
                butConnect.Text = "Connected";
                serCOM.PortName = cboComPort.Text;
                serCOM.BaudRate = Convert.ToInt32(cboBaudrate.Text);

                serCOM.Open();
            }
            else
            {
                butConnect.Text = "Disconnected";
                serCOM.Close();
            }
        }

        private void butExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void butRun_Click(object sender, EventArgs e)
        {
            if (!serCOM.IsOpen)
            {
                MessageBox.Show("COM Port has not been connected!");
            }
            else
            {
                string data_Run = "RUN";

                // Gửi xuống cho ESP32
                serCOM.Write(data_Run);
            }
        }

        private void butSet_Click(object sender, EventArgs e)
        {
            if (!serCOM.IsOpen)
            {
                MessageBox.Show("COM Port has not been connected!");
            }
            else
            {
                string controlMode = cboControlMode.SelectedItem.ToString();
                // Cập nhật giá trị modeValue gửi xuống ESP32 dựa vào lựa chọn từ cboControlMode
                string modeValue = (controlMode == "Position") ? "1" : "2";

                string data_Parameters = "M" + modeValue + "S" + txtSetpoint.Text + "P" + txtKP.Text
                                         + "I" + txtKI.Text + "D" + txtKD.Text + "E" + txtStoptime.Text + "T";

                txtAllData.Text = data_Parameters;

                // Cập nhật tiêu đề của trục Y trên biểu đồ dựa vào lựa chọn từ cboControlMode
                string yAxisTitle = (controlMode == "Position") ? "Position (degree)" : "Speed (RPM)";
                zedGraphControl1.GraphPane.YAxis.Title.Text = yAxisTitle;

                // Gửi xuống cho ESP32
                serCOM.Write(data_Parameters);

                // Cập nhật giá trị tối đa của trục X và Y trên biểu đồ
                UpdateGraphMaxValues(modeValue);
            }
        }

        private StringBuilder receivedData = new StringBuilder();

        private void serCOM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Đọc dữ liệu từ SerialPort
                string newData = "";
                newData = serCOM.ReadLine();
                newData = newData.Trim();
                int len = newData.Length;

                // Thêm dữ liệu vào bộ nhớ đệm
                receivedData.Append(newData);

                // Bóc tách frame nhận được từ ESP32 có dạng: "@...&...#"
                // Dữ liệu nằm giữa '@' và '&' là giá trị Setpoint
                // Dữ liệu nằm giữa '&' và '#' là giá trị đáp ứng hiện tại
                // Kiểm tra xem dữ liệu đã chứa chuỗi kết thúc hay chưa
                if (receivedData.ToString().Contains("@") && receivedData.ToString().Contains("#"))
                {

                    // Lấy chỉ số của ký tự @ đầu tiên
                    int startIndex = receivedData.ToString().IndexOf("@");

                    // Lấy chỉ số của ký tự # cuối cùng
                    int endIndex = receivedData.ToString().LastIndexOf("#");

                    // Trích xuất chuỗi dữ liệu từ vị trí @ đầu tiên đến vị trí # cuối cùng
                    string Value = receivedData.ToString().Substring(startIndex + 1, endIndex - startIndex - 1);

                    // Tách giá trị Setpoint và giá trị đáp ứng hiện tại dựa trên ký tự '&'
                    string[] parts = Value.Split('&');
                    string Setpoint_value = parts[0];
                    string Run_value = parts[1];

                    if (len > 0)
                    {
                        Invoke(new MethodInvoker(() => draw(Convert.ToDouble(Run_value), Convert.ToDouble(Setpoint_value))));
                    }

                    // Xóa bộ nhớ đệm sau khi đã xử lý
                    receivedData.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while reading data from SerialPort: " + ex.Message);
            }

        }

        // Hàm này sẽ xóa dữ liệu đã được vẽ trên đồ thị và đưa giá trị lần vẽ tiếp theo bắt đầu từ điểm (0,0)
        private void btnResetGraph_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu của các đường trên biểu đồ
            LineItem line1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            LineItem line2 = zedGraphControl1.GraphPane.CurveList[1] as LineItem;

            if (line1 != null)
            {
                IPointListEdit list1 = line1.Points as IPointListEdit;
                list1.Clear();
            }

            if (line2 != null)
            {
                IPointListEdit list2 = line2.Points as IPointListEdit;
                list2.Clear();
            }

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();

            sum = 0;

            // Cập nhật giá trị tối đa của trục X và Y trên biểu đồ
            UpdateGraphMaxValues("");
        }
    }
}
