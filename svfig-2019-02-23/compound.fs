( CircleForth - Compound words )

( Useful basic compound words )
: 2drop ( n n -- ) drop drop ;
: 2dup ( a b -- a b a b ) over over ;
: nip ( a b -- b ) swap drop ;
: rdrop ( r: n n -- ) r> r> drop >r ;
: */ ( n n n -- n ) */mod nip ;
: * ( n n -- n ) 1 */ ;
: /mod ( n n -- n n ) 1 swap */mod ;
: / ( n n -- n ) /mod nip ;
: mod ( n n -- n ) /mod drop ;
: invert ( n -- ~n ) -1 xor ;
: negate ( n -- -n ) invert 1 + ;
: - ( n n -- n ) negate + ;
: rot ( a b c -- c a b ) >r swap r> swap ;
: -rot ( a b c -- b c a ) swap >r swap r> ;
: cell+ ( n -- n ) cell + ;
: cells ( n -- n ) cell * ;
: < ( a b -- a<b ) - 0< ;
: > ( a b -- a>b ) swap - 0< ;
: = ( a b -- a!=b ) - 0= ;
: <> ( a b -- a!=b ) = 0= ;
: emit ( n -- ) >r rp@ 1 type rdrop ;
: bl 32 ;   : space bl emit ;
: nl 10 ;   : cr nl emit ;

( Compilation State )
: [ 0 state ! ; immediate
: ] -1 state ! ; immediate

( Quoting words )
: ' parse-name find ;
: ['] ' aliteral ; immediate
: char parse-name drop c@ ;
: [char] char aliteral ; immediate
: literal aliteral ; immediate

( Core Control Flow )
: begin here ; immediate
: again ['] branch , , ; immediate
: until ['] 0branch , , ; immediate
: ahead ['] branch , here 0 , ; immediate
: then here swap ! ; immediate
: if ['] 0branch , here 0 , ; immediate
: else ['] branch , here 0 , swap here swap ! ; immediate

( Compound words requiring conditionals )
: min 2dup < if drop else nip then ;
: max 2dup < if nip else drop then ;

( Postpone - done here so we have ['] and if )
: >flags 2 cells - @ ;
: immediate? >flags 1 and 1 - 0= ;
: postpone ' dup immediate? if , else aliteral ['] , , then ; immediate

( Counted Loops )
: do postpone swap postpone >r postpone >r here ; immediate
: i postpone r@ ; immediate
: unloop postpone rdrop postpone rdrop ; immediate
: +loop postpone r> postpone + postpone r>
        postpone 2dup postpone >r postpone >r
        postpone < postpone 0= postpone until
        postpone unloop ; immediate
: loop 1 aliteral postpone +loop ; immediate

( Constants and Variables )
: constant create , does> @ ;
: variable create 0 , ;

( Exceptions )
variable handler
: catch   sp@ >r handler @ >r rp@ handler ! execute r> handler ! r> drop 0 ;
: throw   handler @ rp! r> handler ! r> swap >r sp! drop r> ;

( Examine Dictionary )
: >link ( xt -- a ) 1 cells - @ ;   : >flags 2 cells - ;
: >name ( xt -- a n ) dup 3 cells - @ swap over - 3 cells - swap ;
: >body ( xt -- a ) cell+ ;
: see. ( xt -- ) >name type space ;
: exit= ( xt -- ) ['] exit = ;
: see-loop   >body begin dup @ see. cell+ dup @ exit= until ;
: see   cr ['] : see.  ' dup see.  see-loop drop  ['] ; see.  cr ;
