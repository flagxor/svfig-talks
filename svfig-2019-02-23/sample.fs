: gcd ( a b -- n ) dup 0= if drop else swap over mod recurse then ;

: factorial ( n -- n! )
  dup 1 <> if dup 1- recurse * then ;
: factorial ( n -- n! )
  1+ 1 swap 1 ?do i * loop ;

: fib ( n -- nfib )
    dup 1 > if dup 1- recurse swap 2 - recurse + then ;
: fib-iter ( n -- nfib )
  0 1 rot 0 ?do swap over + loop drop ;

: sum ( next term b a -- n )
   0 -rot ?do over i swap execute dup . +
             >r over r> swap execute dup .
             +loop nip nip ;
: one 1 ;
: sum-cubes ['] one ['] cube sum ;

: square ( n -- n2 ) dup * ;
: average ( a b -- mid ) + 2/ ;
: improve ( guess x ) over / average ;
: good-enough? ( x guess ) dup >r square - abs r> < ;
: sqrt-iter ( x guess )
   2dup good-enough? if nip else over improve recurse then ;
: sqrt ( x -- rx ) 1 sqrt-iter ;

: fsquare ( n -- n2 ) fdup f* ;
: faverage ( a b -- mid ) f+ 2e f/ ;
: fgood-enough? ( x guess ) fsquare f- fabs 0.001e f< ;
: fimprove ( guess x ) fover f/ faverage ;
: f2dup ( a b -- a b a b ) fover fover ;
: fsqrt-iter ( x guess )
   f2dup fgood-enough? if fnip else fover fimprove recurse then ;
: fsqrt ( x -- rx ) 1.0e fsqrt-iter ;

create denominations 1 , 5 , 10 , 25 , 50 ,
: first-denomination ( kinds-of-coins -- n ) 1- cells denominations + @ ;
: cc ( amount kinds-of-coins )
   recursive
   over 0= if 2drop 1 exit then
   2dup 0= swap 0< or if 2drop 0 exit then
   2dup 1- cc >r dup >r first-denomination - r> cc r> + ;
: count-change ( amount -- n ) 5 cc ;

