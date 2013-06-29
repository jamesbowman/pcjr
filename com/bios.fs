( BIOS operations                            JCB 13:55 06/19/13)

: emit
    h# 200 int21 drop
;

: key
    false h# 800 int21
    h# ff and
;

: terminate ( code -- )
    h# 4c00 or false swap int21
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
