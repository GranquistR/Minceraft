using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameTest
{
    public class Vector3
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
