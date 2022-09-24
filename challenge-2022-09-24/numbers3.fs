#! /usr/bin/env ueforth

: numbers 11 for create next ; numbers
one two three four five six seven eight nine ten eleven twelve
: digit. ( n -- )
  dup 0= if drop exit then
  12 swap - ['] twelve swap for aft >link then next >name type ;

: stem. ( n -- )
  case
    2 of ." twen" endof
    3 of ." thir" endof
    5 of ." fif" endof
    8 of ." eigh" endof
    dup digit.
  endcase
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
  dup 0< throw
  dup 0= if ." zero" drop exit then
  raw-number.
;

0 value length
: pad-type ( a n -- ) pad length + swap dup +to length cmove ;
: {{   0 to length ['] pad-type is type ;
: }}   ['] default-type is type pad length ;
posix z" system" 1 sysfunc system
: }}system   }} over + 0 swap c! system drop ;
: number-say ( n -- ) {{ ." espeak -z '" number. ." '" }}system 300 ms ;

: test 1000000 0 do
    i . space space
    i number.
    i number-say cr
  loop
;
