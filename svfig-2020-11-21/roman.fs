#! /usr/bin/env gforth
: ^bl ( a -- a ) begin dup c@ bl <> while 1+ repeat ;
: nth. ( n a n -- a ) drop swap 0 ?do ^bl 1+ loop dup ^bl over - type ;
: 1s    s"  I II III IV V VI VII VIII IX " nth. ;
: 10s   s"  X XX XXX XL L LX LXX LXXX XC " nth. ;
: 100s  s"  C CC CCC CD D DC DCC DCCC CM " nth. ;
: 1000s  0 ?do ." M" loop ;    : /10 10 /mod ;
: ?-  dup 0< if ." -" negate then ;   : ?0  dup 0= if ." N" then ;
: roman ( n -- ) ?- ?0 /10 /10 /10 1000s 100s 10s 1s space ;
: test 4001 -20 do i . i roman cr loop ;  test   bye
