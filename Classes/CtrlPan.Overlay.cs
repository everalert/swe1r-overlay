using SWE1R.Util;
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
using System.Threading;

namespace SWE1R
{
    public partial class ControlPanel : Form
    {
        //readonly ControlPanel controlpanel;

        // setup
        Win32.RECT rect;
        readonly int[] WINDOW_BORDER = { 10, 32, 10, 10 };
        SizeF WINDOW_SIZE = new SizeF(1280, 720);
        readonly SizeF WINDOW_SIZE_DFLT = new SizeF(1280, 720);
        SizeF WINDOW_SCALE = new SizeF(1, 1);
        readonly string time_format = "m\\:ss\\.fff";
        Stopwatch stopwatch;
        private bool opt_debug = false;

        // racer
        private string racer_state_old, racer_state_new;

        // in race
        private uint race_pod_flags1, race_pod_flags2;
        private bool race_pod_is_boosting, race_pod_is_finished;
        private Vector3 race_pod_loc_new, race_pod_loc_old;
        private double race_pod_dist_frame, race_pod_dist_total;
        private string race_pod_loc_txt;
        private float race_pod_heat, race_pod_heatrate, race_pod_coolrate;
        private string race_pod_heat_txt, race_pod_overheat_txt, race_pod_underheat_txt;
        private string[] race_time;
        private float[] race_time_src;
        private string[] race_time_label;
        readonly private string[] race_time_label_src = { "1", "2", "3", "4", "5", "T" };
        private bool race_dead_old = false;
        private bool race_dead_new = false;
        private uint race_deaths = 0;

        // pod select
        private float[] podsel_statistics;
        private float[] podsel_shown_stats;
        private float[] podsel_hidden_stats;
        readonly int[] podsel_shown_map = { 0, 1, 3, 4, 5, 9, 11 };
        readonly int[] podsel_hidden_map = { 2, 6, 7, 8, 10, 12, 13, 14 };
        readonly private string[] podsel_hidden_stats_names =
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


        // OVERLAY LOGIC FLOW & MAIN LOOP

        //public void Overlay(ControlPanel ctrl, Process tgt, Racer rcr)
        //{
        //    //controlpanel = ctrl;
        //    //this.Icon = controlpanel.Icon;
        //    //input = new Input(this);

        //    // initial setup
        //    InitDX11();
        //    InitResources();
        //    InitOverlay();
        //    stopwatch = new Stopwatch();
        //    stopwatch.Start();

        //    Application.Idle += new EventHandler(OnAppIdle);

        //    // clean up
        //    //DisposeAll();
        //    /* where to call this now that the main loop is asychronous? should eventually garbage collect by itself regardless tho */
        //}


        // RENDERING FUNCTIONS

        private void DrawIconWithText(RectangleF coords, ShaderResourceView image, String text, TextBlockRenderer font, Color color, TextAlignment align, Point offset)
        {
            var fntSz = font.FontSize * WINDOW_SCALE.Width;
            var loc = new Vector2(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new Vector2(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Width); // to avoid img distortion
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
            var fntSz = font.FontSize * WINDOW_SCALE.Width;
            var loc = new Vector2(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new Vector2(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Width); // to avoid img distortion
            sprite.Draw(image, loc, size, new Vector2(0, 0), 0, CoordinateType.Absolute);
            List<RectangleF> regions = new List<RectangleF>() {
                new RectangleF(
                    PointF.Add(
                        new PointF(loc.X, loc.Y), new SizeF(size.X + offset.X * WINDOW_SCALE.Width, size.Y / 2 + offset.Y * WINDOW_SCALE.Height - ((float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.Y) * text.Count() + (sep * (text.Count() - 1)) * WINDOW_SCALE.Width) / 2)),
                        new SizeF((float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.X),(float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.Y)
                    )
                )
            };
            for (var i = 1; i < text.Count(); i++)
                regions.Add(new RectangleF(PointF.Add(regions[0].Location, new SizeF(0, (regions[0].Height + sep) * i)), regions[0].Size));
            for (var i = 0; i < text.Count(); i++)
                font.DrawString(text[i], regions[i], align, fntSz, color[i % text.Count()], CoordinateType.Absolute);
        }
        private void DrawTextList(RectangleF coords, List<String> text, TextBlockRenderer font, Color color, TextAlignment align)
        {
            var fntSz = font.FontSize * WINDOW_SCALE.Width;
            var pos = new PointF(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new SizeF(coords.Width * WINDOW_SCALE.Width, coords.Height / text.Count() * WINDOW_SCALE.Height);
            for (var i = 0; i < text.Count(); i++)
            {
                var region = new RectangleF(pos, size);
                region.Offset(0, size.Height * i);
                font.DrawString(text[i], region, align, fntSz, color, CoordinateType.Absolute);
            }
        }
        private void DrawTextList(RectangleF coords, Array text1, Array text2, TextBlockRenderer font, Color color, TextAlignment align, String gap = "   ")
        {
            var fntSz = font.FontSize * WINDOW_SCALE.Width;
            string[] str = new string[2];
            foreach (dynamic item in text1)
                str[0] += "\n\r" + item.ToString();
            foreach (dynamic item in text2)
                str[1] += "\n\r" + item.ToString();
            var region = new RectangleF(new PointF(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height), new SizeF(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Height));
            var col1_w = (float)Math.Ceiling(font.MeasureString(str[0], fntSz, CoordinateType.Absolute).Size.X);
            var gap_w = (float)Math.Ceiling(font.MeasureString(gap, fntSz, CoordinateType.Absolute).Size.X);
            font.DrawString(str[0], region, align, fntSz, color, CoordinateType.Absolute);
            region.Offset(col1_w + gap_w, 0);
            font.DrawString(str[1], region, align, fntSz, color, CoordinateType.Absolute);
        }
        private void DrawText(RectangleF coords, string text, TextBlockRenderer font, Color color, TextAlignment align)
        {
            var fntSz = font.FontSize * WINDOW_SCALE.Width;
            var pos = new PointF(coords.X * WINDOW_SCALE.Width, coords.Y * WINDOW_SCALE.Height);
            var size = new SizeF(coords.Width * WINDOW_SCALE.Width, coords.Height * WINDOW_SCALE.Height);
            font.DrawString(text, new RectangleF(pos, size), align, fntSz, color, CoordinateType.Absolute);
        }


        // RESOURCE MANAGEMENT

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


        // LOGIC FUNCTIONS

        private string GetGameState()
        {
            var gameInRace = racer.GetData(Racer.Addr.Static.InRace);
            var gameScene = racer.GetData(Racer.Addr.Static.SceneId);
            if (gameInRace == 1)
                return "in_race";
            if (gameScene == 60)
                return "pod_select";
            if (gameScene == 260)
                return "track_select";
            return "";
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
            overlay.Icon = this.Icon;
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
    }
}
