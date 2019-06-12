using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SWE1R_Overlay.Utilities;

namespace SWE1R_Overlay
{
    public partial class FormOverlay : Form
    {
        public const string WINDOW_NAME = "Episode I Racer";
        Interop.RECT rect;
        IntPtr WINDOW_HANDLE = Interop.FindWindow(null, WINDOW_NAME);

        readonly int[] border = { 10, 32, 10, 10 };
        readonly int[] size = { 1280, 720 };
        readonly string time_format = "m\\:ss\\.fff";

        RacerData racer = new RacerData();

        public FormOverlay()
        {
            InitializeComponent();
            Application.Idle += UpdateOverlay;
        }

        private void UpdateOverlay(object sender, EventArgs e)
        {
            while (Interop.IsApplicationIdle())
            {
                Interop.GetWindowRect(WINDOW_HANDLE, out rect);
                this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
                this.Top = rect.top;
                this.Left = rect.left;
                int[] size = { this.Width - border[0] - border[2], this.Height - border[1] - border[3] };
                Font fnt_neutral = new Font("Consolas", 12 * (this.Width - border[0] - border[2]) / 1280, FontStyle.Bold);

                float heat = racer.GetPodDataHeat();
                float heat_rate = racer.GetPodDataHeatRate();
                float cool_rate = racer.GetPodDataCoolRate();
                txt_heatTimers.Location = new Point(border[0] + (int)(1032m / 1280m * size[0]), border[1] + (int)(448m / 720m * size[1]));
                txt_heatTimers.Font = fnt_neutral;
                txt_heatTimers.Text = "OH "+(heat / heat_rate).ToString("00.00s")+"\rUH "+((100-heat) / cool_rate).ToString("00.00s");

                float[] race_time = racer.GetPodTimeALL();
                txt_lapTimes.Location = new Point(border[0] + (int)(120m / 1280m * size[0]), border[1] + (int)(176m / 720m * size[1]));
                txt_lapTimes.Font = fnt_neutral;
                txt_lapTimes.Text = ("1  " + TimeSpan.FromSeconds(race_time[0]).ToString(time_format)) +
                    ((race_time[1] >= 0) ? "\r2  " + TimeSpan.FromSeconds(race_time[1]).ToString(time_format) : "") +
                    ((race_time[2] >= 0) ? "\r3  " + TimeSpan.FromSeconds(race_time[2]).ToString(time_format) : "") +
                    ((race_time[3] >= 0) ? "\r4  " + TimeSpan.FromSeconds(race_time[3]).ToString(time_format) : "") +
                    ((race_time[4] >= 0) ? "\r5  " + TimeSpan.FromSeconds(race_time[4]).ToString(time_format) : "") +
                    ("\rT  " + TimeSpan.FromSeconds(race_time[5]).ToString(time_format));

                //Rectangle olRect = RectangleToScreen(this.ClientRectangle);
                //int[] olRectBdr = { olRect.Top-this.Top, olRect.Left-this.Left };
                //txt_debug.Text = "AIRBORNE "+((RacerData.GetPodDataFlags2()&(1<<9))!=0?true:false).ToString();
            }
        }

        private void FormOverlay_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
        }
    }
}
