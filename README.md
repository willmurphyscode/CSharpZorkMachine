
##Zork Machine in C#

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
`type bit\_size = Bit\_size of int`

