namespace SharpTibiaProxy.Util
{
    partial class ClientChooser
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
            this.uxClients = new System.Windows.Forms.ComboBox();
            this.uxChoose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uxLoginServer = new System.Windows.Forms.ComboBox();
            this.uxLoginServerLabel = new System.Windows.Forms.Label();
            this.uxUseOT = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxClients
            // 
            this.uxClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxClients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxClients.FormattingEnabled = true;
            this.uxClients.Location = new System.Drawing.Point(6, 6);
            this.uxClients.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxClients.Name = "uxClients";
            this.uxClients.Size = new System.Drawing.Size(314, 28);
            this.uxClients.TabIndex = 0;
            this.uxClients.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CommonKeyUp);
            // 
            // uxChoose
            // 
            this.uxChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxChoose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxChoose.Location = new System.Drawing.Point(332, 5);
            this.uxChoose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxChoose.Name = "uxChoose";
            this.uxChoose.Size = new System.Drawing.Size(93, 35);
            this.uxChoose.TabIndex = 1;
            this.uxChoose.Text = "Choose";
            this.uxChoose.UseVisualStyleBackColor = true;
            this.uxChoose.Click += new System.EventHandler(this.uxChoose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.uxLoginServer);
            this.groupBox1.Controls.Add(this.uxLoginServerLabel);
            this.groupBox1.Controls.Add(this.uxUseOT);
            this.groupBox1.Location = new System.Drawing.Point(6, 49);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(418, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // uxLoginServer
            // 
            this.uxLoginServer.FormattingEnabled = true;
            this.uxLoginServer.Location = new System.Drawing.Point(102, 31);
            this.uxLoginServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxLoginServer.Name = "uxLoginServer";
            this.uxLoginServer.Size = new System.Drawing.Size(302, 28);
            this.uxLoginServer.TabIndex = 5;
            // 
            // uxLoginServerLabel
            // 
            this.uxLoginServerLabel.AutoSize = true;
            this.uxLoginServerLabel.Enabled = false;
            this.uxLoginServerLabel.Location = new System.Drawing.Point(10, 35);
            this.uxLoginServerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uxLoginServerLabel.Name = "uxLoginServerLabel";
            this.uxLoginServerLabel.Size = new System.Drawing.Size(88, 20);
            this.uxLoginServerLabel.TabIndex = 4;
            this.uxLoginServerLabel.Text = "Server:Port";
            // 
            // uxUseOT
            // 
            this.uxUseOT.AutoSize = true;
            this.uxUseOT.Location = new System.Drawing.Point(12, 0);
            this.uxUseOT.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uxUseOT.Name = "uxUseOT";
            this.uxUseOT.Size = new System.Drawing.Size(216, 24);
            this.uxUseOT.TabIndex = 2;
            this.uxUseOT.Text = "Use an Open Tibia Server";
            this.uxUseOT.UseVisualStyleBackColor = true;
            this.uxUseOT.CheckedChanged += new System.EventHandler(this.uxUseOT_CheckedChanged);
            // 
            // ClientChooser
            // 
            this.AcceptButton = this.uxChoose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 131);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.uxChoose);
            this.Controls.Add(this.uxClients);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClientChooser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose a Client";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox uxClients;
        private System.Windows.Forms.Button uxChoose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label uxLoginServerLabel;
        private System.Windows.Forms.CheckBox uxUseOT;
        private System.Windows.Forms.ComboBox uxLoginServer;
    }
}