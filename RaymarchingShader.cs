using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{

    //baseado em https://www.youtube.com/watch?v=khblXafu7iA&t=1001s
    internal class RaymarchingShader : BaseShader
    {
        public RaymarchingShader(Resolution r) : base(r)
        {

        }

        public override void mainImage(out Vector4 fragColor, Vector2 fragCoord)
        {
            Vector2 uv = fragCoord / iResolution.xy;
            uv.X = uv.X - 0.5f;
            uv.Y = uv.Y - 0.5f;
            uv.X = uv.X * 2;
            uv.Y = uv.Y * 2;
            uv.X *= (iResolution.x / iResolution.y);
            uv.Y *= (iResolution.y / iResolution.y);

            Vector2 mouseVec = new Vector2(iMouse.X, iMouse.Y);                
            Vector2 m = mouseVec / iResolution.xy;
            m.X = m.X - 0.5f;
            m.Y = m.Y - 0.5f;
            m.X = m.X * 2;
            m.Y = m.Y * 2;
            m.X *= (iResolution.x / iResolution.y);
            m.Y *= (iResolution.y / iResolution.y);

            Vector3 ro = new Vector3(0, 0, -3);
            Vector3 rd = Util.Normalize(new Vector3(uv.X * 1.5f, uv.Y * 1.5f, 1));
            Vector3 col = new Vector3(0, 0, 0);

            float t = 0;

            var mouseRot = rot2D(-m.X);
            ro.X = ro.X * mouseRot.m00 + ro.Z * mouseRot.m10;
            ro.Z = ro.X * mouseRot.m01 + ro.Z * mouseRot.m11;

            rd.X = rd.X * mouseRot.m00 + rd.Z * mouseRot.m10;
            rd.Z = rd.X * mouseRot.m01 + rd.Z * mouseRot.m11;

            

            for (int i = 0; i < 80; i++)
            {
                Vector3 p = ro + rd * t;

                float d = map(p);

                t += d;                

                if (d < .001 || t > 100) break;
                
            }

            col = new Vector3(t * 0.2f, t * 0.2f, t * 0.2f);

            fragColor = new Vector4(col.X, col.Y, col.Z, 1);
        }

        private float map(Vector3 p)
        {
            Vector3 spherePos = new Vector3((float)Math.Sin(iTime) * 3, 0, 0);
            float sphere = sdSphere(p - spherePos, 1);

            Vector3 q = new Vector3(p.X, p.Y, p.Z); //input copy


            //nao funciona. nao sei onde está o erro.
            //parte do video: https://youtu.be/khblXafu7iA?t=1327
            var matRot2D = rot2D(iTime); //rotate around the z axis
            q.X = q.X * matRot2D.m00 + q.Y * matRot2D.m10;
            q.Y = q.X * matRot2D.m01 + q.Y * matRot2D.m11;
            

            float box = sdBox(q, new Vector3(0.75f, 0.75f, 0.75f));

            float ground = p.Y + 0.75f;

            return smin(ground, smin(sphere, box, 2), 1.0f);
        }

        private float sdSphere(Vector3 p, float s)
        {
            return p.Length() - 1;
        }

        private float sdBox(Vector3 p, Vector3 b)
        {
            Vector3 q = new Vector3(Math.Abs(p.X), Math.Abs(p.Y), Math.Abs(p.Z)) - b;

            return (Max(q, 0.0f)).Length() + (float)Math.Min(Math.Max(q.X, Math.Max(q.Y, q.Z)), 0.0);
        }

        private Vector3 Max(Vector3 v, float x)
        {
            return new Vector3(Math.Max(v.X, x), Math.Max(v.Y, x), Math.Max(v.Z, x));
        }

        //smooth minimum
        private float smin(float a, float b, float k)
        {
            float h = Math.Max(k - Math.Abs(a - b), 0.0f) / k;
            return Math.Min(a, b) - h * h * h * k * (1.0f / 6.0f);
        }

        private float Length(Vector3 v)
        {
            return v.Length();
        }

        private float Max(float v, float x)
        {
            return Math.Max(v, x);
        }

        private float Min(float v, float x)
        {
            return Math.Min(v, x);
        }

        private Mat2 rot2D(float angle)
        {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);

            return new Mat2(c, -s, s, c);
        }

        private Vector3 rot3D(Vector3 p, Vector3 axis, float angle)
        {
            //Rodrigues' rotation formula

            Vector3 temp = Util.Dot(axis, p) * axis;

            Vector3 temp2 = new Vector3(
                Util.Mix(temp.X, p.X, (float)Math.Cos(angle)),
                Util.Mix(temp.Y, p.Y, (float)Math.Cos(angle)),
                Util.Mix(temp.Z, p.Z, (float)Math.Cos(angle))
                );

            return temp2 + Vector3.Cross(axis, p) * (float)Math.Sin(angle);
        }


        /*esta seria a formula de rotacao 3d normal (nao testei)/*
        /*private Mat3 rot3D(Vector3 axis, float angle)
        {
            axis = Util.Normalize(axis);

            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            float oc = 1.0f - c;

            return new Mat3(
                oc * axis.X * axis.X + c,
                oc * axis.X * axis.Y - axis.Z * s,
                oc * axis.Z * axis.X + axis.Y * s,
                oc * axis.X * axis.Y + axis.Z * s,
                oc * axis.Y * axis.Y + c,
                oc * axis.Y * axis.Z - axis.Y * s,
                oc * axis.Z * axis.X - axis.Y * s,
                oc * axis.Y * axis.Z + axis.X * s,
                oc * axis.Z * axis.Z + c
                ); ;

        }*/
    }

    public struct Mat2
    {
        public float m00;
        public float m01;
        public float m10;
        public float m11;

        public Mat2(float _m00, float _m01, float _m10, float _m11)
        {
            m00 = _m00;
            m01 = _m01;
            m10 = _m10;
            m11 = _m11;
        }
    }
    public struct Mat3
    {
        public float m00;
        public float m01;
        public float m02;
        public float m10;
        public float m11;
        public float m12;
        public float m20;
        public float m21;
        public float m22;

        public Mat3(float _m00, float _m01, float _m02,
            float _m10, float _m11, float _m12,
            float _m20, float _m21, float _m22)
        {
            m00 = _m00;
            m01 = _m01;
            m02 = _m02;
            m10 = _m10;
            m11 = _m11;
            m12 = _m12;
            m20 = _m20;
            m21 = _m21;
            m22 = _m22;

        }
    }
}
