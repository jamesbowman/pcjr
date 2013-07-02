%include "common.asm"

LPT_DATA        equ     378h ;                                                                                  data0(2)
LPT_STATUS      equ     379h ;  busy(11) ack(10) paperout12) selectin(13) error(15)
LPT_CONTROL     equ     37ah ;                                            selectp(17) initialize(16) autolf(14) strobe(1)

read8:  ; ordered hi4, lo4
        call    read4
        xchg    bp,ax
        call    read4
        shr     al,1
        shr     al,1
        shr     al,1
        shr     al,1
        or      ax,bp
        xor     ah,ah
        jmp     pushax

read4:
        mov     dx,LPT_DATA
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
        ; Drop ready
        mov     al,0
        out     dx,al

        xchg    ax,bx
        ret

sync:
        ; Expecting 50,a0,90,80
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
        cmp     al,80h
        jnz     is5

        jmp     next

create_file:
        xchg    dx,cx
        mov     cx,0
        mov     ax,3c00h
        int     21h
        xchg    cx,ax
        sbb     ax,ax
        jmp     pushax

close_file:     ; ( handle -- ior )
        xchg    bx,cx
        mov     ax,3e00h
        int     21h
        sbb     cx,cx
        and     cx,ax
        jmp     next

write_file:     ; ( c-addr u fileid -- ior )
        xchg    bx,cx
        pop     cx
        pop     dx

        mov     ax,4000h
        int     21h
        sbb     cx,cx
        and     cx,ax
        jmp     next

fname:
        db      "FOOBAR02.ABC", 0

writesector:    ; ( track head sector data -- ax )
        xchg    bx,cx   ; buffer address pointer
        mov     dl,0
        pop     ax
        mov     cl,al   ; sector
        pop     ax
        mov     dh,al   ; head
        pop     ax
        mov     ch,al   ; track

        mov     ax,0301h
        int     13h

        jmp     loadax

dta:    times 128 db 0x94

bytecode:
%include "fwrite.i"
end_bytecode:

