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

bytecode:
%include "main.i"
end_bytecode:

; %include "assets.i"

sample:
%include "../sampling/sample.i"
sample_size equ ($-sample)
