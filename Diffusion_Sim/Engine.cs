using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class Engine
    {
        public GLTFObject Engine_Model;        

        // Constants
        private const float Pi = 3.14f;
        private const float A = .001859f; // coeff coeff
        private const float R = 8.31f; // gas constant
        private const float Na = 6E23f; // Avo
        private const float Atm_Coeff = 101325f; // pascals to atm
        private const float Air_MolarMass = 0.029f; // kg / mol
        private const float Air_Diameter = 55.56f; // Angstroms
        private const float Air_Density = 1.225f; // g / L

        // Ext Params
        private float Ambient_P = 1f; // atm
        private float Ambient_T = 300f; // K

        // Engine Params
        private float Flow_in = 0; // air flux in m3 / s
        private float Choke_out = 1; // ratio to pipe diameter
        private float Pipe_Diameter = 1; // m
        private float Pipe_Length = 1; // m
        private float Pipe_Volume = 1; // m3
        private float Pipe_Area = 1; // m2

        // Simulation Params
        private int Resolution = 100;

        private List<float> P_Values = new List<float>(); // pressure values (atm)
        private List<float> M_Values = new List<float>(); // mass values (mol)

        public Engine(string gltf_file, string spec_file)
        {
            Engine_Model = new GLTFObject(new GLTF_Converter(gltf_file), Program.Shaders["shader"]);

            Pipe_Area = Pi * Pipe_Diameter * Pipe_Diameter;
            Pipe_Volume = Pipe_Area * Pipe_Length / Resolution;

            float initial_mass = (Ambient_P * Atm_Coeff * Pipe_Volume) / (R * Ambient_T);
            for (int i = 0; i < 102; i++)
            {
                M_Values.Add(initial_mass);
                P_Values.Add(Ambient_P);
            }
        }

        public void Timestep()
        {
            M_Values[0] += CalcMolarityIn(Flow_in) - CalcMolarityOut(P_Values[1]);
            Debug.WriteLine("i: 0" + "  dM: " + (CalcMolarityIn(Flow_in) - CalcMolarityOut(P_Values[1])) + "  M+: " + M_Values[0]);

            List<float> lastM = new List<float>(M_Values);
            List<float> lastP = new List<float>(P_Values);

            for (int i = 1; i < M_Values.Count - 1; i++)
            {
                float diff_coeff = A * (float)Math.Pow(Ambient_T, 1.5) * (float)Math.Sqrt(1 / Air_MolarMass + 1 / Air_MolarMass) / (lastP[i] * (float)Math.Pow((Air_Diameter + Air_Diameter) / 2, 2));
                float Divrg = lastM[i - 1] + lastM[i + 1] - 2 * lastM[i];
                //Debug.WriteLine("i: " + i + "  P: " + P_Values[i] + "  DC: " + diff_coeff + "  S: " + Divrg + "  M: " + M_Values[i]);
                M_Values[i] += diff_coeff * Divrg;
                P_Values[i] = CalcPressure(M_Values[i]) / Atm_Coeff;
            }
            M_Values[M_Values.Count - 1] -= CalcMolarityOut(P_Values[P_Values.Count - 2]);
            //Debug.WriteLine("delta mass: " + (M_Values.Sum() - lastM.Sum()));
            Debug.WriteLine("net flow: " + (CalcMolarityIn(Flow_in) - CalcMolarityOut(P_Values[P_Values.Count - 2])));
            //Debug.WriteLine("i: 101" + "  P: " + P_Values[101] + "  M: " + M_Values[101]);
        }

        private float CalcPressure(float M)
        {
            return M * R * Ambient_T / Pipe_Volume; // pascals
        }

        private float CalcMolarityIn(float Flow_in)
        {
            float flow = Flow_in * Air_Density / Air_MolarMass; // mol / s
            return flow * (float)Math.Sqrt(2 * Pi * Air_MolarMass * R * Ambient_T) / (Pipe_Area * R * Ambient_T); // mol
        }

        private float CalcMolarityOut(float pressure)
        {
            float flow = (pressure - Ambient_P) * Pipe_Area * Choke_out / (float)Math.Sqrt(2 * Pi * Air_MolarMass * R * Ambient_T); // mol
            return flow / Pipe_Volume; // mol
        }
    }
}
