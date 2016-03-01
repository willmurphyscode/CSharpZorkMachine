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
        public void WordAllOnesIsTerminal()
        {
            Word terminal = new Word(~0);
            Assert.True(terminal.IsTerminal());

        }
        [Fact]
        public void SettingOnlyTerminalBitMakesTerminalWord()
        {
            Word onlyTerminalBitSet = new Word(1);
            Assert.True(onlyTerminalBitSet.IsTerminal());
        }

        [Fact]
        public void ZeroIsNotATerminalWord()
        {
            Word nonTerminal = new Word(0);
            Assert.False(nonTerminal.IsTerminal());
        }
    }
}
