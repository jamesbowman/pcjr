        cpu   8086
        org   100h 

; Compressed loader.
; Copy self to upper memory (current + 64K)
; Jump to new copy, with 

start: 
        mov   ax,ds
        add   ax,1000h
        mov   es,ax

        mov   si,0x100
        mov   di,si
        rep   movsb

        push  es
        mov   ax,continue
        push  ax
        retf
continue:
        ; DS is the home segment
        ; ES is the current segment
        push  ds
        push  es
        pop   ds
        pop   es

        ; ES is the home segment
        ; DS is the current segment
        ; push the return address
        ; uncompress from DS:SI to ES:DI

        mov   di,100h
        mov   si,cdata
        push  es  ; for below {
        push  di

        call  lz4_decompress
        int 3
        mov   cx,ax ; length of program in CX
        push  es
        pop   ds

        retf      ; }

%include "LZ4_8088.ASM"

cdata:
        ; %include "hdr.i"
