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

branch:
        lodsw
        xchg  si,ax
        NXT
zbranch:
        lodsw
        cmp   cx,0
        pop   cx
        jnz   next
        xchg  si,ax
        NXT

int21:
        mov   ah,cl
        pop   dx
        int   21h
        xchg  ax,cx
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
        mov   ax,sp
        neg   ax
        shr   ax,1
        dec   ax
        jmp   short pushax

tor:
        mov   [di],cx
        add   di,2
        pop   cx
        NXT

rfrom:
        sub   di,2
        mov   ax,[di]
        jmp   short pushax

rat:
        mov   ax,[di-2]
        jmp   short pushax

minus:
        xchg  ax,cx
        pop   cx
        sub   cx,ax
        NXT
plus:
        pop   ax
        add   cx,ax
        NXT
_or:
        pop   ax
        or    cx,ax
        NXT
_xor:
        pop   ax
        xor   cx,ax
        NXT
_and:
        pop   ax
        and   cx,ax
        NXT
nip:
        pop   ax
        NXT

swap:
        pop   ax
        jmp   short pushax
over:
        pop   ax
        push  ax
        jmp   short pushax

dup:
        push  cx
        NXT
cat:
        xchg  bx,cx
        mov   cl,[bx]
zex:    ; zero-extend cl to cx, then NXT
        mov   ch,0
        NXT

lit:
        lodsw
pushax:
        push  cx
        xchg  ax,cx
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

wstore:
        xchg  bx,cx
        pop   ax
        mov   [bx],ax
drop:
        pop   cx
        NXT
cstore:
        xchg  bx,cx
        pop   ax
        mov   [bx],al
        jmp   short drop
wat:
        xchg  bx,cx
        mov   cx,[bx]
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
mk1:    mov   cx,0ffffh
        NXT

ioat:
        xchg  dx,cx
        in    al,dx
        mov   cl,al
        jmp   short zex
iostore:
        xchg  dx,cx
        pop   ax
        out   dx,al
        jmp   short drop

slashmod:
        pop   ax      ; ax cx
        cwd
        idiv  cx
divresult:
        push  dx
        xchg  cx,ax
        NXT

umslashmod:
        pop   dx      ; dx:ax cx
        pop   ax
        div   cx
        jmp   short divresult

eol:
        inc   cx
        mov   ax,cx
        mov   bp,sp
        xor   ax,[bp]
        not   ax
        jmp   short pushax

toes:   mov   es,cx
        jmp   short drop
esstore:
        xchg  bx,cx
        pop   ax
        mov   [es:bx],ax
        jmp   short drop

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; special:
;         %rep    0
;           mov ax,0
;         %endrep
;         jmp   next

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

int10:  ; ( ax -- )
        xchg  ax,cx
        int   10h
        jmp   drop

movsi:  ; ( src dst cnt -- )
        xchg  bp,di
        xchg  bx,si
        pop   di
        pop   si
        rep movsb
        xchg  bp,di
        xchg  bx,si
        jmp   drop

bytecode:
        %include "bytecode.i"
end_bytecode:

        %include "assets.i"
rstack:
        times 64 dw 0
