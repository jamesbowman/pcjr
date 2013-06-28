        cpu 8086

LPT_DATA        equ     378h ;                                                                                  data0(2)
LPT_STATUS      equ     379h ;  busy(11) ack(10) paperout12) selectin(13) error(15)
LPT_CONTROL     equ     37ah ;                                            selectp(17) initialize(16) autolf(14) strobe(1)

        section .loader

start: 

        mov     dx,LPT_CONTROL
        mov     al,00000100b
        out     dx,al
        dec     dx      ; LPT_STATUS
        dec     dx      ; LPT_DATA
        
        mov     cx,RELOC1-RELOC0
        mov     si,RELOC0
        mov     di,100h-(RELOC1-RELOC0)
        movsb
        jmp     100h-(RELOC1-RELOC0)

        ; This section gets relocated to lower memory
RELOC0:
read16: ; ordered hi8, lo8
        call    read8
        push    ax
        call    read8
        pop     bx
        mov     ah,bl
        ret

read8:  ; ordered hi4, lo4
        call    read4
        xchg    bp,ax
        call    read4
        shr     al,1
        shr     al,1
        shr     al,1
        shr     al,1
        or      ax,bp
        ret

read4:
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

; Wait for valid low
        inc     dx      ; LPT_STATUS
waitvalid0:
        in      al,dx
        test    al,8
        jnz     short waitvalid0

        dec     dx      ; LPT_DATA

        xchg    ax,bx
        ret

lptloader:
        ; Expecting 50,a0,90,70
get5:
        call    read4
is5:
        cmp     al,50h
        jnz     get5

        call    read4
        cmp     al,0a0h
        jnz     is5

        call    read4
        cmp     al,90h
        jnz     is5

        call    read4
        cmp     al,70h
        jnz     is5

        call    read16
        xchg    cx,ax
        mov     di,200h
rdloop:
        call    read8
        stosb
        loop    rdloop

        int3
        mov     ax,ds
        add     ax,256/16       ; So that 200 becomes the new start
        mov     ds,ax
        mov     es,ax
        mov     ss,ax

        push    ax
        mov     ax,100h
        push    ax
        retf
RELOC1:
