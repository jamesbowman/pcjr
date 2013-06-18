        cpu 8086
        org 100h 

start: 
; ************************************************************************
; SI: instruction pointer
; CX: top of stack
; DI: RP, stack grows up, DI points to empty slot

        mov   si,end_bytecode-2
        mov   di,rstack
        jmp   next


%macro  NXT   0
        jmp   short next
%endmacro
exit:
        sub   di,2
        mov   si,[di]
        NXT

lit:
        lodsw
        push  cx
        xchg  ax,cx
        NXT

quit:
        mov   ax,4c00h
        int   21h

zbranch:
        lodsw
        cmp   cx,0
        pop   cx
        jnz   next
        xchg  si,ax
        NXT

emit:
        mov   dl,cl
        mov   ah,02
        int   21h

        pop   cx
        NXT

lshift:
        pop   ax
        shl   ax,cl
        xchg  ax,cx
        NXT

rshift:
        pop   ax
        shr   ax,cl
        xchg  ax,cx
        NXT

depth:
        push  cx
        mov   cx,sp
        neg   cx
        shr   cx,1
        sub   cx,2
        NXT

tor:
        mov   [di],cx
        add   di,2
        pop   cx
        NXT

rfrom:
        push  cx
        sub   di,2
        mov   cx,[di]
        NXT

minus:
        xchg  ax,cx
        pop   cx
        sub   cx,ax
        NXT
plus:
        pop   ax
        add   cx,ax
        NXT
_xor:
        pop   ax
        xor   cx,ax
        NXT
_and:
        pop   ax
        and   cx,ax
        NXT
drop:
        pop   cx
        NXT
nip:
        pop   ax
        NXT

swap:
        pop   ax
        xchg  ax,cx
        push  ax
        NXT
over:
        pop   ax
        push  ax
        push  cx
        xchg  ax,cx
        NXT

dup:
        push  cx
        NXT
cat:
        xchg  bx,cx
        mov   cl,[bx]
        mov   ch,0
        NXT

next:
        lodsw
        cmp   ax,bytecode
        jb    codeword
        mov   [di],si
        add   di,2
        xchg  si,ax
        NXT
codeword:
        jmp   ax
wat:
        xchg  bx,cx
        mov   ax,[bx]
        NXT
wstore:
        xchg  bx,cx
        pop   ax
        mov   [bx],ax
        NXT
above:
        pop   ax
        cmp   ax,cx
        ja    mk1
        xor   cx,cx
        NXT

eq:
        pop   ax
        cmp   ax,cx
        jz    mk1
        xor   cx,cx
        NXT

gt:
        pop   ax
        cmp   ax,cx
        jg    mk1
        xor   cx,cx
        NXT
mk1:    mov   cx,1
        NXT

bytecode:
        %include "bytecode.i"
end_bytecode:
        dw    quit

rstack:
        times 64 dw 0
