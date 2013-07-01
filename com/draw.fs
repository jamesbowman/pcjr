: yaddr ( y -- a )
    dup d# 3 and d# 13 lshift
    swap d# 2 rshift d# 160 * +
;

: xyaddr ( x y -- a )
    yaddr swap 2/ +
;

variable color

: square ( x y w -- )
    >r
    xyaddr
    r@
    begin
        swap
        dup r@ 2/ color @ fill
        down1
        swap
    loop drop
    r> drop
;

: setcolor
    dup d# 4 lshift or color !
;

: drawstr ( a u dst -- )
    >r
    bounds
    begin
        2dup xor
    while
        dup c@ r@ drawchar
        r> d# 4 + >r
        1+
    repeat 2drop
    r> drop
;
