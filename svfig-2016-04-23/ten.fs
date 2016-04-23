#! /usr/bin/env gforth

: usage   ." Usage: ten.fs <src>" cr bye ;
: usage-check   argc @ 2 <> if usage then ;
usage-check

( Split words )
variable front
variable back
variable on-word
: space? ( ch -- ) dup 32 = over 10 = or over 13 = or swap 8 = or ;
: this-word ( -- a n ) front @ back @ over - ;
: process-word   this-word dup if on-word @ execute else 2drop then ;
: chomp ( a -- ) back ! process-word back @ 1+ front ! ;
: process-char ( a -- ) dup c@ space? if chomp else drop then ;
: split-words ( a n -- )
   over front ! over + dup rot ?do i process-char loop chomp ;

variable hash-value
: 0hash   0 hash-value ! ;
: +hash1 ( n -- )
   hash-value @ + dup 10 lshift + dup 6 rshift xor hash-value !  ;
: +hash ( a n -- ) over + swap ?do i c@ +hash1 loop ;
: nhash ( n -- n ) hash-value @ swap mod ;

50000000 constant table-size
table-size cells  allocate throw constant word-table
word-table table-size cells 0 fill
: table-cell ( a n -- a ) 0hash +hash table-size nhash cells word-table + ;
: add-to-table ( a a n -- ) table-cell ! ;
: get-from-table ( a n -- a ) table-cell @ ;

variable word-list
: visit-list ( a op -- )
   begin over while dup >r over @ -rot execute r> repeat 2drop ;
: end-visit ( op a -- ) drop 0 ;
: add-word ( a n -- ) here >r word-list @ , , , 1 , r> word-list ! ;
: word>name ( a -- a n ) dup cell+ cell+ @ swap cell+ @ ;
: word>count ( a -- a ) 3 cells + ;
variable target   variable target-len
variable found
: match-word ( a -- )
   dup word>name target @ target-len @ compare 0=
   if found ! end-visit else drop then ;
: find-word ( a n -- )
   target-len ! target ! 0 found !
   target @ target-len @ get-from-table
   ['] match-word visit-list ;
: handle-word ( a n -- )
   2dup find-word found @ if 2drop 1 found @ word>count +!
   else 2dup add-word word-list @ -rot add-to-table then ;
' handle-word on-word !

1 arg slurp-file split-words

: dump-word   dup word>count @ . word>name type cr ;
word-list @ ' dump-word visit-list

bye
