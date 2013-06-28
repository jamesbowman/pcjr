include base.fs
include bios.fs
include debug.fs
include zen.fs
include numeric.fs

: vidfill ( v16 -- )
    d# 0 d# 16384 rot fillw
;

: scramble
  >r
  h# 8000 h# 0 begin
    r> xasm lfsr dup >r over es:!
    2+
    2dup =
  until 2drop
  r>
;

: pause
  d# 60000 begin 1- dup 0= until drop
;

: main
    d# 10 base !
    banner

    h# 947

    hex4 cr
 h# 0008 xasm int10
    \ key hex4 cr
    h# 7 h# 02 vga!
    h# 1b h# 00 vga!
    h# 71 h# 00 6845!
    h# 50 h# 01 6845!
    h# 56 h# 02 6845!
    h# 0c h# 03 6845!
    h# 3f h# 04 6845!

    h# 32 h# 06 6845!
    h# 38 h# 07 6845!
    h# 03 h# 09 6845!

    b# 11110110 h# 3df out

    \ ds> h# 100 + >es
    \ h# 0b800 >es
    h# 1800 >es
    h# 1111 vidfill
    key drop
    h# 4444 vidfill
    key drop

\   h# 8000 scramble drop
\   key drop
    l# sunset
    h# 0000
    d# 32768
    move
    key drop

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

  key drop

\ h# 0003 xasm int10
  quit
;

