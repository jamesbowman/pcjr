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

: main
  d# 10 base !
  banner

  d# 0 part
  d# 1 d# 2 +           d# 3 T

  d# 1 part
  d# 5 d# 8 lshift      h# 0500 T
  h# ffff d# 9 lshift   h# fe00 T
  h# ffff d# 9 rshift   h# 007f T

  d# 2 part
  d# -1 2/              h# ffff T
  d# 1 2/               h# 0000 T
  h# 5555 2/            h# 2aaa T

  depth d# 0 T
  cr
  [char] O emit
  [char] K emit
  quit
;
