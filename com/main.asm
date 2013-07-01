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
        out     dx,al
        loop    playback

        xchg    bp,si
        jmp     drop
bytecode:
%include "main.i"
end_bytecode:

; %include "assets.i"

sample:
%include "../sampling/sample.i"
sample_size equ ($-sample)
