using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    //Shader from the following video: https://www.youtube.com/watch?v=f4s1h2YETNY
    internal class IntroShader : BaseShader
    {
        public IntroShader(Resolution r) : base(r) { }

        public override void mainImage(out Vector4 fragColor, Vector2 fragCoord)
        {
            Vector2 uv = fragCoord / iResolution.xy;
            uv.X = uv.X - 0.5f;
            uv.Y = uv.Y - 0.5f;
            uv.X = uv.X * 2;
            uv.Y = uv.Y * 2;
            uv.X *= (iResolution.x / iResolution.y);

            Vector2 uv0 = uv;

            Vector3 finalColor = new Vector3(0.0f, 0.0f, 0.0f);

            for (int i = 0; i < 4; i++)
            {

                uv *= 1.5f;
                uv.X = Util.Fract(uv.X);
                uv.Y = Util.Fract(uv.Y);
                uv.X = uv.X - 0.5f;
                uv.Y = uv.Y - 0.5f;

                float d = uv.Length() * (float)Math.Exp((double)-uv0.Length());

                Vector3 col = palette(uv0.Length() + i * 0.4f + iTime * 0.4f);

                d = (float)Math.Sin(d * 8.0f + iTime) / 8f;
                d = Math.Abs(d);

                d = (float)Math.Pow(0.01 / (double)d, 1.2);
                //d = 0.02f / d;    

                finalColor += col * d;
            }

            fragColor = new Vector4(finalColor.X, finalColor.Y, finalColor.Z, 1);
        }
        
        private Vector3 palette(float t)
        {
            Vector3 a = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 b = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 c = new Vector3(1f, 1f, 1f);
            Vector3 d = new Vector3(0.263f, 0.416f, 0.557f);

            var res = c * t + d;
            var res2 = 6.28318f * res;

            Vector3 resfinal = new Vector3(
                (float)(a.X + b.X * (float)Math.Cos((float)res2.X)),
                (a.Y + b.Y * (float)Math.Cos((float)res2.Y)),
                (a.Z + b.Z * (float)Math.Cos((float)res2.Z))
                );

            return resfinal;
        }
    }
}
