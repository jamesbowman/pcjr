        cpu 8086
        org 100h 

LPT_DATA        equ     378h ;                                                                                  data0(2)
LPT_STATUS      equ     379h ;  busy(11) ack(10) paperout12) selectin(13) error(15)
LPT_CONTROL     equ     37ah ;                                            selectp(17) initialize(16) autolf(14) strobe(1)

start: 

L0:
        mov     dx,LPT_CONTROL
        mov     al,00000100b
        out     dx,al
        dec     dx      ; LPT_STATUS

montor:
%if 0                   ; monitor STATUS
        in      al,dx
        int     3
        jmp     short montor
%endif

%if 1
        dec     dx      ; LPT_DATA

        mov     di,0200h
        mov     cx,256
readloop:
; Drop ready
        mov     al,0
        out     dx,al
; Wait for valid high
        inc     dx      ;LPT_STATUS
waitvalid1:
        in      al,dx
        test    al,8
        jz      short waitvalid1
        and     al,0f0h ; preserve in bl
        xchg    bx,ax

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
        ; jnz     short waitvalid0

        dec     dx      ; LPT_DATA

; Store bl
        xchg    ax,bx
        stosb
        loop    readloop
        int     3
%endif

%if 0
        mov     ah,09
        mov     dx,message
        int     21h
        mov     ah,4ch
        int     21h
message   db "Hello", 13,10,"$"
%endif
