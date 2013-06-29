include base.fs
include bios.fs
include debug.fs
include zen.fs
include draw.fs
include numeric.fs

: vidfill ( v16 -- )
    d# 0 d# 32768 rot fill
;

: scramble ( r -- r )
  h# 7ffe begin
    swap xasm lfsr swap 2dup es:!
  loop
;

: pause
  d# 3000 begin 1- dup 0= until drop
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

: t0
    get-system-time
;

: took
    get-system-time 2swap d- 
    <# # # [char] . hold #s #> type cr
;

: main
    d# 10 base !
    banner

    h# 947

    320x200x16

    h# 1800 >es

    t0

0 [IF]
    h# 8000 scramble drop
[THEN]

0 [IF]
    d# 4 begin
        d# 100 begin
            dup h# f and setcolor
            dup dup d# 40 square
        loop
    loop
[THEN]

1 [IF]
        l# sunset
        h# 0000
        d# 32768
        move
        \ key drop
[THEN]

    \ d# 24 begin
    \     dup [char] @ +
    \     over d# 8 * dup xyaddr drawchar
    \ loop

    d# 79218. <# #s #> d# 0 drawstr
    pause
    80x25
    took
    snap
    quit
;
