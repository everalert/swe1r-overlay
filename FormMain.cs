using SWE1R_Overlay.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.XInput;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using SpriteTextRenderer;
using SpriteTextRenderer.SlimDX;
using SpriteRenderer = SpriteTextRenderer.SlimDX.SpriteRenderer;
using TextBlockRenderer = SpriteTextRenderer.SlimDX.TextBlockRenderer;
using System.Collections;

namespace SWE1R_Overlay
{
    public partial class FormMain : Form
    {
        // setup
        Interop.RECT rect;
        public const string WINDOW_NAME = "Episode I Racer";
        IntPtr WINDOW_HANDLE = Interop.FindWindow(null, WINDOW_NAME);
        readonly int[] WINDOW_BORDER = { 10, 32, 10, 10 };
        SizeF WINDOW_SIZE = new SizeF(1280, 720);
        SizeF WINDOW_SIZE_DFLT = new SizeF(1280, 720);
        SizeF WINDOW_SCALE = new SizeF(1, 1);
        readonly string time_format = "m\\:ss\\.fff";
        private Controller xinput;
        private State xinput_state_new;
        private State xinput_state_old;
        Stopwatch stopwatch;

        // racer
        RacerData racer = new RacerData();
        private string racer_state;

        // in race
        private uint race_pod_flags1;
        private uint race_pod_flags2;
        private bool race_pod_is_boosting;
        private bool race_pod_is_finished;
        private float race_pod_heat;
        private float race_pod_heatrate;
        private float race_pod_coolrate;
        private string race_pod_heat_txt;
        private string race_pod_overheat_txt;
        private string race_pod_underheat_txt;
        private string[] race_time;
        private float[] race_time_src;
        private string[] race_time_label;
        private string[] race_time_label_src = { "1", "2", "3", "4", "5", "T" };
        private bool race_dead_old = false;
        private bool race_dead_new = false;
        private uint race_deaths = 0;

        // pod select
        private float[] podsel_statistics;
        private float[] podsel_shown_stats;
        private float[] podsel_hidden_stats;
        int[] podsel_shown_map = { 0, 1, 3, 4, 5, 9, 11 };
        int[] podsel_hidden_map = { 2, 6, 7, 8, 10, 12, 13, 14 };
        private string[] podsel_hidden_stats_names =
        {
            "MAX TURN RATE",
            "DECELERATION",
            "BOOST THRUST",
            "HEAT RATE",
            "HOVER HEIGHT",
            "BUMP MASS",
            "DAMAGE IMMUNITY",
            "ISECT RADIUS"
        };


        //private byte[] savestate_PROCESS;
        private byte[] savestate_pod;
        //private byte[] savestate_cam1;
        //private byte[] savestate_cam2;

        RenderForm overlay = new RenderForm("SWE1R Practice Overlay");
        Device device;
        SwapChain swapChain;
        Viewport viewport;
        RenderTargetView renderTarget;
        Texture2D backBuffer;
        private dynamic context;
        SpriteRenderer sprite;
        private Dictionary<string, TextBlockRenderer> ol_font = new Dictionary<string, TextBlockRenderer>();
        private Dictionary<string, ShaderResourceView> ol_img = new Dictionary<string, ShaderResourceView>();
        private Dictionary<string, Color> ol_color = new Dictionary<string, Color>()
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
        private Dictionary<string, RectangleF> ol_coords = new Dictionary<string, RectangleF>()
        {
            { "txt_debug", new RectangleF(4, 4, 1272, 32) },
            { "txt_race_pod_heating", new RectangleF(1032, 448, 32, 32) },
            { "txt_race_pod_cooling", new RectangleF(120, 680, 24, 24) },
            { "txt_race_deaths", new RectangleF(240, 680, 24, 24) },
            { "txt_race_times", new RectangleF(120, 160, 180, 180) },
            { "podsel_shown", new RectangleF(1084, 418, 180, 200) },
            { "podsel_hidden", new RectangleF(24, 390, 380, 216) }
        };

