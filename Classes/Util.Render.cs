using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SlimDX;
using SlimDX.Direct3D11;
using SpriteTextRenderer;
using SpriteRenderer = SpriteTextRenderer.SlimDX.SpriteRenderer;
using TextBlockRenderer = SpriteTextRenderer.SlimDX.TextBlockRenderer;

namespace SWE1R.Util
{
    public static class Render
    {
		//probably a better way to organise this

        public static void DrawIconWithText(Vector2 scale, RectangleF coords, SpriteRenderer sprite, ShaderResourceView image, String text, TextBlockRenderer font, Color color, TextAlignment align, Point offset)
        {
            var fntSz = font.FontSize * scale.X;
            var loc = new Vector2(coords.X * scale.X, coords.Y * scale.Y);
            var size = new Vector2(coords.Width * scale.X, coords.Height * scale.X); // to avoid img distortion
            sprite.Draw(image, loc, size, new Vector2(0, 0), 0, CoordinateType.Absolute);
            var region = new RectangleF(
                PointF.Add(new PointF(loc.X, loc.Y), new SizeF(size.X + offset.X * scale.X, size.Y / 2 + offset.Y * scale.Y - (float)Math.Ceiling(font.MeasureString(text, fntSz, CoordinateType.Absolute).Size.Y) / 2)),
                new SizeF(
                    (float)Math.Ceiling(font.MeasureString(text, fntSz, CoordinateType.Absolute).Size.X),
                    (float)Math.Ceiling(font.MeasureString(text, fntSz, CoordinateType.Absolute).Size.Y)
                )
            );
            font.DrawString(text, region, align, fntSz, color, CoordinateType.Absolute);
        }
        public static void DrawIconWithText(Vector2 scale, RectangleF coords, SpriteRenderer sprite, ShaderResourceView image, List<String> text, TextBlockRenderer font, List<Color> color, TextAlignment align, Point offset, int sep = 0, String measure = "000")
        {
            var fntSz = font.FontSize * scale.X;
            var loc = new Vector2(coords.X * scale.X, coords.Y * scale.Y);
            var size = new Vector2(coords.Width * scale.X, coords.Height * scale.X); // to avoid img distortion
            sprite.Draw(image, loc, size, new Vector2(0, 0), 0, CoordinateType.Absolute);
            List<RectangleF> regions = new List<RectangleF>() {
                new RectangleF(
                    PointF.Add(
                        new PointF(loc.X, loc.Y), new SizeF(size.X + offset.X * scale.X, size.Y / 2 + offset.Y * scale.Y - ((float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.Y) * text.Count() + (sep * (text.Count() - 1)) * scale.X) / 2)),
                        new SizeF((float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.X),(float)Math.Ceiling(font.MeasureString(measure, fntSz, CoordinateType.Absolute).Size.Y)
                    )
                )
            };
            for (var i = 1; i < text.Count(); i++)
                regions.Add(new RectangleF(PointF.Add(regions[0].Location, new SizeF(0, (regions[0].Height + sep) * i)), regions[0].Size));
            for (var i = 0; i < text.Count(); i++)
                font.DrawString(text[i], regions[i], align, fntSz, color[i % text.Count()], CoordinateType.Absolute);
        }
        public static void DrawTextList(Vector2 scale, RectangleF coords, List<String> text, TextBlockRenderer font, Color color, TextAlignment align)
        {
            var fntSz = font.FontSize * scale.X;
            var pos = new PointF(coords.X * scale.X, coords.Y * scale.Y);
            var size = new SizeF(coords.Width * scale.X, coords.Height / text.Count() * scale.Y);
            for (var i = 0; i < text.Count(); i++)
            {
                var region = new RectangleF(pos, size);
                region.Offset(0, size.Height * i);
                font.DrawString(text[i], region, align, fntSz, color, CoordinateType.Absolute);
            }
        }
        public static void DrawTextList(Vector2 scale, RectangleF coords, Array text1, Array text2, TextBlockRenderer font, Color color, TextAlignment align, String gap = "   ")
        {
            var fntSz = font.FontSize * scale.X;
            string[] str = new string[2];
            foreach (dynamic item in text1)
                str[0] += "\n\r" + item.ToString();
            foreach (dynamic item in text2)
                str[1] += "\n\r" + item.ToString();
            var region = new RectangleF(new PointF(coords.X * scale.X, coords.Y * scale.Y), new SizeF(coords.Width * scale.X, coords.Height * scale.Y));
            var col1_w = (float)Math.Ceiling(font.MeasureString(str[0], fntSz, CoordinateType.Absolute).Size.X);
            var gap_w = (float)Math.Ceiling(font.MeasureString(gap, fntSz, CoordinateType.Absolute).Size.X);
            font.DrawString(str[0], region, align, fntSz, color, CoordinateType.Absolute);
            region.Offset(col1_w + gap_w, 0);
            font.DrawString(str[1], region, align, fntSz, color, CoordinateType.Absolute);
        }
        public static void DrawText(Vector2 scale, RectangleF coords, string text, TextBlockRenderer font, Color color, TextAlignment align)
        {
            var fntSz = font.FontSize * scale.X;
            var pos = new PointF(coords.X * scale.X, coords.Y * scale.Y);
            var size = new SizeF(coords.Width * scale.X, coords.Height * scale.Y);
            font.DrawString(text, new RectangleF(pos, size), align, fntSz, color, CoordinateType.Absolute);
        }
    }
}
