( J1 Cross Compiler                          JCB 16:55 05/02/12)

\   Usage gforth cross.fs <machine.fs> <program.fs>
\
\   Where machine.fs defines the target machine
\   and program.fs is the target program
\
include strings.fs

wordlist constant target-wordlist
: add-order ( wid -- ) >r get-order r> swap 1+ set-order ;
: :: get-current >r target-wordlist set-current : r> set-current ;

( Code constructing words                    JCB 06:01 06/12/13)

variable uucount 0 uucount !
: label ( -- u ) \ A new label
  uucount @
  1 uucount +!
;

: .label ( u -- ) \ print a label
  s>d <# # # # # # # [char] L hold #> type
;

: label: ( u -- ) \ define a label
  .label [char] : emit cr
;

: tab
  8 spaces
;
next-arg included       \ include the machine.fs

( Language basics for target                 JCB 19:08 05/02/12)

warnings off
:: ( postpone ( ;
:: \ postpone \ ;

:: include      include ;
:: included     included ;
:: marker       marker ;
:: [if]         postpone [if] ;
:: [else]       postpone [else] ;
:: [then]       postpone [then] ;

: literal
    tab ." dw lit" cr
    tab ." dw " $ffff and . cr
;
: labelliteral ( u -- )
    tab ." dw lit" cr
    tab ." dw " .label cr
;

( Defining words for target                  JCB 19:04 05/02/12)

:: :
    create  label dup , label:
    does>   tab ." dw " @ .label cr
;

:: ;
    tab ." dw exit" cr
;

\ :: ;fallthru ;
\ 

:: constant
    create  ,
    does>   @ literal
;

:: variable
    create  label dup , label:
            tab ." dw  0" cr
    does>   @ labelliteral
;

:: create
    create  label dup , label:
    does>   @ labelliteral
;

:: allot ( u -- )
    tab ." times " . ." db 0" cr
;

( Switching between target and meta          JCB 19:08 05/02/12)

: target    only target-wordlist add-order definitions ;
: ]         target ;
:: meta     forth definitions ;
:: [        forth definitions ;

( eforth's way of handling constants         JCB 13:12 09/03/10)

: sign>number   ( c-addr1 u1 -- ud2 c-addr2 u2 )
    0. 2swap
    over c@ [char] - = if
        1 /string
        >number
        2swap dnegate 2swap
    else
        >number
    then
;

: base>number   ( caddr u base -- )
    base @ >r base !
    sign>number
    r> base !
    dup 0= if
        2drop drop literal
    else
        1 = swap c@ [char] . = and if
            drop dup literal 16 rshift literal
        else
            -1 abort" bad number"
        then
    then ;

:: xasm 
    tab ." dw " bl parse type cr
;

:: d# bl parse 10 base>number ;
:: h# bl parse 16 base>number ;
:: b# bl parse 2 base>number ;
:: ['] ' >body @
  tab ." dw lit" cr
  tab ." dw " .label cr ;
:: [char] char literal ;
:: l# tab ." dw lit" cr 
    tab ." dw " bl parse type cr
;

( Conditionals                               JCB 13:12 09/03/10)

: branch ( label btype -- )
    tab ." dw " type cr
    tab ." dw " .label cr
;

:: if
    label
    dup s" zbranch" branch
;
\ 
:: then
    label:
;

:: else
    label
    dup s" branch" branch
    swap label:
;

:: begin
  label dup label:
;

:: again ( dest -- )
    s" branch" branch
;

:: until
    s" zbranch" branch
;
:: while
    label
    dup s" zbranch" branch
;
:: repeat
    swap
    s" branch" branch
    label:
;
:: loop
    s" _loop" branch
;

\ 
\ :: for      s" d# 0 >r" evaluate there ;
\ :: next     s" loop" evaluate 2/ 0branch s" rdrop" evaluate ;
\ :: i        s" r@" evaluate ;
\ 
: .trim ( a-addr u ) \ shorten string until it ends with '.'
    begin
        2dup + 1- c@ [char] . <>
    while
        1-
    repeat
;
next-arg 2dup .trim >str constant prefix.
: .suffix  ( c-addr u -- c-addr u ) \ e.g. "bar" -> "foo.bar"
    >str prefix. +str str@
;
warnings on

target included                         \ include the program.fs

meta
s" target main meta " evaluate
depth throw
bye
