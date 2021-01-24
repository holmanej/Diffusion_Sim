using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class GraphingObject : GraphicsObject
    {
        private TextObject Title;
        private TextObject XLabel;
        private TextObject YLabel;
        private float ScalingFactor = 1;

        public GraphingObject()
        {
            Controls = new List<GraphicsObject>();

            Title = new TextObject("Title", Program.Fonts["times"])
            {
                Size = 20,
                Color = System.Drawing.Color.Black,
                BGColor = System.Drawing.Color.White
            };
            XLabel = new TextObject("X", Program.Fonts["times"])
            {
                Size = 12,
                Color = System.Drawing.Color.Black,
                BGColor = System.Drawing.Color.White
            };
            YLabel = new TextObject("Y", Program.Fonts["times"])
            {
                Size = 12,
                Color = System.Drawing.Color.Black,
                BGColor = System.Drawing.Color.White
            };
        }

        public void RefreshGraph(List<float> magnitudes)
        {
            Controls.Clear();

            int length = magnitudes.Count;
            float max = magnitudes.Max();
            float scaled = max * ScalingFactor * length / 8;

            if (scaled < length / 5f)
            {
                ScalingFactor += 0.01f;
            }
            else if (scaled >= length / 2f)
            {
                ScalingFactor -= 0.01f;
            }

            Controls.Add(new RectObject() // Y Axis
            {
                Position = new Vector3(0, length / 4f - 0.5f, 0f),
                Scale = new Vector3(1.5f, length / 2f, 1f),
                Color = new Color(0, 0, 0, 1)
            });
            Controls.Add(new RectObject() // X Axis
            {
                Position = new Vector3(length / 2f + 1, 0f, 0f),
                Scale = new Vector3(length + 1, 1, 0),
                Color = new Color(0, 0, 0, 1)
            });

            for (int i = 1; i < length - 1; i++)
            {
                float m = magnitudes[i] * ScalingFactor * length / 8;
                Controls.Add(new RectObject()
                {
                    Position = new Vector3(1 + i, 2 + m / 2f, 0f),
                    Scale = new Vector3(1, m, 0),
                    TLColor = Color.Red,
                    TRColor = Color.Red,
                    BLColor = Color.LightBlue,
                    BRColor = Color.LightBlue
                });
            }

            //Title.Position = new Vector3(0, 0, 0);
            //Title.Position = new Vector3(length / 2f, length + 10, 0);
            Controls.Add(Title);
            //XLabel.Position = new Vector3(length / 2f, -10, 0);
            //Controls.Add(XLabel);
            //YLabel.Position = new Vector3(-10, length / 2f, 0);
            //Controls.Add(YLabel);
        }
    }
}
