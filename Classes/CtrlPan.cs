using SWE1R.Util;
using System;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using SpriteTextRenderer;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Windows;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        // SETUP

        const string TARGET_PROCESS_TITLE = "Episode I Racer";
        //private Process target;
        private Racer racer = new Racer();
        //private Overlay overlay;
        private RenderForm overlay;
        public Input input;

        private bool overlay_show = false,
            overlay_initialized = false;
        private List<Racer.State> savestate_in_race;

        private InRaceData data_in_race = new InRaceData();
        private Racer.GameState game_state = new Racer.GameState();

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
            ShowOverlay(overlay_show);
            stopwatch = new Stopwatch();
            stopwatch.Start();

            Application.Idle += new EventHandler(OnAppIdle);

            // clean up
            //DisposeAll();
            /* where to call this now that the main loop is asychronous? should eventually garbage collect by itself regardless tho */


            // TESTING

            //Racer.Save.Game savetest = new Racer.Save.Game();
            //savetest.ReadFile();
            //Racer.Save.Profile profiletest = new Racer.Save.Profile(@"Z:\GOG\STAR WARS Racer\data\player\TEST.sav");
            //Console.WriteLine(profiletest.PrintString());
        }

        private void OnAppIdle(object o, EventArgs e)
        {
            while (Win32.IsApplicationIdle())
                MainLoop();
        }

        private void MainLoop()
        {
            string txt_debug;

            stopwatch.Stop();
            txt_debug = stopwatch.ElapsedMilliseconds.ToString("00ms");
            stopwatch = Stopwatch.StartNew();

            if (input != null)
                input.Update();

            // discontinue if targets no longer valid
            if (!CheckTargets())
                return;

            // future: clear screen here then somehow not continue if game is not in focus/taking input, i.e. don't render unless game is actually being played
            //context.ClearRenderTargetView(renderTarget, ol_color["clear"]);
            //if (target.MainWindowHandle != Interop.GetForegroundWindow())
            //    return;

            // logic

            game_state.Update(racer);
            if (game_state.EnterOrLeaveRace(racer))
                CheckRaceState();
            txt_debug += "   State:" + game_state.DeepState(racer).ToString();

            if (!overlay.Visible)
                return;

            /* implement generalised new/old state system before adding more UI elements? */
            if (game_state.State(racer) == Racer.GameState.Id.InRace)
            {
                if (game_state.DeepState(racer) == Racer.GameState.Id.RaceStarting)
                {
                    race_deaths = 0;
                    race_pod_dist_total = 0;
                }

                data_in_race.Update(racer);

                if (data_in_race.JustDied(racer))
                    race_deaths++;

                race_pod_heat_txt = data_in_race.Heat(racer).ToString("0.0");
                race_pod_overheat_txt = (data_in_race.Heat(racer) / data_in_race.HeatRate(racer)).ToString("0.0s");
                race_pod_underheat_txt = ((100 - data_in_race.Heat(racer)) / data_in_race.CoolRate(racer)).ToString("0.0s");

                race_time = Helper.FormatTimesArray(data_in_race.AllTimes(racer).Where(item => item >= 0).ToArray(), time_format);
                race_time_label = race_time_label_src.ToList().GetRange(0, race_time.Length).ToArray();
                race_time_label[race_time_label.Length - 1] = race_time_label_src.Last();

                if (!data_in_race.IsFinished(racer) && data_in_race.TimeTotal(racer) > 0)
                    race_pod_dist_total += data_in_race.FrameDistance3D(racer);
                race_pod_loc_txt = data_in_race.FrameDistance3D(racer).ToString("00.000") + " ADU/f  " +
                    data_in_race.Speed3D(racer).ToString("000.0") + " ADU/s  " +
                    race_pod_dist_total.ToString("0.0") + " ADU/race   " +
                    (race_pod_dist_total / data_in_race.TimeTotal(racer)).ToString("000.0") + " avg ADU/s  ";
            }

            if (game_state.DeepState(racer) == Racer.GameState.Id.VehicleSelect)
            {
                podsel_statistics = GetVehicleSelectStats();
                podsel_shown_stats = new float[7];
                for (var i = 0; i < podsel_shown_map.Length; i++)
                    podsel_shown_stats[i] = (float)podsel_statistics.GetValue(podsel_shown_map[i]);
                podsel_hidden_stats = new float[8];
                for (var i = 0; i < podsel_hidden_map.Length; i++)
                    podsel_hidden_stats[i] = (float)podsel_statistics.GetValue(podsel_hidden_map[i]);
            }

            // rendering

            Win32.GetWindowRect(racer.game.MainWindowHandle, out rect);
            WINDOW_SIZE = new Size(rect.right - rect.left - WINDOW_BORDER[0] - WINDOW_BORDER[2], rect.bottom - rect.top - WINDOW_BORDER[1] - WINDOW_BORDER[3]);
            if (WINDOW_SIZE != overlay.Size)
            {
                overlay.Size = WINDOW_SIZE.ToSize();
                WINDOW_SCALE.Width = WINDOW_SIZE.Width / WINDOW_SIZE_DFLT.Width;
                WINDOW_SCALE.Height = WINDOW_SIZE.Height / WINDOW_SIZE_DFLT.Height;
                /* i.e. all scaling is relative to base 1280x720 design */
            }
            overlay.Left = rect.left + WINDOW_BORDER[0];
            overlay.Top = rect.top + WINDOW_BORDER[1];

            context.ClearRenderTargetView(renderTarget, ol_color["clear"]);
            if (opt_debug)
                DrawText(ol_coords["txt_debug"], txt_debug, ol_font["default"], ol_color["txt_debug"], TextAlignment.Left | TextAlignment.Top);

            if (game_state.State(racer) == Racer.GameState.Id.InRace)
            {
                DrawText(ol_coords["txt_debug2"], race_pod_loc_txt, ol_font["default"], ol_color["txt_debug"], TextAlignment.Right | TextAlignment.Bottom);

                // race times
                DrawTextList(ol_coords["txt_race_times"], race_time_label, race_time, ol_font["race_times"], ol_color["txt_race_times"], TextAlignment.Left | TextAlignment.Top, "  ");

                // not displayed on race end screen
                if (game_state.DeepState(racer) != Racer.GameState.Id.RaceEnded)
                {
                    //heating
                    /*  todo - different ms size, colouring */
                    if (data_in_race.IsBoosting(racer))
                        DrawIconWithText(ol_coords["txt_race_pod_heating"], ol_img["heating"], new List<String>() { race_pod_overheat_txt, race_pod_underheat_txt },
                            ol_font["race_heating"], new List<Color>() { ol_color["txt_race_pod_overheat_on"], ol_color["txt_race_pod_underheat_off"] }, TextAlignment.Right | TextAlignment.VerticalCenter, new Point(4, 0), sep: -6, measure: "00.0s");
                    else
                        DrawIconWithText(ol_coords["txt_race_pod_heating"], ol_img["heating"], new List<String>() { race_pod_overheat_txt, race_pod_underheat_txt },
                            ol_font["race_heating"], new List<Color>() { ol_color["txt_race_pod_overheat_off"], ol_color["txt_race_pod_underheat_on"] }, TextAlignment.Right | TextAlignment.VerticalCenter, new Point(4, 0), sep: -6, measure: "00.0s");

                    //bottom left
                    DrawIconWithText(ol_coords["txt_race_pod_cooling"], ol_img["cooling"], race_pod_heat_txt,
                            ol_font["race_botbar"], ol_color["txt_race_pod_cooling"], TextAlignment.Left | TextAlignment.VerticalCenter, new Point(8, 0));
                    DrawIconWithText(ol_coords["txt_race_deaths"], ol_img["deaths"], race_deaths.ToString(),
                            ol_font["race_botbar"], ol_color["txt_race_deaths"], TextAlignment.Left | TextAlignment.VerticalCenter, new Point(8, 0));

                    /*
                        engine notes @ 1280x720
                        51 x (42*3=126)
                        col1 x136 y531
                        col2 x233 y531
                        whole item incl cooling outline - w177 h139 x120 y525 - gap to bottom of screen 56px
                    */
                }
            }
            if (game_state.DeepState(racer) == Racer.GameState.Id.VehicleSelect)
            {
                DrawTextList(ol_coords["podsel_hidden"], podsel_hidden_stats_names, podsel_hidden_stats, ol_font["podsel_hidden"], ol_color["txt_podsel_stats_hidden"], TextAlignment.Left | TextAlignment.Bottom, "   ");
                DrawTextList(ol_coords["podsel_shown"], Helper.ArrayToStrList(podsel_shown_stats), ol_font["podsel_shown"], ol_color["txt_podsel_stats_shown"], TextAlignment.Left | TextAlignment.VerticalCenter);
            }

            sprite.Flush();
            swapChain.Present(0, PresentFlags.None);
        }

        // DEBUG

        private void Opt_enableDebugMenu_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugMenu(opt_enableDebugMenu.Checked);
            SetDebug(opt_enableDebugMenu.Checked);
        }
        private void Opt_enableInvincibility_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugInvincibility(opt_enableInvincibility.Checked);
        }
        private void Opt_showTerrainFlags_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugTerrainLabels(opt_showTerrainFlags.Checked);
        }

        // HOTKEYS

        private void Opt_hotkeyEnable_CheckedChanged(object sender, EventArgs e)
        {
            input.EnableHotkeys(opt_hotkeyEnable.Checked);
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
            SetRacer((Process)cbx_processList.SelectedItem);
        }
        private void Btn_processFind_Click(object sender, EventArgs e)
        {
            FindGameProcess();
        }
        private void SetRacer(Process target)
        {
            if (racer == null)
                racer = new Racer();
            if (racer.UpdateGame(target))
            {
                CheckRaceState();
                gb_stateInRace.Enabled = true;
                gb_debug.Enabled = true;
                txt_selectGame.Hide();
                opt_showOverlay.Show();
                ShowOverlay(overlay_show);
            }
        }
        private void FindGameProcess()
        {
            cbx_processList.DataSource = GetProcessList();
            if (cbx_processList.FindStringExact(TARGET_PROCESS_TITLE) > 0)
            {
                cbx_processList.SelectedIndex = cbx_processList.FindStringExact(TARGET_PROCESS_TITLE);
                SetRacer((Process)cbx_processList.SelectedItem);
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





        private void SetDebugMenu(bool enable)
        {
            racer.WriteData(Racer.Addr.Static.DebugMenu, (uint)(enable ? 0x01 : 0x0));
            racer.WriteData(Racer.Addr.Static.DebugMenuText, (uint)(enable ? 0x3F : 0x0));
            racer.WriteData(Racer.Addr.Static.DebugLevel, (uint)(enable ? 0x06 : 0x0));
        }
        private void SetDebugTerrainLabels(bool enable)
        {
            racer.WriteData(Racer.Addr.Static.DebugTerrainLabels, (uint)(enable ? 0x01 : 0x0));
        }
        private void SetDebugInvincibility(bool enable)
        {
            racer.WriteData(Racer.Addr.Static.DebugInvincibility, (uint)(enable ? 0x01 : 0x0));
        }



        //move to new file?
        private class InRaceData
        {
            private Racer.DataCollection data = new Racer.DataCollection(), data_prev;

            public void Update(Racer r)
            {
                data_prev = (Racer.DataCollection)data.Clone();
                data.Update(r);
            }

            public bool IsBoosting(Racer r)
            {
                return ((data.GetValue(r, Racer.Addr.PodState.Flags1) & (1 << 23)) != 0);
            }

            public bool IsFinished(Racer r)
            {
                return ((data.GetValue(r, Racer.Addr.PodState.Flags2) & (1 << 25)) != 0);
            }

            public bool JustDied(Racer r)
            {
                int i = data_prev.ValueExists(Racer.DataCollection.DataBlock.Path.PodState, (uint)Racer.Addr.PodState.Flags1, Racer.Addr.GetLength(Racer.Addr.PodState.Flags1));
                bool prev = (i < 0)?false:(data_prev.GetValue(i) & (1 << 14)) != 0;
                return (data.GetValue(r, Racer.Addr.PodState.Flags1) & (1 << 14)) != 0 && !prev;
            }

            public Vector3 Location3D(Racer r)
            {
                return new Vector3(data.GetValue(r, Racer.Addr.PodState.X), data.GetValue(r, Racer.Addr.PodState.Y), data.GetValue(r, Racer.Addr.PodState.Z));
            }

            public double FrameDistance3D(Racer r)
            {
                int iX = data_prev.ValueExists(Racer.DataCollection.DataBlock.Path.PodState, (uint)Racer.Addr.PodState.X, Racer.Addr.GetLength(Racer.Addr.PodState.X));
                int iY = data_prev.ValueExists(Racer.DataCollection.DataBlock.Path.PodState, (uint)Racer.Addr.PodState.Y, Racer.Addr.GetLength(Racer.Addr.PodState.Y));
                int iZ = data_prev.ValueExists(Racer.DataCollection.DataBlock.Path.PodState, (uint)Racer.Addr.PodState.Z, Racer.Addr.GetLength(Racer.Addr.PodState.Z));
                Vector3 loc_old = (iX >= 0 && iY >= 0 && iZ >= 0) ? new Vector3(data_prev.GetValue(iX), data_prev.GetValue(iY), data_prev.GetValue(iZ)) : Location3D(r);
                Vector3 loc_new = Location3D(r);
                return Math.Sqrt(Math.Pow(loc_new.X - loc_old.X, 2) + Math.Pow(loc_new.Y - loc_old.Y, 2) + Math.Pow(loc_new.Z - loc_old.Z, 2));
            }

            public double FrameTime(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Static.FrameTime);
            }

            public double Speed3D(Racer r)
            {
                return FrameDistance3D(r) / FrameTime(r);
            }

            public float[] AllTimes(Racer r)
            {
                return new float[6] { TimeLap1(r), TimeLap2(r), TimeLap3(r), TimeLap4(r), TimeLap5(r), TimeTotal(r) };
            }
            public float TimeLap1(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Pod.TimeLap1);
            }
            public float TimeLap2(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Pod.TimeLap2);
            }
            public float TimeLap3(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Pod.TimeLap3);
            }
            public float TimeLap4(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Pod.TimeLap4);
            }
            public float TimeLap5(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Pod.TimeLap5);
            }
            public float TimeTotal(Racer r)
            {
                return data.GetValue(r, Racer.Addr.Pod.TimeTotal);
            }


            public float Heat(Racer r)
            {
                return data.GetValue(r, Racer.Addr.PodState.Heat);
            }
            public float HeatRate(Racer r)
            {
                return data.GetValue(r, Racer.Addr.PodState.StatHeatRate);
            }
            public float CoolRate(Racer r)
            {
                return data.GetValue(r, Racer.Addr.PodState.StatCoolRate);
            }
        }

        private Single[] GetVehicleSelectStats()
        {
            byte[] data = racer.GetData(Racer.Addr.Static.StatAntiSkid, 0x3C);
            List<float> stats = new List<float>();
            for (var i = 0; i < data.Length; i += 4)
                stats.Add(BitConverter.ToSingle(data, i));
            return stats.ToArray();
        }
    }
}
