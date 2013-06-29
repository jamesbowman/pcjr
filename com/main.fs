include base.fs
include bios.fs
include debug.fs
include zen.fs
include numeric.fs

: yaddr
    dup d# 3 and d# 13 lshift
    swap d# 2 rshift d# 160 * +
;

: xyaddr
    yaddr swap 2/ +
;

variable color
: rect40 ( x y -- )
    dup d# 40 + >r
    begin
        2dup xyaddr d# 20 color @ fill
        1+ dup r@ =
    until 2drop r> drop
;

: vidfill ( v16 -- )
    d# 0 d# 32768 rot fill
;

: setcolor
    dup d# 4 lshift or color !
;

: scramble
  h# 7ffe begin
    swap xasm lfsr swap 2dup es:!
  loop drop
;

: pause
  d# 3000 begin 1- dup 0= until drop
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
\   \ key hex4 cr
\   h# f h# 01 vga! \ palette mask
\   h# 0 h# 02 vga!

\   h# 00 h# 10 vga!
\   h# 01 h# 11 vga!
\   h# 02 h# 12 vga!
\   h# 03 h# 13 vga!
\   h# 04 h# 14 vga!
\   h# 05 h# 15 vga!
\   h# 06 h# 16 vga!
\   h# 07 h# 17 vga!
\   h# 08 h# 18 vga!
\   h# 09 h# 19 vga!
\   h# 0a h# 1a vga!
\   h# 0b h# 1b vga!
\   h# 0c h# 1c vga!
\   h# 0d h# 1d vga!
\   h# 0e h# 1e vga!
\   h# 0f h# 1f vga!

\   h# 1b h# 00 vga!
\   h# 71 h# 00 6845!
\   h# 50 h# 01 6845!
\   h# 56 h# 02 6845!
\   h# 0c h# 03 6845!
\   h# 3f h# 04 6845!

\   h# 32 h# 06 6845!
\   h# 38 h# 07 6845!
\   h# 03 h# 09 6845!

\   b# 11110110 h# 3df out

    \ ds> h# 100 + >es
    \ h# 0b800 >es
    h# 1800 >es

    h# 8000 scramble drop
\   key drop

0 [IF]
        l# sunset
        h# 0000
        d# 32768
        move
        key drop
[THEN]

 h# 0003 xasm int10
    ds> hex4 cr
    quit
\   h# 0009 xasm int10

\   key drop
\   h# 2222 vidfill
\   key drop
\   h# 4444 vidfill
\   key drop

\   h# 8000 scramble drop
\   key drop
\     

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

\ h# 0003 xasm int10
  quit
;

