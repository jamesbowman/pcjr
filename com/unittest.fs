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
: dT
    d<> throw
;

: d123
    d# 1 d# 2 d# 3
;

: pause
  d# 3000 begin 1- dup 0= until drop
;

: main
    d# 10 base !
    banner

    d# 0 part
    d# 1 d# 2 +             d# 3 T

    d# 1 part
    d# 5 d# 8 lshift        h# 0500 T
    h# ffff d# 9 lshift     h# fe00 T
    h# ffff d# 9 rshift     h# 007f T

    d# 2 part
    d# -1 2/                h# ffff T
    d# 1 2/                 h# 0000 T
    h# 5555 2/              h# 2aaa T

    d# 3 part
    d123                    d# 3 T d# 2 T d# 1 T
    d123 swap               d# 2 T d# 3 T d# 1 T
    d123 rot                d# 1 T d# 3 T d# 2 T
    d123 -rot               d# 2 T d# 1 T d# 3 T
    d123 tuck               d# 3 T d# 2 T d# 3 T d# 1 T
    d123 xor xor            d# 0 T

    d# 4 part
    d# 8 d# 0
    begin
        2dup <>
    while
        1+
    repeat                  d# 8 T d# 8 T
    true if
        d# 6
    else
        d# 7
    then                    d# 6 T
    false if
        d# 6
    else
        d# 7
    then                    d# 7 T

    d# 5 part
    d# 100 d# 101 max       d# 101 T
    d# 100 d# 101 min       d# 100 T

    d# 6 part
    d# 1000.                d# 0 T d# 1000 T
    d# -1000.               h# FFFF T h# FC18 T

    d# 7 part
    h# 0002 h# 0001 =       false T
    h# ffff h# 0001 =       false T
    h# ffff h# ffff =       true T
    h# 0100 h# 8000 =       false T

    d# 8 part
    h# 0002 h# 0001 u>      true T
    h# ffff h# 0001 u>      true T
    h# ffff h# ffff u>      false T
    h# 0100 h# 8000 u>      false T

    d# 9 part
    h# 0002 h# 0001 >       true T
    h# ffff h# 0001 >       false T
    h# ffff h# ffff >       false T
    h# 0100 h# 8000 >       true T

    d# 10 part
    d# 1 >r
    d# 10
    begin
        r> dup 2* + >r
    loop
    r>                      d# 59049 T

    d# 11 part
    h# dead h# beef
    um*
                            h# a6144983. dT

    d# 12 part
    h# a6144983. h# dead um/mod
                            h# beef T d# 0 T

    d# 13 part
    d# 7700 d# 99 d# 100 */
                            d# 7623 T
    d# -7700 d# 99 d# 100 */
                            d# -7623 T

    d# 14 part
    h# aabbcc. 2>r
    2r@                     h# aabbcc. dT
    2r>                     h# aabbcc. dT

    d# 999 part
    depth d# 0 T
    cr
    [char] O emit
    [char] K emit

    quit
;
