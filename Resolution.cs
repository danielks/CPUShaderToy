using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPUShaderToy
{
    public class Resolution
    {
        private int _width = 500;
        private int _height = 500;

        public Vector2 xy
        {
            get
            {
                return new Vector2(_width, _height);
            }
        }

        public int x
        {
            get { return _width; }
        }

        public int y
        {
            get { return _height; }
        }

    }
}
