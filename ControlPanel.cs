using SWE1R_Overlay.Utilities;
using System;
using System.Drawing;
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
        private Racer racer;
        private Overlay overlay;
        public bool overlay_show;
        private List<SavestateInRace> savestate_in_race;
        private class SavestateInRace
        {
            public byte[] data;
            public byte pod;
            public byte track;
            public byte world;
            //camera?
            //racetime?
        }
        /*
         * savestate
         * - new data management model
         *   - save track (disable state load on track mismatch)
         *   - save time?
         *   - save camera?
         *   - save/load to file?
         */

        // INIT

        public ControlPanel()
        {
            InitializeComponent();
            savestate_in_race = new List<SavestateInRace>();
            this.Icon = new Icon("img\\icon.ico");
            cbx_processList.DisplayMember = "MainWindowTitle";
            cbx_processList.ValueMember = "Id";
            FindGameProcess();
        }

        // CONTROLS

        private void Opt_showOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_showOverlay.Checked)
                ShowOverlay();
            else
                HideOverlay();
        }
        private void ShowOverlay()
        {
            overlay_show = true;
            overlay.Show();
        }
        private void HideOverlay()
        {
            overlay_show = false;
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
                racer.SetDebugMenu(true);
                overlay.SetDebug(true);
            }
            else
            {
                racer.SetDebugMenu(false);
                overlay.SetDebug(false);
            }
        }
        private void Opt_enableInvincibility_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_enableInvincibility.Checked)
                racer.SetDebugInvincibility(true);
            else
                racer.SetDebugInvincibility(false);
        }
        private void Opt_showTerrainFlags_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_showTerrainFlags.Checked)
                racer.SetDebugTerrainLabels(true);
            else
                racer.SetDebugTerrainLabels(false);
        }
        // In-Race Savestate
        public void SaveRaceState()
        {
            if (no_stateSel.Value > savestate_in_race.Count)
            {
                savestate_in_race.Add(new SavestateInRace() { data = racer.GetPodDataALL() });
                btn_stateL.Enabled = true;
                no_stateSel.Enabled = true;
                no_stateSel.Maximum += 1;
            }
            else
            {
                savestate_in_race[(int)no_stateSel.Value-1].data = racer.GetPodDataALL();
            }
            WriteStateInfo();
        }
        public void LoadRaceState()
        {
            if ((int)no_stateSel.Value-1 < savestate_in_race.Count)
                racer.WritePodDataALL(savestate_in_race[(int)no_stateSel.Value-1].data);
        }
        private void No_stateSel_ValueChanged(object sender, EventArgs e)
        {
            btn_stateL.Enabled = ((int)no_stateSel.Value - 1 < savestate_in_race.Count)?true:false;
            WriteStateInfo();
        }
        private void WriteStateInfo()
        {
            bool write = ((int)no_stateSel.Value - 1 < savestate_in_race.Count);
            txt_stateLapLocVal.Text = write?BitConverter.ToSingle(savestate_in_race[(int)no_stateSel.Value - 1].data, (int)Racer.Addr.oPodData["lap_completion_1"]).ToString("0.0%"):"-";
            txt_stateSpdVal.Text = write?BitConverter.ToSingle(savestate_in_race[(int)no_stateSel.Value - 1].data, (int)Racer.Addr.oPodData["speed"]).ToString("0.0"+
                ((BitConverter.ToUInt32(savestate_in_race[(int)no_stateSel.Value - 1].data, (int)Racer.Addr.oPodData["flags1"]) & (1 << 23)) != 0?"*":"")) :"-";
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
        private void Btn_processFind_Click(object sender, EventArgs e)
        {
            FindGameProcess();
        }
        private void SetRacer(Process tgt)
        {
            if (racer == null)
                racer = new Racer(tgt);
            else
                racer.SetGameTarget(tgt);
            gb_stateInRace.Enabled = true;
            WriteStateInfo();
            gb_debug.Enabled = true;
        }
        private void SetOverlay(Process tgt, Racer rcr)
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
                overlay.UpdateOverlay(tgt, rcr);
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
