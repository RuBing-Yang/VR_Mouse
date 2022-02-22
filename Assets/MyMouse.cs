using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    class MyMouse
    {
        public int id;
        public int type = 0; //0平移, 1方向
        public int x = 0;
        public int y = 0;

        public override string ToString() =>
            $"{id:X8}, ({x}, {y})";
    }
}
