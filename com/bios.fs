( BIOS operations                            JCB 13:55 06/19/13)

\ int21 ( dx ax -- ax bx cx dx )

: emit
    h# 200 int21 2drop 2drop
;

: key
    false h# 800 int21
    2drop drop h# ff and
;

: terminate ( code -- )
    h# 4c00 or false swap int21
;

: hilo ( a|b w ) \ Return a*w+b
    swap
    dup lobyte >r
    d# 8 rshift *
    r> +
;
: get-system-time ( -- u. ) \ Returns 100ths
    false h# 2C00 int21 2swap 2drop ( h|m s|cs )
    d# 100 hilo     ( h|m cs )
    swap d# 60 hilo ( cs m )
    d# 6000 um*     ( cs cs. )
    rot s>d d+
;

: quit
    h# 5c c@ d# 16 > if
        h# 5c @ exec
    else
        d# 0 terminate
    then
;

: abort
    [char] * emit
    [char] A emit
    d# 1 terminate
;

: throw
    if abort then
;

: 300x200x16
    h# 0009 xasm int10
;

: 80x25
    h# 0003 xasm int10
;
