        cpu 8086
        org 100h 

start: 
; ************************************************************************
; SI: instruction pointer
; CX: top of stack
; DI: RP, stack grows up, DI points to empty slot

        mov   si,end_bytecode-2
        mov   di,rstack

next:
        lodsw
        cmp   ax,bytecode
        jb    codeword
        mov   [di],si
        add   di,2
        xchg  si,ax
        jmp   next
codeword:
        jmp   ax

exit:
        sub   di,2
        mov   si,[di]
        jmp   next

lit:
        lodsw
        push  cx
        xchg  ax,cx
        jmp   next

quit:
        mov   ax,4c00h
        int   21h

zbranch:
        lodsw
        cmp   cx,0
        pop   cx
        jnz   next
        xchg  si,ax
        jmp   next

emit:
        mov   dl,cl
        mov   ah,02
        int   21h

        pop   cx
        jmp   next

lshift:
        pop   ax
        shl   ax,cl
        xchg  ax,cx
        jmp   next

rshift:
        pop   ax
        shr   ax,cl
        xchg  ax,cx
        jmp   next

depth:
        push  cx
        mov   cx,sp
        neg   cx
        shr   cx,1
        sub   cx,2
        jmp   next

tor:
        mov   [di],cx
        add   di,2
        pop   cx
        jmp   next

rfrom:
        push  cx
        sub   di,2
        mov   cx,[di]
        jmp   next

minus:
        xchg  ax,cx
        pop   cx
        sub   cx,ax
        jmp   next
plus:
        pop   ax
        add   cx,ax
        jmp   next
_xor:
        pop   ax
        xor   cx,ax
        jmp   next
_and:
        pop   ax
        and   cx,ax
        jmp   next
drop:
        pop   cx
        jmp   next
nip:
        pop   ax
        jmp   next
swap:
        pop   ax
        xchg  ax,cx
        push  ax
        jmp   next
over:
        pop   ax
        push  ax
        push  cx
        xchg  ax,cx
        jmp   next
dup:
        push  cx
        jmp   next
cat:
        xchg  bx,cx
        mov   cl,[bx]
        mov   ch,0
        jmp   next
wat:
        xchg  bx,cx
        mov   ax,[bx]
        jmp   next
wstore:
        xchg  bx,cx
        pop   ax
        mov   [bx],ax
        jmp   next
above:
        pop   ax
        cmp   ax,cx
        ja    mk1
        xor   cx,cx
        jmp   next

eq:
        pop   ax
        cmp   ax,cx
        jz    mk1
        xor   cx,cx
        jmp   next

gt:
        pop   ax
        cmp   ax,cx
        jg    mk1
        xor   cx,cx
        jmp   next
mk1:    mov   cx,1
        jmp   next

bytecode:
        %include "bytecode.i"
end_bytecode:
        dw    quit

rstack:
        times 64 dw 0
