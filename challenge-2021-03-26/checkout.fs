#! /usr/bin/env ueforth

0 value owed
0 value paid
: $. ( n -- ) <# # # [char] . hold #s [char] $ hold #> type ;
: status   ." CHARGE: " owed $. ."    PAID: " paid $. cr ;
: reset   0 to paid 0 to owed ;
create coins 2000 , 1000 , 500 , 100 , 25 , 10 , 5 , 1 ,
: n-coin ( $ coin -- )
   dup -rot /mod 0 ?do over $. space loop nip ;
: emit-coins ( n -- )
   ." DISPENSING ------ "
   coins swap begin dup while over @ n-coin >r cell+ r> repeat
   cr 2drop ;
: make-change ( n -- ) paid owed - emit-coins reset ;
: ?make-change paid owed >= if make-change then status ;
: charge ( n -- ) +to owed ?make-change ;   : c charge ;
: pay ( n -- ) +to paid ?make-change ;      : p pay ;
: cancel   0 to owed ?make-change ;

." INSTRUCTIONS" cr
." ------------" cr
." <amount> c[harge] --- charge an amount in cents" cr
." <amount> p[ay]    --- pay an amount in cents" cr
." cancel            --- cancel current transaction" cr
." bye               --- exit" cr
status
quit
