using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct AbbreviationNumber
    {
        private int value;
        public AbbreviationNumber(int value)
        {
            this.value = value; 
        }
        public int Value { get { return this.value; }  }
    }
}
