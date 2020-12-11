#! /usr/bin/env gforth

0 WARNINGS !

1000000 constant pair-count   : pairs ( n -- n ) 2* cells ;
pair-count pairs 2* allocate throw constant pair-heap
variable pair-brk   variable pair-base   pair-heap pair-base !

: pair+? ( p -- f ) pair-heap - dup 0>= swap pair-count pairs 2* < and ;
: pair? ( p -- f ) pair-base @ - dup 0>= swap pair-count pairs < and ;
: old? ( p -- f) dup pair+? swap pair? 0= and ;
: car ( p -- n ) @ ;    : cdr ( p -- n ) cell+ @ ;
: car! ( n p -- ) ! ;   : cdr! ( n p -- ) cell+ ! ;

defer gc   create nil
: full? ( -- f ) pair-brk @ pair-count >= ;
: reserve   full? if gc then ;
: pair-rel ( n -- p ) pairs pair-base @ + ;
: pair-allot ( -- p ) pair-brk @ pair-rel 1 pair-brk +! ;
: pair ( x y -- p ) reserve pair-allot dup dup >r >r cdr! r> car! r> ;
: unpair ( p -- x y ) dup car swap cdr ;   : cons pair ;

: bind ( a xt -- p ) pair ;
: invoke ( p/a -- ) begin dup pair? while unpair repeat execute ;

: foreach' ( l op -- last )
  begin over pair? while dup >r >r unpair r> swap >r invoke r> r> repeat drop ;
: foreach ( l op -- ) foreach' nil <> throw ;

variable roots   nil roots !
: +root ( a -- ) roots @ pair roots ! ;
: root value lastxt +root ;

variable broken-heart
: relocate ( a -- )
  dup @ unpair pair over @ broken-heart over car! over swap cdr! swap ! ;
: conserve ( a -- )
  dup @ car broken-heart = if dup @ cdr swap ! else relocate then ;
: preserve ( a -- ) dup @ old? if conserve else drop then ;
: newfixup1 ( n -- ) dup pair-rel dup preserve cell+ preserve 1+ ;
: newfixup 0 begin dup pair-brk @ < while newfixup1 repeat drop ;

: rpwalk rp@ rp0 @ swap - cell / 0 ?do rp@ i cells + preserve loop ;
: spwalk depth 0 ?do sp@ i cells + preserve loop ;
: rootwalk roots preserve roots @ ['] preserve foreach ;
: newspace   0 pair-brk !   pair-base @ pair-heap =
  if pair-heap pair-count pairs + else pair-heap then pair-base ! ;
: collect   newspace rpwalk spwalk rootwalk newfixup
            full? if abort" heap full" then ;
' collect is gc   : used pair-brk @ ;

: ^pair swap pair ;
: reverse-append ( a b -- l ) swap ['] ^pair foreach ;
: reverse ( l -- l' ) nil reverse-append ;
: append ( a b -- l ) >r reverse r> reverse-append ;

: chain1 ( a b -- ) >r invoke r> invoke ;
: chain ( a b -- p ) ['] chain1 bind bind ;

: nilpair nil nil pair ;
: prepend! ( a p -- ) dup >r car pair r> car! ;
: map1 ( a op p -- ) >r invoke r> prepend! ;
: map ( l op -- l' ) nilpair dup >r ['] map1 bind bind foreach r> car reverse ;
: filter1 ( a op p -- )
  >r over >r invoke r> r> rot if prepend! else 2drop then ;
: filter ( l op -- l' )
  nilpair dup >r ['] filter1 bind bind foreach r> car reverse ;
: range ( a b -- p ) 1- nil -rot ?do i ^pair -1 +loop ;

: list1 ( .. b n -- p ) 0 ?do pair loop ;
: list ( .. n -- p ) nil swap list1 ;
variable list-depth   variable is-dotted
: ?dot ( pxn n ) is-dotted @ 0= if nil swap else 1- then ;
: ((   list-depth @ is-dotted @ 0 is-dotted ! depth list-depth ! ;
: ))   depth list-depth @ - ?dot list1 >r is-dotted ! list-depth ! r> ;
: ..   -1 is-dotted ! ;
: l. recursive dup pair? if
  ." ( " ['] l. foreach' dup nil = if
    drop else ." . " l. then ." ) "
  else
    dup nil = if ." nil " else . then
  then ;
: l= ( a b -- f ) recursive dup pair? if 2dup car swap car l= >r cdr swap cdr l= r> and else = then ;

( --------------------- )

: sum ( l -- n ) 0 swap ['] + foreach ;
: length ( l -- n ) 0 swap ['] drop ['] 1+ chain foreach ;
: average ( l -- n ) dup sum swap length / ;

( --------------------- )

: sample (( 1 2 3 4 5 6 7 8 9 )) ;
: square dup * ;
: prime?
  dup 2 < if drop 0 exit then
  dup 2 ?do dup i mod 0= if unloop drop 0 exit then loop ;

: digits ( n -- l ) nil swap begin base @ /mod >r ^pair r> dup 0= until drop ;
: palindrome? ( n -- f ) digits dup reverse l= ;
: palindromes  0 100000 range ['] palindrome? filter ;

: test sample sample append dup append dup ['] square map append dup l. sum . ;
