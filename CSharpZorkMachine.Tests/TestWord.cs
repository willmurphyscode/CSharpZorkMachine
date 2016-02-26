using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpZorkMachine;
using Xunit;

namespace CSharpZorkMachine.Tests
{
    public class TestWord
    {
        [Fact]
        public void WordDotIsTerminalReturnsCorrectValue()
        {
            Word noTerminal = new Word(0);
            Word terminal = new Word(~0);
            Word onlyTerminalBitSet = new Word(65535 / 2 + 1);

            Assert.True(terminal.IsTerminal());
            Assert.True(onlyTerminalBitSet.IsTerminal());
            Assert.True(!noTerminal.IsTerminal());
        }
    }
}
