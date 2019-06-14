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
            this.btn_stateS = new System.Windows.Forms.Button();
            this.btn_stateL = new System.Windows.Forms.Button();
            this.txt_stateTitle = new System.Windows.Forms.Label();
            this.opt_enableDebugMenu = new System.Windows.Forms.CheckBox();
            this.opt_enableInvincibility = new System.Windows.Forms.CheckBox();
            this.opt_showTerrainFlags = new System.Windows.Forms.CheckBox();
            this.txt_stateSaveNote = new System.Windows.Forms.Label();
            this.txt_stateLoadNote = new System.Windows.Forms.Label();
            this.cbx_processList = new System.Windows.Forms.ComboBox();
            this.txt_selectGame = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // opt_showOverlay
            // 
            this.opt_showOverlay.AutoSize = true;
            this.opt_showOverlay.Location = new System.Drawing.Point(11, 228);
            this.opt_showOverlay.Name = "opt_showOverlay";
            this.opt_showOverlay.Size = new System.Drawing.Size(92, 17);
            this.opt_showOverlay.TabIndex = 0;
            this.opt_showOverlay.Text = "Show Overlay";
            this.opt_showOverlay.UseVisualStyleBackColor = true;
            this.opt_showOverlay.Visible = false;
            this.opt_showOverlay.CheckedChanged += new System.EventHandler(this.Opt_showOverlay_CheckedChanged);
            // 
            // btn_stateS
            // 
            this.btn_stateS.Location = new System.Drawing.Point(11, 32);
            this.btn_stateS.Name = "btn_stateS";
            this.btn_stateS.Size = new System.Drawing.Size(48, 23);
            this.btn_stateS.TabIndex = 1;
            this.btn_stateS.Text = "Save";
            this.btn_stateS.UseVisualStyleBackColor = true;
            this.btn_stateS.Click += new System.EventHandler(this.Btn_stateS_Click);
            // 
            // btn_stateL
            // 
            this.btn_stateL.Location = new System.Drawing.Point(11, 61);
            this.btn_stateL.Name = "btn_stateL";
            this.btn_stateL.Size = new System.Drawing.Size(48, 23);
            this.btn_stateL.TabIndex = 2;
            this.btn_stateL.Text = "Load";
            this.btn_stateL.UseVisualStyleBackColor = true;
            this.btn_stateL.Click += new System.EventHandler(this.Btn_stateL_Click);
            // 
            // txt_stateTitle
            // 
            this.txt_stateTitle.AutoSize = true;
            this.txt_stateTitle.Location = new System.Drawing.Point(12, 13);
            this.txt_stateTitle.Name = "txt_stateTitle";
            this.txt_stateTitle.Size = new System.Drawing.Size(101, 13);
            this.txt_stateTitle.TabIndex = 3;
            this.txt_stateTitle.Text = "In-Race Save State";
            // 
            // opt_enableDebugMenu
            // 
            this.opt_enableDebugMenu.AutoSize = true;
            this.opt_enableDebugMenu.Location = new System.Drawing.Point(11, 107);
            this.opt_enableDebugMenu.Name = "opt_enableDebugMenu";
            this.opt_enableDebugMenu.Size = new System.Drawing.Size(88, 17);
            this.opt_enableDebugMenu.TabIndex = 4;
            this.opt_enableDebugMenu.Text = "Debug Menu";
            this.opt_enableDebugMenu.UseVisualStyleBackColor = true;
            this.opt_enableDebugMenu.CheckedChanged += new System.EventHandler(this.Opt_enableDebugMenu_CheckedChanged);
            // 
            // opt_enableInvincibility
            // 
            this.opt_enableInvincibility.AutoSize = true;
            this.opt_enableInvincibility.Location = new System.Drawing.Point(11, 131);
            this.opt_enableInvincibility.Name = "opt_enableInvincibility";
            this.opt_enableInvincibility.Size = new System.Drawing.Size(77, 17);
            this.opt_enableInvincibility.TabIndex = 5;
            this.opt_enableInvincibility.Text = "Invincibility";
            this.opt_enableInvincibility.UseVisualStyleBackColor = true;
            this.opt_enableInvincibility.CheckedChanged += new System.EventHandler(this.Opt_enableInvincibility_CheckedChanged);
            // 
            // opt_showTerrainFlags
            // 
            this.opt_showTerrainFlags.AutoSize = true;
            this.opt_showTerrainFlags.Location = new System.Drawing.Point(11, 155);
            this.opt_showTerrainFlags.Name = "opt_showTerrainFlags";
            this.opt_showTerrainFlags.Size = new System.Drawing.Size(93, 17);
            this.opt_showTerrainFlags.TabIndex = 6;
            this.opt_showTerrainFlags.Text = "Terrain Labels";
            this.opt_showTerrainFlags.UseVisualStyleBackColor = true;
            this.opt_showTerrainFlags.CheckedChanged += new System.EventHandler(this.Opt_showTerrainFlags_CheckedChanged);
            // 
            // txt_stateSaveNote
            // 
            this.txt_stateSaveNote.AutoSize = true;
            this.txt_stateSaveNote.Location = new System.Drawing.Point(66, 37);
            this.txt_stateSaveNote.Name = "txt_stateSaveNote";
            this.txt_stateSaveNote.Size = new System.Drawing.Size(79, 13);
            this.txt_stateSaveNote.TabIndex = 7;
            this.txt_stateSaveNote.Text = "X360 DPad Dn";
            // 
            // txt_stateLoadNote
            // 
            this.txt_stateLoadNote.AutoSize = true;
            this.txt_stateLoadNote.Location = new System.Drawing.Point(66, 66);
            this.txt_stateLoadNote.Name = "txt_stateLoadNote";
            this.txt_stateLoadNote.Size = new System.Drawing.Size(79, 13);
            this.txt_stateLoadNote.TabIndex = 8;
            this.txt_stateLoadNote.Text = "X360 DPad Up";
            // 
            // cbx_processList
            // 
            this.cbx_processList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_processList.FormattingEnabled = true;
            this.cbx_processList.Location = new System.Drawing.Point(11, 251);
            this.cbx_processList.Name = "cbx_processList";
            this.cbx_processList.Size = new System.Drawing.Size(201, 21);
            this.cbx_processList.TabIndex = 9;
            this.cbx_processList.DropDown += new System.EventHandler(this.Cbx_processList_DropDown);
            this.cbx_processList.SelectionChangeCommitted += new System.EventHandler(this.Cbx_processList_SelectionChangeCommitted);
            // 
            // txt_selectGame
            // 
            this.txt_selectGame.AutoSize = true;
            this.txt_selectGame.Location = new System.Drawing.Point(11, 229);
            this.txt_selectGame.Name = "txt_selectGame";
            this.txt_selectGame.Size = new System.Drawing.Size(110, 13);
            this.txt_selectGame.TabIndex = 10;
            this.txt_selectGame.Text = "Select Game Window";
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 282);
            this.Controls.Add(this.txt_selectGame);
            this.Controls.Add(this.cbx_processList);
            this.Controls.Add(this.txt_stateLoadNote);
            this.Controls.Add(this.txt_stateSaveNote);
            this.Controls.Add(this.opt_showTerrainFlags);
            this.Controls.Add(this.opt_enableInvincibility);
            this.Controls.Add(this.opt_enableDebugMenu);
            this.Controls.Add(this.txt_stateTitle);
            this.Controls.Add(this.btn_stateL);
            this.Controls.Add(this.btn_stateS);
            this.Controls.Add(this.opt_showOverlay);
            this.MaximumSize = new System.Drawing.Size(240, 320);
            this.MinimumSize = new System.Drawing.Size(240, 320);
            this.Name = "ControlPanel";
            this.Text = "SWE1R Controls";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox opt_showOverlay;
        private System.Windows.Forms.Button btn_stateS;
        private System.Windows.Forms.Button btn_stateL;
        private System.Windows.Forms.Label txt_stateTitle;
        private System.Windows.Forms.CheckBox opt_enableDebugMenu;
        private System.Windows.Forms.CheckBox opt_enableInvincibility;
        private System.Windows.Forms.CheckBox opt_showTerrainFlags;
        private System.Windows.Forms.Label txt_stateSaveNote;
        private System.Windows.Forms.Label txt_stateLoadNote;
        private System.Windows.Forms.ComboBox cbx_processList;
        private System.Windows.Forms.Label txt_selectGame;
    }
}

