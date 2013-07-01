( Pictured numeric output                    JCB 15:18 08/21/12)

variable base
variable hld
create pad $100 allot create pad|

: <# ( -- ) ( 6.1.0490 )( h# 96 ) pad| HLD ! ;
: DIGIT ( u -- c ) d# 9 OVER < d# 7 AND + [CHAR] 0 + ;
: HOLD ( c -- ) ( 6.1.1670 ) HLD @ 1- DUP HLD ! C! ;

: # ( d -- d ) ( 6.1.0030 )
  d# 0 base @ UM/MOD >R base @ UM/MOD SWAP DIGIT HOLD R> ;

: #s ( d -- d ) ( 6.1.0050 ) BEGIN # 2DUP OR 0= UNTIL ;
: #> ( d -- a u ) ( 6.1.0040 ) 2DROP HLD @ pad| OVER - ;

: SIGN ( n -- ) ( 6.1.2210 ) 0< IF [CHAR] - HOLD THEN ;

: .
    s>d <# #s #> type space
;
