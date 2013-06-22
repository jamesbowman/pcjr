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

        in      al,dx
        or      al,30h
        mov     dl,al
        mov     ah,2
        int     21h
        jmp     short L0

        mov     ah,09
        mov     dx,message
        int     21h
        mov     ah,4ch
        int     21h
message   db "Hello", 13,10,"$"
