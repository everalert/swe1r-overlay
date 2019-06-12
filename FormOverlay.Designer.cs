namespace SWE1R_Overlay
{
    partial class FormOverlay
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
            this.txt_heatTimers = new System.Windows.Forms.Label();
            this.txt_debug = new System.Windows.Forms.Label();
            this.txt_lapTimes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_heatTimers
            // 
            this.txt_heatTimers.AutoSize = true;
            this.txt_heatTimers.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_heatTimers.ForeColor = System.Drawing.Color.Azure;
            this.txt_heatTimers.Location = new System.Drawing.Point(1008, 400);
            this.txt_heatTimers.Name = "txt_heatTimers";
            this.txt_heatTimers.Size = new System.Drawing.Size(18, 19);
            this.txt_heatTimers.TabIndex = 0;
            this.txt_heatTimers.Text = "0";
            // 
            // txt_debug
            // 
            this.txt_debug.AutoSize = true;
            this.txt_debug.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.txt_debug.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_debug.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txt_debug.Location = new System.Drawing.Point(10, 32);
            this.txt_debug.Name = "txt_debug";
            this.txt_debug.Size = new System.Drawing.Size(0, 13);
            this.txt_debug.TabIndex = 1;
            // 
            // txt_lapTimes
            // 
            this.txt_lapTimes.AutoSize = true;
            this.txt_lapTimes.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_lapTimes.ForeColor = System.Drawing.Color.Azure;
            this.txt_lapTimes.Location = new System.Drawing.Point(85, 201);
            this.txt_lapTimes.Name = "txt_lapTimes";
            this.txt_lapTimes.Size = new System.Drawing.Size(18, 19);
            this.txt_lapTimes.TabIndex = 2;
            this.txt_lapTimes.Text = "0";
            // 
            // FormOverlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.txt_lapTimes);
            this.Controls.Add(this.txt_debug);
            this.Controls.Add(this.txt_heatTimers);
            this.Name = "FormOverlay";
            this.Padding = new System.Windows.Forms.Padding(8, 32, 8, 8);
            this.ShowInTaskbar = false;
            this.Text = "Overlay";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormOverlay_Load);
            //this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormOverlay_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txt_heatTimers;
        private System.Windows.Forms.Label txt_debug;
        private System.Windows.Forms.Label txt_lapTimes;
    }
}