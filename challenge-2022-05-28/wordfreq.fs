#! /usr/bin/env ueforth

s" /usr/share/dict/words" r/o open-file throw value dict-words
dict-words file-size throw constant dict-size
dict-size 1+ allocate throw value dict-data
dict-data dict-size dict-words read-file throw drop
0 dict-data dict-size + c!  ( null terminate )

: next-word ( a -- a ) begin 1+ dup c@ nl = until 1+ ;
: each-step ( a -- a' a n ) dup next-word swap over over - 1- ;
: each{   postpone begin postpone dup postpone c@
          postpone while postpone each-step ; immediate
: }each   postpone repeat ; immediate

create letters 256 cells allot
letters 256 cells 0 fill
0 value total

: letter+ ( ch -- ) cells letters + 1 swap +! ;
: letter@ ( ch -- n ) cells letters + @ ;
: init-frequencies
  dict-data each{
    0 ?do dup i + c@ letter+ 1 +to total loop drop 
  }each
;

: n. ( n -- )
  10000 total */ <# # # # # [char] . hold #s #> type ;

: table.
  [char] z 1+ [char] a do
    i emit space i letter@ n. cr
  loop
;

: letter? ( ch -- f ) dup [char] a >= swap [char] z <= and ;
: letters? ( a n ) -1 swap 0 do >r dup c@ letter? >r 1+ r> r> and loop nip ;

: word-freq ( a n -- n ) 
  0 -rot 0 do dup i + c@ letter@ rot + swap loop drop ;
: run
  init-frequencies
  table.
  cr
  dict-data each{
    dup 5 = >r 2dup letters? r> and if
      2dup type space
      word-freq n. cr
    else
      2drop
    then
  }each
;
run bye
