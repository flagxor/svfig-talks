#! /usr/bin/env ueforth

: digit. ( n -- )
  dup 0= if drop exit then
  dup 1 = if ." one" drop exit then
  dup 2 = if ." two" drop exit then
  dup 3 = if ." three" drop exit then
  dup 4 = if ." four" drop exit then
  dup 5 = if ." five" drop exit then
  dup 6 = if ." six" drop exit then
  dup 7 = if ." seven" drop exit then
  dup 8 = if ." eight" drop exit then
  dup 9 = if ." nine" drop exit then
  dup 10 = if ." ten" drop exit then
  dup 11 = if ." eleven" drop exit then
  dup 12 = if ." twelve" drop exit then
  1 throw
;

: stem. ( n -- )
  dup 2 = if ." twen" drop exit then
  dup 3 = if ." thir" drop exit then
  dup 5 = if ." fif" drop exit then
  dup 8 = if ." eigh" drop exit then
  digit.
;

: ?ty ( n -- n ) dup 0= if ." ty" else ." ty-" then ;
 
: raw-number. ( n -- )
  dup 13 < if digit. exit then
  dup 20 < if 10 - stem. ." teen" exit then
  dup 100 < if 10 /mod stem. ?ty digit. exit then
  dup 1000 < if 100 /mod recurse ."  hundred " recurse exit then
  dup 1000000 < if 1000 /mod recurse ."  thousand " recurse exit then
  dup 1000000000 < if 1000000 /mod recurse ."  million " recurse exit then
  dup 1000000000000 < if 1000000000 /mod recurse ."  billion " recurse exit then
  1 throw
;

: number. ( n -- )  
  dup 0< if 1 throw then
  dup 0= if ." zero" drop exit then
  raw-number.
;

: test 1000000 0 do i i . space space number. cr depth 0<> throw loop ;
test
bye
