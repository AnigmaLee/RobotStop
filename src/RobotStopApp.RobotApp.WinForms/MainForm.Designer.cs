namespace RobotStopApp.RobotApp.WinForms
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

        private void InitializeComponent()
        {
            this.lblBaseUrl = new System.Windows.Forms.Label();
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.lblApiKey = new System.Windows.Forms.Label();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.btnCheckStatus = new System.Windows.Forms.Button();
            this.lblApiConnected = new System.Windows.Forms.Label();
            this.lblApiConnectedValue = new System.Windows.Forms.Label();
            this.lblRobotRunOk = new System.Windows.Forms.Label();
            this.lblRobotRunOkValue = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblBaseUrl
            // 
            this.lblBaseUrl.AutoSize = true;
            this.lblBaseUrl.Location = new System.Drawing.Point(12, 15);
            this.lblBaseUrl.Name = "lblBaseUrl";
            this.lblBaseUrl.Size = new System.Drawing.Size(48, 13);
            this.lblBaseUrl.TabIndex = 0;
            this.lblBaseUrl.Text = "API URL:";
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(95, 12);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(510, 20);
            this.txtBaseUrl.TabIndex = 1;
            // 
            // lblApiKey
            // 
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Location = new System.Drawing.Point(12, 41);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(48, 13);
            this.lblApiKey.TabIndex = 2;
            this.lblApiKey.Text = "API Key:";
            // 
            // txtApiKey
            // 
            this.txtApiKey.Location = new System.Drawing.Point(95, 38);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.Size = new System.Drawing.Size(510, 20);
            this.txtApiKey.TabIndex = 3;
            // 
            // btnCheckStatus
            // 
            this.btnCheckStatus.Location = new System.Drawing.Point(95, 72);
            this.btnCheckStatus.Name = "btnCheckStatus";
            this.btnCheckStatus.Size = new System.Drawing.Size(150, 30);
            this.btnCheckStatus.TabIndex = 4;
            this.btnCheckStatus.Text = "Check Robot Status";
            this.btnCheckStatus.UseVisualStyleBackColor = true;
            this.btnCheckStatus.Click += new System.EventHandler(this.btnCheckStatus_Click);
            // 
            // lblApiConnected
            // 
            this.lblApiConnected.AutoSize = true;
            this.lblApiConnected.Location = new System.Drawing.Point(12, 120);
            this.lblApiConnected.Name = "lblApiConnected";
            this.lblApiConnected.Size = new System.Drawing.Size(77, 13);
            this.lblApiConnected.TabIndex = 5;
            this.lblApiConnected.Text = "API connected:";
            // 
            // lblApiConnectedValue
            // 
            this.lblApiConnectedValue.AutoSize = true;
            this.lblApiConnectedValue.Location = new System.Drawing.Point(95, 120);
            this.lblApiConnectedValue.Name = "lblApiConnectedValue";
            this.lblApiConnectedValue.Size = new System.Drawing.Size(81, 13);
            this.lblApiConnectedValue.TabIndex = 6;
            this.lblApiConnectedValue.Text = "Not Connected";
            // 
            // lblRobotRunOk
            // 
            this.lblRobotRunOk.AutoSize = true;
            this.lblRobotRunOk.Location = new System.Drawing.Point(12, 143);
            this.lblRobotRunOk.Name = "lblRobotRunOk";
            this.lblRobotRunOk.Size = new System.Drawing.Size(77, 13);
            this.lblRobotRunOk.TabIndex = 7;
            this.lblRobotRunOk.Text = "isRobotRunOK:";
            // 
            // lblRobotRunOkValue
            // 
            this.lblRobotRunOkValue.AutoSize = true;
            this.lblRobotRunOkValue.Location = new System.Drawing.Point(95, 143);
            this.lblRobotRunOkValue.Name = "lblRobotRunOkValue";
            this.lblRobotRunOkValue.Size = new System.Drawing.Size(32, 13);
            this.lblRobotRunOkValue.TabIndex = 8;
            this.lblRobotRunOkValue.Text = "False";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 170);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(53, 13);
            this.lblMessage.TabIndex = 9;
            this.lblMessage.Text = "Message:";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(95, 167);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(510, 120);
            this.txtMessage.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 300);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblRobotRunOkValue);
            this.Controls.Add(this.lblRobotRunOk);
            this.Controls.Add(this.lblApiConnectedValue);
            this.Controls.Add(this.lblApiConnected);
            this.Controls.Add(this.btnCheckStatus);
            this.Controls.Add(this.txtApiKey);
            this.Controls.Add(this.lblApiKey);
            this.Controls.Add(this.txtBaseUrl);
            this.Controls.Add(this.lblBaseUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Robot App Status (WinForms)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblBaseUrl;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.Label lblApiKey;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.Button btnCheckStatus;
        private System.Windows.Forms.Label lblApiConnected;
        private System.Windows.Forms.Label lblApiConnectedValue;
        private System.Windows.Forms.Label lblRobotRunOk;
        private System.Windows.Forms.Label lblRobotRunOkValue;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtMessage;
    }
}
