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

    public class Vector2
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Cell
    {
        public BlockTypes blockType { get; set; }
        public Cell()
        {
            blockType = BlockTypes.air;
        }

        public Cell(BlockTypes blockType)
        {
            this.blockType = blockType;
        }
    }

    public enum BlockTypes { 
        air,
        grass,
        logs,
        leaves,
        stone,
        dirt,
        cursor,
        planks,
        bricks,
        sand,
        cobble,
        mossyCobble,
    }
}
