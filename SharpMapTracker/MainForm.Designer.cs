namespace SharpMapTracker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.traceTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.trackTibiaCastFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNPCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackMoveableItemsCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.trackSplashesCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.trackMonstersCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.trackNpcsCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.trackOnlyCurrentFloorCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.retrackTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.npcAutoTalkCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.alwaysOnTopCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.highlitghtMissingTilesCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.shareTrackedMapCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tileCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.npcCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.monsterCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.miniMap = new SharpMapTracker.MiniMap();
            this.changeIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // traceTextBox
            // 
            this.traceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traceTextBox.Location = new System.Drawing.Point(13, 351);
            this.traceTextBox.Multiline = true;
            this.traceTextBox.Name = "traceTextBox";
            this.traceTextBox.ReadOnly = true;
            this.traceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.traceTextBox.Size = new System.Drawing.Size(318, 114);
            this.traceTextBox.TabIndex = 0;
            this.traceTextBox.TabStop = false;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(343, 24);
            this.menuStrip.TabIndex = 20;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadClientToolStripMenuItem,
            this.loadMapToolStripMenuItem,
            this.toolStripSeparator4,
            this.trackTibiaCastFilesToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveMapToolStripMenuItem,
            this.saveNPCsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem,
            this.changeIPToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadClientToolStripMenuItem
            // 
            this.loadClientToolStripMenuItem.Name = "loadClientToolStripMenuItem";
            this.loadClientToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.loadClientToolStripMenuItem.Text = "Load &Client...";
            this.loadClientToolStripMenuItem.Click += new System.EventHandler(this.loadClientToolStripMenuItem_Click);
            // 
            // loadMapToolStripMenuItem
            // 
            this.loadMapToolStripMenuItem.Name = "loadMapToolStripMenuItem";
            this.loadMapToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.loadMapToolStripMenuItem.Text = "Load &Map...";
            this.loadMapToolStripMenuItem.Click += new System.EventHandler(this.loadMapToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(187, 6);
            // 
            // trackTibiaCastFilesToolStripMenuItem
            // 
            this.trackTibiaCastFilesToolStripMenuItem.Name = "trackTibiaCastFilesToolStripMenuItem";
            this.trackTibiaCastFilesToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.trackTibiaCastFilesToolStripMenuItem.Text = "&Track TibiaCast Files...";
            this.trackTibiaCastFilesToolStripMenuItem.Click += new System.EventHandler(this.trackTibiaCastFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveMapToolStripMenuItem.Text = "&Save Map...";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // saveNPCsToolStripMenuItem
            // 
            this.saveNPCsToolStripMenuItem.Name = "saveNPCsToolStripMenuItem";
            this.saveNPCsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveNPCsToolStripMenuItem.Text = "Save &NPCs...";
            this.saveNPCsToolStripMenuItem.Click += new System.EventHandler(this.saveNPCsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(187, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trackMoveableItemsCheckBox,
            this.trackSplashesCheckBox,
            this.trackMonstersCheckBox,
            this.trackNpcsCheckBox,
            this.trackOnlyCurrentFloorCheckBox,
            this.retrackTilesToolStripMenuItem,
            this.npcAutoTalkCheckBox,
            this.toolStripSeparator3,
            this.alwaysOnTopCheckBox,
            this.highlitghtMissingTilesCheckBox,
            this.shareTrackedMapCheckBox,
            this.toolStripSeparator5,
            this.clearToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // trackMoveableItemsCheckBox
            // 
            this.trackMoveableItemsCheckBox.Checked = true;
            this.trackMoveableItemsCheckBox.CheckOnClick = true;
            this.trackMoveableItemsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.trackMoveableItemsCheckBox.Name = "trackMoveableItemsCheckBox";
            this.trackMoveableItemsCheckBox.Size = new System.Drawing.Size(204, 22);
            this.trackMoveableItemsCheckBox.Text = "Track Moveable Items";
            // 
            // trackSplashesCheckBox
            // 
            this.trackSplashesCheckBox.Checked = true;
            this.trackSplashesCheckBox.CheckOnClick = true;
            this.trackSplashesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.trackSplashesCheckBox.Name = "trackSplashesCheckBox";
            this.trackSplashesCheckBox.Size = new System.Drawing.Size(204, 22);
            this.trackSplashesCheckBox.Text = "Track Splashes";
            // 
            // trackMonstersCheckBox
            // 
            this.trackMonstersCheckBox.Checked = true;
            this.trackMonstersCheckBox.CheckOnClick = true;
            this.trackMonstersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.trackMonstersCheckBox.Name = "trackMonstersCheckBox";
            this.trackMonstersCheckBox.Size = new System.Drawing.Size(204, 22);
            this.trackMonstersCheckBox.Text = "Track Monsters";
            // 
            // trackNpcsCheckBox
            // 
            this.trackNpcsCheckBox.Checked = true;
            this.trackNpcsCheckBox.CheckOnClick = true;
            this.trackNpcsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.trackNpcsCheckBox.Name = "trackNpcsCheckBox";
            this.trackNpcsCheckBox.Size = new System.Drawing.Size(204, 22);
            this.trackNpcsCheckBox.Text = "Track NPCs";
            // 
            // trackOnlyCurrentFloorCheckBox
            // 
            this.trackOnlyCurrentFloorCheckBox.Name = "trackOnlyCurrentFloorCheckBox";
            this.trackOnlyCurrentFloorCheckBox.Size = new System.Drawing.Size(204, 22);
            this.trackOnlyCurrentFloorCheckBox.Text = "Track Only Current Floor";
            // 
            // retrackTilesToolStripMenuItem
            // 
            this.retrackTilesToolStripMenuItem.CheckOnClick = true;
            this.retrackTilesToolStripMenuItem.Name = "retrackTilesToolStripMenuItem";
            this.retrackTilesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.retrackTilesToolStripMenuItem.Text = "Retrack Tiles";
            // 
            // npcAutoTalkCheckBox
            // 
            this.npcAutoTalkCheckBox.CheckOnClick = true;
            this.npcAutoTalkCheckBox.Name = "npcAutoTalkCheckBox";
            this.npcAutoTalkCheckBox.Size = new System.Drawing.Size(204, 22);
            this.npcAutoTalkCheckBox.Text = "NPC Auto Talk";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(201, 6);
            // 
            // alwaysOnTopCheckBox
            // 
            this.alwaysOnTopCheckBox.Checked = true;
            this.alwaysOnTopCheckBox.CheckOnClick = true;
            this.alwaysOnTopCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
            this.alwaysOnTopCheckBox.Size = new System.Drawing.Size(204, 22);
            this.alwaysOnTopCheckBox.Text = "Always On Top";
            // 
            // highlitghtMissingTilesCheckBox
            // 
            this.highlitghtMissingTilesCheckBox.CheckOnClick = true;
            this.highlitghtMissingTilesCheckBox.Name = "highlitghtMissingTilesCheckBox";
            this.highlitghtMissingTilesCheckBox.Size = new System.Drawing.Size(204, 22);
            this.highlitghtMissingTilesCheckBox.Text = "Highlitght Missing Tiles";
            this.highlitghtMissingTilesCheckBox.Click += new System.EventHandler(this.highlitghtMissingTilesToolStripMenuItem_Click);
            // 
            // shareTrackedMapCheckBox
            // 
            this.shareTrackedMapCheckBox.Checked = true;
            this.shareTrackedMapCheckBox.CheckOnClick = true;
            this.shareTrackedMapCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.shareTrackedMapCheckBox.Name = "shareTrackedMapCheckBox";
            this.shareTrackedMapCheckBox.Size = new System.Drawing.Size(204, 22);
            this.shareTrackedMapCheckBox.Text = "Share Tracked Map";
            this.shareTrackedMapCheckBox.Click += new System.EventHandler(this.shareTrackedMapCheckBox_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(201, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.clearToolStripMenuItem.Text = "&Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileCountLabel,
            this.npcCountLabel,
            this.monsterCountLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 469);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(343, 24);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 21;
            // 
            // tileCountLabel
            // 
            this.tileCountLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tileCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tileCountLabel.Name = "tileCountLabel";
            this.tileCountLabel.Size = new System.Drawing.Size(47, 19);
            this.tileCountLabel.Text = "Tiles: 0";
            // 
            // npcCountLabel
            // 
            this.npcCountLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.npcCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.npcCountLabel.Name = "npcCountLabel";
            this.npcCountLabel.Size = new System.Drawing.Size(52, 19);
            this.npcCountLabel.Text = "NPCs: 0";
            // 
            // monsterCountLabel
            // 
            this.monsterCountLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.monsterCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.monsterCountLabel.Name = "monsterCountLabel";
            this.monsterCountLabel.Size = new System.Drawing.Size(72, 19);
            this.monsterCountLabel.Text = "Monsters: 0";
            // 
            // miniMap
            // 
            this.miniMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.miniMap.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.miniMap.CenterLocation = null;
            this.miniMap.Floor = 0;
            this.miniMap.HighlightMissingTiles = false;
            this.miniMap.Location = new System.Drawing.Point(13, 27);
            this.miniMap.Map = null;
            this.miniMap.Name = "miniMap";
            this.miniMap.Size = new System.Drawing.Size(318, 318);
            this.miniMap.TabIndex = 15;
            // 
            // changeIPToolStripMenuItem
            // 
            this.changeIPToolStripMenuItem.Name = "changeIPToolStripMenuItem";
            this.changeIPToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.changeIPToolStripMenuItem.Text = "Change IP";
            this.changeIPToolStripMenuItem.Click += new System.EventHandler(this.changeIPToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 493);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.miniMap);
            this.Controls.Add(this.traceTextBox);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "SharpMapTracker";
            this.TopMost = true;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox traceTextBox;
        private MiniMap miniMap;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadClientToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tileCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel npcCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel monsterCountLabel;
        private System.Windows.Forms.ToolStripMenuItem trackMoveableItemsCheckBox;
        private System.Windows.Forms.ToolStripMenuItem trackSplashesCheckBox;
        private System.Windows.Forms.ToolStripMenuItem trackMonstersCheckBox;
        private System.Windows.Forms.ToolStripMenuItem trackNpcsCheckBox;
        private System.Windows.Forms.ToolStripMenuItem trackOnlyCurrentFloorCheckBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopCheckBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem trackTibiaCastFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveNPCsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem npcAutoTalkCheckBox;
        private System.Windows.Forms.ToolStripMenuItem retrackTilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highlitghtMissingTilesCheckBox;
        private System.Windows.Forms.ToolStripMenuItem shareTrackedMapCheckBox;
        private System.Windows.Forms.ToolStripMenuItem changeIPToolStripMenuItem;
    }
}