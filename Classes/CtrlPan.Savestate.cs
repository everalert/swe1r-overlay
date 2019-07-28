using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        private List<Racer.Savestate> savestate_in_race;

        private void Btn_stateS_Click(object sender, EventArgs e)
        {
            SaveRaceState();
        }
        private void Btn_stateL_Click(object sender, EventArgs e)
        {
            LoadRaceState();
        }

        // In-Race Savestate

        public void SaveRaceState()
        {
            if (no_stateSel.Value > savestate_in_race.Count)
                savestate_in_race.Add(new Racer.Savestate(racer));
            else
                savestate_in_race[(int)no_stateSel.Value - 1].Save(racer);
            CheckRaceState();
        }
        public void LoadRaceState()
        {
            int thisSlot = (int)no_stateSel.Value - 1;
            if (thisSlot < savestate_in_race.Count)
                savestate_in_race[thisSlot].Load(racer);
            CheckRaceState();
        }
        private void No_stateSel_ValueChanged(object sender, EventArgs e)
        {
            CheckRaceState();
        }
        public void CheckRaceState()
        {
            //maybe rewrite/cleanup?
            bool inRace = (racer.GetData(Racer.Addr.Static.InRace) > 0);
            btn_stateS.Enabled = inRace;
            no_stateSel.Enabled = (savestate_in_race.Count > 0);
            while (no_stateSel.Maximum <= savestate_in_race.Count)
                no_stateSel.Maximum++;
            bool canWrite = ((int)no_stateSel.Value - 1 < savestate_in_race.Count);
            btn_stateSFile.Enabled = canWrite;
            string output;
            if (canWrite)
            {
                Racer.Savestate savestate = savestate_in_race[(int)no_stateSel.Value - 1];
                byte[] podData = savestate.PodState;
                txt_statePod.Text = Racer.Value.Vehicle.Name.TryGetValue(savestate.Vehicle, out output) != false ? output : "-";
                txt_stateTrack.Text = Racer.Value.Track.Name.TryGetValue(savestate.Track, out output) != false ? output : "-";
                txt_stateLapLocVal.Text = podData.Length == (int)Racer.Addr.PtrLen.PodState ? BitConverter.ToSingle(podData, (int)Racer.Addr.PodState.LapCompletion).ToString("0.0%") : "-";
                txt_stateSpdVal.Text = podData.Length == (int)Racer.Addr.PtrLen.PodState ? BitConverter.ToSingle(podData, (int)Racer.Addr.PodState.Speed).ToString("0.0" +
                    ((BitConverter.ToUInt32(podData, (int)Racer.Addr.PodState.Flags1) & (1 << 23)) != 0 ? "*" : "")):"-";
                if (inRace)
                    btn_stateL.Enabled = (racer.GetData(Racer.Addr.Race.SelectedTrack) == savestate.Track && racer.GetData(Racer.Addr.Race.SelectedVehicle) == savestate.Vehicle);
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
                    savestate_in_race[thisSlot].Export(dlg_stateSFile.FileName);
        }
        private void Btn_stateLFile_Click(object sender, EventArgs e)
        {
            if (dlg_stateLFile.ShowDialog() != DialogResult.OK)
                return;
            if (no_stateSel.Value > savestate_in_race.Count)
            {
                //implement once Savestate.Export() actually assigns data to the state
                //savestate_in_race.Add(new Racer.Savestate());
                //savestate_in_race[savestate_in_race.Count - 1].Import(dlg_stateLFile.FileName);
            }
            else
            {
                try
                {
                    savestate_in_race[(int)no_stateSel.Value - 1].Import(dlg_stateLFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            CheckRaceState();
        }
    }
}
