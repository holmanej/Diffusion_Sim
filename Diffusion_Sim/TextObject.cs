using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diffusion_Sim
{
    class TextObject : GraphicsObject
    {
        private GlyphTypeface Font;
        private string _Content;
        private double _Size;
        private double RenderSize = 64;
        private System.Windows.Media.Color _Color;

        public TextObject(GlyphTypeface font)
        {
            Shader = Program.Shaders["text"];
            RenderSections = new List<Section>()
            {
                new Section()
                {
                    metal = 1f,
                    rough = 1f
                }
            };

            Font = font;
            _Size = 8;
            _Color = System.Windows.Media.Color.FromRgb(255, 0, 0);
        }

        private void CreateString()
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();
            Brush brush = new SolidColorBrush(_Color);

            double xAdvance = 0;
            double yAdvance = Font.AdvanceHeights[0] * RenderSize;

            foreach (char c in _Content.ToArray())
            {
                ushort glyph = Font.CharacterToGlyphMap[c];
                context.PushTransform(new TranslateTransform(xAdvance, yAdvance));
                context.DrawGeometry(brush, null, Font.GetGlyphOutline(glyph, RenderSize, 1));
                context.Pop();
                xAdvance += Font.AdvanceWidths[glyph] * RenderSize;
            }
            context.Close();

            RenderTargetBitmap bmp_render = new RenderTargetBitmap((int)xAdvance, (int)yAdvance, 72, 96, PixelFormats.Pbgra32);
            bmp_render.Clear();
            bmp_render.Render(visual);
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp_render));
            encoder.Save(stream);
            RenderSections[0].ImageData = stream.ToArray();
            RenderSections[0].ImageSize = new System.Drawing.Size((int)xAdvance, (int)yAdvance);

            float w = (float)(xAdvance / RenderSize * _Size);
            float h = (float)(yAdvance / RenderSize * _Size);
            RenderSections[0].VBOData = new List<float>()
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                    0, h, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                    0, h, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    w, h, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1
                };
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);
            bmp.Save("text_test.bmp", ImageFormat.Bmp);
        }

        public string Content
        {
            get { return _Content; }
            set
            {
                if (value != _Content)
                {
                    _Content = value;
                    CreateString();
                }
            }
        }

        public double Size
        {
            get { return _Size; }
            set
            {
                if (value != _Size)
                {
                    _Size = value;
                    CreateString();
                }
            }
        }
    }
}
