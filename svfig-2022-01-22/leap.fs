#! /usr/bin/env gforth

\ : leap? ( n -- f )
\   4 /mod 25 /mod 4 mod 0= swap 0<> or swap 0= and ;

\ : leap? ( n -- f )
\   4 /mod 25 /mod 4 mod 0= or 0<> swap 0= and ;

: leap? ( n -- f )
   4 /mod 25 /mod 4 mod 0= or swap 0= and ;

variable prior
: numeral ( n ) create ,
   does> @ prior @ over < if prior @ 2* - then dup prior ! + ;
 1 numeral I     5 numeral V     10 numeral X
50 numeral L   100 numeral C    500 numeral D   1000 numeral M
: digit ( a ) 1 find-name name>int execute ;
: no 0 prior !  0 
  bl parse 0 do dup >r digit r> 1+ loop drop ;
: yr no ;
