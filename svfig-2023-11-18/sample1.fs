#! /usr/bin/env ueforth

needs lambda.fs
needs utils.fs

: adder { n } [: { m } m n + :] ;

: factorial { n }
   0 { fact! }
   [: { n }
      n 0= if
         1
      else
         n 1- fact! invoke n *
      then
   :] to fact!
   n fact! invoke
;

: factorial2 { n }
   0 { iter }
   [: { product counter }
      counter n > if
         product
      else
         counter product * counter 1+ iter invoke
      then
   :] to iter
   1 1 iter invoke
;

: cons { a d } [: if d else a then :] ;
: car ( c ) 0 swap invoke ;
: cdr ( c ) 1 swap invoke ;

internals
: tail   current @ @ cell+ aliteral postpone >r postpone exit ; immediate
forth

: gcd ( a b ) dup 0= if drop else swap over mod tail then ;

: gcd2 { a b } b 0= if a else b a b mod recurse then ;

: make-rat { n d }
   n d gcd2 { g }
   n g / d g / cons ;
: numer ( r ) car ;
: denom ( r ) cdr ;
: print-rat { x } x numer . ." / " x denom . ;

: add-rat { x y } x numer y denom * y numer x denom * +
                  x denom y denom * make-rat ;
: sub-rat { x y } x numer y denom * y numer x denom * -
                  x denom y denom * make-rat ;
: mul-rat { x y } x numer y numer *
                  x denom y denom * make-rat ;
: div-rat { x y } x numer y denom *
                  x denom y numer * make-rat ;
: equal-rat? { x y } x numer y denom * y numer x denom * = ;

create nil
: null? ( c -- f ) nil = ;

: print-list { c }
  ." ( "
  begin c null? 0= while
    c car .
    c cdr to c
  repeat
  ." ) "
;

: map { proc items }
   items null? if
      nil
   else
      items car proc invoke
      proc items cdr recurse cons
   then
;
