namespace SWE1R_Overlay
{
    partial class ControlPanel
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
            this.opt_showOverlay = new System.Windows.Forms.CheckBox();
            this.cbx_processList = new System.Windows.Forms.ComboBox();
            this.txt_selectGame = new System.Windows.Forms.Label();
            this.btn_processFind = new System.Windows.Forms.Button();
            this.gb_stateInRace = new System.Windows.Forms.GroupBox();
            this.txt_stateSpdVal = new System.Windows.Forms.Label();
            this.txt_stateLapLocVal = new System.Windows.Forms.Label();
            this.txt_stateSpdLabel = new System.Windows.Forms.Label();
            this.txt_stateLapLocLabel = new System.Windows.Forms.Label();
            this.txt_stateNoLabel = new System.Windows.Forms.Label();
            this.no_stateSel = new System.Windows.Forms.NumericUpDown();
            this.txt_stateLoadNote = new System.Windows.Forms.Label();
            this.txt_stateSaveNote = new System.Windows.Forms.Label();
            this.btn_stateL = new System.Windows.Forms.Button();
            this.btn_stateS = new System.Windows.Forms.Button();
            this.gb_debug = new System.Windows.Forms.GroupBox();
            this.opt_showTerrainFlags = new System.Windows.Forms.CheckBox();
            this.opt_enableInvincibility = new System.Windows.Forms.CheckBox();
            this.opt_enableDebugMenu = new System.Windows.Forms.CheckBox();
            this.gb_stateInRace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.no_stateSel)).BeginInit();
            this.gb_debug.SuspendLayout();
            this.SuspendLayout();
            // 
            // opt_showOverlay
            // 
            this.opt_showOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.opt_showOverlay.AutoSize = true;
            this.opt_showOverlay.Location = new System.Drawing.Point(11, 278);
            this.opt_showOverlay.Name = "opt_showOverlay";
            this.opt_showOverlay.Size = new System.Drawing.Size(92, 17);
            this.opt_showOverlay.TabIndex = 0;
            this.opt_showOverlay.Text = "Show Overlay";
            this.opt_showOverlay.UseVisualStyleBackColor = true;
            this.opt_showOverlay.Visible = false;
            this.opt_showOverlay.CheckedChanged += new System.EventHandler(this.Opt_showOverlay_CheckedChanged);
            // 
            // cbx_processList
            // 
            this.cbx_processList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbx_processList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_processList.FormattingEnabled = true;
            this.cbx_processList.Location = new System.Drawing.Point(11, 297);
            this.cbx_processList.Name = "cbx_processList";
            this.cbx_processList.Size = new System.Drawing.Size(115, 21);
            this.cbx_processList.TabIndex = 9;
            this.cbx_processList.DropDown += new System.EventHandler(this.Cbx_processList_DropDown);
            this.cbx_processList.SelectionChangeCommitted += new System.EventHandler(this.Cbx_processList_SelectionChangeCommitted);
            // 
            // txt_selectGame
            // 
            this.txt_selectGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txt_selectGame.AutoSize = true;
            this.txt_selectGame.ForeColor = System.Drawing.Color.Red;
            this.txt_selectGame.Location = new System.Drawing.Point(11, 279);
            this.txt_selectGame.Name = "txt_selectGame";
            this.txt_selectGame.Size = new System.Drawing.Size(110, 13);
            this.txt_selectGame.TabIndex = 10;
            this.txt_selectGame.Text = "Select Game Window";
            // 
            // btn_processFind
            // 
            this.btn_processFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_processFind.Location = new System.Drawing.Point(132, 296);
            this.btn_processFind.Name = "btn_processFind";
            this.btn_processFind.Size = new System.Drawing.Size(48, 23);
            this.btn_processFind.TabIndex = 11;
            this.btn_processFind.Text = "Find";
            this.btn_processFind.UseVisualStyleBackColor = true;
            this.btn_processFind.Click += new System.EventHandler(this.Btn_processFind_Click);
            // 
            // gb_stateInRace
            // 
            this.gb_stateInRace.Controls.Add(this.txt_stateSpdVal);
            this.gb_stateInRace.Controls.Add(this.txt_stateLapLocVal);
            this.gb_stateInRace.Controls.Add(this.txt_stateSpdLabel);
            this.gb_stateInRace.Controls.Add(this.txt_stateLapLocLabel);
            this.gb_stateInRace.Controls.Add(this.txt_stateNoLabel);
            this.gb_stateInRace.Controls.Add(this.no_stateSel);
            this.gb_stateInRace.Controls.Add(this.txt_stateLoadNote);
            this.gb_stateInRace.Controls.Add(this.txt_stateSaveNote);
            this.gb_stateInRace.Controls.Add(this.btn_stateL);
            this.gb_stateInRace.Controls.Add(this.btn_stateS);
            this.gb_stateInRace.Enabled = false;
            this.gb_stateInRace.Location = new System.Drawing.Point(11, 12);
            this.gb_stateInRace.Margin = new System.Windows.Forms.Padding(8);
            this.gb_stateInRace.Name = "gb_stateInRace";
            this.gb_stateInRace.Padding = new System.Windows.Forms.Padding(8);
            this.gb_stateInRace.Size = new System.Drawing.Size(169, 145);
            this.gb_stateInRace.TabIndex = 13;
            this.gb_stateInRace.TabStop = false;
            this.gb_stateInRace.Text = "In-Race Savestate";
            // 
            // txt_stateSpdVal
            // 
            this.txt_stateSpdVal.AutoSize = true;
            this.txt_stateSpdVal.Location = new System.Drawing.Point(88, 67);
            this.txt_stateSpdVal.Name = "txt_stateSpdVal";
            this.txt_stateSpdVal.Size = new System.Drawing.Size(10, 13);
            this.txt_stateSpdVal.TabIndex = 23;
            this.txt_stateSpdVal.Text = "-";
            // 
            // txt_stateLapLocVal
            // 
            this.txt_stateLapLocVal.AutoSize = true;
            this.txt_stateLapLocVal.Location = new System.Drawing.Point(88, 51);
            this.txt_stateLapLocVal.Name = "txt_stateLapLocVal";
            this.txt_stateLapLocVal.Size = new System.Drawing.Size(10, 13);
            this.txt_stateLapLocVal.TabIndex = 22;
            this.txt_stateLapLocVal.Text = "-";
            // 
            // txt_stateSpdLabel
            // 
            this.txt_stateSpdLabel.AutoSize = true;
            this.txt_stateSpdLabel.Location = new System.Drawing.Point(12, 67);
            this.txt_stateSpdLabel.Margin = new System.Windows.Forms.Padding(3);
            this.txt_stateSpdLabel.Name = "txt_stateSpdLabel";
            this.txt_stateSpdLabel.Size = new System.Drawing.Size(38, 13);
            this.txt_stateSpdLabel.TabIndex = 21;
            this.txt_stateSpdLabel.Text = "Speed";
            // 
            // txt_stateLapLocLabel
            // 
            this.txt_stateLapLocLabel.AutoSize = true;
            this.txt_stateLapLocLabel.Location = new System.Drawing.Point(12, 51);
            this.txt_stateLapLocLabel.Name = "txt_stateLapLocLabel";
            this.txt_stateLapLocLabel.Size = new System.Drawing.Size(69, 13);
            this.txt_stateLapLocLabel.TabIndex = 20;
            this.txt_stateLapLocLabel.Text = "Lap Location";
            // 
            // txt_stateNoLabel
            // 
            this.txt_stateNoLabel.AutoSize = true;
            this.txt_stateNoLabel.Location = new System.Drawing.Point(57, 28);
            this.txt_stateNoLabel.Name = "txt_stateNoLabel";
            this.txt_stateNoLabel.Size = new System.Drawing.Size(25, 13);
            this.txt_stateNoLabel.TabIndex = 19;
            this.txt_stateNoLabel.Text = "Slot";
            // 
            // no_stateSel
            // 
            this.no_stateSel.Enabled = false;
            this.no_stateSel.Location = new System.Drawing.Point(11, 24);
            this.no_stateSel.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.no_stateSel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.no_stateSel.Name = "no_stateSel";
            this.no_stateSel.Size = new System.Drawing.Size(40, 20);
            this.no_stateSel.TabIndex = 18;
            this.no_stateSel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.no_stateSel.ValueChanged += new System.EventHandler(this.No_stateSel_ValueChanged);
            // 
            // txt_stateLoadNote
            // 
            this.txt_stateLoadNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txt_stateLoadNote.AutoSize = true;
            this.txt_stateLoadNote.Location = new System.Drawing.Point(65, 116);
            this.txt_stateLoadNote.Name = "txt_stateLoadNote";
            this.txt_stateLoadNote.Size = new System.Drawing.Size(79, 13);
            this.txt_stateLoadNote.TabIndex = 17;
            this.txt_stateLoadNote.Text = "X360 DPad Up";
            // 
            // txt_stateSaveNote
            // 
            this.txt_stateSaveNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txt_stateSaveNote.AutoSize = true;
            this.txt_stateSaveNote.Location = new System.Drawing.Point(65, 93);
            this.txt_stateSaveNote.Name = "txt_stateSaveNote";
            this.txt_stateSaveNote.Size = new System.Drawing.Size(79, 13);
            this.txt_stateSaveNote.TabIndex = 16;
            this.txt_stateSaveNote.Text = "X360 DPad Dn";
            // 
            // btn_stateL
            // 
            this.btn_stateL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_stateL.Enabled = false;
            this.btn_stateL.Location = new System.Drawing.Point(11, 111);
            this.btn_stateL.Margin = new System.Windows.Forms.Padding(0);
            this.btn_stateL.Name = "btn_stateL";
            this.btn_stateL.Size = new System.Drawing.Size(48, 23);
            this.btn_stateL.TabIndex = 14;
            this.btn_stateL.Text = "Load";
            this.btn_stateL.UseVisualStyleBackColor = true;
            this.btn_stateL.Click += new System.EventHandler(this.Btn_stateL_Click);
            // 
            // btn_stateS
            // 
            this.btn_stateS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_stateS.Location = new System.Drawing.Point(11, 88);
            this.btn_stateS.Margin = new System.Windows.Forms.Padding(0);
            this.btn_stateS.Name = "btn_stateS";
            this.btn_stateS.Size = new System.Drawing.Size(48, 23);
            this.btn_stateS.TabIndex = 13;
            this.btn_stateS.Text = "Save";
            this.btn_stateS.UseVisualStyleBackColor = true;
            this.btn_stateS.Click += new System.EventHandler(this.Btn_stateS_Click);
            // 
            // gb_debug
            // 
            this.gb_debug.Controls.Add(this.opt_showTerrainFlags);
            this.gb_debug.Controls.Add(this.opt_enableInvincibility);
            this.gb_debug.Controls.Add(this.opt_enableDebugMenu);
            this.gb_debug.Enabled = false;
            this.gb_debug.Location = new System.Drawing.Point(11, 166);
            this.gb_debug.Margin = new System.Windows.Forms.Padding(8);
            this.gb_debug.Name = "gb_debug";
            this.gb_debug.Padding = new System.Windows.Forms.Padding(8);
            this.gb_debug.Size = new System.Drawing.Size(169, 97);
            this.gb_debug.TabIndex = 14;
            this.gb_debug.TabStop = false;
            this.gb_debug.Text = "Debug";
            // 
            // opt_showTerrainFlags
            // 
            this.opt_showTerrainFlags.AutoSize = true;
            this.opt_showTerrainFlags.Location = new System.Drawing.Point(11, 71);
            this.opt_showTerrainFlags.Name = "opt_showTerrainFlags";
            this.opt_showTerrainFlags.Size = new System.Drawing.Size(93, 17);
            this.opt_showTerrainFlags.TabIndex = 9;
            this.opt_showTerrainFlags.Text = "Terrain Labels";
            this.opt_showTerrainFlags.UseVisualStyleBackColor = true;
            this.opt_showTerrainFlags.CheckedChanged += new System.EventHandler(this.Opt_showTerrainFlags_CheckedChanged);
            // 
            // opt_enableInvincibility
            // 
            this.opt_enableInvincibility.AutoSize = true;
            this.opt_enableInvincibility.Location = new System.Drawing.Point(11, 48);
            this.opt_enableInvincibility.Name = "opt_enableInvincibility";
            this.opt_enableInvincibility.Size = new System.Drawing.Size(77, 17);
            this.opt_enableInvincibility.TabIndex = 8;
            this.opt_enableInvincibility.Text = "Invincibility";
            this.opt_enableInvincibility.UseVisualStyleBackColor = true;
            this.opt_enableInvincibility.CheckedChanged += new System.EventHandler(this.Opt_enableInvincibility_CheckedChanged);
            // 
            // opt_enableDebugMenu
            // 
            this.opt_enableDebugMenu.AutoSize = true;
            this.opt_enableDebugMenu.Location = new System.Drawing.Point(11, 24);
            this.opt_enableDebugMenu.Name = "opt_enableDebugMenu";
            this.opt_enableDebugMenu.Size = new System.Drawing.Size(88, 17);
            this.opt_enableDebugMenu.TabIndex = 7;
            this.opt_enableDebugMenu.Text = "Debug Menu";
            this.opt_enableDebugMenu.UseVisualStyleBackColor = true;
            this.opt_enableDebugMenu.CheckedChanged += new System.EventHandler(this.Opt_enableDebugMenu_CheckedChanged);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192, 330);
            this.Controls.Add(this.gb_debug);
            this.Controls.Add(this.gb_stateInRace);
            this.Controls.Add(this.btn_processFind);
            this.Controls.Add(this.txt_selectGame);
            this.Controls.Add(this.cbx_processList);
            this.Controls.Add(this.opt_showOverlay);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(288, 512);
            this.MinimumSize = new System.Drawing.Size(128, 192);
            this.Name = "ControlPanel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SWE1R Controls";
            this.gb_stateInRace.ResumeLayout(false);
            this.gb_stateInRace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.no_stateSel)).EndInit();
            this.gb_debug.ResumeLayout(false);
            this.gb_debug.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox opt_showOverlay;
        private System.Windows.Forms.ComboBox cbx_processList;
        private System.Windows.Forms.Label txt_selectGame;
        private System.Windows.Forms.Button btn_processFind;
        private System.Windows.Forms.GroupBox gb_stateInRace;
        private System.Windows.Forms.Label txt_stateSpdVal;
        private System.Windows.Forms.Label txt_stateLapLocVal;
        private System.Windows.Forms.Label txt_stateSpdLabel;
        private System.Windows.Forms.Label txt_stateLapLocLabel;
        private System.Windows.Forms.Label txt_stateNoLabel;
        private System.Windows.Forms.NumericUpDown no_stateSel;
        private System.Windows.Forms.Label txt_stateLoadNote;
        private System.Windows.Forms.Label txt_stateSaveNote;
        private System.Windows.Forms.Button btn_stateL;
        private System.Windows.Forms.Button btn_stateS;
        private System.Windows.Forms.GroupBox gb_debug;
        private System.Windows.Forms.CheckBox opt_showTerrainFlags;
        private System.Windows.Forms.CheckBox opt_enableInvincibility;
        private System.Windows.Forms.CheckBox opt_enableDebugMenu;
    }
}

