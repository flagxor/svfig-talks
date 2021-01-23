( Use strong random numbers! )
s" /dev/urandom" r/o open-file throw constant dev-random
: rnd   0 >r rp@ cell dev-random read-file throw cell <> throw r> ;
: rmask ( n -- n ) 1 begin 2dup > while 2* 1+ repeat nip ;
: random ( n -- nr ) dup >r rmask begin rnd over and dup r@ < if rdrop and exit then drop again ;

: id. ( xt -- ) >name type space ;   : tab 9 emit ;
variable things   variable verbose  -1 verbose !
: thing create things @ , last @ , 1 things +! ;
: t. ( thing -- ) cell+ @ verbose @ if id. then ;
thing rock   thing paper   thing scissors

: play ( thing thing -- n ) @ swap @ swap - 1+ things @ mod 1- ;
: doline   pad 40 accept pad swap evaluate ;
: rps page ." Player A: " doline
      page ." Player B: " doline
      page ." Press enter to reveal..." key drop cr
      2dup swap t. t. play dup 0= if drop ." Tie!" exit then
      0< if ." Player B wins!" else ." Player A wins!" then ;

: spin ( xt -- thing ) verbose @ if dup id. then
   execute verbose @ if dup t. tab tab then ;
: round ( xt xt -- f ) spin swap spin play verbose @ if dup . cr then ;
: rounds ( xt xt n -- n )
   cr 0 swap 1- for >r 2dup round r> + next
   swap id. ." score: " . drop cr ;

: <odds ( r p s -- <r <p <s ) >r over + r> over + ;
: player ( r p s -- thing )
   <odds random rot >r dup r> < if 2drop rock exit then
   > if paper else scissors then ;

: Allen ( -- thing ) 50 25 25 player ;
: Betty ( -- thing ) 1 1 1 player ;