        public FormMain()
        {
            InitializeComponent();
        }


        // OVERLAY LOGIC FLOW & MAIN LOOP

        private void FormMain_Load(object sender, EventArgs e)
        {
            // initial setup
            InitDX11();
            InitResources();
            InitOverlay();
            stopwatch = new Stopwatch();
            stopwatch.Start();

                // main loop
                MessagePump.Run(overlay, () =>
                {
                    stopwatch.Stop();
                    var txt_debug = stopwatch.ElapsedMilliseconds.ToString();
                    stopwatch = Stopwatch.StartNew();

                // logic

                UpdateXInput();
                    if (CheckXInputButtonDown(0x0001)) //DUp
                    racer.WritePodDataALL(savestate_pod);
                    if (CheckXInputButtonDown(0x0002)) //DDown
                    savestate_pod = racer.GetPodDataALL();

                    racer_state = GetGameState();

                /* implement generalised new/old state sytem before adding more UI elements? */
                    if (racer_state == "in_race")
                    {
                        race_pod_flags1 = racer.GetPodDataFlags1();
                        race_pod_flags2 = racer.GetPodDataFlags2();
                        race_pod_is_boosting = ((race_pod_flags1 & (1 << 23)) != 0);
                        race_pod_is_finished = ((race_pod_flags2 & (1 << 25)) != 0);
                        race_dead_old = race_dead_new;
                        race_dead_new = ((race_pod_flags1 & (1 << 14)) != 0);
                        if (race_dead_new && !race_dead_old)
                            race_deaths++;

                        race_pod_heat = racer.GetPodDataHeat();
                        race_pod_heatrate = racer.GetPodDataHeatRate();
                        race_pod_coolrate = racer.GetPodDataCoolRate();
                        race_pod_heat_txt = race_pod_heat.ToString("0.0");
                        race_pod_overheat_txt = (race_pod_heat / race_pod_heatrate).ToString("0.0s");
                        race_pod_underheat_txt = ((100 - race_pod_heat) / race_pod_coolrate).ToString("0.0s");

                        race_time_src = racer.GetPodTimeALL();
                        race_time = Helper.FormatTimesArray(race_time_src.Where(item => item >= 0).ToArray(), time_format);
                        race_time_label = race_time_label_src.ToList().GetRange(0, race_time.Length).ToArray();
                        race_time_label[race_time_label.Length - 1] = race_time_label_src.Last();
                    }
                    else
                        race_deaths = 0;

                    if (racer_state == "pod_select")
                    {
                        podsel_statistics = racer.GetStaticPodSelStats();
                        podsel_shown_stats = new float[7];
                        for (var i = 0; i < podsel_shown_map.Length; i++)
                            podsel_shown_stats[i] = (float)podsel_statistics.GetValue(podsel_shown_map[i]);
                        podsel_hidden_stats = new float[8];
                        for (var i = 0; i < podsel_hidden_map.Length; i++)
                            podsel_hidden_stats[i] = (float)podsel_statistics.GetValue(podsel_hidden_map[i]);

                    }

                // rendering

                Interop.GetWindowRect(WINDOW_HANDLE, out rect);
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
                    if (opt_enableDebugMenu.Checked)
                        ol_font["default"].DrawString(txt_debug, ol_coords["txt_debug"], TextAlignment.Left | TextAlignment.Top, ol_font["default"].FontSize * WINDOW_SCALE.Height, ol_color["txt_debug"], CoordinateType.Absolute);

                    if (racer_state == "in_race")
                    {
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

                        //cooling
                        DrawIconWithText(ol_coords["txt_race_pod_cooling"], ol_img["cooling"], race_pod_heat_txt,
                                ol_font["race_botbar"], ol_color["txt_race_pod_cooling"], TextAlignment.Left | TextAlignment.VerticalCenter, new Point(8, 0));

                        //deaths
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
                    if (racer_state == "pod_select")
                    {
                    //hidden stats
                    DrawTextList(ol_coords["podsel_hidden"], podsel_hidden_stats_names, podsel_hidden_stats, ol_font["podsel_hidden"], ol_color["txt_podsel_stats_hidden"], TextAlignment.Left | TextAlignment.Bottom, "   ");

                    //shown stats
                    DrawTextList(ol_coords["podsel_shown"], Helper.ArrayToStrList(podsel_shown_stats), ol_font["podsel_shown"], ol_color["txt_podsel_stats_shown"], TextAlignment.Left | TextAlignment.VerticalCenter);
                    }
                    sprite.Flush();
                    swapChain.Present(0, PresentFlags.None);
                });

            // clean up
            DisposeAll();
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
            InitXInput();
            UpdateXInput();
        }
        private void InitOverlay()
        {
            int initialStyle = Interop.GetWindowLong(overlay.Handle, -20);
            Interop.SetWindowLong(overlay.Handle, -20, initialStyle | 0x80000 | 0x20);
            overlay.Resize += OverlayResize;
            overlay.TransparencyKey = ol_color["clear"];
            overlay.FormBorderStyle = FormBorderStyle.None;
            overlay.TopMost = true;
            overlay.Show();
            opt_showOverlay.Checked = true;
        }
        private void DisposeAll()
        {
            context.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
            sprite.Dispose();
            DisposeFont();
            DisposeImg();
            overlay.Dispose();
        }


        // RESOURCE MANAGEMENT

        private void InitDevice()
        {
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
            sprite = new SpriteRenderer(device);
            sprite.HandleBlendState = true;
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

        private void InitXInput()
        {
            // https://csharp.hotexamples.com/examples/SlimDX.XInput/Controller/-/php-controller-class-examples.html
            xinput = new Controller(new UserIndex());
        }
        private void UpdateXInput()
        {
            if (xinput_state_new != null)
                xinput_state_old = xinput_state_new;
            xinput_state_new = xinput.GetState();
        }
        private bool CheckXInputButtonDown(uint button)
        {
            return ((xinput_state_old.Gamepad.Buttons.GetHashCode() & button) == 0 && (xinput_state_new.Gamepad.Buttons.GetHashCode() & button) != 0);
        }


        // LOGIC FUNCTIONS

        private string GetGameState()
        {
            var gameInRace = racer.GetStaticInRace();
            var gameScene = racer.GetStaticScene();
            if (gameInRace == 1)
                return "in_race";
            if (gameScene == 60)
                return "pod_select";
            if (gameScene == 260)
                return "track_select";
            return "";
        }


        // RENDERING FUNCTIONS

        private void DrawIconWithText(RectangleF coords, ShaderResourceView image, String text, TextBlockRenderer font, Color color, TextAlignment align, Point offset)
        {
            var fntSz = font.FontSize * WINDOW_SCALE.Height;
            var loc = new Vector2(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new Vector2(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Height);
            sprite.Draw(image, loc, size, new Vector2(0, 0), 0, CoordinateType.Absolute);
            var region = new RectangleF(
                PointF.Add(new PointF(loc.X, loc.Y), new SizeF(size.X + offset.X * WINDOW_SCALE.Width, size.Y / 2 + offset.Y * WINDOW_SCALE.Height - (float)Math.Ceiling(font.MeasureString(text, fntSz, CoordinateType.Absolute).Size.Y) / 2)),
                new SizeF(
                    (float)Math.Ceiling(font.MeasureString(text, fntSz, CoordinateType.Absolute).Size.X),
                    (float)Math.Ceiling(font.MeasureString(text, fntSz, CoordinateType.Absolute).Size.Y)
                )
            );
            font.DrawString(text, region, align, fntSz, color, CoordinateType.Absolute);
        }
        private void DrawIconWithText(RectangleF coords, ShaderResourceView image, List<String> text, TextBlockRenderer font, List<Color> color, TextAlignment align, Point offset, int sep = 0, String measure = "000")
        {
            var fntSz = font.FontSize * WINDOW_SCALE.Height;
            var loc = new Vector2(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new Vector2(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Height);
            sprite.Draw(image, loc, size, new Vector2(0, 0), 0, CoordinateType.Absolute);
            List<RectangleF> regions = new List<RectangleF>();
            regions.Add(new RectangleF(
                PointF.Add(new PointF(loc.X, loc.Y), new SizeF(size.X + offset.X * WINDOW_SCALE.Width,
                    size.Y / 2 + offset.Y * WINDOW_SCALE.Height - ((float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.Y) * text.Count() + (sep * (text.Count() - 1)) * WINDOW_SCALE.Width) / 2)),
                new SizeF(
                    (float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.X),
                    (float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.Y)
                )
            ));
            for (var i = 1; i < text.Count(); i++)
                regions.Add(new RectangleF(PointF.Add(regions[0].Location, new SizeF(0, (regions[0].Height + sep) * i)), regions[0].Size));
            for (var i = 0; i < text.Count(); i++)
                font.DrawString(text[i], regions[i], align, fntSz, color[i % text.Count()], CoordinateType.Absolute);
        }
        private void DrawTextList(RectangleF coords, List<String> text, TextBlockRenderer font, Color color, TextAlignment align)
        {
            var pos = new PointF(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new SizeF(coords.Width * WINDOW_SCALE.Width, coords.Height / text.Count() * WINDOW_SCALE.Height);
            for (var i = 0; i < text.Count(); i++)
            {
                var region = new RectangleF(pos, size);
                region.Offset(0, size.Height * i);
                font.DrawString(text[i], region, align, font.FontSize * WINDOW_SCALE.Height, color, CoordinateType.Absolute);
            }
        }
        private void DrawTextList(RectangleF coords, Array text1, Array text2, TextBlockRenderer font, Color color, TextAlignment align, String gap = "   ")
        {
            string[] str = new string[2];
            foreach (dynamic item in text1)
                str[0] += "\n\r" + item.ToString();
            foreach (dynamic item in text2)
                str[1] += "\n\r" + item.ToString();
            var region = new RectangleF(new PointF(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height), new SizeF(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Height));
            var col1_w = (float)Math.Ceiling(font.MeasureString(str[0], font.FontSize * WINDOW_SCALE.Height, CoordinateType.Absolute).Size.X);
            var gap_w = (float)Math.Ceiling(font.MeasureString(gap, font.FontSize * WINDOW_SCALE.Height, CoordinateType.Absolute).Size.X);
            font.DrawString(str[0], region, align, font.FontSize * WINDOW_SCALE.Height, color, CoordinateType.Absolute);
            region.Offset(col1_w + gap_w, 0);
            font.DrawString(str[1], region, align, font.FontSize * WINDOW_SCALE.Height, color, CoordinateType.Absolute);
        }


        // CONTROL PANEL

        private void opt_showOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_showOverlay.Checked)
                overlay.Show();
            else
                overlay.Hide();
        }
        private void Btn_stateS_Click(object sender, EventArgs e)
        {
            savestate_pod = racer.GetPodDataALL();
        }
        private void Btn_stateL_Click(object sender, EventArgs e)
        {
            racer.WritePodDataALL(savestate_pod);
        }
        private void Opt_enableDebugMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_enableDebugMenu.Checked)
                racer.EnableDebugMenu();
            else
                racer.DisableDebugMenu();
        }
        private void Opt_enableInvincibility_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_enableInvincibility.Checked)
                racer.EnableDebugInvincibility();
            else
                racer.DisableDebugInvincibility();
        }
        private void Opt_showTerrainFlags_CheckedChanged(object sender, EventArgs e)
        {
            if (opt_showTerrainFlags.Checked)
                racer.EnableDebugTerrainLabels();
            else
                racer.DisableDebugTerrainLabels();
        }
    }
}
