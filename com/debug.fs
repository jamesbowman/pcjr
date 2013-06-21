( Debugging                                  JCB 17:58 06/12/13)

: cr
  d# 13 emit
  d# 10 emit
;

: hex1
  h# 0f and
  dup d# 9 > if
    d# 7 +
  then
  [char] 0 + emit
;

: hex2
  dup d# 4 rshift hex1 hex1
;

: hex4
  dup d# 8 rshift hex2 hex2
;

: space     bl emit ;

: snap0
  depth if
    >r snap0 r> hex4 space
  then
;

: snap
  depth hex2 space space
  snap0
  cr quit
;

: bar
  d# 72
  begin
    [char] = emit
    1- dup 0=
  until
  drop
  cr
;

: banner
  cr
  bar
  [char] O emit
  [char] K emit
  cr
  bar
;

: type
  bounds
  begin
    2dup <>
  while
    dup c@ emit
    1+
  repeat
  2drop
;
