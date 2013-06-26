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
: 2*    dup + ;

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
: 2+        1+ 1+ ;
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

: count     dup 1+ swap c@ ;
: bounds ( a u -- a+u a )
    over + swap ;

: 2!
    dup >r ! r> cell+ !
;
: 2@
    dup >r cell+ @ r> @
;

( Double word                                JCB 10:14 06/25/13)

: s>d       dup 0< ;
: d+                              ( augend . addend . -- sum . )
    rot + >r                      ( augend addend)
    over +                        ( augend sum)
    dup rot                       ( sum sum augend)
    u< if                         ( sum)
        r> 1+
    else
        r>
    then                          ( sum . )
;
: d1+       d# 1. d+ ;
: dnegate
    invert swap invert swap
    d1+
;
: d- dnegate d+ ;
: d=
    rot = -rot = and
;
: dabs ( d -- ud ) dup 0< if dnegate then ;

: *
    um* drop
;

: SM/REM ( d n -- r q ) ( 6.1.2214 ) ( symmetric )
  OVER >R >R  DABS R@ ABS UM/MOD
  R> R@ XOR 0< IF NEGATE THEN  R> 0< IF >R NEGATE R> THEN ;
: /     /mod nip ;
: mod   /mod drop ;
: */mod >R M* R> SM/REM ;
: */    */mod nip ;
