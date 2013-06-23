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

    d# 20 d# 0 begin
        xasm rpit
        xasm simple
        xasm rpit
        - d# 0 <# #s #> type cr
        1+ 2dup =
    until
    quit
;
