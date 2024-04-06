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
    }
}
