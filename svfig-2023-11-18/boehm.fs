vocabulary gc  also gc definitions

1000 constant HEAP-NODES
HEAP-NODES 2* cells constant HEAP-SIZE

create nodes HEAP-SIZE allot
create marks HEAP-NODES allot
0 value free-list

here constant heap0

: full? ( -- f ) free-list 0= ;
: shifted? ( a -- n ) 2 cells 1- and ;
: within? ( n a b ) >r over >r u< 0= r> r> u< and ;
: inside? ( a -- f )
  dup nodes - shifted? if drop 0 exit then
  nodes dup HEAP-SIZE + within?
;
: mark1 ( a -- )
  dup inside? 0= if drop exit then
  dup nodes - marks + c@ if drop exit then
  1 over nodes - 2/ cell/ marks + c!
  dup @ recurse cell+ @ recurse
;
: mark-range ( a b -- )
  over - cell/ for aft
    dup @ mark1 cell+
  then next drop
;
: mark
  marks HEAP-NODES 0 fill
  sp0 sp@ cell+ mark-range
  rp0 rp@ cell+ mark-range
  heap0 here mark-range
;
: sweep
  HEAP-NODES 1- for
    i marks + c@ 0= if
      free-list i 2* cells nodes + !
      i 2* cells nodes + to free-list
    then
  next
;

also forth definitions

: collect   mark sweep ;

: pair ( a d -- c )
  full? if collect then
  full? if abort" out of gc heap" then
  free-list dup @ to free-list
  >r r@ cell+ ! r@ ! r> ;

: unpair ( c -- a d ) dup @ swap cell+ @ ;

: pair? ( c -- f ) inside? ;

: set-car! ( a c -- ) ! ;
: set-cdr! ( d c -- ) cell+ ! ;

previous previous
