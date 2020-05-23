: 2dup over over ;

: fill ( a n c ) swap >r swap r> 0 do 2dup c! 1+ loop drop drop ;

: erase ( a n ) 0 fill ;

: cmove ( a a n ) 0 do 2dup swap c@ swap c! 1+ swap 1+ swap loop drop drop ;

vocabulary grf grf definitions

create shifts

128 c, 64 c, 32 c, 16 c, 8 c, 4 c, 2 c, 1 c,

: 1<<n shifts + c@ ;

1024 constant charset-size

8192 constant tiles

11264 constant vcharset

7547 constant rom-charset

create charset charset-size allot

32 variable ink

0 variable nib

: +! ( n a -- ) dup @ rot + swap ! ;

: ++ ( a -- ) 1 swap +! ;

: >chpos ( x y -- a ) 8 / 32 * swap 8 / + tiles + ;

: nib-pos nib @ 8 * ;

: nib-char nib-pos charset + ;

: nib-vchar nib-pos vcharset + ;

: blank-char ( a -- ) 8 erase ;

: nib-blank nib-char blank-char nib-vchar blank-char ;

: allot-ink ( a -- ) ink ++ ink @ dup nib ! nib-blank swap c! ;

: stencil ( x y ) >chpos dup c@ 32 = if allot-ink else drop then ;

: rowplot ( mask n ) dup >r nib-char + c@ or r> 2dup nib-char + c! nib-vchar + c! ;

: chplot ( x y ) swap 8 mod 1<<n swap 8 mod rowplot ;

: blank-nchar ( n -- ) 8 * vcharset + blank-char ;

: bank1 32 - 7 * rom-charset + 7 ;

: bank2 64 - 6 * 32 7 * + 1- rom-charset + 6 ;

: bank3 96 - 7 * 32 13 * + rom-charset + 7 ;

: rom-nchar ( n -- a n ) dup 63 < if bank1 exit then dup 95 < if bank2 exit then bank3 ;

: copy-nchar ( n -- ) dup rom-nchar >r swap 8 * vcharset + 1+ r> cmove ;

: reset-char ( n ) dup blank-nchar copy-nchar ;

forth definitions grf

: gplot ( x y ) 191 swap - 2dup stencil chplot ;

: greset 128 32 do i reset-char loop ;

forth

: test invis cls 150 0 do i i i * 150 / gplot loop ;

