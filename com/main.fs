( Basic Forth words                          JCB 17:57 06/12/13)

32 constant bl
: false d# 0 ;
: true  d# -1 ;
: rot   >r swap r> swap ;
: -rot  swap >r swap r> ;
: 0=    false = ;
: tuck  swap over ;
: 2drop drop drop ;
: ?dup  dup if dup then ;
: 2dup  over over ;
: +!    tuck @ + swap ! ;

\ Comparisons
: invert    true xor ;
: <>        = invert ;
: 0<>       0= invert ;
: <         swap > ;
: 0<        false < ;
: 0>=       0< invert ;
: 0>        d# 0 > ;
: >=        < invert ;
: <=        > invert ;
: u<        swap u> ;

\ Arithmetic
: 1-        true + ;
: 1+        d# 1 + ;
: negate    1- invert ;
: within  ( test low high -- flag )
    over - >r - r>  u<
;
: abs       dup 0< if negate then ;
: ?: ( u0 u1 f -- uf )
            >r over xor r> and xor
;
: min       2dup > ?: ;
: max       2dup < ?: ;
: cells     d# 2 lshift ;
: cell+     d# 4 + ;
: 2/        d# 1 rshift ;

: count     dup 1+ swap c@ ;
: bounds ( a u -- a+u a )
    over + swap ;

: 2!
    dup >r ! r> cell+ !
;
: 2@
    dup >r cell+ @ r> @
;

( Debugging                                  JCB 17:58 06/12/13)
: cr
  d# 13 emit
  d# 10 emit
;

: digit
  [char] 0 + emit
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

: . hex4 space ;

: snap0
  depth if
    >r snap0 r> .
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

: main
  banner

  d# 50000 begin
    1- dup 0=
  until drop
  h# 1111 h# 2222 h# 3333
  snap
;
