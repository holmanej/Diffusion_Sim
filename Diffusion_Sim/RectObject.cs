using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class RectObject : GraphicsObject
    {
        public List<float> Vertices;

        private Color _TLColor = Color.Black;
        private Color _TRColor = Color.Black;
        private Color _BLColor = Color.Black;
        private Color _BRColor = Color.Black;

        public RectObject()
        {
            Vertices = new List<float>()
            {
            //  X      Y      Z   Nx  Ny  Nz  R   G   B   A   Texture
                -0.5f, -0.5f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f, 0f,
                 0.5f, -0.5f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f, 0f,
                 0.5f,  0.5f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f, 0f,
                 0.5f,  0.5f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f, 0f,
                -0.5f,  0.5f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f, 0f,
                -0.5f, -0.5f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f, 0f
            };

            RenderSections = new List<Section>
            {
                new Section()
                {
                    VBOData = Vertices,
                    metal = 1f,
                    rough = 1f,
                }
            };

            Shader = Program.Shaders["rect"];
        }

        public Color TopColor
        {
            get { return _TLColor; }
            set
            {
                if (value != _TLColor)
                {
                    Vertices[34] = value.R;
                    Vertices[35] = value.G;
                    Vertices[36] = value.B;
                    Vertices[37] = value.A;

                    RenderSections[0].VBOData = Vertices;
                }
            }
        }

    }
}
