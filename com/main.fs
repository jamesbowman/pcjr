include base.fs
include bios.fs
include debug.fs
include zen.fs
include draw.fs
include numeric.fs

: vidfill ( v16 -- )
    d# 0 d# 32768 rot fill
;

: scramble ( r -- r )
  h# 7ffe begin
    swap xasm lfsr swap 2dup es:!
  loop
;

: 00sleep
    s>d get-system-time d+
    begin
        2dup get-system-time d- nip 0<
    until
    2drop
;

: pause
  d# 100 00sleep
;


: 10th
  d# 200 begin 1- dup 0= until drop
;

: 6845start ( u -- ) \ set 14-bit 6845 start address
    dup d# 8 rshift d# 12 6845!
    d# 13 6845!
;

: wobble
    h# 4f h# 01 6845!
    h# 32 h# 06 6845!
    d# 4080 d# 0 begin
        dup 6845start
        key drop
        d# 80 +
        2dup =
    until 2drop
    d# 0 6845start
;

: flicker ( u -- u )
    >r
    h# 20 h# 10 begin
        r@ 0< over vga!
        r> xasm lfsr >r
        1+ 2dup =
    until 2drop
    r>
;

: border
    h# 02 vga!
;

: -t-
    get-system-time
;

: took
    2swap d- 
    <# # # [char] . hold #s #> type cr
;

: quitkey
    key? dup if
        drop key d# 27 =
    then
;
        
( Sound composition                          JCB 09:00 06/30/13)

\ See http://www.smspower.org/Development/SN76489

0 [IF]
: >au
    \ dup hex2 cr
    h# c0 out
;

: freq ( u voice -- )
    over h# f and or >au
    d# 4 rshift >au
;
[THEN]

: silence
    b# 10011111 >au   \ attenuation=0
    b# 10111111 >au   \ attenuation=0
    b# 11011111 >au   \ attenuation=0
    b# 11111111 >au   \ attenuation=0
;

0x80 constant VOICE1
0xa0 constant VOICE2
0xc0 constant VOICE3
0xe0 constant NOISE

: ntype ( u -- )
    h# e0 or >au
;

: atten ( a voice -- )
    b# 00010000 or or >au
;

: pip ( f -- )
    VOICE1 freq  d# 4 VOICE1 atten   10th
    silence                          10th
;

: /sound
    h# 61 in b# 01100000 or h# 61 out

    \ b# 10010000 >au   \ attenuation=0
    \ b# 10001111 >au   \ counter=512, so 3.579e6/(32*512) = 218 Hz
    \ b# 00111111 >au

    d# 70 pip
    d# 60 pip
;

\ : pcm ( u -- )
\     dup VOICE1 atten
\     dup VOICE2 atten
\         VOICE3 atten
\ ;

: shape ( u -- )
    waitvsync
    d# 15 xor
    NOISE atten
;

: music
    false if
        pause

        d# 4   VOICE1 atten
        d# 512 VOICE1 freq
        pause

        d# 2   VOICE2 atten
        d# 400 VOICE2 freq
        pause

        d# 0 VOICE3 atten
        d# 1023 begin
            dup VOICE3 freq
        loop
        silence
    then

    false if
        d# 0 NOISE atten
        d# 5 ntype
        d# 300 begin
            dup .
            d# 4 ntype
            dup begin loop
        loop
    then

    false if
        d# 8 NOISE atten
        d# 4 ntype

        d# 114 d# 110 begin
            2dup <>
        while
            dup . cr
            d# 0 VOICE1 atten
            dup VOICE1 freq     \ 100: 
            pause
            cli
            d# 1000 begin
                d# 0 VOICE1 atten
                d# 15 VOICE1 atten
            loop
            sti
            1+
        repeat
    then

    d# 1 VOICE1 freq
    \ d# 1 VOICE2 freq
    \ d# 1 VOICE3 freq
\   false if
\       cli
\       d# 20 begin
\           include ../sampling/0
\       loop
\       sti
\   then

    false if
        begin
            l# sample
            l# sample_size
            scratch
        quitkey until
    then

    false if
        d# 10 VOICE1 freq
        d# 0 VOICE1 atten
        begin
            waitvsync d# 15 VOICE1 atten
            waitvsync d# 0 VOICE1 atten
        key? until
        key drop
    then

    true if
        d# 4 ntype
        begin
            d# 8 shape
            d# 15 shape
            d# 12 shape
            d# 8 shape
            d# 4 shape
            d# 0 shape
            pause
        quitkey until
    then

    silence
;

: slide
    xasm open_file throw >r
    pad d# 32768 r@ xasm read_file throw
    pad h# 0000 d# 32768 move
    r> xasm close_file throw
    d# 10 begin
        key? if
            key bl = if
                drop exit
            else
                80x25
                quit
            then
        then
        pause
    loop
