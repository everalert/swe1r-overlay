using SWE1R_Overlay.Utilities;
using System;
using System.Windows.Forms;

namespace SWE1R_Overlay
{
    public partial class ControlPanel : Form
    {
        // setup
        RacerData racer = new RacerData();
        private byte[] savestate_pod;

        Overlay overlay = new Overlay();


        // INIT

        public ControlPanel()
        {
            InitializeComponent();
            overlay.Text = "SWE1R Overlay";
            //overlay.ShowInTaskbar = false;   /* doesn't seem to behave as expected - but, would be nice to not clutter the taskbar/tab window since it's controlled by this form */
            overlay.controlpanel = this;
            overlay.Show();
            opt_showOverlay.Checked = true;
        }

        // CONTROLS

        private void opt_showOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_showOverlay.Checked)
                overlay.Show();
            else
                overlay.Hide();
        }
        private void Btn_stateS_Click(object sender, EventArgs e)
        {
            SaveRaceState();
        }
        private void Btn_stateL_Click(object sender, EventArgs e)
        {
            LoadRaceState();
        }
        private void Opt_enableDebugMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_enableDebugMenu.Checked)
            {
                racer.EnableDebugMenu();
                overlay.DebugOn();
            }
            else
            {
                racer.DisableDebugMenu();
                overlay.DebugOff();
            }
        }
        private void Opt_enableInvincibility_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_enableInvincibility.Checked)
                racer.EnableDebugInvincibility();
            else
                racer.DisableDebugInvincibility();
        }
        private void Opt_showTerrainFlags_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_showTerrainFlags.Checked)
                racer.EnableDebugTerrainLabels();
            else
                racer.DisableDebugTerrainLabels();
        }


        public void SaveRaceState()
        {
            savestate_pod = racer.GetPodDataALL();
        }
        public void LoadRaceState()
        {
            racer.WritePodDataALL(savestate_pod);
        }
    }
}
