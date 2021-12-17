#! /usr/bin/env gforth

200 constant places
create digits   places allot
: 1digits!   digits places 0 fill   1 digits c! ;
: digits.   places digits + begin 1- dup c@ 0<> until
  begin dup c@ [char] 0 + emit 1- dup digits 1- = until drop ;
: *+ ( a n c -- a+1 n c' )
  >r over c@ over * r> + 10 /mod ( a n d c' )
  >r >r over r> swap c! r> ( a n c' ) 
  rot 1+ -rot ;
: digits* ( n -- ) digits swap 0 places 1- for *+ next 2drop drop ;
: factorial ( n -- ) 1digits! 1 swap 1- for dup digits* 1+ next drop ;
." COMPUTED: " 100 factorial digits. cr
." EXPECTED: 93326215443944152681699238856266700490715968264381621468592963895217599993229915608941463976156518286253697920827223758251185210916864000000000000000000000000" cr
bye
