vocabulary gc  also gc definitions

1000 constant HEAP-NODES
HEAP-NODES 2* cells constant HEAP-SIZE

create space1 HEAP-SIZE allot
create space2 HEAP-SIZE allot
space1 value old-space
space2 value new-space
new-space value free-mark
0 value scan-mark

here constant heap0

: remaining ( -- n ) new-space HEAP-SIZE + free-mark - cell/ 2/ ;
: full? ( -- f ) free-mark new-space HEAP-SIZE + = ;
: shifted? ( a -- n ) 2 cells 1- and ;
: within? ( n a b ) >r over >r u< 0= r> r> u< and ;
: inside? ( a sp -- f )
  2dup - shifted? if 2drop 0 exit then
  dup HEAP-SIZE + within?
;
: new? ( a -- f ) new-space inside? ;
: old? ( a -- f ) old-space inside? ;
: swap-spaces
  old-space new-space to old-space to new-space
  new-space to free-mark
  new-space to scan-mark
;
: relocate1 ( a -- )
  dup @ new? if drop exit then
  dup @ old? 0= if drop exit then
  dup @ @ new? if dup @ @ swap ! exit then ( broken heart )
  dup @ free-mark 2 cells cmove
  free-mark over @ !
  free-mark swap !
  2 cells +to free-mark
;
: relocate ( a b -- )
  over - cell/
  for aft dup relocate1 cell+ then next drop ;
: scan
  begin scan-mark free-mark u< while
    scan-mark relocate1
    cell +to scan-mark
  repeat
;

also forth definitions

: collect
  swap-spaces
  sp0 sp@ cell+ relocate
  rp0 rp@ cell+ relocate
  heap0 here relocate
  scan
;

: pair ( a d -- c )
  full? if collect then
  full? if abort" out of gc heap" then
  free-mark cell+ ! free-mark !
  free-mark
  2 cells +to free-mark ;

: unpair ( c -- a d ) dup @ swap cell+ @ ;

: pair? ( c -- f ) new? ;

: set-car! ( a c -- ) ! ;
: set-cdr! ( d c -- ) cell+ ! ;

previous previous
