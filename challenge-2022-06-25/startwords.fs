#! /usr/bin/env ueforth

s" /usr/share/dict/words" r/o open-file throw value dict-words
dict-words file-size throw constant dict-size
dict-size 1+ allocate throw value dict-data
dict-data dict-size dict-words read-file throw drop
0 dict-data dict-size + c!  ( null terminate )

: upper ( ch -- ch) 95 and ;
: upper*s ( a n -- ) 0 do dup c@ upper over c! 1+ loop drop ;
dict-data dict-size upper*s

: next-word ( a -- a ) begin 1+ dup c@ nl = until 1+ ;
: each-step ( a -- a' a n ) dup next-word swap over over - 1- ;
: each{   postpone begin postpone dup postpone c@
          postpone while postpone each-step ; immediate
: }each   postpone repeat postpone drop ; immediate

: letter? ( ch -- f ) dup [char] A >= swap [char] Z <= and ;
: letters? ( a n ) -1 swap 0 do >r dup c@ letter? >r 1+ r> r> and loop nip ;

: wordl? ( a n -- f ) 5 = if 5 letters? else drop 0 then ;

0 value wordls#
create wordls 10000 cells allot
: init-wordls
  dict-data each{
    2dup wordl? if
      drop wordls# cells wordls + !
      1 +to wordls#
    else
      2drop
    then
  }each
;
init-wordls

: score1 { a b }
  0
  5 0 do a i + c@ b i + c@ = 2* 2* - loop
  5 0 do 5 0 do a i + c@ b j + c@ = - loop loop
  5 0 do 5 0 do a i + c@ a j + c@ = + loop loop
;

: score { a }
  0 wordls# 0 do
    a i cells wordls + @ score1 +
  loop
;

: run
  wordls# 0 do
    i cells wordls + @ score .
    i cells wordls + @ 5 type cr
  loop
;
run bye
