namespace DiscordAngryBot.GUI
{
    partial class MainForm
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
            this.messageTextBox = new System.Windows.Forms.RichTextBox();
            this.serverChooserBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.channelChooserBox = new System.Windows.Forms.ComboBox();
            this.messageSender = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // messageTextBox
            // 
            this.messageTextBox.Location = new System.Drawing.Point(12, 64);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(615, 96);
            this.messageTextBox.TabIndex = 0;
            this.messageTextBox.Text = "";
            // 
            // serverChooserBox
            // 
            this.serverChooserBox.FormattingEnabled = true;
            this.serverChooserBox.Location = new System.Drawing.Point(13, 37);
            this.serverChooserBox.Name = "serverChooserBox";
            this.serverChooserBox.Size = new System.Drawing.Size(169, 21);
            this.serverChooserBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(197, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Channel";
            // 
            // channelChooserBox
            // 
            this.channelChooserBox.FormattingEnabled = true;
            this.channelChooserBox.Location = new System.Drawing.Point(200, 37);
            this.channelChooserBox.Name = "channelChooserBox";
            this.channelChooserBox.Size = new System.Drawing.Size(169, 21);
            this.channelChooserBox.TabIndex = 4;
            // 
            // messageSender
            // 
            this.messageSender.Location = new System.Drawing.Point(12, 167);
            this.messageSender.Name = "messageSender";
            this.messageSender.Size = new System.Drawing.Size(615, 57);
            this.messageSender.TabIndex = 5;
            this.messageSender.Text = "Send message";
            this.messageSender.UseVisualStyleBackColor = true;
            this.messageSender.Click += new System.EventHandler(this.messageSender_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 247);
            this.Controls.Add(this.messageSender);
            this.Controls.Add(this.channelChooserBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverChooserBox);
            this.Controls.Add(this.messageTextBox);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox messageTextBox;
        private System.Windows.Forms.ComboBox serverChooserBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox channelChooserBox;
        private System.Windows.Forms.Button messageSender;
    }
}