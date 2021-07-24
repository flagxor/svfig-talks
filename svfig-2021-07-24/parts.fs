#! /usr/bin/env gforth

variable @part
: part ( "name" ) create here @part ! 0 , 0 , ;
: var ( sz "name" ) create @part @ , @part @ cell+ @ ,
                    @part @ cell+ +!  
                    does> dup @ @ swap cell+ @ + ;
: tuple ( n sz "name" ) swap 1- for dup var next drop ;
: part@ ( pt -- pt ) @ ;   : part! ( a pt -- ) ! ;
: part+! ( n a -- ) dup part@ rot + swap part! ;
: size ( a -- ) cell+ @ ;

: singleton ( pt -- ) here over size allot swap part! ;

: stack ( n t "name" )
   create 2dup , , 0 , here over part! size * allot ;
: pop ( st -- ) dup @ size negate swap @ part+! ;
: push ( st -- ) @ dup part@ over size dup >r over + r> cmove
                 dup size swap part+! ;

part point
  3 cell tuple x y z
: p! ( x y z ) z ! y ! x ! ;
: p?   x @ . y @ . z @ . ;

\ point singleton

30 point stack pstack
: pdup   pstack push ;
: pdrop   pstack pop ;
: p.   p? pdrop ;
: p+   x @ y @ z @ pdrop z +! y +! x +! ;

1 2 3 p! pdup
2 3 4 p! pdup
5 6 7 p! p+
p. cr
p. cr

