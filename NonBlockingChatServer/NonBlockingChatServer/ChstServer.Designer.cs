namespace NonBlockingChatServer
{
    partial class ChatServer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelAddr = new System.Windows.Forms.Label();
            this.inputAddr = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.inputPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.outputMsg = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelAddr
            // 
            this.labelAddr.AutoSize = true;
            this.labelAddr.Location = new System.Drawing.Point(11, 12);
            this.labelAddr.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAddr.Name = "labelAddr";
            this.labelAddr.Size = new System.Drawing.Size(59, 15);
            this.labelAddr.TabIndex = 0;
            this.labelAddr.Text = "서버 주소";
            // 
            // inputAddr
            // 
            this.inputAddr.Location = new System.Drawing.Point(73, 10);
            this.inputAddr.Margin = new System.Windows.Forms.Padding(2);
            this.inputAddr.Name = "inputAddr";
            this.inputAddr.Size = new System.Drawing.Size(98, 23);
            this.inputAddr.TabIndex = 1;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(175, 12);
            this.labelPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(59, 15);
            this.labelPort.TabIndex = 2;
            this.labelPort.Text = "포트 번호";
            // 
            // inputPort
            // 
            this.inputPort.Location = new System.Drawing.Point(237, 10);
            this.inputPort.Margin = new System.Windows.Forms.Padding(2);
            this.inputPort.Name = "inputPort";
            this.inputPort.Size = new System.Drawing.Size(98, 23);
            this.inputPort.TabIndex = 3;
            this.inputPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(339, 9);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(73, 22);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "생성";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.OnStart);
            // 
            // outputMsg
            // 
            this.outputMsg.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.outputMsg.Location = new System.Drawing.Point(11, 40);
            this.outputMsg.Margin = new System.Windows.Forms.Padding(2);
            this.outputMsg.Multiline = true;
            this.outputMsg.Name = "outputMsg";
            this.outputMsg.ReadOnly = true;
            this.outputMsg.Size = new System.Drawing.Size(402, 296);
            this.outputMsg.TabIndex = 7;
            // 
            // ChatServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 347);
            this.Controls.Add(this.outputMsg);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.inputPort);
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.inputAddr);
            this.Controls.Add(this.labelAddr);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChatServer";
            this.Text = "Chat_Server";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAddr;
        private System.Windows.Forms.TextBox inputAddr;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.TextBox inputPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox outputMsg;
    }
}

