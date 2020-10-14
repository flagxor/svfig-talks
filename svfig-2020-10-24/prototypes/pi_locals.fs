#! /usr/bin/env gforth

1 50 lshift constant one

: bd. ( n b -- ) base @ >r base ! 0 <# # #> type r> base ! ;
: *mod ( a b m -- n ) */mod drop ;
: odd? ( n -- f ) 1 and ;
: 3drop ( n n n -- ) 2drop drop ;

: **mod 1 { a b m r -- n }
   begin
     b odd? if a r m *mod to r then
     b 2/ to b
     b 0= if r exit then
     a a m *mod to a
   again ;

: 16^-n ( n -- n ) one swap 4 * 63 min rshift ;
: s-right { d j -- n } 0 17 1 do i 16^-n i d + 8 * j + / + loop ;
: slterm { d j i -- n } one 16 d i - i 8 * j + dup >r **mod r> */ ;
: s-left { d j -- n } 0 d 1+ 0 do d j i slterm + loop ;
: s { d j -- n } d j s-left d j s-right + ;

: pi<< { d -- n } d 1 s 4 * d 4 s 2* - d 5 s - d 6 s - 16 swap one */ 15 and ;
: pih-n ( n -- ) ." 3." 0 do i pi<< 16 bd. loop ;
