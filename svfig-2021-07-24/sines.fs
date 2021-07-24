#! /usr/bin/env gforth

          0.0000000000000000000000000000000000000 2constant ZERO
( sin36 = 0.5877852522924731291687059546390727685976524376431459910722724807 )
          0.5877852522924731291687059546390727686 2constant SIN36
( sin72 = 0.9510565162951535721164393333793821434056986341257502224473056444 )
          0.9510565162951535721164393333793821434 2constant SIN72
          1.0000000000000000000000000000000000000 2constant ONE
SIN36 DNEGATE 2constant -SIN36
SIN72 DNEGATE 2constant -SIN72

3.1415926535897932384626433832795028842 2constant dPI

          1000000000000000000 constant sONE
( sin1 = 0.0174524064372835128194189785163161924722527203071396426836124276 )
          0017452406437283513 constant SIN1
( cos1 = 0.9998476951563912391570115588139148516927403105831859396583207145 )
          0999847695156391239 constant COS1

create ref-sines
  ZERO 2,  SIN36 2,  SIN72 2,  SIN72 2,  SIN36 2,
  ZERO 2, -SIN36 2, -SIN72 2, -SIN72 2, -SIN36 2, ZERO 2,
: ref-sin ( n -- d ) 2* cells ref-sines + 2@ ;

: SIN1* ( n -- n ) SIN1 sONE */ ;
: COS1* ( n -- n ) COS1 sONE */ ;

( sin[a+1] = sin[a]*cos[1]+cos[a]*sin[1] )
( cos[a+1] = cos[a]*cos[1]-sin[a]*sin[1] )
: sines-step ( sin cos -- sin+1 cos+1 )
   2dup COS1* swap SIN1* - >r SIN1* swap COS1* + r> ;

: fill-sines   0 sONE 179 for over , sines-step next ;
create sines   fill-sines

: sin ( n -- d ) 180 /mod swap cells sines + @
                 swap 2 mod if negate then ONE rot sONE m*/ ;

: deg>rad ( n -- d ) dPI rot 1 * 180 m*/ ;

: deg. ( n -- ) s>d <# # # #s #> type ;
: un. ( d -- ) <# 36 for # next [char] . hold #s #> type ;
: n. ( d -- ) 2dup d0< if [char] - emit dnegate else space then un. ;
: sinerow. ( n -- )
   dup 36 * deg.         ."  ~ "
   dup 36 * deg>rad n.   ."  : "
   dup ref-sin n.        ."    "
   dup ref-sin rot 36 * sin d- dabs n. cr
   dup 49 spaces 36 * sin n. cr ;
: dashes ( n -- ) 1- for [char] - emit next ;
: center ( a n w -- ) over - 2 /mod dup spaces + >r type r> spaces ;
: gap   4 spaces ;
: column ( a n ) gap 39 center ;  : dcolumn   gap 39 dashes ;
: split   3 dashes dcolumn dcolumn dcolumn cr ;
: sines. ." DEG" s" RAD" column s" REF-SIN  vs  COMP-SIN" column
         s" DIFF" column cr split
         0 10 for dup sinerow. 1+ next drop ;

sines. bye
