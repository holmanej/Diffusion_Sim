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
        private float ScalingFactor = 1;
        private float ScalingDelta = 0.5f;
        private int ScaleCnt = 100;

        public GraphingObject()
        {
            Controls = new List<TransformObject>();
        }

        public void RefreshGraph(List<float> magnitudes)
        {
            Controls.Clear();

            int length = magnitudes.Count;
            float max = magnitudes.Max();

            for (int i = 0; i < ScaleCnt; i++)
            {
                float scaled = max * ScalingFactor * length / 8;

                if (scaled < length / 5f)
                {
                    ScalingFactor += ScalingDelta;
                }
                else if (scaled >= length / 2f)
                {
                    ScalingFactor -= ScalingDelta;
                }
            }

            ScaleCnt = 1;
            ScalingDelta = 0.01f;

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
        }
    }
}
