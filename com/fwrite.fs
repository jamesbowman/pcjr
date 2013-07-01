include base.fs
include bios.fs
include debug.fs
include zen.fs
include draw.fs
include numeric.fs

: status
    [char] - emit
    space
    emit cr
;

: main
    d# 10 base !

\   xasm create_file throw
\   >r
\   d# 100 begin
\       dup s>d <# [char] , hold #s #> r@ xasm write_file throw
\   loop
\   r> xasm close_file throw

    [char] R status
    xasm sync
    begin
        [char] S status
        [char] [ emit
        pad
        begin
            xasm read8
            dup
        while
            dup emit
            over c!
            1+
        repeat
        swap c!
        [char] ] emit cr

        pad c@ 0= if
            d# 0 terminate
        then

        pad xasm create_file throw
        >r

        begin
            xasm read8 xasm read8 d# 8 lshift or
            dup 0<>
        while
    dup hex4 cr
            pad swap    ( pad u )
            2dup        ( pad u pad u )
            bounds
            begin
                2dup xor
            while
                xasm read8 over c!
                1+
            repeat 2drop
                        ( pad u )
            r@ xasm write_file throw
    depth hex2 cr
        repeat

        r> xasm close_file throw
    again

    snap
    quit
;
