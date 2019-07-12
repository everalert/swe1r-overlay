using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        private void Btn_stateS_Click(object sender, EventArgs e)
        {
            SaveRaceState();
        }
        private void Btn_stateL_Click(object sender, EventArgs e)
        {
            LoadRaceState();
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
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.Pod, 0x60, racer.GetPodCustom(0x60, 0x19))); // times + lap byte
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.PodData, 0, racer.GetPodDataALL()));
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.Rendering, 0x7C, racer.GetRenderingCustom(0x7C, 0x4))); // camera mode, 0x4 len, also resets death camera
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.Rendering, 0x2A8, racer.GetRenderingCustom(0x2A8, 0x4))); // fog flags, 0x4 len, helps but not enough to completely save fog state
                data.Add(new Racer.State.StateBlock(Racer.State.BlockType.Rendering, 0x2C8, racer.GetRenderingCustom(0x2C8, 0x40))); // fog col, dist
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
                            case Racer.State.BlockType.Rendering:
                                racer.WriteRenderingCustom(BitConverter.ToUInt32(block.offset, 0), block.data);
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
            no_stateSel.Enabled = (savestate_in_race.Count > 0);
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
                MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (no_stateSel.Value > savestate_in_race.Count)
                savestate_in_race.Add(state);
            else
                savestate_in_race[(int)no_stateSel.Value - 1] = state;
            CheckRaceState();
        }
    }
}
