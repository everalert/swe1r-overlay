using SWE1R_Overlay.Utilities;
using System;
using System.Drawing;
using System.Linq;
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
            public SavestateInRace(byte[] podData, byte racePod, byte raceTrack, float[] raceTimes, byte raceLap)
            {
                data = podData;
                track = raceTrack;
                time = raceTimes;
                lap = raceLap;
                pod = racePod;
            }
            public byte[] data;
            public byte lap;
            public float[] time;
            public byte pod;
            public byte track;
            //public byte world;
            //camera?
            //racetime?
        }

        public ControlPanel()
        {
            InitializeComponent();
            savestate_in_race = new List<SavestateInRace>();
            this.Icon = new Icon("img\\icon.ico");
            cbx_processList.DisplayMember = "MainWindowTitle";
            cbx_processList.ValueMember = "Id";
            FindGameProcess();
            UpdateHotkeyLabels();
        }

        // In-Race Savestate

        public void SaveRaceState()
        {
            if (racer.GetStatic("in_race", "byte") > 0)
            {
                var track = racer.GetRaceSetting("selected_track", "byte");
                var pod = racer.GetRaceSetting("selected_pod", "byte");
                var time = racer.GetPodTimeALL();
                var lap = racer.GetPod("lap", "byte");
                var data = racer.GetPodDataALL();
                if (no_stateSel.Value > savestate_in_race.Count)
                {
                    savestate_in_race.Add(new SavestateInRace(data, pod, track, time, lap));
                    btn_stateL.Enabled = true;
                    no_stateSel.Enabled = true;
                    no_stateSel.Maximum += 1;
                }
                else
                {
                    var savestate = savestate_in_race[(int)no_stateSel.Value - 1];
                    savestate.data = data;
                    savestate.track = track;
                    savestate.time = time;
                    savestate.lap = lap;
                    savestate.pod = pod;
                }
            }
            CheckRaceState();
        }
        public void LoadRaceState()
        {
            if ((int)no_stateSel.Value - 1 < savestate_in_race.Count)
            {
                var savestate = savestate_in_race[(int)no_stateSel.Value - 1];
                var thisTrack = racer.GetRaceSetting("selected_track", "byte");
                var thisPod = racer.GetRaceSetting("selected_pod", "byte");
                if (thisTrack == savestate.track && thisPod == savestate.pod)
                {
                    var path = new uint[] { Racer.Addr.pPod, Racer.Addr.oPod["time_lap_1"] };
                    var data = new byte[4 * 6 + 1];
                    data[4 * 6] = savestate.lap;
                    Buffer.BlockCopy(savestate.time, 0, data, 0, data.Length - 1);
                    racer.WriteCustom(path, data);
                    racer.WritePodDataALL(savestate.data);
                }
            }
        }
        private void No_stateSel_ValueChanged(object sender, EventArgs e)
        {
            CheckRaceState();
        }
        public void CheckRaceState()
        {
            bool inRace = (racer.GetStatic("in_race", "byte") > 0);
            btn_stateS.Enabled = inRace;
            bool canWrite = ((int)no_stateSel.Value - 1 < savestate_in_race.Count);
            string output;
            if (canWrite)
            {
                var savestate = savestate_in_race[(int)no_stateSel.Value - 1];
                Racer.Val.pods.TryGetValue(savestate.pod, out output);
                txt_statePod.Text = output;
                Racer.Val.tracks.TryGetValue(savestate.track, out output);
                txt_stateTrack.Text = output;
                txt_stateLapLocVal.Text = BitConverter.ToSingle(savestate.data, (int)Racer.Addr.oPodData["lap_completion_1"]).ToString("0.0%");
                txt_stateSpdVal.Text = BitConverter.ToSingle(savestate.data, (int)Racer.Addr.oPodData["speed"]).ToString("0.0" +
                    ((BitConverter.ToUInt32(savestate.data, (int)Racer.Addr.oPodData["flags1"]) & (1 << 23)) != 0 ? "*" : ""));
                if (inRace)
                    btn_stateL.Enabled = (racer.GetRaceSetting("selected_track", "byte") == savestate.track || racer.GetRaceSetting("selected_pod", "byte") == savestate.pod);
                else
                    btn_stateL.Enabled = false;
            }
            else
            {
                txt_statePod.Text = "-";
                txt_stateTrack.Text = "-";
                txt_stateLapLocVal.Text = "-";
                txt_stateSpdVal.Text = "-";
            }
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

        // HOTKEYS

        private void Opt_hotkeyEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_hotkeyEnable.Checked)
                overlay.input.EnableHotkeys(true);
            else
                overlay.input.EnableHotkeys(false);
        }
        private void Opt_hotkeyAltLayout_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_hotkeyAltLayout.Checked)
                overlay.input.map = 1;
            else
                overlay.input.map = 0;
            UpdateHotkeyLabels();
        }
        private void UpdateHotkeyLabels()
        {
            string[] labels;
            if (overlay == null)
                return;
            Input.HotkeyMap hkMap = overlay.input.GetCurrentMap();
            labels = hkMap.MAP["state_inrace_save"].GetLabels();
            txt_stateSaveNote.Text = (labels[0].Length > 0 ? "X360 " + labels[0] : "") +
                ((labels[0].Length > 0 && labels[1].Length > 0) ? " / " : "") +
                (labels[1].Length > 0 ? "KB " + labels[1] : "");
            labels = hkMap.MAP["state_inrace_load"].GetLabels();
            txt_stateLoadNote.Text = (labels[0].Length > 0 ? "X360 " + labels[0] : "") +
                ((labels[0].Length > 0 && labels[1].Length > 0) ? " / " : "") +
                (labels[1].Length > 0 ? "KB " + labels[1] : "");
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
            CheckRaceState();
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
