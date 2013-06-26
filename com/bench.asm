%include "common.asm"

simple:
        nop
        nop
        nop
        nop
        nop
        mov ax,3
        jmp next

bytecode:
%include "bench.i"
end_bytecode:

