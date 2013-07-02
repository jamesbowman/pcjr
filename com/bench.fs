include base.fs
include bios.fs
include debug.fs
include zen.fs
include numeric.fs

: part ( n -- )
    s>d <# [char] : hold #s #> type cr
;

: T
    <> throw
;

: d123
    d# 1 d# 2 d# 3
;

: main
    d# 10 base !
    banner

    d# 0.
    cli
    d# 100 begin
        >r
        xasm rpit
        \ d# 100 d# 0 begin
        \     1+ 2dup =
        \ until 2drop
        xasm simple
        xasm rpit
        -
        \ d# 110 - d# 4 um*
        d# 0 d+
        r>
    loop
    sti
    <# # # [char] . hold #s #> type cr
    quit
;
