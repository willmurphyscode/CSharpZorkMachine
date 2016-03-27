
##Zork Machine in C# #

Eric Lippert is writing a wonderful blog series,
in which he implements a Z-Machine (the virtual machine
layer for playing Zork and similar games) in OCaml. 

I don't know any OCaml at all, and I wanted to follow along,
so I am writing the same code in C#. 

Please feel free to make comments or suggestions. 

I think, so far, that there is a lot more ceremony 
in the C# code then in the OCaml code.

For example, my BitSize is a struct in its own 
source file, and Eric's whole implementation is just
`type bit_size = Bit_size of int`

At first, I was making a million little .cs files that look like this:

    public struct SpecialCaseOfInt 
    {
        private int value;
        public SpecialCaseOfInt(int value) 
        {
            this.value = value; 
        }
        public int Value { get {return this.value; } }
    }

That gets pretty annoying, and is not nearly as succinct as the OCaml type system. 

I am on the fence about whether to keep these all in their own files. The downside is that 
I have a whole *file* where OCaml just has a one liner, but the plus side is that I have a
convenient place to keep helper functions. 

All these types are as immutable as possible. They are structs, just because they are 
wrappers around a single value, and all operations that are defined for them return a 
mutated copy, but don't mutate the original instance. 

