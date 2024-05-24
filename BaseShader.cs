using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    internal abstract class BaseShader
    {
        protected Resolution iResolution;
        public float iTime;
        public Vector4 iMouse;


        public BaseShader(Resolution r)
        {
            iResolution = r;            
        }

        public abstract void mainImage(out Vector4 fragColor, Vector2 fragCoord);
    }
}
