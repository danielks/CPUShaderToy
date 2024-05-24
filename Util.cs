using System;
using System.Collections.Generic;
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

        public static float Mix(float x, float y, float a)
        {
            return x * (1 - a) + y * a;
        }

        public static float[] DotProduct(Vector3 vec1, Vector3 vec2)
        {
            return new float[] { vec1[0] * vec2[0], vec1[1] * vec2[1], vec1[2] * vec2[2] };
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
