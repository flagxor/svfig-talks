#! /usr/bin/env gforth

( Alternate version )

: reverse ( n -- n )
  0 swap begin base @ /mod >r swap base @ * + r> dup 0= until drop ;
: palindrome? ( n -- f ) dup reverse = ;
: hdpalindrome? ( n -- f ) dup hex palindrome? swap decimal palindrome? and ;
: pal# 0 100000 0 do i palindrome? if i . 1+ then loop ;
pal# cr ." Total: " . cr
: hdpal# 0 100000 0 do i hdpalindrome? if i . 1+ then loop ;
hdpal# cr ." Total: " . cr
bye
