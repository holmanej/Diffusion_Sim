using OpenTK;
using OpenTK.Graphics.ES20;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class PrimitiveHelper
    {
        private List<float> Vertices = new List<float>();        

        private RenderWindow Window;

        public PrimitiveHelper(RenderWindow window)
        {
            Window = window;
        }

        public void ClearVertices()
        {
            Vertices.Clear();
        }

        public void UpdateVertices()
        {
            Debug.WriteLine("Vertices Count: " + Vertices.Count);
            Window.Vertices = Vertices.ToArray();
        }

        public void AppendTriangle(List<float> vertices)
        {
            Vertices.AddRange(vertices);
        }

        public void AppendSquare(float x, float y, float z, float w, float h, float r, float b, float g, float a)
        {
            List<float> vertices = new List<float>()
            {
                x, y, z,              r, b, g, a,		// 0
                x + w, y, z,          r, b, g, a,		// 1
                x, y + h, z,          r, b, g, a,		// 2
                x + w, y, z,          r, b, g, a,		// 1
                x, y + h, z,          r, b, g, a,		// 2
                x + w, y + h, z,      r, b, g, a,		// 3
            };
            Vertices.AddRange(vertices);
        }

    }
}
