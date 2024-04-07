
namespace GUI_DC_Motor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.serCOM = new System.IO.Ports.SerialPort(this.components);
            this.Communication = new System.Windows.Forms.GroupBox();
            this.cboBaudrate = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboComPort = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.butConnect = new System.Windows.Forms.Button();
            this.butExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Data = new System.Windows.Forms.GroupBox();
            this.txtStoptime = new System.Windows.Forms.TextBox();
            this.txtKD = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAllData = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.butSet = new System.Windows.Forms.Button();
            this.txtKI = new System.Windows.Forms.TextBox();
            this.txtKP = new System.Windows.Forms.TextBox();
            this.txtSetpoint = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtControlMode = new System.Windows.Forms.TextBox();
            this.Operation = new System.Windows.Forms.GroupBox();
            this.butRun = new System.Windows.Forms.Button();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.Communication.SuspendLayout();
            this.Data.SuspendLayout();
            this.Operation.SuspendLayout();
            this.SuspendLayout();
            // 
            // serCOM
            // 
            this.serCOM.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serCOM_DataReceived);
            // 
            // Communication
            // 
            this.Communication.Controls.Add(this.cboBaudrate);
            this.Communication.Controls.Add(this.label3);
            this.Communication.Controls.Add(this.cboComPort);
            this.Communication.Controls.Add(this.label2);
            this.Communication.Controls.Add(this.butConnect);
            this.Communication.Location = new System.Drawing.Point(22, 36);
            this.Communication.Name = "Communication";
            this.Communication.Size = new System.Drawing.Size(417, 211);
            this.Communication.TabIndex = 17;
            this.Communication.TabStop = false;
            this.Communication.Text = "Communication";
            // 
            // cboBaudrate
            // 
            this.cboBaudrate.FormattingEnabled = true;
            this.cboBaudrate.Location = new System.Drawing.Point(196, 83);
            this.cboBaudrate.Name = "cboBaudrate";
            this.cboBaudrate.Size = new System.Drawing.Size(197, 24);
            this.cboBaudrate.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Select Baudrate:";
            // 
            // cboComPort
            // 
            this.cboComPort.FormattingEnabled = true;
            this.cboComPort.Location = new System.Drawing.Point(196, 31);
            this.cboComPort.Name = "cboComPort";
            this.cboComPort.Size = new System.Drawing.Size(197, 24);
            this.cboComPort.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select COM:";
            // 
            // butConnect
            // 
            this.butConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butConnect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.butConnect.Location = new System.Drawing.Point(93, 135);
            this.butConnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.butConnect.Name = "butConnect";
            this.butConnect.Size = new System.Drawing.Size(232, 55);
            this.butConnect.TabIndex = 7;
            this.butConnect.Text = "Connect";
            this.butConnect.UseVisualStyleBackColor = true;
            this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
            // 
            // butExit
            // 
            this.butExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.butExit.Location = new System.Drawing.Point(258, 47);
            this.butExit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.butExit.Name = "butExit";
            this.butExit.Size = new System.Drawing.Size(135, 55);
            this.butExit.TabIndex = 8;
            this.butExit.Text = "Exit";
            this.butExit.UseVisualStyleBackColor = true;
            this.butExit.Click += new System.EventHandler(this.butExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(1125, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 38);
            this.label1.TabIndex = 18;
            this.label1.Text = "Control DC Motor";
            // 
            // Data
            // 
            this.Data.Controls.Add(this.txtStoptime);
            this.Data.Controls.Add(this.txtKD);
            this.Data.Controls.Add(this.label10);
            this.Data.Controls.Add(this.label9);
            this.Data.Controls.Add(this.label8);
            this.Data.Controls.Add(this.txtAllData);
            this.Data.Controls.Add(this.label4);
            this.Data.Controls.Add(this.label7);
            this.Data.Controls.Add(this.butSet);
            this.Data.Controls.Add(this.txtKI);
            this.Data.Controls.Add(this.txtKP);
            this.Data.Controls.Add(this.txtSetpoint);
            this.Data.Controls.Add(this.label6);
            this.Data.Controls.Add(this.label5);
            this.Data.Controls.Add(this.txtControlMode);
            this.Data.Location = new System.Drawing.Point(22, 275);
            this.Data.Name = "Data";
            this.Data.Size = new System.Drawing.Size(417, 508);
            this.Data.TabIndex = 19;
            this.Data.TabStop = false;
            this.Data.Text = "Data";
            // 
            // txtStoptime
            // 
            this.txtStoptime.Location = new System.Drawing.Point(196, 327);
            this.txtStoptime.Name = "txtStoptime";
            this.txtStoptime.Size = new System.Drawing.Size(197, 22);
            this.txtStoptime.TabIndex = 25;
            // 
            // txtKD
            // 
            this.txtKD.Location = new System.Drawing.Point(196, 268);
            this.txtKD.Name = "txtKD";
            this.txtKD.Size = new System.Drawing.Size(197, 22);
            this.txtKD.TabIndex = 24;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(21, 329);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(125, 20);
            this.label10.TabIndex = 23;
            this.label10.Text = "Stop time (s):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(23, 270);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 20);
            this.label9.TabIndex = 22;
            this.label9.Text = "KD:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(21, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 20);
            this.label8.TabIndex = 21;
            this.label8.Text = "KP:";
            // 
            // txtAllData
            // 
            this.txtAllData.Location = new System.Drawing.Point(196, 383);
            this.txtAllData.Name = "txtAllData";
            this.txtAllData.Size = new System.Drawing.Size(197, 22);
            this.txtAllData.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(23, 385);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Data Frame:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(23, 209);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 20);
            this.label7.TabIndex = 20;
            this.label7.Text = "KI:";
            // 
            // butSet
            // 
            this.butSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butSet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.butSet.Location = new System.Drawing.Point(150, 440);
            this.butSet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.butSet.Name = "butSet";
            this.butSet.Size = new System.Drawing.Size(123, 45);
            this.butSet.TabIndex = 19;
            this.butSet.Text = "Set";
            this.butSet.UseVisualStyleBackColor = true;
            this.butSet.Click += new System.EventHandler(this.butSet_Click);
            // 
            // txtKI
            // 
            this.txtKI.Location = new System.Drawing.Point(196, 207);
            this.txtKI.Name = "txtKI";
            this.txtKI.Size = new System.Drawing.Size(197, 22);
            this.txtKI.TabIndex = 17;
            // 
            // txtKP
            // 
            this.txtKP.Location = new System.Drawing.Point(196, 151);
            this.txtKP.Name = "txtKP";
            this.txtKP.Size = new System.Drawing.Size(197, 22);
            this.txtKP.TabIndex = 16;
            // 
            // txtSetpoint
            // 
            this.txtSetpoint.Location = new System.Drawing.Point(196, 98);
            this.txtSetpoint.Name = "txtSetpoint";
            this.txtSetpoint.Size = new System.Drawing.Size(197, 22);
            this.txtSetpoint.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 20);
            this.label6.TabIndex = 14;
            this.label6.Text = "Setpoint:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(23, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "Control mode:";
            // 
            // txtControlMode
            // 
            this.txtControlMode.Location = new System.Drawing.Point(196, 42);
            this.txtControlMode.Name = "txtControlMode";
            this.txtControlMode.Size = new System.Drawing.Size(197, 22);
            this.txtControlMode.TabIndex = 12;
            // 
            // Operation
            // 
            this.Operation.Controls.Add(this.butRun);
            this.Operation.Controls.Add(this.butExit);
            this.Operation.Location = new System.Drawing.Point(22, 805);
            this.Operation.Name = "Operation";
            this.Operation.Size = new System.Drawing.Size(417, 137);
            this.Operation.TabIndex = 20;
            this.Operation.TabStop = false;
            this.Operation.Text = "Operation";
            // 
            // butRun
            // 
            this.butRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.butRun.Location = new System.Drawing.Point(25, 47);
            this.butRun.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.butRun.Name = "butRun";
            this.butRun.Size = new System.Drawing.Size(135, 55);
            this.butRun.TabIndex = 7;
            this.butRun.Text = "Run";
            this.butRun.UseVisualStyleBackColor = true;
            this.butRun.Click += new System.EventHandler(this.butRun_Click);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(560, 104);
            this.zedGraphControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(1351, 838);
            this.zedGraphControl1.TabIndex = 21;
            this.zedGraphControl1.UseExtendedPrintDialog = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1924, 980);
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.Operation);
            this.Controls.Add(this.Data);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Communication);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Communication.ResumeLayout(false);
            this.Communication.PerformLayout();
            this.Data.ResumeLayout(false);
            this.Data.PerformLayout();
            this.Operation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serCOM;
        private System.Windows.Forms.GroupBox Communication;
        private System.Windows.Forms.ComboBox cboBaudrate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboComPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button butConnect;
        private System.Windows.Forms.Button butExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox Data;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button butSet;
        private System.Windows.Forms.TextBox txtKI;
        private System.Windows.Forms.TextBox txtKP;
        private System.Windows.Forms.TextBox txtSetpoint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtAllData;
        private System.Windows.Forms.TextBox txtControlMode;
        private System.Windows.Forms.TextBox txtStoptime;
        private System.Windows.Forms.TextBox txtKD;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox Operation;
        private System.Windows.Forms.Button butRun;
        private ZedGraph.ZedGraphControl zedGraphControl1;
    }
}

