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
            cboBaudrate.Text = "9600";

            // Khởi tạo biểu đồ
            GraphPane myPanne = zedGraphControl1.GraphPane;
            myPanne.Title.Text = "Đồ thị đáp ứng";
            myPanne.XAxis.Title.Text = "Thời gian";
            myPanne.YAxis.Title.Text = "Giá trị đáp ứng";

            RollingPointPairList list = new RollingPointPairList(500000);

            LineItem line = myPanne.AddCurve("Đáp ứng", list, Color.Red, SymbolType.Circle);

            // Giá trị nhỏ nhất
            myPanne.XAxis.Scale.Min = 0;
            // Giá trị lớn nhất
            //myPanne.XAxis.Scale.Max = Convert.ToInt32(txtStoptime.Text);
            myPanne.XAxis.Scale.Max = 5000;
            myPanne.XAxis.Scale.MinorStep = 24; 
            myPanne.XAxis.Scale.MajorStep = 100; 

            // Giá trị nhỏ nhất
            myPanne.YAxis.Scale.Min = 0;
            // Giá trị lớn nhất
            //myPanne.YAxis.Scale.Max = Convert.ToInt32(txtSetpoint.Text);
            myPanne.YAxis.Scale.Max = 200;
            myPanne.YAxis.Scale.MinorStep = 10; 
            myPanne.YAxis.Scale.MajorStep = 50;

            zedGraphControl1.AxisChange();
        }

        int sum = 0;

        public void draw(double line)
        {
            LineItem duongline = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (duongline == null) return;
            IPointListEdit list = duongline.Points as IPointListEdit;
            if (duongline == null) return;

            list.Add(sum, line);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            sum += 24;
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
                string data_Parameters = "M" + txtControlMode.Text + "S" + txtSetpoint.Text + "P" + txtKP.Text
                                         + "I" + txtKI.Text + "D" + txtKD.Text + "E" + txtStoptime.Text + "T";

                txtAllData.Text = data_Parameters;

                // Gửi xuống cho ESP32
                serCOM.Write(data_Parameters);
            }
        }


        private void serCOM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Đọc dữ liệu từ SerialPort
            string newData = "";
            newData = serCOM.ReadLine();
            newData = newData.Trim();
            int len = newData.Length;

            if (len > 0)
            {
                Invoke(new MethodInvoker(() => draw(Convert.ToDouble(newData))));
            }
        }
    }
}
