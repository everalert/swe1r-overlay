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
        private Process target;
        private Racer racer;
        //private Overlay overlay;
        private RenderForm overlay;
        public Input input;

        private bool overlay_show = false,
            overlay_initialized = false;
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
            ShowOverlay(overlay_show);
            stopwatch = new Stopwatch();
            stopwatch.Start();

            Application.Idle += new EventHandler(OnAppIdle);

            // clean up
            //DisposeAll();
            /* where to call this now that the main loop is asychronous? should eventually garbage collect by itself regardless tho */


            // TESTING

            //Racer.Save.GameSave savetest = new Racer.Save.GameSave();
            //savetest.ReadFile();
        }

        private void OnAppIdle(object o, EventArgs e)
        {
            while (Win32.IsApplicationIdle())
                MainLoop();
        }

        private void MainLoop()
        {
            stopwatch.Stop();
            var txt_debug = stopwatch.ElapsedMilliseconds.ToString();
            stopwatch = Stopwatch.StartNew();

            /* this should probably be done in the control panel directly? */
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

            racer_state_old = racer_state_new;
            racer_state_new = GetGameState();

            if (racer_state_new == "in_race" ^ racer_state_old != "in_race")
                CheckRaceState();

            if (!overlay.Visible)
                return;

            /* implement generalised new/old state system before adding more UI elements? */
            if (racer_state_new == "in_race")
            {
                var frame_time = racer.GetStatic("frame_time", "double");

                race_pod_flags1 = racer.GetPodData("flags1", "uint");
                race_pod_flags2 = racer.GetPodData("flags2", "uint");
                race_pod_is_boosting = ((race_pod_flags1 & (1 << 23)) != 0);
                race_pod_is_finished = ((race_pod_flags2 & (1 << 25)) != 0);
                race_dead_old = race_dead_new;
                race_dead_new = ((race_pod_flags1 & (1 << 14)) != 0);
                if (race_dead_new && !race_dead_old)
                    race_deaths++;

                race_pod_heat = racer.GetPodData("heat", "float");
                race_pod_heatrate = racer.GetPodData("heat_rate", "float");
                race_pod_coolrate = racer.GetPodData("cool_rate", "float");
                race_pod_heat_txt = race_pod_heat.ToString("0.0");
                race_pod_overheat_txt = (race_pod_heat / race_pod_heatrate).ToString("0.0s");
                race_pod_underheat_txt = ((100 - race_pod_heat) / race_pod_coolrate).ToString("0.0s");

                race_time_src = racer.GetPodTimeALL();
                race_time = Helper.FormatTimesArray(race_time_src.Where(item => item >= 0).ToArray(), time_format);
                race_time_label = race_time_label_src.ToList().GetRange(0, race_time.Length).ToArray();
                race_time_label[race_time_label.Length - 1] = race_time_label_src.Last();

                if (race_pod_loc_new != null)
                    race_pod_loc_old = race_pod_loc_new;
                race_pod_loc_new = new Vector3(racer.GetPodData("xpos", "float"), racer.GetPodData("ypos", "float"), racer.GetPodData("zpos", "float"));
                race_pod_dist_frame = race_pod_loc_old != null ? Math.Sqrt(Math.Pow(race_pod_loc_new.X - race_pod_loc_old.X, 2) + Math.Pow(race_pod_loc_new.Y - race_pod_loc_old.Y, 2) + Math.Pow(race_pod_loc_new.Z - race_pod_loc_old.Z, 2)) : 0;
                if (!race_pod_is_finished && race_time_src[race_time_src.Length - 1] > 0)
                    race_pod_dist_total += race_pod_dist_frame;
                race_pod_loc_txt = race_pod_dist_frame.ToString("00.000") + " ADU/f  " +
                    (race_pod_dist_frame / frame_time).ToString("000.0") + " ADU/s  " +
                    race_pod_dist_total.ToString("0.0") + " ADU/race   " +
                    (race_pod_dist_total / race_time_src[race_time_src.Length - 1]).ToString("000.0") + " avg ADU/s  ";
            }
            else
            {
                race_deaths = 0;
                race_pod_dist_total = 0;
            }

            if (racer_state_new == "pod_select")
            {
                podsel_statistics = racer.GetStatsALL();
                podsel_shown_stats = new float[7];
                for (var i = 0; i < podsel_shown_map.Length; i++)
                    podsel_shown_stats[i] = (float)podsel_statistics.GetValue(podsel_shown_map[i]);
                podsel_hidden_stats = new float[8];
                for (var i = 0; i < podsel_hidden_map.Length; i++)
                    podsel_hidden_stats[i] = (float)podsel_statistics.GetValue(podsel_hidden_map[i]);

            }

            // rendering

            Win32.GetWindowRect(target.MainWindowHandle, out rect);
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

            if (racer_state_new == "in_race")
            {
                DrawText(ol_coords["txt_debug2"], race_pod_loc_txt, ol_font["default"], ol_color["txt_debug"], TextAlignment.Right | TextAlignment.Bottom);

                // race times
                DrawTextList(ol_coords["txt_race_times"], race_time_label, race_time, ol_font["race_times"], ol_color["txt_race_times"], TextAlignment.Left | TextAlignment.Top, "  ");

                // not displayed on race end screen
                if (!race_pod_is_finished)
                {
                    //heating
                    /*  todo - different ms size, colouring */
                    if (race_pod_is_boosting)
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
            if (racer_state_new == "pod_select")
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
            if (opt_enableDebugMenu.Checked)
            {
                racer.SetDebugMenu(true);
                SetDebug(true);
            }
            else
            {
                racer.SetDebugMenu(false);
                SetDebug(false);
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
            //SetOverlay(target, racer);
            SetOverlay();
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
        private void FindGameProcess()
        {
            cbx_processList.DataSource = GetProcessList();
            if (cbx_processList.FindStringExact(TARGET_PROCESS_TITLE) > 0)
            {
                cbx_processList.SelectedIndex = cbx_processList.FindStringExact(TARGET_PROCESS_TITLE);
                target = (Process)cbx_processList.SelectedItem;
                SetRacer(target);
                txt_selectGame.Hide();
                opt_showOverlay.Show();
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
