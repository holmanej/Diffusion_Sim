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

        public Color TLColor
        {
            get { return _TLColor; }
            set
            {
                if (value != _TLColor)
                {
                    Vertices[54] = value.R;
                    Vertices[55] = value.G;
                    Vertices[56] = value.B;
                    Vertices[57] = value.A;

                    RenderSections[0].VBOData = Vertices;
                }
            }
        }

        public Color Color
        {
            set
            {
                TLColor = value;
                TRColor = value;
                BLColor = value;
                BRColor = value;

                RenderSections[0].VBOData = Vertices;
            }
        }

        public Color TRColor
        {
            get { return _TRColor; }
            set
            {
                if (value != _TRColor)
                {
                    Vertices[30] = value.R;
                    Vertices[31] = value.G;
                    Vertices[32] = value.B;
                    Vertices[33] = value.A;

                    Vertices[42] = value.R;
                    Vertices[43] = value.G;
                    Vertices[44] = value.B;
                    Vertices[45] = value.A;

                    RenderSections[0].VBOData = Vertices;
                }
            }
        }

        public Color BLColor
        {
            get { return _BLColor; }
            set
            {
                if (value != _BLColor)
                {
                    Vertices[6] = value.R;
                    Vertices[7] = value.G;
                    Vertices[8] = value.B;
                    Vertices[9] = value.A;

                    Vertices[66] = value.R;
                    Vertices[67] = value.G;
                    Vertices[68] = value.B;
                    Vertices[69] = value.A;

                    RenderSections[0].VBOData = Vertices;
                }
            }
        }

        public Color BRColor
        {
            get { return _BRColor; }
            set
            {
                if (value != _BRColor)
                {
                    Vertices[18] = value.R;
                    Vertices[19] = value.G;
                    Vertices[20] = value.B;
                    Vertices[21] = value.A;

                    RenderSections[0].VBOData = Vertices;
                }
            }
        }

    }
}
