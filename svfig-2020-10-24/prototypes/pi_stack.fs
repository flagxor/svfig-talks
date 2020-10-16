#! /usr/bin/env gforth

1 50 lshift constant one

: bd. ( n b -- ) base @ >r base ! 0 <# # #> type r> base ! ;
: *mod ( a b m -- n ) */mod drop ;
: odd? ( n -- f ) 1 and ;
: 3drop ( n n n -- ) 2drop drop ;

: a^2 ( r a m b -- r a*a m b/2 ) >r 2dup >r swap *mod r> r> 2/ ;
: a* ( r a m b -- r*a a m b ) >r 2dup >r >r *mod r> r> r> ;
: ?a* ( r a m b -- ?r*a a m b ) dup odd? if a* then ;
: **mod0 ( a b m -- 1 a m b ) >r 1 -rot r> swap ;
: **mod ( a b m -- n ) **mod0 begin ?a* a^2 dup 0= until 3drop ;

: under+ ( x y z n -- x+n y z ) >r rot r> + -rot ;
: 16^-n ( n -- n ) one swap 4 * 63 min rshift ;
: s-right ( d j -- n )
   0 -rot 17 1 do over i + 8 * over + i 16^-n swap / under+ loop 2drop ;
: slterm ( d-i i*8+j -- n ) dup >r >r >r one 16 r> r> **mod r> */ ;
: s-left ( d j -- n )
   0 -rot over 1+ 0 do
      i 8 * over + rot dup i - >r -rot r> swap slterm under+
   loop 2drop ;
: s ( d j -- n ) 2dup s-left >r s-right r> + ;

: pi<< ( d -- n )
   dup 1 s 4 * over 4 s 2* - over 5 s - over 6 s - 16 swap one */ 15 and ;
: pih-n ( n -- ) ." 3." 0 do i pi<< 16 bd. loop ;
