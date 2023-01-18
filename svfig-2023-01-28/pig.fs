#! /usr/bin/env ueforth

( Shared buffer )

: between? ( ch a b ) >r over r> <= >r >= r> and ;
: upper? ( ch -- f ) [char] A [char] Z between? ;
: lower? ( ch -- f ) [char] a [char] z between? ;
: letter? ( ch -- f ) dup upper? swap lower? or ;
: cupper ( ch -- ch ) dup lower? if [char] a - [char] A + then ;
: clower ( ch -- ch ) dup upper? if [char] A - [char] a + then ;

: contains? ( ch a n -- f )
  1- for dup c@ >r over r> = if rdrop 2drop -1 exit then 1+ next 2drop 0 ;
: vowely? ( ch -- f ) clower s" aeiouy" contains? ;
: consonant? ( ch -- f ) clower s" bcdfghjklmnpqrstvwxz" contains? ;

1000 constant capacity
create buffer capacity allot
create alt-buffer capacity allot
variable slot  buffer slot !

: clear   buffer slot ! ;
: +buffer ( ch -- ) slot @ c!  1 slot +! ;
: full? ( -- f ) buffer slot @ <> ;
: buffer> ( -- a n ) buffer slot @ buffer - ;
: shift-buffer   buffer> >r dup 1+ swap r> cmove  -1 slot +! ;

( Pig Latin )

: ltype ( a n -- ) for aft dup c@ clower emit 1+ then next drop ;
: buffer.   buffer> ltype ;

variable stemmed
: capital? ( -- f ) full? buffer c@ upper? and ;
: capvowel ( ch -- ch ) stemmed @ 0= capital? and if cupper then ;
: +consonant ( ch -- ) stemmed @ if emit else +buffer then ;

: finish   full? if buffer. ." ay" clear
                 else stemmed @ if ." way" then then
           0 stemmed ! ;
: pig1 ( ch -- ) dup vowely? if capvowel emit -1 stemmed ! exit then
                 dup consonant? if +consonant exit then
                 finish emit ;
: pig   begin key dup 0< if bye then pig1 again ;

( Dictionary handling )

s" /usr/share/dict/words" r/o open-file throw value dict-words
dict-words file-size throw constant dict-size
dict-size 1+ allocate throw value dict-data
dict-data dict-size dict-words read-file throw drop
0 dict-data dict-size + c!  ( null terminate )

: next-word ( a -- a ) begin 1+ dup c@ nl = until 1+ ;
: each-step ( a -- a' a n ) dup next-word swap over over - 1- ;
: each{   postpone begin postpone dup postpone c@
          postpone while postpone each-step ; immediate
: }each   postpone repeat postpone drop ; immediate

: ?nonproper? ( a n -- a n f ) over c@ lower? ;
: nonpropers{   postpone each{ postpone ?nonproper? postpone if ; immediate
: }nonpropers   postpone else postpone 2drop postpone then
                postpone }each ; immediate

: case= ( a n a n -- f )
   >r swap r@ <> if rdrop 2drop 0 exit then r>
   for aft
     2dup c@ clower swap c@ clower <> if rdrop 2drop 0 exit then
     1+ swap 1+ swap
   then next 2drop -1 ;
: word? ( -- f ) dict-data each{ buffer> case= if drop -1 exit then }each 0 ;
: word-w? ( -- f )
   dict-data each{ buffer> 1- 0 max case= if drop -1 exit then }each 0 ;
: nonproper? ( -- f )
   dict-data nonpropers{ buffer> case= if drop -1 exit then }nonpropers 0 ;
: nonproper-w? ( -- f )
   dict-data nonpropers{ buffer> 1- 0 max case= if drop -1 exit then }nonpropers 0 ;

( Un-Pig Latin )

: drop-buffer   slot @ 1- buffer max slot ! ;
: 1rot-buffer   buffer> buffer 1+ swap cmove> buffer> + c@ buffer c! ;
: rot-buffer ( n -- ) for aft 1rot-buffer then next ;
: stash-buffer   buffer> alt-buffer swap cmove ;
: unstash-buffer   buffer> >r alt-buffer swap r> cmove ;

: capupper.   buffer c@ cupper emit  shift-buffer ;
: w? ( -- f ) full? buffer buffer> + 1- max c@ [char] w = and ;

: clusters: begin create latestxt >name nip 5 > until ;
vocabulary clusters sealed  also clusters definitions
clusters: bl br ch chl chr cl cr cs cz dh dj dr dw fj fl fr gh gl gn gr
          kh kl kn kr ks ll ls mn ph phl phr pl pn pr ps pt qu rh
          sc sch schl schm schn schr scht schw scl scr sh shl shr sht
          sk sl sm sn sp sph spl spr squ sr st str sv sw th thr thw tr
          ts tw tz wh wr zw xxxxxxxx
only forth definitions
: cluster? ( a n -- f )
   dup 1 = >r over c@ consonant? r> and if 2drop -1 exit then
   clusters find forth ;

: lastn ( n -- a n ) buffer> nip min >r buffer> r> dup >r - + r> ;
: try-rot ( xt n -- f )
   stash-buffer rot-buffer execute dup 0= if unstash-buffer then ;
: try-cluster-rot ( xt n -- f )
   dup lastn cluster? if try-rot else 2drop 0 then ;
: try-rots ( -- f )
   2 4 do ['] word? i try-rot if unloop -1 exit then -1 +loop 0 ;
: try-clusters-rot { xt -- f }
   1 4 do xt i try-cluster-rot if unloop -1 exit then -1 +loop 0 ;
: rot-stem  w? if nonproper-w? if drop-buffer exit then then
            ['] nonproper? try-clusters-rot if exit then
            w? if word-w? if drop-buffer exit then then
            ['] word? try-clusters-rot if exit then
            try-rots if exit then
            w? if drop-buffer else 1 rot-buffer then ;
: unvent   full? if capital? >r drop-buffer drop-buffer rot-stem
                    r> if capupper. then buffer. clear then ;
: unpig1 ( ch -- ) dup letter? if +buffer exit then unvent emit ;

: unpig   begin key dup 0< if bye then unpig1 again ;

: usage   ." USAGE: pig.fs [-r]" cr 1 terminate ;
: run
   argc 3 > if usage then
   argc 3 = if 2 argv s" -r" str= 0= if usage then unpig exit then
   pig
;
run
