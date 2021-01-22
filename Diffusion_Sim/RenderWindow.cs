using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        public List<GraphicsObject> Controls = new List<GraphicsObject>();
        Shader shader;
        private const int VPosition_loc = 0;
        private const int VNormal_loc = 1;
        private const int VColor_loc = 2;
        private const int TexCoord_loc = 3;

        public Matrix4 Model;
        public Matrix4 View;
        public Matrix4 Projection;

        private int VertexArrayObject;
        private int VertexBufferObject;
        private int TextureBufferObject;


        private int VerticesLength;
        private float ZPosition = -5; // zoom
        private float XRotation = 0; // l/r
        private float ZRotation = 0; // l/r
        private float YRotation = 0; // u/d

        public RenderWindow(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            TextureBufferObject = GL.GenTexture();

            MouseMove += RenderWindow_MouseMove;
            MouseWheel += RenderWindow_MouseWheel;
            KeyPress += RenderWindow_KeyPress;
        }

        private void RenderWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void RenderWindow_MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
            {                
                XRotation += e.YDelta / 10f * (float)Math.Cos(YRotation * 3.14f / 180f);
                ZRotation += e.YDelta / 10f * (float)Math.Sin(YRotation * 3.14f / 180f);
                YRotation += e.XDelta / 10f;
                //Debug.WriteLine(YRotation + "  " + XRotation + "  " + ZRotation + "  " + Math.Cos(YRotation * 3.14f / 180f) + "  " + Math.Sin(YRotation * 3.14f / 180f));
            }
        }

        private void RenderWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.DeltaPrecise > 0)
            {
                ZPosition += 0.1f;
            }
            else
            {
                ZPosition -= 0.1f;
            }
        }

        public void BufferObject(float[] vertices, byte[] pixels, Size texSize)
        {
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            VerticesLength = vertices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.BindTexture(TextureTarget.Texture2D, TextureBufferObject);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texSize.Width, texSize.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            foreach (Engine engine in Program.Engines)
            {
                engine.Timestep();
                engine.Engine_Model.Position = new Vector3(0, 0, ZPosition);
                engine.Engine_Model.Rotation = new Vector3(XRotation, YRotation, ZRotation);
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            Model = Matrix4.CreateScale(9 / 16f, 1f, 1f);
            View = Matrix4.CreateTranslation(0f, 0f, 0f);
            Projection = Matrix4.CreatePerspectiveFieldOfView(90f * 3.14f / 180f, 1, 0.01f, 200f);

            int stride = 12;
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.EnableVertexAttribArray(VPosition_loc);
            GL.VertexAttribPointer(VPosition_loc, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0);

            GL.EnableVertexAttribArray(VNormal_loc);
            GL.VertexAttribPointer(VNormal_loc, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(VColor_loc);
            GL.VertexAttribPointer(VColor_loc, 4, VertexAttribPointerType.Float, false, stride * sizeof(float), 6 * sizeof(float));

            GL.EnableVertexAttribArray(TexCoord_loc);
            GL.VertexAttribPointer(TexCoord_loc, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), 10 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteTexture(TextureBufferObject);
            Controls.ForEach(obj => { if (obj.Shader != null) obj.Shader.Dispose(); });

            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VertexArrayObject);            

            foreach (GraphicsObject control in Controls)
            {
                TraverseObjects(control);
            }
            GL.BindVertexArray(0);

            Debug.WriteLine(RenderTime);
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void TraverseObjects(GraphicsObject graphicsObject)
        {
            if (graphicsObject.Enabled)
            {
                foreach (GraphicsObject control in graphicsObject.Controls)
                {
                    if (control.Controls == null)
                    {
                        RenderObject(control);
                    }
                    else
                    {
                        TraverseObjects(control);
                    }
                }
            }
        }

        private void RenderObject(GraphicsObject graphicsObject)
        {
            shader = graphicsObject.Shader;
            shader.Use();

            shader.SetMatrix4("model", Model);
            shader.SetMatrix4("view", View);
            shader.SetMatrix4("projection", Projection);

            shader.SetMatrix4("obj_translate", graphicsObject.matPos);
            shader.SetMatrix4("obj_scale", graphicsObject.matScale);
            shader.SetMatrix4("obj_rotate", graphicsObject.matRot);

            shader.SetTexture("texture0", 0);

            foreach (GraphicsObject.Section section in graphicsObject.RenderSections)
            {
                BufferObject(section.VBOData.ToArray(), section.ImageData, section.ImageSize);
                GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesLength);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
    }
}
