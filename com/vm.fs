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

:: +       s" plus   "   aword ;
:: -       s" minus  "   aword ;

:: =       s" eq     "   aword ;
:: >       s" gt     "   aword ;
:: u>      s" above  "   aword ;

:: int21   s" int21  "   aword ;
:: eol     s" eol"    aword ;
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

map vga!    vgastore"
