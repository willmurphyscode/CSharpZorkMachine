using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct Word
    {
        private int value; 
        public Word(int value)
        {
            this.value = value; 
        }

        public short Value { get { return (short)this.value; } }


    }
}
