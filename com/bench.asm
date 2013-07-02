%include "common.asm"

simple:
        xchg    cx,bp
        mov     cx,111
dwell:  
        loop    dwell
        xchg    cx,bp
        jmp next

bytecode:
%include "bench.i"
end_bytecode:

