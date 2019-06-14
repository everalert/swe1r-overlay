using SWE1R_Overlay.Utilities;
using System;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SWE1R_Overlay
{
    public partial class ControlPanel : Form
    {
        // SETUP

        const string TARGET_PROCESS_TITLE = "Episode I Racer";
        private Process target;
        private RacerData racer;
        private Overlay overlay;
        private byte[] savestate_pod;

        // INIT

        public ControlPanel()
        {
            InitializeComponent();
            cbx_processList.DisplayMember = "MainWindowTitle";
            cbx_processList.ValueMember = "Id";
            FindGameProcess();
        }

        // CONTROLS

        private void Opt_showOverlay_CheckedChanged(object sender, EventArgs e)
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

        // GAME DETECTION/ASSIGNMENT

        private void Cbx_processList_DropDown(object sender, EventArgs e)
        {
            cbx_processList.DataSource = GetProcessList();
        }
        private void Cbx_processList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            target = (Process)cbx_processList.SelectedItem;
            SetRacer(target);
            SetOverlay(target, racer);
        }
        private void SetRacer(Process tgt)
        {
            if (racer == null)
                racer = new RacerData(tgt);
            else
                racer.SetGameTarget(tgt);
        }
        private void SetOverlay(Process tgt, RacerData rcr)
        {
            if (overlay == null)
            {
                overlay = new Overlay(this, tgt, rcr);
                overlay.Show();
                opt_showOverlay.Checked = true;
                txt_selectGame.Hide();
                opt_showOverlay.Show();
            }
            else
                overlay.UpdateOverlay(tgt);
        }
        private void FindGameProcess()
        {
            cbx_processList.DataSource = GetProcessList();
            if (cbx_processList.FindStringExact(TARGET_PROCESS_TITLE) > 0)
            {
                cbx_processList.SelectedIndex = cbx_processList.FindStringExact(TARGET_PROCESS_TITLE);
                target = (Process)cbx_processList.SelectedItem;
                SetRacer(target);
                SetOverlay(target, racer);
            }
        }
        private List<Process> GetProcessList()
        {
            List<Process> output = new List<Process>();
            foreach (Process process in Process.GetProcesses())
                if (process.MainWindowTitle.Length > 0)
                    output.Add(process);
            return output;
        }
    }
}