;

: slideshow
    begin
        z" eclairs.pic" slide
        z" CAKE.PIC" slide
        z" THREE.PIC" slide
        z" BEACH.PIC" slide
        z" FELIX.PIC" slide
        z" FRUIT.PIC" slide
        z" LIGHTS.PIC" slide
    again
;

\ : wait1
\    wait
\   l# tick240 @
\   begin
\       dup
\       l# tick240 @
\       <>
\   until
\   drop
\ ;

: waitn
    begin
        wait1
    loop
;
: wait10 d# 10 waitn ;
: wait15 d# 15 waitn ;
: wait20 d# 20 waitn ;
: wait30 d# 30 waitn ;
: wait60 d# 60 waitn ;
: wait240 d# 240 waitn ;

: full12
    d# 0 VOICE1 atten
    d# 0 VOICE2 atten
;

: full123
    d# 0 VOICE1 atten
    d# 0 VOICE2 atten
    d# 0 VOICE3 atten
;

: a123 ( a -- )
    dup VOICE1 atten
    dup VOICE2 atten
    VOICE3 atten
;

: a12 ( a -- )
    dup VOICE1 atten
    VOICE2 atten
;

: perfect5th ( 3:2 )
    \ d# 3 d# 2 */
    dup 2/ +
;

: major3rd ( 5:4 )
    dup 2/ 2/ +
;

: minor3rd ( 6:5 )
    dup h# 3333 um* nip +
;

: augmented5th ( 25:16 )
    dup h# 9000 um* nip +
;

: diminished5th ( 25:18 )
    dup h# 638e um* nip +
;
: major6th ( 5:3 )
    dup h# 240b um* nip +
;

: dyad ( c c -- )
    wait1
    VOICE1 freq
    VOICE2 freq
    full12
    d# 0 border
;

: triad ( c c c -- )
    wait1
    VOICE1 freq
    VOICE2 freq
    VOICE3 freq
    full123
    d# 0 border
;

: majortriad ( c -- c c c )
    dup major3rd over perfect5th
;
: minortriad ( c -- c c c )
    dup minor3rd over perfect5th
;
: diminishedtriad ( c -- c c c )
    dup minor3rd over diminished5th
;
: augmentedtriad ( c -- c c c )
    dup minor3rd over augmented5th
;

: sweep5th/64 ( end start -- ) \ sweep a perfect 5th over 64 ticks
    d# 0 begin
        >r
        2dup r@ lerp
        minortriad triad
        r> d# 2048 + dup 0=
    until drop 2drop
;

: tremolo
    d# 4 rshift
    begin
        d# 0 a123 d# 8 waitn
        d# 1 a123 d# 8 waitn
    loop
;

: tremolo12
    d# 4 rshift
    begin
        d# 0 a12 d# 8 waitn
        d# 1 a12 d# 8 waitn
    loop
;

: silence12
    d# 15 a12
;

: dyad-taptap ( n -- )
    >r 
    dyad r@ waitn
    silence12 r@ waitn
    full12 r@ waitn
    silence12 r> waitn
;

: drumup
    NOISE d# 3 atten
    wait10
    NOISE d# 15 atten
    wait10
;

: drumdown
    NOISE d# 8 atten
    wait10
    NOISE d# 15 atten
    wait10
;

: note
    dyad        drumup
    silence12   drumdown
    full12      drumup
    silence12   drumdown
;

: cycle-a
    d# 5 ntype
    h# 100 dup perfect5th note
    h# 180 dup perfect5th note
    h# 240 dup perfect5th note
    d# 303 dup perfect5th note
    d# 454 dup perfect5th note
    d# 682 dup perfect5th note
;

: cycle-b
    d# 4 ntype
    h# aa dup perfect5th  note
    h# 180 dup perfect5th note
    h# 240 dup perfect5th note
    d# 202 dup perfect5th note
    d# 303 dup perfect5th note
    d# 454 dup perfect5th note
;

: cycle-c
    d# 4 ntype
    h# 71 dup perfect5th  note
    h# aa dup perfect5th  note
    h# 180 dup perfect5th note
    d# 134 dup perfect5th note
    d# 202 dup perfect5th note
    d# 303 dup perfect5th note
;

: 3.cycle-a d# 3 begin cycle-a loop ;
: 3.cycle-b d# 3 begin cycle-b loop ;
: 3.cycle-c d# 3 begin cycle-c loop ;

