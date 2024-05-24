using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    internal class FractalPyramidShader : IShader
    {

        private Resolution iResolution;
        public float iTime;

        public FractalPyramidShader(Resolution r)
        {
            iResolution = r;
        }
        public void mainImage(out Vector4 fragColor, Vector2 fragCoord)
        {
            //Vector2 uv = (fragCoord - (iResolution.xy / 2.0f)) / iResolution.x;

            Vector2 uv = fragCoord / iResolution.xy;
            uv.X = uv.X - 0.5f;
            uv.Y = uv.Y - 0.5f;
            uv.X = uv.X * 2;
            uv.Y = uv.Y * 2;
            uv.X *= (iResolution.x / iResolution.y);

            


            Vector3 ro = new Vector3(0, 0, -50f);
            Vector2 temp = rotate(new Vector2(ro.X, ro.Z), iTime);
            ro.X = temp.X;
            ro.Z = temp.Y;

            Vector3 cf = Util.Normalize(-ro);

            Vector3 cs = Util.Normalize(Vector3.Cross(cf, new Vector3(0, 1, 0)));
            Vector3 cu = Util.Normalize(Vector3.Cross(cf, cs));

            Vector3 uuv = ro + cf * 3.0f + uv.X * cs + uv.Y * cu;

            Vector3 rd = Util.Normalize(uuv - ro);

            Vector4 col = rm(ro, rd);

            

            fragColor = col;
        }

        Vector3 palette(float d)
        {
            return new Vector3(
                Util.Mix(0.2f, 1.0f, d),
                Util.Mix(0.7f, 0.0f, d),
                Util.Mix(0.9f, 1.0f, d));
        }

        Vector2 rotate(Vector2 p, float a)
        {
            float c = (float)Math.Cos(a);
            float s = (float)Math.Sin(a);

            Mat2 rotationMatrix = new Mat2(c, s, -s, c);


           


            return new Vector2(
                 p.X * rotationMatrix.m00 + p.Y * rotationMatrix.m10,
                 p.X * rotationMatrix.m01 + p.Y * rotationMatrix.m11);
        }


        private float map(Vector3 p)
        {
            for (int i = 0; i < 8; ++i)
            {
                float t = iTime * 0.2f;

                Vector2 temp = rotate(new Vector2(p.X, p.Z), t);
                p.X = temp.X;
                p.Z = temp.Y;

                temp = rotate(new Vector2(p.X, p.Y), t * 1.89f);
                p.X = temp.X;
                p.Y = temp.Y;

                p.X = Math.Abs(p.X);
                p.Z = Math.Abs(p.Z);

                p.X -= 0.5f;
                p.Z -= 0.5f;
            }

            return Util.Dot(Util.Sign(p), p) / 5.0f;
        }

        Vector4 rm(Vector3 ro, Vector3 rd)
        {
            float t = 0;
            Vector3 col = new Vector3();
            float d = 0;

            for (int i = 0; i < 64; i++)
            //for (int i = 0; i < 4; i++)
            {
                Vector3 p = ro + rd * t;

                d = map(p) * .5f;

                if (d < 0.02f)
                {
                    break;
                }
                if (d > 100.0f)
                {
                    break;
                }
                

                col += palette(p.Length() * .1f) / (400.0f* (d));

                t += d;
            }

            //return new Vector4(col.X, col.Y, col.Z, 1.0f/ (d * 100.0f));
            return new Vector4(col.X, col.Y, col.Z, 1.0f / (d * 100.0f));
        }








        public struct Mat2
        {
            public float m00, m01, m10, m11;

            public Mat2(float m00, float m01, float m10, float m11)
            {
                this.m00 = m00;
                this.m01 = m01;
                this.m10 = m10;
                this.m11 = m11;
            }
        }
    }
}
