include base.fs
include bios.fs
include draw.fs
include numeric.fs

: border ( u -- )
    h# 02 vga!
;

: tile ( x y - )
    over d# 16 + over d# 16 + xyaddr >r
    d# 40 square
    color @ h# f and s>d <# #s #> r> drawstr
;

: main
    d# 10 base !
    320x200x16

    h# 1800 >es

    d# 0 begin
        dup setcolor
        dup d# 7 and d# 40 * over d# 8 and d# 5 *
        tile
        1+ dup d# 16 =
    until drop

    d# 7 setcolor d# 120 d# 120 tile
    d# 8 setcolor d# 160 d# 120 tile

    key drop
    80x25
    quit
;

