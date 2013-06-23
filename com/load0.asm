        cpu 8086
        org 100h 

LPT_DATA        equ     378h
LPT_STATUS      equ     379h ;  busy(11) ack(10) paperout12) selectin(13) error(15)
LPT_CONTROL     equ     37ah ;                                            selectp(17) initialize(16) autolf(14) strobe(14)

start: 

L0:
        mov     dx,LPT_CONTROL
        mov     al,00000100b
        out     dx,al
%if 0
        in      al,dx
        or      al,30h
        mov     dl,al
        mov     ah,2
        int     21h
        jmp     short L0
%endif

        mov     di,0200h
        mov     cx,256
readloop:
; Wait for valid high
        dec     dx      ;LPT_STATUS
waitvalid1:
        in      al,dx
        test    al,8
        jz      short waitvalid1
        and     al,0f0h ; preserve in bl
        xchg    ax,bx

; Signal READY by raising DATA.0
        dec     dx      ; LPT_DATA
        mov     al,1
        out     dx,al

; Capture data in bl
        inc     dx
        inc     dx      ; LPT_CONTROL
        in      al,dx
        and     al,00fh
        or      bl,al

; Wait for valid low
        dec     dx      ; LPT_STATUS
waitvalid0:
        in      al,dx
        test    al,8
        jnz     short waitvalid0

; Drop ready
        dec     dx      ; LPT_DATA
        mov     al,0
        out     dx,al

; Store bl
        mov     al,bl
        stosb
        loop    readloop
        int     3

%if 0
        mov     ah,09
        mov     dx,message
        int     21h
        mov     ah,4ch
        int     21h
message   db "Hello", 13,10,"$"
%endif
