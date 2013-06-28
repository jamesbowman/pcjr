: aword  tab ." dw " type cr ;

: map
    ::
    postpone s"
    postpone aword
    postpone ;
;

:: and  s" _and"   aword ;
:: or   s" _or "   aword ;
:: xor  s" _xor"   aword ;

:: lshift  s" lshift"   aword ;
:: rshift  s" rshift"   aword ;
:: 2/      s" _2div "   aword ;

:: +       s" plus   "   aword ;
:: -       s" minus  "   aword ;
:: 1-      s" oneminus"  aword ;
:: 1+      s" oneplus"   aword ;
:: um*     s" umstar  "   aword ;
:: m*      s" mstar  "   aword ;

:: =       s" eq     "   aword ;
:: >       s" gt     "   aword ;
:: u>      s" above  "   aword ;

:: int21   s" int21  "   aword ;
:: eol     s" eol"    aword ;
:: exec    s" exec"      aword ;
:: >es     s" toes"      aword ;
:: es:!    s" esstore"   aword ;
:: /mod    s" slashmod"  aword ;
:: um/mod  s" umslashmod" aword ;

:: dup     s" dup    "   aword ;
:: drop    s" drop   "   aword ;

:: !       s" wstore "   aword ;
:: @       s" wat    "   aword ;
:: c!      s" cstore "   aword ;
:: c@      s" cat    "   aword ;
:: io!     s" iostore "   aword ;
:: io@     s" ioat    "   aword ;

:: depth   s" depth  "   aword ;
:: halt    s" halt   "   aword ;
:: swap    s" swap   "   aword ;
:: over    s" over   "   aword ;
:: nip     s" nip    "   aword ;

:: >r      s" tor    "   aword ;
:: r@      s" rat    "   aword ;
:: r>      s" rfrom  "   aword ;

:: move    s" move" aword ;
:: fill    s" fill" aword ;
:: fillw   s" fillw" aword ;

map vga!    vgastore"
\ map 6845!   _6845store"
map out     _out"
map in      _in"

: str-name ( "name" -- addr ) >in @ name >str swap >in ! ; immediate

: xword
    str-name
    :
     literal  
    postpone str@
    postpone type
    postpone ;
;

