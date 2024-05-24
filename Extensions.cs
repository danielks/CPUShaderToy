using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{

    public static class VectorExtensions
    {
        public static Vector3 XY(this Vector3 v)
        {
            return new Vector3(v.X, v.Y, 0);
        }

        public static Vector3 XZ(this Vector3 v)
        {
            return new Vector3(v.X, 0, v.Z);
        }

    }
    
}
