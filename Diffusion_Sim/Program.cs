using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class Program
    {
        static public Dictionary<string, Shader> Shaders;
        static public Dictionary<string, FontFamily> Fonts;
        static public List<Engine> Engines = new List<Engine>();

        static void Main(string[] args)
        {
            using (RenderWindow SimWin = new RenderWindow(1600, 900, "Diffusion Simulator"))
            {
                Shaders = LoadShaders();
                Fonts = LoadFonts();

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

        static Dictionary<string, Shader> LoadShaders()
        {
            SetDir(@"/resources/shaders");

            Debug.WriteLine("Loading Shaders");
            Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

            for (int i = 0; i < files.Length; i += 2)
            {
                //Debug.WriteLine(files[i + 1]);
                Shader shader = new Shader(files[i + 1], files[i]);
                string label = files[i].Substring(files[i].LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);

                shaders.Add(label, shader);
            }

            return shaders;
        }

        static Dictionary<string, FontFamily> LoadFonts()
        {
            SetDir(@"/resources/fonts");
            Debug.WriteLine("Loading Fonts");

            Dictionary<string, FontFamily> fonts = new Dictionary<string, FontFamily>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ttf");
            foreach (string f in files)
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(f);
                string label = f.Substring(f.LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);

                fonts.Add(label, pfc.Families[0]);
            }

            return fonts;
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
        public static Color Black = new Color(0f, 0f, 0f, 1f);
        public static Color Red = new Color(1f, 0.25f, 0.25f, 1f);
        public static Color DarkRed = new Color(1f, 0f, 0f, 1f);
        public static Color Blue = new Color(0f, 0f, 1f, 1f);
        public static Color LightBlue = new Color(0.25f, 0.25f, 0.75f, 1f);
    }
}
