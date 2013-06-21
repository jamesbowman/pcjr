( BIOS operations                            JCB 13:55 06/19/13)

: emit
  d# 2 int21 drop
;

: key
  d# 0 h# 08 int21
  h# ff and
;

: quit
  false h# 4c int21 drop
;


