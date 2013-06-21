include base.fs
include bios.fs
include debug.fs
include zen.fs
include numeric.fs

: vidfill
    >r
    d# 32768
    d# 0
    begin
        r@ over es:!
        2+ 2dup =
    until
    2drop
    r> drop
;

: scramble
  >r
  h# 8000 h# 0 begin
    2dup <>
  while
    r> xasm lfsr dup >r over es:!
    2+
  repeat 2drop
  r>
;

: pause
  d# 60000 begin 1- dup 0= until drop
;

: main
  d# 10 base !
  banner

  h# 947
  h# 0009 xasm int10
  h# 0b800 >es

  h# 1111 vidfill

  l# sunset
  h# 0000
  d# 32768
  xasm movsi

  key drop

  d# 0 d# 0 do
      h# f h# 14 vga!
      h# 0 h# 14 vga!
  loop
    
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

