using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    //from: https://www.shadertoy.com/view/MdX3zr
    internal class FlameShader : BaseShader
    {
        public FlameShader(Resolution r) : base(r)
        {
            
        }
        
        public override void mainImage(out Vector4 fragColor, Vector2 fragCoord)
        {
            Vector2 v = new Vector2();
            v.X = -1.0f + 2.0f * fragCoord.X / iResolution.x;
            v.X = -1.0f + 2.0f * fragCoord.Y / iResolution.y;

            v.X *= iResolution.x / iResolution.y;



            Vector3 org = new Vector3(0.0f, -2.0f, 4.0f);
            Vector3 dir = Util.Normalize(new Vector3(v.X * 1.6f, -v.Y, -1.5f));

            Vector4 p = raymarch(org, dir);
            float glow = p.W;

           //  Vector4 col = Util.Mix(new Vector4(1.0f, .5f, .1f, 1.0f).Length(), new Vector4(0.1f, .5f, 1.0f, 1.0f).Length(), p.Y * .02f + .4f);

            fragColor = new Vector4(
                Util.Mix(0f, 0,
                (float)Math.Pow(glow * 2.0f, 4.0f)));
            

        }


        // Created by anatole duprat - XT95/2013
        // License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

        float noise(Vector3 p) //Thx to Las^Mercury
        {
            Vector3 i = new Vector3(
                (float)Math.Floor(p.X),
                (float)Math.Floor(p.Y),
                (float)Math.Floor(p.Z));



           //Vector4 a = dot(i, Vector3(1., 57., 21.))



           Vector4 a = new Vector4(
               i.X * (float)Math.Cos((p.X - i.X) * Math.Acos(-1.0f)) * (0.5f) + 0.5f + 0.0f,
               i.Y * (float)Math.Cos((p.Y - i.Y) * Math.Acos(-1.0f)) * (0.5f) + 0.5f + 57.0f,
               i.Z * (float)Math.Cos((p.Z - i.Z) * Math.Acos(-1.0f)) * (0.5f) + 0.5f + 21.0f,
               78.0f
               );

            Vector4 f = a;






            //a = new Vector3(0, 0, 0);
                
                //  Util.Mix((float)Math.Sin(Math.Cos((double)a.X) * (double)a.Y),
                //(float)Math.Sin(Math.Cos(1.0f + a.Z) * (1.0f + a.Length())),
                //f.X);

            a.X = Util.Mix(a.X, a.Y, f.Y);
            a.Y = Util.Mix(a.X, a.Y, f.Y);


            return Util.Mix(a.X, a.Y, f.Z);
        }

        float sphere(Vector3 p, Vector4 spr)
        {
            return (new Vector3(spr.X, spr.Y, spr.Z) - p).Length() - spr.W;
        }

        float flame(Vector3 p)
        {
            float d = sphere(p * new Vector3(1.0f, .5f, 1.0f), new Vector4(0.0f, -1.0f, 0.0f, 1.0f));
            return d + (float)(noise(p + new Vector3(0.0f, iTime * 2.0f, 0.0f)) + noise(p * 3.0f) * .5) * 0.25f * (p.Y);
        }

        float scene(Vector3 p)
        {
            return Math.Min(100.0f - p.Length(), Math.Abs(flame(p)));
        }

        Vector4 raymarch(Vector3 org, Vector3 dir)
        {
            float d = 0.0f, glow = 0.0f, eps = 0.02f;
            Vector3 p = new Vector3( org.X, org.Y, org.Z);
            bool glowed = false;

            for (int i = 0; i < 64; i++)
            {
                d = scene(p) + eps;
                p += d * dir;
                if (d > eps)
                {
                    if (flame(p) < .0)
                        glowed = true;
                    if (glowed)
                        glow = (float)i / 64.0f;
                }
            }
            return new Vector4(p.X, p.Y, p.Z, glow);
        }
    }
}
