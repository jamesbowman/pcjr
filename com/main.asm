%include "common.asm"

pcm:
        mov     al,10010000b | (0 << 5)
        or      al,cl
        out     0c0h,al
        mov     al,10010000b | (0 << 5)
        or      al,cl
        out     0c0h,al
        mov     al,10010000b | (0 << 5)
        or      al,cl
        out     0c0h,al
        jmp     drop

scratch:
        xchg    bp,si
        pop     si
        mov     dx,0c0h
playback:
        lodsb
        ; mov     ah,al
        ; shr     al,1
        ; shr     al,1
        ; shr     al,1
        ; shr     al,1
        ; or      al,90h
        ; out     dx,al

        ; mov     al,ah
        ; and     al,0fh
        ; or      al,90h
        out     dx,al

        loop    playback

        xchg    bp,si
        jmp     drop

spin:
        push    si
        xchg    bp,cx
        xor     cx,cx
dwell:  
        %rep 63
                lodsb
                xlat
        %endrep
        ; mov     ax,0xffff
        ; mov     bx,0
        ; mul     bx
        loop    dwell
        xchg    bp,cx
        pop     si
        jmp     next

; ------------------------------------------------------------------------

create_file:
        xchg    dx,cx
        mov     cx,0
        mov     ax,3c00h
        int     21h
        xchg    cx,ax
        sbb     ax,ax
        jmp     pushax

open_file:
        xchg    dx,cx
        mov     cx,0
        mov     ax,3d00h
        int     21h
        xchg    cx,ax
        sbb     ax,ax
        jmp     pushax

write_file:     ; ( c-addr u fileid -- ior )
        xchg    bx,cx
        pop     cx
        pop     dx

        mov     ax,4000h
        int     21h
        sbb     cx,cx
        and     cx,ax
        jmp     next

read_file:      ; ( c-addr u fileid -- ior )
        xchg    bx,cx   ; bx = file handle
        pop     cx      ; cx = number of bytes to read
        pop     dx      ; ds:dx = buffer for data
        mov     ax,3f00h
        int     21h
        sbb     cx,cx
        and     cx,ax
        jmp     next

close_file:     ; ( handle -- ior )
        xchg    bx,cx
        mov     ax,3e00h
        int     21h
        sbb     cx,cx
        and     cx,ax
        jmp     next

; ------------------------------------------------------------------------
oldtimer:
        dw      0,0


; Exchange (DX,AX) with timer interrupt
xchgtimer:
        mov     bx,020h

; Exchange (DX,AX) with interrupt at BX
xchgint:
        push    es
        xor     bp,bp
        mov     es,bp

        xchg    ax,[es:bx]
        xchg    dx,[es:bx+2]
        pop     es
        ret
        
timer_replace:
        cli
        mov     dx,ds
        mov     ax,timer
        call    xchgtimer
        mov     [oldtimer],dx
        mov     [oldtimer+2],ax

        mov     al,36h
        out     43h,al

        ; Channel 0 generates fast interrupt
FREQ    equ     19912/4
        mov     al,FREQ & 0xff
        out     40h,al
        mov     al,FREQ >> 8
        out     40h,al

        mov     al,0feh
        out     21h,al
        sti
        jmp     next

timer_restore:
        cli
        mov     dx,[oldtimer]
        mov     ax,[oldtimer+2]
        call    xchgtimer
        sti
        jmp     next

timer:
        push    ax
        push    bx
        push    cx
        push    dx
        push    ds

        mov     ax,cs
        mov     ds,ax
        inc     word [tick240]
        mov     dx,3dah
        in      al,dx
        mov     al,02h
        out     dx,al
        mov     al,[tick240]
        and     al,3
        or      al,4
        out     dx,al

        mov     al,20h  ; EOI to 8259
        out     20h,al

        pop     ds
        pop     dx
        pop     cx
        pop     bx
        pop     ax
        iret

tick240:
        dw      7

wait1:
        mov     al,[tick240]
.wait1: cmp     al,[tick240]
        jz      .wait1
        jmp     next

toau:   ; ( v -- )
        xchg    ax,cx
        out     0c0h,al
        jmp     drop

freq: ; ( u voice -- )
        pop     bx
        mov     al,bl
        and     al,0fh
        or      al,cl
        shr     bx,1
        shr     bx,1
        shr     bx,1
        shr     bx,1

        out     0c0h,al
        xchg    ax,bx
        out     0c0h,al

        jmp     drop

lerp: ; ( b a t -- x )
        pop     bx
        pop     ax
        sub     ax,bx
        mul     cx
        add     dx,bx
        mov     cx,dx
        jmp     next

; ------------------------------------------------------------------------
%if 0

%include "dsprite.i"

dsprite_cold:
        push    di
        push    si
        push    cx

        mov     di,0
        xor     ax,ax
        mov     cx,8
.eight:
        push    cx
        mov     cx,8
.one:   call    dsprite_0
        loop    .one
        call    nextline

        pop     cx
        loop    .eight

        pop     cx
        pop     si
        pop     di
        jmp     next

dsprite:
        push    di
        push    si

        call    computeall

        pop     si
        pop     di
        jmp     drop

        %macro  compute1 0
        ; Given a 2-bit code in CX, compute a new state
        ; and push the draw routine
%%ld:   mov     bx,0

        shr     cx,1
        rcr     bl,1
        shr     cx,1
        rcr     bl,1
        ; now bx is the index into dsprite_*
        mov     al,[si+bx]      ; SI=dsprite_delta
        mov     [%%ld+1],al     ; yee haw
        
        shl     bx,1
        push    word [di+bx]      ; DI=dsprite_table
        %endm

schedule:
        incbin "dsanim.bi" 

computeall:
        xor     dx,dx
        mov     si,schedule
        shl     cx,1
        shl     cx,1
        shl     cx,1
        shl     cx,1
        add     si,cx
        mov     bp,dsprite_delta
        mov     di,dsprite_table
        %rep 8
                mov     ax,nextline
                push    ax
                lodsw
                xchg    cx,ax
                xchg    bp,si
                %rep 8
                        compute1
                %endrep
                xchg    bp,si
        %endrep

        mov     di,0
        xor     ax,ax
        ret
%endif
; ------------------------------------------------------------------------

bytecode:
%include "main.i"
end_bytecode:

; %include "assets.i"

sample:
; %include "../sampling/sample.i"
sample_size equ ($-sample)
        ENDIT
