using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class GLTFObject : GraphicsObject
    {
        public GLTFObject(GLTF_Converter gltf)
        {
            RenderSections = gltf.GetBufferData();
            RenderSections.Sort((a, b) => b.VBOData[9].CompareTo(a.VBOData[9]));
            Shader = Program.Shaders["gltf"];
        }
    }
}
