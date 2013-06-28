        cpu 8086
        org 100h 

start: 
        mov   ax,0009h
        int     10h
        mov   ax,0003h
        int     10h

        mov   ah,09
        mov   dx,message
        int   21h
        mov   ax,4c00h
        int   21h
message   db "Hello", 13,10,"$"
