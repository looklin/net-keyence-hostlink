using System;
using System.Drawing;
using System.Windows.Forms;

namespace Keyence.HostLink.Tests.Net8.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpConnection = new GroupBox();
            this.lblStatus = new Label();
            this.btnDisconnect = new Button();
            this.btnConnect = new Button();
            this.lblPort = new Label();
            this.txtPort = new TextBox();
            this.lblHost = new Label();
            this.txtHost = new TextBox();
            this.grpReadWrite = new GroupBox();
            this.btnReadContinuous = new Button();
            this.txtReadCount = new TextBox();
            this.lblReadCount = new Label();
            this.btnWrite = new Button();
            this.txtWriteValue = new TextBox();
            this.lblWriteValue = new Label();
            this.btnRead = new Button();
            this.txtAddress = new TextBox();
            this.lblAddress = new Label();
            this.grpResult = new GroupBox();
            this.txtResult = new TextBox();
            this.grpLog = new GroupBox();
            this.txtLog = new TextBox();
            this.btnClearLog = new Button();
            this.grpAdvanced = new GroupBox();
            this.btnBatchRead = new Button();
            this.btnBatchWrite = new Button();
            this.txtBatchValues = new TextBox();
            this.lblBatchValues = new Label();
            this.lblNet8Features = new Label();
            this.grpConnection.SuspendLayout();
            this.grpReadWrite.SuspendLayout();
            this.grpResult.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.lblStatus);
            this.grpConnection.Controls.Add(this.btnDisconnect);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.lblPort);
            this.grpConnection.Controls.Add(this.txtPort);
            this.grpConnection.Controls.Add(this.lblHost);
            this.grpConnection.Controls.Add(this.txtHost);
            this.grpConnection.Location = new Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new Size(760, 85);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Connection Settings (.NET 8)";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = Color.Gray;
            this.lblStatus.Location = new Point(520, 25);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(100, 20);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Status: Disconnected";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new Point(420, 50);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new Size(90, 28);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += this.btnDisconnect_Click;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new Point(320, 50);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new Size(90, 28);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += this.btnConnect_Click;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new Point(210, 25);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new Size(35, 20);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new Point(250, 22);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new Size(60, 27);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "8501";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new Point(10, 25);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new Size(35, 20);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Host:";
            // 
            // txtHost
            // 
            this.txtHost.Location = new Point(50, 22);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new Size(150, 27);
            this.txtHost.TabIndex = 0;
            this.txtHost.Text = "192.168.3.100";
            // 
            // grpReadWrite
            // 
            this.grpReadWrite.Controls.Add(this.btnReadContinuous);
            this.grpReadWrite.Controls.Add(this.txtReadCount);
            this.grpReadWrite.Controls.Add(this.lblReadCount);
            this.grpReadWrite.Controls.Add(this.btnWrite);
            this.grpReadWrite.Controls.Add(this.txtWriteValue);
            this.grpReadWrite.Controls.Add(this.lblWriteValue);
            this.grpReadWrite.Controls.Add(this.btnRead);
            this.grpReadWrite.Controls.Add(this.txtAddress);
            this.grpReadWrite.Controls.Add(this.lblAddress);
            this.grpReadWrite.Location = new Point(12, 103);
            this.grpReadWrite.Name = "grpReadWrite";
            this.grpReadWrite.Size = new Size(760, 115);
            this.grpReadWrite.TabIndex = 1;
            this.grpReadWrite.TabStop = false;
            this.grpReadWrite.Text = "Read / Write Operations";
            // 
            // btnReadContinuous
            // 
            this.btnReadContinuous.Location = new Point(660, 70);
            this.btnReadContinuous.Name = "btnReadContinuous";
            this.btnReadContinuous.Size = new Size(90, 28);
            this.btnReadContinuous.TabIndex = 8;
            this.btnReadContinuous.Text = "Read Cont.";
            this.btnReadContinuous.UseVisualStyleBackColor = true;
            this.btnReadContinuous.Click += this.btnReadContinuous_Click;
            // 
            // txtReadCount
            // 
            this.txtReadCount.Location = new Point(590, 72);
            this.txtReadCount.Name = "txtReadCount";
            this.txtReadCount.Size = new Size(60, 27);
            this.txtReadCount.TabIndex = 7;
            this.txtReadCount.Text = "10";
            // 
            // lblReadCount
            // 
            this.lblReadCount.AutoSize = true;
            this.lblReadCount.Location = new Point(510, 75);
            this.lblReadCount.Name = "lblReadCount";
            this.lblReadCount.Size = new Size(74, 20);
            this.lblReadCount.TabIndex = 6;
            this.lblReadCount.Text = "Read Count:";
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new Point(660, 30);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new Size(90, 28);
            this.btnWrite.TabIndex = 5;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += this.btnWrite_Click;
            // 
            // txtWriteValue
            // 
            this.txtWriteValue.Location = new Point(400, 32);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new Size(250, 27);
            this.txtWriteValue.TabIndex = 4;
            // 
            // lblWriteValue
            // 
            this.lblWriteValue.AutoSize = true;
            this.lblWriteValue.Location = new Point(350, 35);
            this.lblWriteValue.Name = "lblWriteValue";
            this.lblWriteValue.Size = new Size(44, 20);
            this.lblWriteValue.TabIndex = 3;
            this.lblWriteValue.Text = "Value:";
            // 
            // btnRead
            // 
            this.btnRead.Location = new Point(250, 30);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new Size(90, 28);
            this.btnRead.TabIndex = 2;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += this.btnRead_Click;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new Point(70, 32);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new Size(170, 27);
            this.txtAddress.TabIndex = 1;
            this.txtAddress.Text = "DM0";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new Point(10, 35);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new Size(60, 20);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Address:";
            // 
            // grpResult
            // 
            this.grpResult.Controls.Add(this.txtResult);
            this.grpResult.Location = new Point(12, 354);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new Size(374, 200);
            this.grpResult.TabIndex = 2;
            this.grpResult.TabStop = false;
            this.grpResult.Text = "Result";
            // 
            // txtResult
            // 
            this.txtResult.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.txtResult.Location = new Point(10, 22);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = ScrollBars.Vertical;
            this.txtResult.Size = new Size(354, 168);
            this.txtResult.TabIndex = 0;
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Controls.Add(this.btnClearLog);
            this.grpLog.Location = new Point(392, 354);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new Size(380, 200);
            this.grpLog.TabIndex = 3;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.txtLog.Location = new Point(10, 22);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.Size = new Size(360, 138);
            this.txtLog.TabIndex = 0;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new Point(280, 165);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new Size(90, 28);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += this.btnClearLog_Click;
            // 
            // grpAdvanced
            // 
            this.grpAdvanced.Controls.Add(this.lblNet8Features);
            this.grpAdvanced.Controls.Add(this.lblBatchValues);
            this.grpAdvanced.Controls.Add(this.txtBatchValues);
            this.grpAdvanced.Controls.Add(this.btnBatchWrite);
            this.grpAdvanced.Controls.Add(this.btnBatchRead);
            this.grpAdvanced.Location = new Point(12, 224);
            this.grpAdvanced.Name = "grpAdvanced";
            this.grpAdvanced.Size = new Size(760, 124);
            this.grpAdvanced.TabIndex = 4;
            this.grpAdvanced.TabStop = false;
            this.grpAdvanced.Text = "Advanced Operations (.NET 8 Features)";
            // 
            // btnBatchRead
            // 
            this.btnBatchRead.Location = new Point(10, 30);
            this.btnBatchRead.Name = "btnBatchRead";
            this.btnBatchRead.Size = new Size(120, 28);
            this.btnBatchRead.TabIndex = 0;
            this.btnBatchRead.Text = "Batch Read (Span)";
            this.btnBatchRead.UseVisualStyleBackColor = true;
            this.btnBatchRead.Click += this.btnBatchRead_Click;
            // 
            // btnBatchWrite
            // 
            this.btnBatchWrite.Location = new Point(10, 70);
            this.btnBatchWrite.Name = "btnBatchWrite";
            this.btnBatchWrite.Size = new Size(120, 28);
            this.btnBatchWrite.TabIndex = 1;
            this.btnBatchWrite.Text = "Batch Write";
            this.btnBatchWrite.UseVisualStyleBackColor = true;
            this.btnBatchWrite.Click += this.btnBatchWrite_Click;
            // 
            // txtBatchValues
            // 
            this.txtBatchValues.Location = new Point(150, 72);
            this.txtBatchValues.Name = "txtBatchValues";
            this.txtBatchValues.Size = new Size(500, 27);
            this.txtBatchValues.TabIndex = 2;
            this.txtBatchValues.Text = "100,200,300";
            // 
            // lblBatchValues
            // 
            this.lblBatchValues.AutoSize = true;
            this.lblBatchValues.Location = new Point(150, 45);
            this.lblBatchValues.Name = "lblBatchValues";
            this.lblBatchValues.Size = new Size(197, 20);
            this.lblBatchValues.TabIndex = 3;
            this.lblBatchValues.Text = "Values (comma separated):";
            // 
            // lblNet8Features
            // 
            this.lblNet8Features.AutoSize = true;
            this.lblNet8Features.ForeColor = Color.Green;
            this.lblNet8Features.Location = new Point(580, 33);
            this.lblNet8Features.Name = "lblNet8Features";
            this.lblNet8Features.Size = new Size(170, 20);
            this.lblNet8Features.TabIndex = 4;
            this.lblNet8Features.Text = "Using .NET 8 Features";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 566);
            this.Controls.Add(this.grpAdvanced);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpResult);
            this.Controls.Add(this.grpReadWrite);
            this.Controls.Add(this.grpConnection);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Keyence HostLink Test Tool (.NET 8)";
            this.FormClosing += this.MainForm_FormClosing;
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.grpReadWrite.ResumeLayout(false);
            this.grpReadWrite.PerformLayout();
            this.grpResult.ResumeLayout(false);
            this.grpResult.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.grpAdvanced.ResumeLayout(false);
            this.grpAdvanced.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private GroupBox grpConnection;
        private Label lblStatus;
        private Button btnDisconnect;
        private Button btnConnect;
        private Label lblPort;
        private TextBox txtPort;
        private Label lblHost;
        private TextBox txtHost;
        private GroupBox grpReadWrite;
        private Button btnReadContinuous;
        private TextBox txtReadCount;
        private Label lblReadCount;
        private Button btnWrite;
        private TextBox txtWriteValue;
        private Label lblWriteValue;
        private Button btnRead;
        private TextBox txtAddress;
        private Label lblAddress;
        private GroupBox grpResult;
        private TextBox txtResult;
        private GroupBox grpLog;
        private TextBox txtLog;
        private Button btnClearLog;
        private GroupBox grpAdvanced;
        private Label lblNet8Features;
        private Label lblBatchValues;
        private TextBox txtBatchValues;
        private Button btnBatchWrite;
        private Button btnBatchRead;
    }
}
