using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffusion_Sim
{
    public class TransformObject
    {
        public Vector3 _Position = new Vector3(0, 0, 0);
        public Vector3 _Scale = new Vector3(1, 1, 1);
        public Vector3 _Rotation = new Vector3(0, 0, 0);

        public Matrix4 matPos = Matrix4.Identity;
        public Matrix4 matScale = Matrix4.Identity;
        public Matrix4 matRot = Matrix4.Identity;

        public List<Matrix4> Transforms;

        public TransformObject()
        {
            Transforms = new List<Matrix4> { matPos, matRot, matScale };
        }

        public TransformObject(TransformObject obj)
        {
            Position = obj.Position;
            Scale = obj.Scale;
            Rotation = obj.Rotation;
            Transforms = new List<Matrix4>(obj.Transforms);
        }

        public void Translate(float x, float y, float z)
        {
            Position = new Vector3(Position.X + x, Position.Y + y, Position.Z + z);
        }

        public void ReSize(float x, float y, float z)
        {
            Scale = new Vector3(Scale.X * x, Scale.Y * y, Scale.Z * z);
        }

        public void Rotate(float x, float y, float z)
        {
            Rotation = new Vector3(Rotation.X + x, Rotation.Y + y, Rotation.Z + z);
        }

        public void Translate(Vector3 t)
        {
            Position = new Vector3(Position.X + t.X, Position.Y + t.Y, Position.Z + t.Z);
        }

        public void ReSize(Vector3 s)
        {
            Scale = new Vector3(Scale.X * s.X, Scale.Y * s.Y, Scale.Z * s.Z);
        }

        public void Rotate(Vector3 r)
        {
            Rotation = new Vector3(Rotation.X + r.X, Rotation.Y + r.Y, Rotation.Z + r.Z);
        }

        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                if (value != _Position)
                {
                    _Position = value;
                    matPos = Matrix4.CreateTranslation(_Position);
                    Transforms[0] = matPos;
                }
            }
        }

        public Vector3 Scale
        {
            get { return _Scale; }
            set
            {
                if (value != _Scale)
                {
                    _Scale = value;
                    matScale = Matrix4.CreateScale(_Scale);
                    Transforms[2] = matScale;
                }
            }
        }

        public Vector3 Rotation
        {
            get { return _Rotation; }
            set
            {
                if (value != _Rotation)
                {
                    _Rotation = value;
                    matRot = Matrix4.CreateRotationX(_Rotation.X * 3.14f / 180) * Matrix4.CreateRotationZ(_Rotation.Z * 3.14f / 180) * Matrix4.CreateRotationY(_Rotation.Y * 3.14f / 180);
                    Transforms[1] = matRot;
                }
            }
        }
    }
}
