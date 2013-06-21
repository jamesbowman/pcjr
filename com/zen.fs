( Zen timer port                             JCB 13:56 06/18/13)

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
