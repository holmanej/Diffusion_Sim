using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Diffusion_Sim
{
    class Program
    {
        static public Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        static public Dictionary<string, GlyphTypeface> Fonts = new Dictionary<string, GlyphTypeface>();
        static public List<Engine> Engines = new List<Engine>();

        static void Main(string[] args)
        {
            using (RenderWindow SimWin = new RenderWindow(1600, 900, "Diffusion Simulator"))
            {
                LoadShaders();
                LoadFonts();

                SetDir(@"/resources/models");
                //SimWin.GraphicsObjects.Add(new GLTFObject(new GLTF_Converter("compressor cylinder.gltf"), Shaders["shader"]));
                //SimWin.GraphicsObjects.Add(new TextObject("Hewwo", Fonts["times"], Shaders["text"]) { Position = new Vector3(-1f, -1f, 0f), Color = System.Drawing.Color.White, BGColor = System.Drawing.Color.Black, Size = 8 });
                Engine firstEngine = new Engine("cylinder_1m.gltf", "");
                Engines.Add(firstEngine);
                SimWin.Controls.Add(firstEngine);

                SimWin.VSync = VSyncMode.On;
                SimWin.Run(60, 60);
            }
        }

        static void LoadShaders()
        {
            SetDir(@"/resources/shaders");

            Debug.WriteLine("Loading Shaders");
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

            for (int i = 0; i < files.Length; i += 2)
            {
                //Debug.WriteLine(files[i + 1]);
                Shader shader = new Shader(files[i + 1], files[i]);
                string label = files[i].Substring(files[i].LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);
                shader.name = label;

                Shaders.Add(label, shader);
            }
        }

        static void LoadFonts()
        {
            SetDir(@"/resources/fonts");
            Debug.WriteLine("Loading Fonts");

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ttf");
            foreach (string f in files)
            {
                string label = f.Substring(f.LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);

                Fonts.Add(label, new GlyphTypeface(new Uri(f)));
            }
        }

        public static void SetDir(string name)
        {
            for (int i = 0; i < 10 && !Directory.GetCurrentDirectory().EndsWith("Diffusion_Sim"); i++)
            {
                Directory.SetCurrentDirectory("..");
            }
            Directory.SetCurrentDirectory("." + name);
            //Debug.WriteLine("Setting Directory");
            //Debug.WriteLine(Directory.GetCurrentDirectory());
        }
    }

    public class Color
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color White = new Color(1f, 1f, 1f, 1f);
        public static Color Black = new Color(0, 0, 0, 1f);
        public static Color Red = new Color(1f, 0.25f, 0.25f, 1f);
        public static Color DarkRed = new Color(1f, 0f, 0f, 1f);
        public static Color Blue = new Color(0f, 0f, 1f, 1f);
        public static Color LightBlue = new Color(0.25f, 0.25f, 0.75f, 1f);
        public static Color Green = new Color(0f, 0.75f, 0f, 1f);
    }
}
