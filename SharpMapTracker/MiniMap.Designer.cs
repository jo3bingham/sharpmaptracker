namespace SharpMapTracker
{
    partial class MiniMap
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.coorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // coorLabel
            // 
            this.coorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coorLabel.AutoSize = true;
            this.coorLabel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.coorLabel.Location = new System.Drawing.Point(-3, 295);
            this.coorLabel.Name = "coorLabel";
            this.coorLabel.Size = new System.Drawing.Size(0, 13);
            this.coorLabel.TabIndex = 0;
            // 
            // MiniMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.coorLabel);
            this.DoubleBuffered = true;
            this.Name = "MiniMap";
            this.Size = new System.Drawing.Size(342, 308);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label coorLabel;
    }
}
