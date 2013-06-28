        cpu 8086
        org 100h 

start: 
; ************************************************************************
; SI: instruction pointer
; CX: top of stack
; DI: RP, stack grows up, DI points to empty slot

%macro  NXT     0
        jmp     short next
%endmacro

%macro  NXTAX   0
        jmp     short loadax
%endmacro

        mov     si,end_bytecode-2
        mov     di,rstack
        NXT

exit:
        dec     di
        dec     di
        mov     si,[di]
        NXT

zbranch:
        lodsw
        or      cx,cx
        pop     cx
        jnz     next
        jmp     short jmpax
branch:
        lodsw
        jmp     jmpax

_loop:
        lodsw   ;
        loop    jmpax
        jmp     short drop

int21:
        xchg    ax,cx
        pop     dx
        int     21h
        NXTAX

lshift:
        pop     ax
        shl     ax,cl
        NXTAX

rshift:
        pop     ax
        shr     ax,cl
        NXTAX

_2div:
        sar     cx,1
        NXT

depth:
        mov     ax,sp
        neg     ax
        shr     ax,1
        dec     ax
        jmp     short pushax

;_do:    ; ( b a ) means that b is [di-2], a at [di-4]
;        mov     [di],cx
;        inc     di
;        inc     di
;        pop     cx
tor:
        mov     [di],cx
        inc     di
        inc     di
        jmp     short drop

rfrom:
        dec     di
        dec     di
        mov     ax,[di]
        jmp     short pushax

rat:
        mov     ax,[di-2]
        jmp     short pushax

minus:
        xchg    ax,cx
        pop     cx
        sub     cx,ax
        NXT
plus:
        pop     ax
        add     cx,ax
        NXT
_or:
        pop     ax
        or      cx,ax
        NXT
_xor:
        pop     ax
        xor     cx,ax
        NXT
_and:
        pop     ax
        and     cx,ax
        NXT
nip:
        pop     ax
        NXT

swap:
        pop     ax
        jmp     short pushax
over:
        pop     ax
        push    ax
        jmp     short pushax

dup:
        push    cx
        NXT
cat:
        xchg    bx,cx
        mov     cl,[bx]
zex:    ; zero-extend cl to cx, then NXT
        mov     ch,0
        NXT

lit:
        lodsw
pushax:
        push    cx
loadax:
        xchg    ax,cx
next:
        lodsw
        cmp     ax,bytecode
        jb      codeword
        mov     [di],si
        inc     di
        inc     di
jmpax:
        xchg    si,ax
        NXT
codeword:
        jmp     ax

exec:
        jmp     cx

wstore:
        xchg    bx,cx
        pop     ax
        mov     [bx],ax
drop:
        pop     cx
        NXT
cstore:
        xchg    bx,cx
        pop     ax
        mov     [bx],al
        jmp     short drop
wat:
        xchg    bx,cx
        mov     cx,[bx]
        NXT
above:
        pop     ax
        cmp     cx,ax
        sbb     cx,cx
        NXT

eq:
        pop     ax
        cmp     ax,cx
        mov     cx,0
        jne     short next
oneminus:
        dec     cx
        NXT
oneplus:
        inc     cx
        NXT

gt:
        pop     ax
        cmp     ax,cx
        mov     cx,0
        jle     short next
        jmp     short oneminus

ioat:
        xchg    dx,cx
        in      al,dx
        mov     cl,al
        jmp     short zex
iostore:
        xchg    dx,cx
        pop     ax
        out     dx,al
        jmp     short drop

slashmod:
        pop     ax      ; ax cx
        cwd
        idiv    cx
divresult:
        push    dx
        NXTAX

umslashmod:
        pop     dx      ; dx:ax cx
        pop     ax
        div     cx
        jmp     short divresult

umstar:
        pop     ax
        mul     cx
        xchg    ax,dx
        jmp     short divresult

mstar:
        pop     ax
        imul    cx
        xchg    ax,dx
        jmp     short divresult

toes:   mov     es,cx
        jmp     short drop
esstore:
        xchg    bx,cx
        pop     ax
        mov     [es:bx],ax
        jmp     short drop

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; special:
;         %rep    0
;           mov ax,0
;         %endrep
;         jmp   next

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

int10:  ; ( ax -- )
        push    si
        push    di
        xchg    ax,cx
        int     10h
        pop     di
        pop     si
        jmp     drop

move:   ; ( src dst cnt -- )
        xchg    bp,di
        xchg    bx,si
        pop     di
        pop     si
        shr     cx,1
        rep movsw
        adc     cx,cx
        rep movsb
        xchg    bp,di
        xchg    bx,si
        jmp     drop

fillw:  ; ( dst cnt val16 -- )
        xchg    bp,di
        pop     ax      ; cnt
        pop     di      ; dst
        xchg    cx,ax
        rep stosw
        xchg    bp,di
        jmp     drop

fill:  ; ( dst cnt val16 -- )
        xchg    bp,di
        pop     ax      ; cnt
        pop     di      ; dst
        xchg    cx,ax
        rep stosb
        xchg    bp,di
        jmp     drop

lfsr:
        shr     cx,1
        sbb     al,al
        and     al,0xb4
        xor     ch,al
        jmp     next

_in:
        xchg    dx,cx
        in      al,dx
        xor     ah,ah
        xchg    cx,ax
        jmp     next

_out:
        xchg    dx,cx
        pop     ax
        out     dx,al
        jmp     drop

%if 0
vgastore:
        mov     dx,3dah
        in      al,dx
        xchg    ax,cx
        out     dx,al

        pop     ax
        out     dx,al
        jmp     drop

_6845store:
        mov     dx,3d4h
        xchg    ax,cx
        out     dx,al
        inc     dx

        pop     ax
        out     dx,al
        jmp     drop
%endif

rpit:
        cli
        mov     al, 00000000b    ; al = channel in bits 6 and 7, remaining bits clear
        out     0x43, al         ; Send the latch command
 
        in      al, 0x40          ; al = low byte of count
        mov     ah, al           ; ah = low byte of count
        in      al, 0x40          ; al = high byte of count
        xchg    al,ah        ; al = low byte, ah = high byte
        jmp     pushax

rstack:
        times 64 dw 0
