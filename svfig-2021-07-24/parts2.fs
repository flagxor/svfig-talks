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

: named ( pt "name" -- ) create dup , size allot
                         does> dup cell+ swap @ part! ;

part stack-part
  cell var kind
  cell var start
  cell var items
: stack ( n t "name" )
   stack-part named lastxt execute
   kind ! items !  here start !
   kind @ size items @ * allot
   start @ kind @ part! ;
: stack+! ( n -- ) kind @ size * kind @ part+! ;
: stack-s ( -- a n ) kind @ part@ kind @ size ;
: pop ( st -- ) -1 stack+! ;
: push ( st -- ) stack-s 2dup + swap cmove   1 stack+! ;

part point
  3 cell tuple x y z
: p! ( x y z ) z ! y ! x ! ;
: p?   x @ . y @ . z @ . ;

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

