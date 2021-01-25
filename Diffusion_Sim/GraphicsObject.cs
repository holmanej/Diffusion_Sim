using OpenTK;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    public class GraphicsObject : TransformObject
    {
        public class Section
        {
            public List<float> VBOData;
            public byte[] ImageData;
            public Size ImageSize;
            public float metal;
            public float rough;
        }

        public List<TransformObject> Controls;
        public List<Section> RenderSections;
        public Shader Shader;

        public bool Enabled = true;
        public bool Collidable = false;

        public GraphicsObject()
        {

        }

        public GraphicsObject(TransformObject obj)
        {
            Controls = new List<TransformObject>()
            {
                obj
            };
        }
    }
}
