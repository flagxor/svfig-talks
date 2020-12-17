#! /usr/bin/env gforth

needs pairs.fs

( Symbols and Pairs )
: atom   create latest , ;
: atom>string ( x -- a n ) @ name>string ;
: atom. ( x -- ) atom>string type space ;

( Reinterpret Floats and Numbers )
variable ftemp
: f->n ftemp f! ftemp @ ;
: n->f ftemp ! ftemp f@ ;

( Utility )
: private[[   get-order wordlist swap 1+ set-order definitions ;
: ]]private   previous definitions ;
: fsquare   fdup f* ;

( Type Tags )
: attach-tag swap cons ;
: type-tag car ;
: contents cdr ;

( Type Table )
variable table
: put ( item op type -- )
   cons swap cons table @ cons table ! ;
: equiv ( a b -- f ) 2dup car swap car = -rot cdr swap cdr = and ;
: get ( op type -- item )
   cons table @ begin dup while
     2dup car car equiv if nip car cdr exit then 
     cdr repeat -1 throw ;

( Applying generic ops )
: apply-generic ( .. op -- .. )
   over type-tag get swap contents swap execute ;
: apply-generic2 ( .. op -- .. )
   over type-tag get rot contents rot contents rot execute ;

( Generic Complex Ops )
atom 'real-part   atom 'imag-part
atom 'magnitude   atom 'angle
atom 'make-from-real-imag   atom 'make-from-mag-ang
atom 'rectangular   atom 'polar
: real-part 'real-part apply-generic ;
: imag-part 'imag-part apply-generic ;
: magnitude 'magnitude apply-generic ;
: angle 'angle apply-generic ;
: rect>z 'make-from-real-imag 'rectangular get execute ;
: polar>z 'make-from-mag-ang 'polar get execute ;

( Complex Math )
: z+ ( z1 z2 -- z )
   2dup real-part real-part f+
   imag-part imag-part f+ rect>z ;
: z- ( z1 z2 -- z )
   2dup real-part real-part fswap f-
   imag-part imag-part fswap f- rect>z ;
: z* ( z1 z2 -- z )
   2dup magnitude magnitude f*
   angle angle f+ polar>z ;
: z/ ( z1 z2 -- z )
   2dup magnitude magnitude fswap f/
   angle angle fswap f- polar>z ;
: zsquare ( z -- z2 ) dup z* ;
: z. ( z -- )
   ." ( " dup real-part f. ." + i * " imag-part f. ." ) " ;
: zp. ( z -- )
   ." ( " dup magnitude f. ." * e^ ( i * " angle f. ." ) ) " ;

( Rectangular Complex Numbers )
private[[
: real-part car n->f ;
: imag-part cdr n->f ;
: magnitude
   dup real-part fsquare
   imag-part fsquare f+ fsqrt ;
: angle
   dup imag-part
   real-part fatan2 ;
: make-from-real-imag
   f->n f->n swap cons 'rectangular attach-tag ;

' real-part 'real-part 'rectangular put
' imag-part 'imag-part 'rectangular put
' magnitude 'magnitude 'rectangular put
' angle 'angle 'rectangular put
' make-from-real-imag 'make-from-real-imag 'rectangular put
]]private

( Polar Complex Numbers )
private[[
: magnitude car n->f ;
: angle cdr n->f ;
: real-part dup magnitude angle fcos f* ;
: imag-part dup magnitude angle fsin f* ;
: make-from-mag-ang
  f->n f->n swap cons 'polar attach-tag ;

' real-part 'real-part 'polar put
' imag-part 'imag-part 'polar put
' magnitude 'magnitude 'polar put
' angle 'angle 'polar put
' make-from-mag-ang 'make-from-mag-ang 'polar put
]]private