: audio
    \ wait240
    \ exit

    \ d# 202 dup perfect5th dyad  d# 240 tremolo
    \ d# 303 d# 202 sweep5th/64   d# 240 tremolo
    \ d# 454 d# 303 sweep5th/64   d# 240 tremolo
    \ d# 682 d# 454 sweep5th/64   d# 240 tremolo

    d# 1 begin
        false if
            d# 2 begin
                h# 100 dup perfect5th dyad wait240
                h# 180 dup perfect5th dyad wait240
                h# 240 dup perfect5th dyad d# 300 tremolo12
                d# 303 dup perfect5th dyad wait240
                d# 454 dup perfect5th dyad wait240
                d# 682 dup perfect5th dyad d# 300 tremolo12
                silence12 wait60
                silence12 wait60
            loop
        then

        false if
            d# 10 begin
                drumup
                drumup
                drumdown
                drumdown
            loop
        then

        true if
            3.cycle-a
            3.cycle-b
            3.cycle-a
            3.cycle-c
        then

        \ third voice enters on 71
        false if
            h# 71 VOICE3 freq
            d# 15 begin
                dup VOICE3 atten
                cycle-a
            loop
            h# aa VOICE3 freq
            3.cycle-a
            h# 71 VOICE3 freq
            3.cycle-a

            d# 6 begin
                cycle-a
                d# 15 over 2* - VOICE3 atten
            loop
        then
        false if
            h# 4c VOICE3 freq
            d# 6 begin
                dup 2* VOICE3 atten
                cycle-a
            loop
            h# 32 VOICE3 freq
            d# 3 begin
                cycle-a
            loop
            h# 4c VOICE3 freq
            d# 3 begin
                cycle-a
            loop
            d# 3 begin
                cycle-b
            loop
            h# 71 VOICE3 freq
            d# 3 begin
                cycle-a
            loop
            d# 3 begin
                cycle-c
            loop
            h# aa VOICE3 freq
            d# 3 begin
                cycle-a
            loop
            d# 3 begin
                cycle-b
            loop
            h# 100 VOICE3 freq
            d# 3 begin
                cycle-a
            loop
            d# 3 begin
                cycle-c
            loop
            d# 3 begin
                cycle-a
            loop
            d# 6 begin
                cycle-b
                d# 15 over 2* - VOICE3 atten
            loop
            d# 3 begin
                cycle-a
            loop
        then
    loop
;

: main
    d# 10 base !

    h# 947

    \ 320x200x4 h# 1c00 >es
    \ 320x200x16 h# 1800 >es

0 [IF]
    h# 8000 scramble
    d# 600 begin
        swap
        waitvsync

        dup h# 10 vga! xasm lfsr
        dup h# 11 vga! xasm lfsr
        dup h# 12 vga! xasm lfsr
        dup h# 13 vga! xasm lfsr
        dup h# 14 vga! xasm lfsr
        dup h# 15 vga! xasm lfsr
        dup h# 16 vga! xasm lfsr
        dup h# 17 vga! xasm lfsr
        dup h# 18 vga! xasm lfsr
        dup h# 19 vga! xasm lfsr
        dup h# 1a vga! xasm lfsr
        dup h# 1b vga! xasm lfsr
        dup h# 1c vga! xasm lfsr
        dup h# 1d vga! xasm lfsr
        dup h# 1e vga! xasm lfsr
        dup h# 1f vga! xasm lfsr

        h# 00 h# 02 vga!
        swap
    loop
    drop
[THEN]

0 [IF]
    d# 4 begin
        d# 100 begin
            dup h# f and setcolor
            dup dup d# 40 square
        loop
    loop
[THEN]

0 [IF]
        l# sunset
        h# 0000
        d# 32768
        move
        key drop
[THEN]

0 [IF]

        d# 0 begin
            dup waitvsync 6845start
            d# 80 +
            dup d# 4000 =
        until drop
        waitvsync d# 0 6845start
        pause
[THEN]

0 [IF]
    d# 100 begin
        d# 8000 d# 0
        begin
            l# sunset
            over
            d# 160 move
            down1
            2dup =
        until
        2drop
    loop
[THEN]

    \ d# 1 setcolor d# 120 d# 120 d# 40 square

0 [IF]
    xasm dsprite_cold
    -t-
    d# 200 d# 0 begin
        waitvsync
        d# 1 border
        dup xasm dsprite
        d# 0 border
        1+ 2dup =
    until 2drop
            \ h# 1 border
            \ h# 0 border
    -t-

    pause
[THEN]

1 [IF]
    /sound
    xasm timer_replace
    \ m = 3579545/32.
    audio
    xasm timer_restore
    silence
[THEN]

    \ d# 24 begin
    \     dup [char] @ +
    \     over d# 8 * dup xyaddr drawchar
    \ loop

    \ slideshow

    80x25
    \ took

    \ /sound music
    h# 1000 h# 100 h# ffff lerp

    snap
    quit
;
