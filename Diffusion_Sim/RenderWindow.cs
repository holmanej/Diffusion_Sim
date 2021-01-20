using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Diffusion_Sim
{
    class RenderWindow : GameWindow
    {
        public float[] Vertices;
        public float[] Magnitudes;

        public Matrix4 Model;
        public Matrix4 View;
        public Matrix4 Projection;

        private int VertexArrayObject;
        private int VertexBufferObject;
        private int MagnitudeBufferObject;
        private Shader Shader0;

        private int BufferLength;
        private float ZPosition = -70; // zoom
        private float XPosition = -50; // l/r
        private float YPosition = -40; // u/d

        private float start = 0;
        

        public RenderWindow(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            MagnitudeBufferObject = GL.GenBuffer();

            MouseMove += RenderWindow_MouseMove;
            MouseWheel += RenderWindow_MouseWheel;
            KeyPress += RenderWindow_KeyPress;
        }

        private void RenderWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Fan Speed
            if (e.KeyChar == 'w')
            {
                Program.Flow_in++;
            }
            if (e.KeyChar == 's' && Program.Flow_in > 0)
            {
                Program.Flow_in--;
            }

            // Temp
            if (e.KeyChar == 'e')
            {
                Program.T += 10;
            }
            if (e.KeyChar == 'd' && Program.T > 20)
            {
                Program.T -= 10;
            }

            // Pipe Diameter
            if (e.KeyChar == 'r')
            {
                Program.Pipe_Diameter += 0.01f;
            }
            if (e.KeyChar == 'f' && Program.Pipe_Diameter > 0.02f)
            {
                Program.Pipe_Diameter -= 0.01f;
            }

            // Pipe Length
            if (e.KeyChar == 't')
            {
                Program.Pipe_Length += 0.01f;
            }
            if (e.KeyChar == 'g' && Program.Pipe_Length > 0.02f)
            {
                Program.Pipe_Length -= 0.01f;
            }

            // Reset
            if (e.KeyChar == 'c')
            {
                Program.Flow_in = 0;
                List<float> list = new List<float>();
                for (int i = 0; i < 102; i++) list.Add(15);
                Program.P_Values = list;
            }
            Console.WriteLine("Fan: " + Program.Flow_in + "  Temp: " + Program.T + "  Pipe D: " + Program.Pipe_Diameter + "  Pipe L: " + Program.Pipe_Length + "  Total Mass: " + Program.M_Values.Sum());
        }

        private void RenderWindow_MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
            {
                YPosition -= e.YDelta / 50f;
                XPosition += e.XDelta / 50f;
            }
        }

        private void RenderWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.DeltaPrecise > 0)
            {
                ZPosition += 1f;
            }
            else
            {
                ZPosition -= 1f;
            }
        }

        public void BufferObjects()
        {
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            BufferLength = Vertices.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, MagnitudeBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Magnitudes.Length * sizeof(float), Magnitudes, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            start++;
            if (start > 100) Program.UpdateCalc(this);
            BufferObjects();

            base.OnUpdateFrame(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            Shader0 = new Shader(@"shader.vert", @"shader.frag");
            Shader0.Use();

            Model = Matrix4.CreateRotationX(0f);
            View = Matrix4.CreateTranslation(0f, 0f, 0f);
            Projection = Matrix4.CreatePerspectiveFieldOfView(90f * 3.14f / 180f, 1, 0.01f, 200f);

            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, MagnitudeBufferObject);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 1 * sizeof(float), 0);
            GL.VertexAttribDivisor(2, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Console.WriteLine("Fan: " + Program.Flow_in + "  Temp: " + Program.T + "  Pipe D: " + Program.Pipe_Diameter + "  Pipe L: " + Program.Pipe_Length);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.DeleteBuffer(VertexBufferObject);
            Shader0.Dispose();

            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Shader0.Use();

            View = Matrix4.CreateTranslation(XPosition, YPosition, ZPosition);

            Shader0.SetMatrix4("model", Model);
            Shader0.SetMatrix4("view", View);
            Shader0.SetMatrix4("projection", Projection);

            GL.BindVertexArray(VertexArrayObject);
            //Debug.WriteLine(BufferLength);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, BufferLength, Magnitudes.Length);
            GL.BindVertexArray(0);

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
    }
}