( Generic Ops )
atom '+   atom '-   atom '*   atom '/   atom '.   atom 'make
atom 'number   atom 'float   atom 'complex
atom 'rational   atom 'polynomial
: g+ '+ apply-generic2 ;
: g- '- apply-generic2 ;
: g* '* apply-generic2 ;
: g/ '/ apply-generic2 ;
: g. '. apply-generic ;
: make-number 'make 'number get execute ;
: make-float 'make 'float get execute ;
: make-complex 'make 'complex get execute ;
: make-rect-complex rect>z make-complex ;
: make-polar-complex polar>z make-complex ;
: make-rational 'make 'rational get execute ;
: make-poly 'make 'polynomial get execute ;

( Simple Numbers )
private[[
: tag 'number attach-tag ;
: add + tag ;
: sub - tag ;
: mul * tag ;
: div / tag ;
: print . ;
: make tag ;

' add '+ 'number put
' sub '- 'number put
' mul '* 'number put
' div '/ 'number put
' print '. 'number put
' make 'make 'number put
]]private

( Simple Floats )
private[[
: tag 'float attach-tag ;
: add f+ tag ;
: sub f- tag ;
: mul f* tag ;
: div f/ tag ;
: print n->f f. ;
: make f->n tag ;

' add '+ 'float put
' sub '- 'float put
' mul '* 'float put
' div '/ 'float put
' print '. 'float put
' make 'make 'float put
]]private

( Complex )
private[[
: tag 'complex attach-tag ;
: add z+ tag ;
: sub z- tag ;
: mul z* tag ;
: div z/ tag ;
: print z. ;
: make tag ;

' add '+ 'complex put
' sub '- 'complex put
' mul '* 'complex put
' div '/ 'complex put
' print '. 'complex put
' make 'make 'complex put
]]private

( Rational )
private[[
: gcd ( a b -- n ) dup 0= if drop else swap over mod recurse then ;
: reduce ( a b -- a' b' ) 2dup gcd swap over / -rot / swap ;
: tag 'rational attach-tag ;
: numer car ;
: denom cdr ;
: make-rat reduce cons tag ;
: add 2dup numer swap denom * >r
      2dup denom swap numer * r> + -rot
      denom swap denom * make-rat ;
: sub 2dup numer swap denom * >r
      2dup denom swap numer * r> - -rot
      denom swap denom * make-rat ;
: mul 2dup numer swap numer * -rot
      denom swap denom * make-rat ;
: div 2dup denom swap numer * -rot
      numer swap denom * make-rat ;
: print ." ( " dup numer . ." / " denom . ." ) " ;
: make make-rat ;

' add '+ 'rational put
' sub '- 'rational put
' mul '* 'rational put
' div '/ 'rational put
' print '. 'rational put
' make 'make 'rational put
]]private

( Polynomial )
private[[
: make-poly cons ;
: variable car ;
: term-list cdr ;

: make-term cons ;
: coeff car ;
: order cdr ;
: +terms ( L1 L2 -- L )
   dup null? if drop exit then
   over null? if nip exit then
   2dup car swap car swap
   2dup order swap order swap
   2dup > if 2drop drop -rot swap cdr recurse cons exit then
   < if nip -rot cdr recurse cons exit then
   dup order -rot coeff swap coeff g+ swap make-term
   -rot cdr swap cdr recurse cons
;
: add-poly   dup variable -rot term-list swap term-list +terms make-poly ;
: t*terms ( t L -- L )
   dup null? if nip exit then
   over swap ( t t L )
   dup >r car 2dup order swap order +
   -rot coeff swap coeff g* swap make-term
   swap r> cdr recurse cons
;
: *terms ( L1 L2 -- L )
   dup null? if nip exit then
   2dup car swap t*terms
   -rot cdr recurse +terms
;
: mul-poly   dup variable -rot term-list swap term-list *terms make-poly ;
: print ." [ "
  dup variable swap term-list
  begin dup null? 0= while
    dup car coeff g. ." " over atom. ." ^" dup car order .
    cdr dup null? 0= if ." + " then
  repeat
  2drop ." ] "
;
: tag 'polynomial attach-tag ;
: add add-poly tag ;
: mul mul-poly tag ;
: make make-poly tag ;

' add '+ 'polynomial put
' mul '* 'polynomial put
' print '. 'polynomial put
' make 'make 'polynomial put
]]private

