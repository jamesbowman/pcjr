include base.fs
include bios.fs

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

: setcolor
    dup d# 4 lshift or color !
;

: main
    300x200x16

    h# 1800 >es

    d# 0 begin
        dup setcolor
        dup d# 7 and d# 40 * over d# 8 and d# 5 *
        rect40
        1+ dup d# 16 =
    until drop

    d# 7 setcolor d# 120 d# 120 rect40
    d# 8 setcolor d# 160 d# 120 rect40

    key drop

    80x25
    quit
;

