include base.fs
include bios.fs
include debug.fs
include zen.fs
include numeric.fs

: vidfill ( v16 -- )
    d# 0 d# 16384 rot fillw
;

: scramble
  h# 7ffe begin
    swap xasm lfsr swap 2dup es:!
  loop drop
;

: pause
  d# 60000 begin 1- dup 0= until drop
;

: 6845!
    h# 3d4 out
    h# 3d5 out
;

: vga!
     h# 3da in drop
     h# 3da tuck out
     out
;

: 6845start ( u -- ) \ set 14-bit 6845 start address
    dup d# 8 rshift d# 12 6845!
    d# 13 6845!
;

: wobble
    h# 4f h# 01 6845!
    h# 32 h# 06 6845!
    d# 4080 d# 0 begin
        dup 6845start
        key drop
        d# 80 +
        2dup =
    until 2drop
    d# 0 6845start
;

: flicker ( u -- u )
    >r
    h# 20 h# 10 begin
        r@ 0< over vga!
        r> xasm lfsr >r
        1+ 2dup =
    until 2drop
    r>
;

: main
    d# 10 base !
    banner

    h# 947
    h# 0009 xasm int10
    h# 0b800 >es

    \ h# 1111 vidfill
    \ key drop
    \ h# 2222 vidfill
    \ key drop
    \ h# 4444 vidfill
    \ key drop

    h# 8000 scramble
    d# 1000 begin
        swap flicker swap
    loop 2drop

    \ l# sunset
    \ h# 0000
    \ d# 32768
    \ move

    \ wobble

\ key drop

\  d# 0 d# 0 do
\      h# f h# 14 vga!
\      h# 0 h# 14 vga!
\  loop
    
  \ h# 8000
  \ d# 2 d# 0 do
  \     scramble
  \     h# 1111 vidfill
  \ loop
  \ drop

  key drop

  h# 0003 xasm int10
  snap
;

