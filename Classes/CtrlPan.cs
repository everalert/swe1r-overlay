using SWE1R.Util;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using SpriteTextRenderer;
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
        private VehicleSelectData data_vehicle_select = new VehicleSelectData();
        private Racer.GameState game_state = new Racer.GameState();

        public ControlPanel()
        {
            InitializeComponent();
            savestate_in_race = new List<Racer.State>();
            Icon = new Icon("img\\icon.ico");
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
            txt_debug = stopwatch.Elapsed.TotalMilliseconds.ToString("00.0ms");
            stopwatch = Stopwatch.StartNew();

            if (input != null)
                input.Update();

            // discontinue if targets no longer valid
            if (!CheckTargets())
                return;

            //prep new frame
            RepositionOverlay();
            context.ClearRenderTargetView(renderTarget, ol_color["clear"]);

            // future: check here and somehow not continue if game is not in focus/taking input, i.e. don't render unless game is actually being played


            // rendering overlay

            game_state.Update(racer);
            if (game_state.EnterOrLeaveRace(racer))
                CheckRaceState();
            txt_debug += "   State:" + game_state.DeepState(racer).ToString();

            if (!overlay.Visible)
                goto finalize_rendered_frame;

            if (game_state.State(racer) == Racer.GameState.Id.InRace)
            {
                //move this to be contained in InRaceData?
                if (game_state.DeepState(racer) == Racer.GameState.Id.RaceStarting)
                {
                    race_deaths = 0;
                    race_pod_dist_total = 0;
                }

                data_in_race.Update(racer);

                if (data_in_race.JustDied(racer))
                    race_deaths++;

                if (!data_in_race.IsFinished(racer) && data_in_race.TimeTotal(racer) > 0)
                    race_pod_dist_total += data_in_race.FrameDistance3D(racer);

                OverlayRenderer.InRace.RenderTimes(this, data_in_race.AllTimes(racer));
                OverlayRenderer.InRace.RenderMovementData(this, data_in_race.FrameDistance3D(racer), data_in_race.Speed3D(racer), race_pod_dist_total, data_in_race.TimeTotal(racer));

                if (game_state.DeepState(racer) != Racer.GameState.Id.RaceEnded)
                {
                    OverlayRenderer.InRace.RenderHeatTimers(this, data_in_race.Heat(racer), data_in_race.HeatRate(racer), data_in_race.CoolRate(racer), data_in_race.IsBoosting(racer));
                    OverlayRenderer.InRace.RenderEngineBar(this, data_in_race.Heat(racer), race_deaths);
                }
            }

            if (game_state.DeepState(racer) == Racer.GameState.Id.VehicleSelect)
            {
                data_vehicle_select.Update(racer);
                OverlayRenderer.VehicleSelect.RenderMainStats(this, data_vehicle_select.AllStats(racer));
                OverlayRenderer.VehicleSelect.RenderHiddenStats(this, data_vehicle_select.AllStats(racer));
            }

            //debug output
            if (opt_debug)
                Render.DrawText(WINDOW_SCALE, ol_coords["txt_debug"], txt_debug, ol_font["default"], ol_color["txt_debug"], TextAlignment.Left | TextAlignment.Top);

            //finalize
            finalize_rendered_frame:
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
    }
}
