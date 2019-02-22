#! /usr/bin/env gforth

( Raw postscript drawing )
: line ( x1 y1 x2 y2 )
   ." newpath " swap . . ."  moveto "
   swap . . ."  lineto 10 setlinewidth stroke" cr ;
: showpage ." showpage" cr ;

( Transform stack )
1000000 constant divisor
100000 constant size
variable org-x   variable org-y
variable hx   variable hy   divisor hx !
variable vx   variable vy   divisor vy !
create transform-stack 1000 cells allot
variable tsp   transform-stack tsp !
: >t   tsp @ !  cell tsp +! ;
: t>   -1 cells tsp +!  tsp @ @ ;
: t{ org-x @ >t  org-y @ >t  hx @ >t  hy @ >t  vx @ >t  vy @ >t ;
: }t t> vy !  t> vx !  t> hy !  t> hx !  t> org-y !  t> org-x ! ;

( Transforming lines )
: @* @ divisor */ ;
: transform ( x y -- x' y' )
   2dup vy @* swap hy @* + org-y @ + >r
        vx @* swap hx @* + org-x @ + r> ;
: tline ( x1 y1 x2 y2 ) transform >r >r transform r> r> line ;

( Basic Transformations )
: scale-x ( n -- ) dup hx @* hx !  hy @* hy ! ;
: scale-y ( n -- ) dup vx @* vx !  vy @* vy ! ;
: scale ( n -- ) dup scale-x scale-y ;
: trans ( x y -- ) transform org-y ! org-x ! ;
: swap! ( a1 a2 -- ) 2dup @ swap @ rot ! swap ! ;
: diag-flip   hx vx swap!  hy vy swap! ;
: turn45   hx @ vx @ + 2/   hy @ vy @ + 2/
           hx @ vx @ - 2/   hy @ vy @ - 2/
           vy ! vx ! hy ! hx ! ;

( Setup for usual page )
." .1 .1 scale" cr
500 500 trans
divisor 5000 size */ scale

( Pairs )
: atom ( a -- c ) noname create , latestxt ;
: cons ( a b -- c ) noname create , , latestxt ;

( Composing )
: beside ( a b -- c ) cons does>
   t{ size 2/ 0 trans   divisor 1 2 */ scale-x  dup @ execute }t
   t{ divisor 1 2 */ scale-x  cell+ @ execute }t ; 
: above ( a b -- c ) cons does>
   t{ 0 size 2/ trans   divisor 1 2 */ scale-y  dup cell+ @ execute }t
   t{ divisor 1 2 */ scale-y  @ execute }t ; 
: beside3rd ( a b -- c ) cons does>
   t{ size 3 / 0 trans   divisor 2 3 */ scale-x  dup @ execute }t
   t{ divisor 1 3 */ scale-x  cell+ @ execute }t ; 
: above3rd ( a b -- c ) cons does>
   t{ 0 size 2 3 */ trans   divisor 1 3 */ scale-y  dup cell+ @ execute }t
   t{ divisor 2 3 */ scale-y  @ execute }t ; 
: hflip ( a -- a' ) atom does>
   t{ size 0 trans  divisor negate scale-x  @ execute }t ;
: vflip ( a -- a' ) atom does>
   t{ 0 size trans   divisor negate scale-y  @ execute }t ;
: dmirror ( a -- a' ) atom does> t{ diag-flip  @ execute }t ;
: rot90 ( a -- a' ) dmirror hflip ;
: rot45' ( a -- a' ) atom does> t{ turn45  @ execute }t ;
: rot45 ( a -- a' ) rot90 rot45' vflip ;
: both ( a b -- c ) cons does> dup @ execute cell+ @ execute ;

: /10 size swap 10 */ ;
: /20 size swap 20 */ ;
: fish
   0    0       size 0    tline
   0    0       0    size tline
   size 0       0    size tline
   2 /10  1 /10     2 /10  5 /10  tline
   2 /10  5 /10     4 /10  5 /10  tline
   2 /10  3 /10     3 /10  3 /10  tline
   4 /10  1 /10     4 /10  3 /10  tline
   5 /10  1 /10     7 /10  1 /10  tline
   5 /10  1 /10     5 /10  2 /10  tline
   5 /10  2 /10     7 /10  2 /10  tline
   7 /10  1 /20     7 /10  2 /10  tline
   7 /10  1 /20     4 /10  1 /20  tline
;

: blanky ;
: fish2   ['] fish rot45 hflip execute ;
: fish3   ['] fish2 rot90 rot90 rot90 execute ;
: tile   fish fish2 fish3 ;
: tile-u   ['] fish2 dup rot90 dup rot90 dup rot90 both both both execute ;

: quartet ( p q r s -- c ) beside >r beside r> above ;
: cycle ( p -- c ) dup rot90 dup rot90 dup rot90 quartet ;
: side ( n -- c )
   dup 0= if drop ['] blanky else
     1- recurse dup ['] tile dup rot90 swap quartet then ;
: corner ( n -- c )
   dup 0= if drop ['] blanky else
     1- dup recurse swap side dup rot90 ['] tile-u quartet then ;
: nonet ( p q r s t u v w x )
   beside beside3rd >r
   beside beside3rd r> above >r
   beside beside3rd r> above3rd ;
: squarelimit ( n -- c )
   dup corner swap side 2dup >r >r over rot90 rot90 rot90
   over rot90 ['] tile-u over rot90 rot90
   r> rot90 r> rot90 rot90 over rot90 nonet ;

3 squarelimit execute
showpage

bye
