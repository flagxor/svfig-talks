#! /usr/bin/env ueforth

: upper ( ch -- CH ) 95 and ;
: vowel? ( ch -- f )
  upper dup [char] A = over [char] E = or over [char] I = or
        over [char] O = or over [char] U = or swap [char] Y = or ;
: vowel# ( a n -- n ) 0 swap 0 do over i + c@ vowel? if 1+ then loop nip ;
: stem ( a n -- )
  2dup vowel# 0= if type exit then
  2dup vowel# 2 < if type space exit then
  dup 5 <= if
    2dup vowel# over 2 + 2/ >= if type space exit then
  then
  2dup 2/ recurse
  dup dup 2/ - >r 2/ + r> recurse
;
: test bl parse 2dup type space space stem cr ;

test big
test robot
test caterpillar
test sundial
test fungible

test general
test Robert
test tangential
test presidential
test robotic

test fight
test valley
test tangled
test troubled
test loose

test pill
test seen
test language
test syllable
test greenery

test tragedy
test comedy
test playbill
test endurance
test nightly

bye
