using SWE1R.Util;
using System;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        // SETUP

        const string TARGET_PROCESS_TITLE = "Episode I Racer";
        private Process target;
        private Racer racer;
        private Overlay overlay;
        public Input input;

        public bool overlay_show;
        private List<Racer.State> savestate_in_race;

        public ControlPanel()
        {
            InitializeComponent();
            savestate_in_race = new List<Racer.State>();
            this.Icon = new Icon("img\\icon.ico");
            cbx_processList.DisplayMember = "MainWindowTitle";
            cbx_processList.ValueMember = "Id";
            input = new Input(this);
            FindGameProcess();
            UpdateHotkeyLabels();


            //testcode
            //Racer.Save.GameSave savetest = new Racer.Save.GameSave();
            //savetest.ReadFile();
        }

        // In-Race Savestate

        //move to state class?
        public void SaveRaceState()
        {
            if (racer.GetStatic("in_race", "byte") > 0)
            {
                var track = racer.GetRaceSetting("selected_track", "byte");
                var pod = racer.GetRaceSetting("selected_pod", "byte");
                List<Racer.State.StateBlock> data = new List<Racer.State.StateBlock>();
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.Pod, 0x60, racer.GetPodCustom(0x60,0x19))); // times + lap byte
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.PodData, 0, racer.GetPodDataALL()));
                Racer.State state = new Racer.State(data.ToArray(), pod, track);
                if (no_stateSel.Value > savestate_in_race.Count)
                    savestate_in_race.Add(state);
                else
                    savestate_in_race[(int)no_stateSel.Value - 1] = state;
            }
            CheckRaceState();
        }
        //move to state class?
        public void LoadRaceState()
        {
            int thisSlot = (int)no_stateSel.Value - 1;
            if (thisSlot < savestate_in_race.Count)
            {
                var savestate = savestate_in_race[thisSlot];
                var thisTrack = racer.GetRaceSetting("selected_track", "byte");
                var thisPod = racer.GetRaceSetting("selected_pod", "byte");
                if (thisTrack == savestate.track && thisPod == savestate.pod)
                {
                    foreach (Racer.State.StateBlock block in savestate.data)
                    {
                        switch (block.type)
                        {
                            case Racer.State.BlockType.Pod:
                                racer.WriteCustom(new uint[2] { Racer.Addr.pPod, BitConverter.ToUInt32(block.offset, 0) }, block.data);
                                break;
                            case Racer.State.BlockType.PodData:
                                racer.WriteCustom(new uint[3] { Racer.Addr.pPod, Racer.Addr.oPod["pPodData"], BitConverter.ToUInt32(block.offset, 0) }, block.data);
                                //breaks when loading state from old session, probably some pointers being written?
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            CheckRaceState();
        }
        private void No_stateSel_ValueChanged(object sender, EventArgs e)
        {
            CheckRaceState();
        }
        public void CheckRaceState()
        {
            bool inRace = (racer.GetStatic("in_race", "byte") > 0);
            btn_stateS.Enabled = inRace;
            no_stateSel.Enabled = (savestate_in_race.Count>0);
            while (no_stateSel.Maximum <= savestate_in_race.Count)
                no_stateSel.Maximum++;
            bool canWrite = ((int)no_stateSel.Value - 1 < savestate_in_race.Count);
            btn_stateSFile.Enabled = canWrite;
            string output;
            if (canWrite)
            {
                Racer.State savestate = savestate_in_race[(int)no_stateSel.Value - 1];
                byte[] podData = savestate.data[savestate.DataFirstIndexOfId(Racer.State.BlockType.PodData)].data;
                Racer.Value.Vehicle.Name.TryGetValue(savestate.pod, out output);
                txt_statePod.Text = output;
                Racer.Value.Track.Name.TryGetValue(savestate.track, out output);
                txt_stateTrack.Text = output;
                txt_stateLapLocVal.Text = BitConverter.ToSingle(podData, (int)Racer.Addr.oPodData["lap_completion_1"]).ToString("0.0%");
                txt_stateSpdVal.Text = BitConverter.ToSingle(podData, (int)Racer.Addr.oPodData["speed"]).ToString("0.0" +
                    ((BitConverter.ToUInt32(podData, (int)Racer.Addr.oPodData["flags1"]) & (1 << 23)) != 0 ? "*" : ""));
                if (inRace)
                    btn_stateL.Enabled = (racer.GetRaceSetting("selected_track", "byte") == savestate.track && racer.GetRaceSetting("selected_pod", "byte") == savestate.pod);
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
        private void Btn_stateSFile_Click(object sender, EventArgs e)
        {
            int thisSlot = (int)no_stateSel.Value - 1;
            if (thisSlot < savestate_in_race.Count)
                if (dlg_stateSFile.ShowDialog() == DialogResult.OK)
                    savestate_in_race[thisSlot].SaveStateToFile(dlg_stateSFile.FileName);
        }
        private void Btn_stateLFile_Click(object sender, EventArgs e)
        {
            if (dlg_stateLFile.ShowDialog() != DialogResult.OK)
                return;
            Racer.State state;
            try
            {
                state = Racer.State.LoadStateFromFile(dlg_stateLFile.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (no_stateSel.Value > savestate_in_race.Count)
                savestate_in_race.Add(state);
            else
                savestate_in_race[(int)no_stateSel.Value - 1] = state;
            CheckRaceState();
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
                input.EnableHotkeys(true);
            else
                input.EnableHotkeys(false);
        }
        private void Opt_hotkeyAltLayout_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_hotkeyAltLayout.Checked)
                input.map = 1;
            else
                input.map = 0;
            UpdateHotkeyLabels();
        }
        private void UpdateHotkeyLabels()
        {
            if (overlay == null)
                return;
            Input.HotkeyMap hkMap = input.GetCurrentMap();
            tt_stateS.SetToolTip(btn_stateS, hkMap.MAP["state_inrace_save"].GetLabel("Hotkeys"+Environment.NewLine, Environment.NewLine));
            tt_stateL.SetToolTip(btn_stateL, hkMap.MAP["state_inrace_load"].GetLabel("Hotkeys"+Environment.NewLine, Environment.NewLine));
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
