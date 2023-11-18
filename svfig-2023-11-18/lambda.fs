needs boehm.fs

vocabulary closures
also internals
also closures definitions

0 value identifier
: allot-id ( -- n ) identifier 1 +to identifier ;

0 value environment
: scope-clear   0 to environment
                1001 to identifier
                0 scope !
                locals-area locals-here ! ;
scope-clear

: }? ( a n -- ) 1 <> if drop 0 exit then c@ [char] } = ;
: --? ( a n -- ) s" --" str= ;

: init-local! ( id -- ) pair environment swap pair to environment ;
: get-local ( id -- n )
   >r environment
   begin dup while
     unpair unpair
     r@ = if nip rdrop exit then
     drop
   repeat
   rdrop ( zero on stack )
;
: set-local! ( v id -- )
   >r environment
   begin dup while
     unpair dup unpair nip
     r@ = if nip set-car! rdrop exit then
     drop
   repeat
   rdrop ( zero on stack )
;

: compile-get-local ( id -- ) aliteral postpone get-local ;

: scope-create ( a n -- id )
   dup >r $place align ( name )
   scope @ , r> 8 lshift 1 or , ( IMMEDIATE ) here scope ! ( link, flags&length )
   ['] scope-clear @ ( docol ) ,
   allot-id dup aliteral postpone compile-get-local postpone exit
;

: (local) ( a n -- )
   dup 0= if 2drop exit then
   ?room <>locals scope-create <>locals
   aliteral postpone init-local! ;

: (locals)
   bl parse
   dup 0= if scope-clear -1 throw then
   2dup --? if 2drop [char] } parse 2drop exit then
   2dup }? if 2drop exit then
   recurse (local) ;

: callit ( code -- ) >r ;
: bind ( code -- a ) environment pair ;

: save-env   r> environment >r >r ;
: restore-env   r> r> to environment >r ;

also forth definitions also closures

: invoke ( a -- ) environment >r unpair to environment
                  callit r> to environment ;

: {   (locals) ; immediate

: [:   scope @ postpone ahead here ; immediate
: :]   postpone exit >r postpone then r>
       aliteral postpone bind scope ! ; immediate

: ;   scope-clear postpone ; ; immediate

: to ( n -- "name" ) ' 2 cells + @ aliteral
                     postpone set-local! ; immediate

: recurse   postpone save-env
            postpone recurse
            postpone restore-env ; immediate

previous previous previous previous
