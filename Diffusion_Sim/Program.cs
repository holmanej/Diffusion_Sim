using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class Program
    {
        public static float Flow_in = 0; // air flux in m3 / s
        public static float Choke_out = 1; // ration to pipe diameter
        public static float Pi = 3.14f;
        public static float A = .001859f;
        public static float T = 300f;
        public static float R = 8.31f;
        public static float Na = 6 * (float)Math.Pow(10, 23);
        public static float Ambient_P = 1f;
        public static float Atm_Coeff = 101325f;
        public static float Air_MolarMass = 0.029f;
        public static float Air_Diameter = 55.56f;
        public static float Air_Density = 1.225f; // g / L
        public static float Pipe_Diameter = 1;
        public static float Pipe_Length = 1;
        public static float Pipe_Volume = 1;
        public static float Pipe_Area = 1;
        public static List<float> P_Values = new List<float>(); // pressure values in atm
        public static List<float> M_Values = new List<float>(); // molarity values mol / m3

        static void Main(string[] args)
        {
            using (RenderWindow SimWin = new RenderWindow(800, 800, "Diffusion Simulator"))
            {
                PrimitiveHelper pHelp = new PrimitiveHelper(SimWin);

                pHelp.AppendSquare(0f, 0f, 0f, 1f, 1f, 1f, 1f, 1f, 1f);
                pHelp.UpdateVertices();

                Pipe_Area = Pi * Pipe_Diameter * Pipe_Diameter;
                Pipe_Volume = Pipe_Area * Pipe_Length / 100;
                float initial_molarity = (Ambient_P * Atm_Coeff * Pipe_Volume) / (R * T);
                for (int i = 0; i < 102; i++)
                {
                    M_Values.Add(initial_molarity);
                    P_Values.Add(Ambient_P);
                }
                SimWin.Magnitudes = P_Values.ToArray();

                SimWin.VSync = OpenTK.VSyncMode.Adaptive;
                SimWin.Run(60, 60);
            }
        }

        public static void UpdateCalc(RenderWindow SimWin)
        {
            M_Values[0] += CalcMolarityIn(Flow_in) - CalcMolarityOut(P_Values[1]);
            Debug.WriteLine("i: 0" + "  dM: " + (CalcMolarityIn(Flow_in) - CalcMolarityOut(P_Values[1])) + "  M+: " + M_Values[0]);

            List<float> lastM = new List<float>(M_Values);
            List<float> lastP = new List<float>(P_Values);

            for (int i = 1; i < M_Values.Count - 1; i++)
            {                
                float diff_coeff = A * (float)Math.Pow(T, 1.5) * (float)Math.Sqrt(1 / Air_MolarMass + 1 / Air_MolarMass) / (lastP[i] * (float)Math.Pow((Air_Diameter + Air_Diameter) / 2, 2));
                float Divrg = lastM[i - 1] + lastM[i + 1] - 2 * lastM[i];
                //Debug.WriteLine("i: " + i + "  P: " + P_Values[i] + "  DC: " + diff_coeff + "  S: " + Divrg + "  M: " + M_Values[i]);
                M_Values[i] += diff_coeff * Divrg;
                P_Values[i] = CalcPressure(M_Values[i]) / Atm_Coeff;
            }
            M_Values[M_Values.Count - 1] -= CalcMolarityOut(P_Values[P_Values.Count - 2]);
            //Debug.WriteLine("delta mass: " + (M_Values.Sum() - lastM.Sum()));
            Debug.WriteLine("net flow: " + (CalcMolarityIn(Flow_in) - CalcMolarityOut(P_Values[P_Values.Count - 2])));
            //Debug.WriteLine("i: 101" + "  P: " + P_Values[101] + "  M: " + M_Values[101]);
            SimWin.Magnitudes = P_Values.ToArray();
        }

        public static float CalcPressure(float M)
        {
            return M * R * T / Pipe_Volume; // pascals
        }

        public static float CalcMolarityIn(float Flow_in)
        {
            float flow = Flow_in * Air_Density / Air_MolarMass; // mol / s
            return flow * (float)Math.Sqrt(2 * Pi * Air_MolarMass * R * T) / (Pipe_Area * R * T); // Pascals
        }

        public static float CalcMolarityOut(float pressure)
        {
            float flow = (pressure - Ambient_P) * Pipe_Area * Choke_out / (float)Math.Sqrt(2 * Pi * Air_MolarMass * R * T); // mol / s
            return flow / Pipe_Volume; // mol / s*m3
        }
    }
}
