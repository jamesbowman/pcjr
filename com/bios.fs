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
    d# 0 terminate
;

: abort
    [char] * emit
    [char] A emit
    d# 1 terminate
;

: throw
    if abort then
;

