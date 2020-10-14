#! /usr/bin/env gforth

1 50 lshift constant one

: bd. ( n b -- ) base @ >r base ! 0 <# # #> type r> base ! ;
: *mod ( a b m -- n ) */mod drop ;
: odd? ( n -- f ) 1 and ;
: 3drop ( n n n -- ) 2drop drop ;

: b^2 ( r a m b -- r a*a m b/2 ) >r 2dup >r swap *mod r> r> 2/ ;
: b* ( r a m b -- r*a a m b ) >r 2dup >r >r *mod r> r> r> ;
: ?b* ( r a m b -- ?r*a a m b ) dup odd? if b* then ;
: **mod0 ( a b m -- 1 a m b ) >r 1 -rot r> swap ;
: **mod ( a b m -- n ) **mod0 begin ?b* b^2 dup 0= until 3drop ;

: 16^-n ( n -- n ) one swap 4 * 63 min rshift ;
: s-right { d j -- n } 0 17 1 do i 16^-n i d + 8 * j + / + loop ;
: slterm { d j i -- n } one 16 d i - i 8 * j + dup >r **mod r> */ ;
: s-left { d j -- n } 0 d 1+ 0 do d j i slterm + loop ;
: s { d j -- n } d j s-left d j s-right + ;
: pi<< { d -- n } d 1 s 4 * d 4 s 2* - d 5 s - d 6 s - 16 swap one */ 15 and ;

: pi-cell<< ( d -- n ) 0 16 0 do 4 lshift over 16 * i + pi<< or loop nip ;
: pi-fill ( a n -- ) 0 do i ." ." pi-cell<< over i cells + ! loop drop cr ;
: 10th 0 { a n t -- n }
   0 n 1- do i cells a + dup @ 10 um* t 0 d+ to t swap ! -1 +loop t ;

: pih-n ( n -- ) ." 3." 0 do i pi<< 16 bd. loop ;
: pi-n ( n -- )
   here over pi-fill ." 3." dup 18 * 0 do here over 10th 10 bd. loop drop ;
