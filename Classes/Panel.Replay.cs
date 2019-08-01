﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        private Racer.Replay race_replay = new Racer.Replay();

        private void CheckReplay()
        {
            bool in_race = game_state.State(racer) == Racer.GameState.Id.InRace;
            if (in_race)
                race_replay.Reset(racer);
            btn_replayImport.Enabled = !in_race;
            btn_replayExport.Enabled = !in_race && race_replay.CheckExportable();
            btn_replayInfo.Enabled = true;
        }

        private void Btn_replayImport_Click(object sender, EventArgs e)
        {
            if (dlg_replayImport.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    race_replay.Import(dlg_replayImport.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            CheckReplay();
        }

        private void Btn_replayExport_Click(object sender, EventArgs e)
        {
            if (dlg_replayExport.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    race_replay.Export(dlg_replayExport.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void Btn_replayInfo_Click(object sender, EventArgs e)
        {
            string bodytext = race_replay.CheckExportable() ? race_replay.TrackName + Environment.NewLine + race_replay.VehicleName : "No replay data";
            MessageBox.Show(bodytext,
                "Replay Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
