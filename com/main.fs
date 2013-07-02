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

: >au
    \ dup hex2 cr
    h# c0 out
;

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

: freq ( u voice -- )
    over h# f and or >au
    d# 4 rshift >au
;

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

: main
    d# 10 base !

    h# 947

    320x200x16

    h# 1800 >es

    -t-

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

    \ xasm spin

    -t-

    \ d# 24 begin
    \     dup [char] @ +
    \     over d# 8 * dup xyaddr drawchar
    \ loop

    80x25
    took

    /sound music

    snap
    quit
;
