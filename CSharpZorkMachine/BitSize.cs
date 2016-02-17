using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct BitSize
    {
        private int value;
        public BitSize(int size)
        {
            this.value = size;
        }
        public int Size { get { return this.value; } }


        public static readonly BitSize Size1 = new BitSize(1);
        public static readonly BitSize Size2 = new BitSize(2);
        public static readonly BitSize Size3 = new BitSize(3);
        public static readonly BitSize Size4 = new BitSize(4);
        public static readonly BitSize Size5 = new BitSize(5);
        public static readonly BitSize Size6 = new BitSize(6);
        public static readonly BitSize Size7 = new BitSize(7);
        public static readonly BitSize Size8 = new BitSize(8);
        public static readonly BitSize Size9 = new BitSize(9);
        public static readonly BitSize Size10 = new BitSize(10);
        public static readonly BitSize Size11 = new BitSize(11);
        public static readonly BitSize Size12 = new BitSize(12);
        public static readonly BitSize Size13 = new BitSize(13);
        public static readonly BitSize Size14 = new BitSize(14);
        public static readonly BitSize Size15 = new BitSize(15);
        public static readonly BitSize Size16 = new BitSize(16);

    }
}
