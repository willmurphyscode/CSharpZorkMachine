using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpZorkMachine.Tests
{
    public class TestAbbreviationTableStructs
    {
        [Fact]
        public void RightHexCodesMakesThe()
        {
            //19 t 0d h 0a e 00 _ 05 ? 05 ?
            //1e y 14 o 1a u 17 r 00 _ 05 ?
            Zchar t = new Zchar(0x19);
            Zchar h = new Zchar(0x0d);
            Zchar e = new Zchar(0x0a);
            List<Zchar> printme = new List<Zchar>
            {
                t, h, e
            };
            //Console.WriteLine($"Should be 'the': {Zchar.PrintFromZchar(printme)}");
            string expected = "the";
            string actual = Zchar.PrintFromZchar(printme);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Abreviation32YieldsExpectedValueFromMiniZork()
        {

        }

    }
}
