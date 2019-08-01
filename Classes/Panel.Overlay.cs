using SWE1R.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using SpriteTextRenderer;
using SpriteRenderer = SpriteTextRenderer.SlimDX.SpriteRenderer;
using TextBlockRenderer = SpriteTextRenderer.SlimDX.TextBlockRenderer;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        //todo
        //- maybe d3d setup stuff can be moved to util class too?
        //- move/make overlay update classes here
        //  - data processing management classes by game state, general data rendering class

        // setup
        Win32.RECT rect;
        readonly int[] WINDOW_BORDER = { 10, 32, 10, 10 };
        Vector2 WINDOW_SIZE = new Vector2(1280, 720);
        readonly Vector2 WINDOW_SIZE_DFLT = new Vector2(1280, 720);
        Vector2 WINDOW_SCALE = new Vector2(1, 1);
        private const string time_format = "m\\:ss\\.fff";
        Stopwatch stopwatch;
        private bool opt_debug = false;

        // in race
        private double race_pod_dist_total = 0d;
        private uint race_deaths = 0;

        // dx11/rendering
        Device device;
        SwapChain swapChain;
        Viewport viewport;
        RenderTargetView renderTarget;
        Texture2D backBuffer;
        private dynamic context;
        SpriteRenderer sprite;
        readonly private Dictionary<string, TextBlockRenderer> ol_font = new Dictionary<string, TextBlockRenderer>();
        readonly private Dictionary<string, ShaderResourceView> ol_img = new Dictionary<string, ShaderResourceView>();
        readonly private Dictionary<string, Color> ol_color = new Dictionary<string, Color>()
        {
            { "clear", Color.FromArgb(0,0,0,0) },
            { "txt_debug", Color.FromArgb(0xFF,0xFF,0xFF,0xFF) },
            { "txt_race_pod_overheat_on", Color.FromArgb(0xFF,0xFF,0x88,0x88) },
            { "txt_race_pod_overheat_off", Color.FromArgb(0xFF,0x44,0x00,0x00) },
            { "txt_race_pod_underheat_on", Color.FromArgb(0xFF,0x88,0xCC,0xFF) },
            { "txt_race_pod_underheat_off", Color.FromArgb(0xFF,0x00,0x00,0x88) },
            { "txt_race_pod_cooling", Color.FromArgb(0xFF,0xFF,0xFF,0xFF) },
            { "txt_race_deaths", Color.FromArgb(0xFF,0xFF,0xFF,0xFF) },
            { "txt_race_times", Color.FromArgb(0xFF,0xFF,0xFF,0xFF) },
            { "txt_podsel_stats_shown", Color.FromArgb(0xFF,0xFF,0xFF,0x00) },
            { "txt_podsel_stats_hidden", Color.FromArgb(0xFF,0x33,0xFF,0xFF) }
        };
        readonly private Dictionary<string, RectangleF> ol_coords = new Dictionary<string, RectangleF>()
        {
            { "txt_debug", new RectangleF(4, 4, 1272, 32) },
            { "txt_debug2", new RectangleF(640, 684, 636, 32) },
            { "txt_race_pod_heating", new RectangleF(1032, 448, 32, 32) },
            { "txt_race_pod_cooling", new RectangleF(120, 680, 24, 24) },
            { "txt_race_deaths", new RectangleF(240, 680, 24, 24) },
            { "txt_race_times", new RectangleF(120, 160, 180, 180) },
            { "podsel_shown", new RectangleF(1084, 418, 180, 200) },
            { "podsel_hidden", new RectangleF(24, 390, 380, 216) }
        };


        // RESOURCE MANAGEMENT
        // maybe some of this can be moved to util class too?

        private void InitDevice()
        {
            if (overlay == null)
                throw new Exception("Overlay not set.");
            var description = new SwapChainDescription()
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = overlay.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out swapChain);
            device.Factory.SetWindowAssociation(overlay.Handle, WindowAssociationFlags.IgnoreAll);
        }
        private void InitContext()
        {
            context = device.ImmediateContext;
        }
        private void InitViewport()
        {
            if (overlay == null)
                throw new Exception("Overlay not set.");
            viewport = new Viewport(0.0f, 0.0f, overlay.ClientSize.Width, overlay.ClientSize.Height);
            context.Rasterizer.SetViewports(viewport);
        }
        private void InitRenderTarget()
        {
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);
            context.OutputMerger.SetTargets(renderTarget);
        }
        private void OverlayResize(object o, EventArgs e)
        {
            if (overlay == null)
                throw new Exception("Overlay not set.");
            context.Dispose();
            renderTarget.Dispose();
            sprite.Dispose();
            DisposeFont();
            swapChain.ResizeBuffers(1, overlay.ClientSize.Width, overlay.ClientSize.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            InitContext();
            InitRenderTarget();
            InitViewport();
            InitFont();
        }

        private void InitFont()
        {
            // eventually style everything properly, setup for now
            sprite = new SpriteRenderer(device) { HandleBlendState = true };
            ol_font.Add("default",
                new TextBlockRenderer(sprite, "Consolas",
                SlimDX.DirectWrite.FontWeight.DemiBold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                12)
            );
            ol_font.Add("race_botbar",
                new TextBlockRenderer(sprite, "Consolas",
                SlimDX.DirectWrite.FontWeight.DemiBold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                20)
            );
            ol_font.Add("race_heating",
                new TextBlockRenderer(sprite, "Consolas",
                SlimDX.DirectWrite.FontWeight.DemiBold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                20)
            );
            ol_font.Add("race_times",
                new TextBlockRenderer(sprite, "Consolas",
                SlimDX.DirectWrite.FontWeight.DemiBold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                18)
            );
            ol_font.Add("podsel_shown",
                new TextBlockRenderer(sprite, "Consolas",
                SlimDX.DirectWrite.FontWeight.DemiBold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                18)
            );
            ol_font.Add("podsel_hidden",
                new TextBlockRenderer(sprite, "Consolas",
                SlimDX.DirectWrite.FontWeight.DemiBold,
                SlimDX.DirectWrite.FontStyle.Normal,
                SlimDX.DirectWrite.FontStretch.Normal,
                18)
            );
        }
        private void DisposeFont()
        {
            foreach (KeyValuePair<string, TextBlockRenderer> font in ol_font.ToList())
            {
                font.Value.Dispose();
                ol_font.Remove(font.Key);
            }
        }

        private void InitImg()
        {
            ol_img.Add("cooling", //https://www.iconfinder.com/icons/1588582/snow_snowflake_icon
                new ShaderResourceView(device, Texture2D.FromFile(device, "img\\race_cooling.png"))
            );
            ol_img.Add("heating", //https://www.iconfinder.com/icons/3917272/burning_fire_icon
                new ShaderResourceView(device, Texture2D.FromFile(device, "img\\race_heating.png"))
            );
            ol_img.Add("deaths", //https://www.flaticon.com/free-icon/human-skull_63529
                new ShaderResourceView(device, Texture2D.FromFile(device, "img\\race_deaths.png"))
            );
        }
        private void DisposeImg()
        {
            foreach (KeyValuePair<string, ShaderResourceView> img in ol_img.ToList())
            {
                img.Value.Resource.Dispose();
                img.Value.Dispose();
                ol_img.Remove(img.Key);
            }
        }


        // META FUNCTIONS

        private void InitDX11()
        {
            InitDevice();
            InitContext();
            InitRenderTarget();
            InitViewport();
        }
        private void InitResources()
        {
            InitFont();
            InitImg();
        }
        private void SetOverlay()
        {
            if (overlay == null)
                overlay = new RenderForm();
        }
        private void InitOverlay()
        {
            if (overlay == null)
                throw new Exception("Overlay not set.");
            overlay.Resize += OverlayResize;
            overlay.Text = "SWE1R Overlay";
            overlay.Icon = Icon;
            overlay.TransparencyKey = ol_color["clear"];
            overlay.FormBorderStyle = FormBorderStyle.None;
            overlay.TopMost = true;
            //overlay.ShowInTaskbar = false;   /* doesn't seem to behave as expected; seems to prevent window from redrawing/running main loop? - but, would be nice to not clutter the taskbar/tab window since it's controlled by this form */
            int initialStyle = Win32.GetWindowLong(overlay.Handle, -20);
            Win32.SetWindowLong(overlay.Handle, -20, initialStyle | 0x80000 | 0x20);
        }
        private void DisposeAll()
        {
            if (overlay == null)
                throw new Exception("Overlay not set.");
            context.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
            sprite.Dispose();
            DisposeFont();
            DisposeImg();
            overlay.Dispose();
        }
        public void SetDebug(bool enable)
        {
            opt_debug = enable;
        }
        private bool CheckTargets()
        {
            // confirm everything is setup
            if (racer == null || racer.game == null || !overlay_initialized)
                return false;

            // abort and auto hide if target has closed
            if (racer.game.HasExited)
            {
                racer.game.Dispose();
                racer.game = null;
                overlay.Visible = false;
                return false;
            }

            // re-enable hidden window if auto hide no longer valid
            if (overlay_show && !overlay.Visible)
                overlay.Visible = true;

            return true;
        }

        private void Opt_showOverlay_CheckedChanged(object sender, EventArgs e)
        {
            ShowOverlay(opt_showOverlay.Checked);
        }
        private void ShowOverlay(bool show)
        {
            overlay_show = show;
            //opt_showOverlay.Checked = show;
            if (show && (overlay == null || !overlay_initialized))
            {
                SetOverlay();
                InitDX11();
                InitResources();
                InitOverlay();
                overlay_initialized = true;
            }
            if (overlay_initialized)
                overlay.Visible = show;
        }


        private void RepositionOverlay()
        {
            Win32.GetWindowRect(racer.game.MainWindowHandle, out rect);
            WINDOW_SIZE = new Vector2(rect.Width - WINDOW_BORDER[0] - WINDOW_BORDER[2], rect.Height - WINDOW_BORDER[1] - WINDOW_BORDER[3]);
            if (WINDOW_SIZE != new Vector2(overlay.Size.Width, overlay.Size.Height))
            {
                overlay.Size = new Size((int)WINDOW_SIZE.X, (int)WINDOW_SIZE.Y);
                WINDOW_SCALE.X = WINDOW_SIZE.X / WINDOW_SIZE_DFLT.X;
                WINDOW_SCALE.Y = WINDOW_SIZE.Y / WINDOW_SIZE_DFLT.Y;
                /* i.e. all scaling is relative to base 1280x720 design */
            }
            overlay.Left = rect.Left + WINDOW_BORDER[0];
            overlay.Top = rect.Top + WINDOW_BORDER[1];
        }


        // INRACE DATA HANDLER

        private class InRaceData : Racer.TwoFrameDataCollection
        {
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
                bool prev = (i < 0) ? false : (data_prev.GetValue(i) & (1 << 14)) != 0;
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


        // VEHICLE SELECT DATA HANDLING

        private class VehicleSelectData : Racer.TwoFrameDataCollection
        {
            public float[] AllStats(Racer r)
            {
                List<float> stats = new List<float>();
                byte[] raw = data.GetValue(r, Racer.DataCollection.DataBlock.Path.Static, (uint)Racer.Addr.Static.StatAntiSkid, Racer.DataType.None, 0x3C);
                for (var i = 0; i < raw.Length; i += 4)
                    stats.Add(BitConverter.ToSingle(raw, i));
                return stats.ToArray();
            }
        }


        // TEXT HANDLER

        private static class OverlayRenderer
        {
            private const string time_format = "m\\:ss\\.fff";

            public static class InRace
            {
                public static void RenderHeatTimers(ControlPanel cp, float heat, float heatrate, float coolrate, bool boosting)
                {
                    /*  todo - different ms size, colouring */
                    List<string> outTimers = new List<string>() { (heat / heatrate).ToString("0.0s"), ((100 - heat) / coolrate).ToString("0.0s") };

                    if (boosting)
                        Render.DrawIconWithText(cp.WINDOW_SCALE, cp.ol_coords["txt_race_pod_heating"], cp.sprite, cp.ol_img["heating"], outTimers,
                            cp.ol_font["race_heating"], new List<Color>() { cp.ol_color["txt_race_pod_overheat_on"], cp.ol_color["txt_race_pod_underheat_off"] }, TextAlignment.Right | TextAlignment.VerticalCenter, new Point(4, 0), sep: -6, measure: "00.0s");
                    else
                        Render.DrawIconWithText(cp.WINDOW_SCALE, cp.ol_coords["txt_race_pod_heating"], cp.sprite, cp.ol_img["heating"], outTimers,
                            cp.ol_font["race_heating"], new List<Color>() { cp.ol_color["txt_race_pod_overheat_off"], cp.ol_color["txt_race_pod_underheat_on"] }, TextAlignment.Right | TextAlignment.VerticalCenter, new Point(4, 0), sep: -6, measure: "00.0s");
                }

                public static void RenderEngineBar(ControlPanel cp, float heat, uint deaths)
                {
                    Render.DrawIconWithText(cp.WINDOW_SCALE, cp.ol_coords["txt_race_pod_cooling"], cp.sprite, cp.ol_img["cooling"], heat.ToString("0.0"),
                            cp.ol_font["race_botbar"], cp.ol_color["txt_race_pod_cooling"], TextAlignment.Left | TextAlignment.VerticalCenter, new Point(8, 0));
                    Render.DrawIconWithText(cp.WINDOW_SCALE, cp.ol_coords["txt_race_deaths"], cp.sprite, cp.ol_img["deaths"], deaths.ToString(),
                            cp.ol_font["race_botbar"], cp.ol_color["txt_race_deaths"], TextAlignment.Left | TextAlignment.VerticalCenter, new Point(8, 0));
                }

                public static void RenderTimes(ControlPanel cp, float[] timelist, string format = time_format)
                {
                    string[] labelSrc = { "1", "2", "3", "4", "5", "T" };
                    string[] times = Helper.FormatTimesArray(timelist.Where(item => item >= 0).ToArray(), format);
                    string[] labels = labelSrc.ToList().GetRange(0, times.Length).ToArray();
                    labels[labels.Length - 1] = labelSrc.Last();
                    Render.DrawTextList(cp.WINDOW_SCALE, cp.ol_coords["txt_race_times"], labels, times, cp.ol_font["race_times"], cp.ol_color["txt_race_times"], TextAlignment.Left | TextAlignment.Top, "  ");
                }

                public static void RenderMovementData(ControlPanel cp, double distance3d, double speed3d, double distancetotal, float timetotal)
                {
                    string output = distance3d.ToString("00.000") + " ADU/f  " + speed3d.ToString("000.0") + " ADU/s  " + distancetotal.ToString("0.0") + " ADU/race   " + (distancetotal / timetotal).ToString("000.0") + " avg ADU/s  ";
                    Render.DrawText(cp.WINDOW_SCALE, cp.ol_coords["txt_debug2"], output, cp.ol_font["default"], cp.ol_color["txt_debug"], TextAlignment.Right | TextAlignment.Bottom);
                }
            }

            public static class VehicleSelect
            {
                private static float[] ExtractStats(float[] vehiclestats, int[] map)
                {
                    float[] output = new float[map.Length];
                    for (var i = 0; i < map.Length; i++)
                        output[i] = (float)vehiclestats.GetValue(map[i]);
                    return output;
                }

                public static void RenderMainStats(ControlPanel cp, float[] vehiclestats)
                {
                    float[] stats = ExtractStats(vehiclestats, new int[] { 0, 1, 3, 4, 5, 9, 11 });
                    Render.DrawTextList(cp.WINDOW_SCALE, cp.ol_coords["podsel_shown"], Helper.ArrayToStrList(stats), cp.ol_font["podsel_shown"], cp.ol_color["txt_podsel_stats_shown"], TextAlignment.Left | TextAlignment.VerticalCenter);
                }

                public static void RenderHiddenStats(ControlPanel cp, float[] vehiclestats)
                {
                    string[] statNames = { "MAX TURN RATE", "DECELERATION", "BOOST THRUST", "HEAT RATE", "HOVER HEIGHT", "BUMP MASS", "DAMAGE IMMUNITY", "ISECT RADIUS" };
                    float[] stats = ExtractStats(vehiclestats, new int[] { 2, 6, 7, 8, 10, 12, 13, 14 });
                    Render.DrawTextList(cp.WINDOW_SCALE, cp.ol_coords["podsel_hidden"], statNames, stats, cp.ol_font["podsel_hidden"], cp.ol_color["txt_podsel_stats_hidden"], TextAlignment.Left | TextAlignment.Bottom, "   ");
                }
            }
        }
    }
}
