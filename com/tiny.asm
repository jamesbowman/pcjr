        cpu 8086
        org 100h 

start: 
        mov   ax,cx
        call  hex4
        call  cr

        ; mov   si,book
        ; mov   di,8000h
        ; call  lz4_decompress
        ; call  hex4
        ; call  cr

        mov   ah,09
        mov   dx,message
        int   21h
        
        call  here
here:   pop   ax
        call  hex4
        call  cr

        mov   ax,4c00h
        int   21h
cr:
        mov   al,13
        call  emit
        mov   al,10
        jmp   emit
hex4:
        push  ax
        mov   al,ah
        call  hex2
        pop   ax
hex2:
        push  ax
        mov   cl,4
        shr   al,cl
        call  hex1
        pop   ax
hex1:
        and   al,15
        add   al,0x30
        cmp   al,0x3a
        jl    emit
        add   al,7
emit:
        mov   dl,al
        mov   ah,02
        int   21h
        ret

message   db "**************************************************", 13,10,"$"
; %include "LZ4_8088.ASM"
; book:
;         %include "hdr.i"
