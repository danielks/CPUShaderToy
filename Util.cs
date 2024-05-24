using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    public static class Util
    {
        public static float Step(float threshold, float value)
        {
            return value < threshold ? 0 : 1;
        }

        //ChatGPT wrote this.
        public static float Smoothstep(float edge0, float edge1, float x)
        {

            // Ensure that x is clamped between edge0 and edge1
            x = Math.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            // Perform the smooth Hermite interpolation
            return x * x * (3 - 2 * x);
        }

        public static float Fract(float x)
        {
            return x - (float)Math.Floor(x);
        }


        public static Vector3 Normalize(Vector3 v)
        {
            float length = v.Length();
            return new Vector3(v.X / length, v.Y / length, v.Z / length);
        }

        public static byte DeNormalize(float v)
        {
            if (v < 0) { return 0; }

            if (v > 1.0f) { v = 1; }

            byte max = Byte.MaxValue;

            return Convert.ToByte(v * max);
        }

        public static Raylib_cs.Color ColorFromVector4(Vector4 v)
        {
            return new Raylib_cs.Color(
                    Util.DeNormalize(v.X),
                    Util.DeNormalize(v.Y),
                    Util.DeNormalize(v.Z),
                    Util.DeNormalize(v.W));
        }

        public static float Mix(float x, float y, float a)
        {
            return x * (1 - a) + y * a;
        }        

        public static Vector3 Sign(Vector3 v)
        {
            return new Vector3(Math.Sign(v.X), Math.Sign(v.Y), Math.Sign(v.Z));
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
    }
}
