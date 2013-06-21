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
: 2+        d# 2 + ;
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

( Debugging                                  JCB 17:58 06/12/13)
1 [IF]
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
[THEN]

( Zen timer port                             JCB 13:56 06/18/13)
0 [IF]
\ The address of the timer 0 count registers in the 8253.
\
0x40 constant TIMER_0_8253
\
\ The address of the mode register in the 8253.
\
0x43 constant MODE_8253
\
\ The address of Operation Command Word 3 in the 8259 Programmable
\ Interrupt Controller (PIC) (write only, and writable only when
\ bit 4 of the byte written to this address is 0 and bit 3 is 1).
\
0x20 constant OCW3
\
\ The address of the Interrupt Request register in the 8259 PIC
\ (read only, and readable only when bit 1 of OCW3 = 1 and bit 0
\ of OCW3 = 0).
\
0x20 constant IRR

: ztimer
    b# 00110100 MODE_8253 io! \ mode 2
    d# 0 TIMER_0_8253 io!
    d# 0 TIMER_0_8253 io!
;
: timer@
    b# 00000000 MODE_8253 io! \ latch timer 0
    TIMER_0_8253 io@
    TIMER_0_8253 io@
    d# 8 lshift or
    negate
;
[ENDIF]

( Pictured numeric output                    JCB 15:18 08/21/12)

variable base
variable hld
create pad $14 allot create pad|

: <# ( -- ) ( 6.1.0490 )( h# 96 ) pad| HLD ! ;
: DIGIT ( u -- c ) d# 9 OVER < d# 7 AND + [CHAR] 0 + ;
: HOLD ( c -- ) ( 6.1.1670 ) HLD @ 1- DUP HLD ! C! ;

: # ( d -- d ) ( 6.1.0030 )
  d# 0 base @ UM/MOD >R base @ UM/MOD SWAP DIGIT HOLD R> ;

: #s ( d -- d ) ( 6.1.0050 ) BEGIN # 2DUP OR 0= UNTIL ;
: #> ( d -- a u ) ( 6.1.0040 ) 2DROP HLD @ pad| OVER - ;

: SIGN ( n -- ) ( 6.1.2210 ) 0< IF [CHAR] - HOLD THEN ;

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

: vidfill
    >r
    d# 32768
    d# 0
    begin
        r@ over es:!
        2+ 2dup =
    until
    2drop
    r> drop
;

: pause
  d# 60000 begin 1- dup 0= until drop
;

: scramble
  >r
  h# 8000 h# 0 begin
    2dup <>
  while
    r> xasm lfsr dup >r over es:!
    2+
  repeat 2drop
  r>
;

: main
  d# 10 base !
  banner

  h# 947
  h# 0009 xasm int10
  h# 0b800 >es

  h# 1111 vidfill

  l# sunset
  h# 0000
  d# 32768
  xasm movsi

  key drop

  d# 0 d# 0 do
      h# f h# 14 vga!
      h# 0 h# 14 vga!
  loop
    
  \ h# 8000
  \ d# 2 d# 0 do
  \     scramble
  \     h# 1111 vidfill
  \ loop
  \ drop

  key drop

  h# 0003 xasm int10
  snap
;

