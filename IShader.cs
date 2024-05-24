using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    internal interface IShader
    {   
        public void mainImage(out Vector4 fragColor, Vector2 fragCoord);
        
    }
}
