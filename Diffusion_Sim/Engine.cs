using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    class Engine : GraphicsObject
    {
        public GraphicsObject Engine_Model;
        private GraphingObject PrsrGraph;
        private GraphingObject MassGraph;

        // Constants
        private const float Pi = 3.14f;
        private const float A = .001859f; // coeff coeff
        private const float B = 8.31f; // gas constant
        private const float Na = 6E23f; // Avo
        private const float Atm_Coeff = 101325f; // pascals to atm
        private const float Air_MolarMass = 0.029f; // kg / mol
        private const float Air_Diameter = 55.56f; // Angstroms
        private const float Air_Density = 1.225f; // g / L

        // Ext Params
        private float Ambient_P = 1f; // atm
        private float Ambient_T = 300f; // K

        // Engine Params
        private float Flow_in = 10; // air flux (m3)
        private float Choke_Diameter = 1; // m
        private float Choke_Length = 1; // m
        private float Engine_Diameter = 0.25f; // m
        private float Engine_Length = 0.5f; // m
        private float Engine_Volume = 1; // m3
        private float Engine_Area = 1; // m2

        // Simulation Params
        private int Resolution = 100;

        private List<float> P_Values = new List<float>(); // pressure values (atm)
        private List<float> M_Values = new List<float>(); // mass values (mol)

        public Engine(string gltf_file, string spec_file)
        {
            Engine_Model = new GLTFObject(new GLTF_Converter(gltf_file))
            {
                Scale = new Vector3(Engine_Diameter, Engine_Diameter, Engine_Length)
            };

            MassGraph = new GraphingObject()
            {
                Scale = new Vector3(0.004f, 0.01f, 1f),
                Position = new Vector3(-0.9f, -0.9f, 0f)
            };

            PrsrGraph = new GraphingObject()
            {
                Scale = new Vector3(0.004f, 0.01f, 1f),
                Position = new Vector3(-0.4f, -0.9f, 0f)
            };

            var text = new TextObject("hewwo", Program.Fonts["times"]);

            Controls = new List<GraphicsObject>
            {
                Engine_Model
                //MassGraph
                //PrsrGraph
                //text
            };

            Engine_Area = Pi * Engine_Diameter * Engine_Diameter;
            Engine_Volume = Engine_Area * Engine_Length / Resolution;

            float initial_mass = (Ambient_P * Atm_Coeff * Engine_Volume) / (B * Ambient_T);
            for (int i = 0; i < 102; i++)
            {
                M_Values.Add(initial_mass);
                P_Values.Add(Ambient_P);
            }
        }

        public void Timestep()
        {
            M_Values[0] += Influx(Flow_in) - OutFlux(P_Values[1]);
            //Debug.WriteLine("i: 0" + "  dM: " + (Influx(Flow_in) - OutFlux(P_Values[1])) + "  M+: " + M_Values[0]);

            List<float> lastM = new List<float>(M_Values);
            List<float> lastP = new List<float>(P_Values);

            for (int i = 1; i < M_Values.Count - 1; i++)
            {
                float diff_coeff = A * (float)Math.Pow(Ambient_T, 1.5) * (float)Math.Sqrt(1 / Air_MolarMass + 1 / Air_MolarMass) / (lastP[i] * (float)Math.Pow((Air_Diameter + Air_Diameter) / 2, 2)); // cm2 / s
                float Divrg = lastM[i - 1] + lastM[i + 1] - 2 * lastM[i]; // mol
                //Debug.WriteLine("i: " + i + "  P: " + P_Values[i] + "  DC: " + diff_coeff + "  S: " + Divrg + "  M: " + M_Values[i]);
                M_Values[i] += diff_coeff / 100 / Engine_Volume * Divrg;
                P_Values[i] = CalcPressure(M_Values[i]);
            }
            M_Values[M_Values.Count - 1] -= OutFlux(P_Values[P_Values.Count - 2]);
            //Debug.WriteLine("delta mass: " + (M_Values.Sum() - lastM.Sum()));
            //Debug.WriteLine("net flow: " + (Influx(Flow_in) - OutFlux(P_Values[P_Values.Count - 2])));
            //Debug.WriteLine("i: 101" + "  P: " + P_Values[101] + "  M: " + M_Values[101]);

            //PrsrGraph.RefreshGraph(P_Values);
            MassGraph.RefreshGraph(M_Values);
        }

        private float CalcPressure(float n)
        {
            return n * B * Ambient_T / Engine_Volume / Atm_Coeff; // atm
        }

        private float Influx(float Flow_in)
        {
            return Flow_in * Ambient_P / (B * Ambient_T); // mols
        }

        private float OutFlux(float pressure)
        {
            return (pressure - Ambient_P) * Engine_Area * Choke_Diameter / (float)Math.Sqrt(2 * Pi * Air_MolarMass * B * Ambient_T); // mols
        }
    }
}
