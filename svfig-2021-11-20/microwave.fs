#! /usr/bin/env gforth

0 WARNINGS !

: square ( n -- n^2 ) dup * ;
: sqrt ( n -- n^1/2 ) 1 30 0 do 2dup / + 2/ loop nip ;
: -! ( n a -- ) swap negate swap +! ;

: um ;
: mm 1000 * ;
: cm 10 mm * ;

: signoformat ( n -- n d ) s>d swap over dabs ;
: n. ( n -- ) signoformat <# #s rot sign #> type ;
: mm. ( n -- ) signoformat <# # # # [char] . hold #s rot sign #> type ;

: nonfan ." M107 ; fan off" cr ;
: fan ." M106 S255 ; fan on" cr ;
: temperature
   ( temp -- )  ." M104 S" dup n. ."  ; set temperature to " n. cr ;
: temperatured
   ( temp -- ) ." M109 S" dup n. ."  ; wait for temperature " n. cr ;
: bed ( temp -- ) ." M190 S" dup n. ."  ; set bed temperature to " n. cr ;
: millimeters ." M21 ; set units to millimeters" cr ;
: absocoordinate ." G90 ; use absolute coordinates" cr ;
: absodistinate ." M82 ; use absolute distances for extrusion" cr ;
: home ." G28 ; home all axes" cr ;
: nonmotor ." M84 ; disable motors" cr ;
: speed ( s -- ) ." G1 F" dup n. ."  ; set speed " n. ." mm/min" cr ;

variable x   variable y   variable z   variable e
variable flow  30 flow !

: dist ( x y -- n ) y @ - square swap x @ - square + sqrt ;
: absolute ( x y z -- )
   z ! y ! x ! ." G1 X" x @ mm. ."  Y" y @ mm. ."  Z" z @ mm. cr ;
: reextrude ." G92 E0 ; set zero position (E=extruder)" cr  0 e ! ;
: extrude ( e -- ) e +! ." G1 E" e @ mm. cr ;
: penup ." G1 Z" z @ 1 mm + mm. ."  E" e @ 1200 - mm. cr ;
: pendown ." G1 Z" z @ mm. ."  E" e @ mm. cr ;
: moveto ( x y -- )
    2dup dist 600 um > dup >r if penup then
    y ! x ! ." G1 X" x @ mm. ."  Y" y @ mm. cr
    r> if pendown then ;
: lineto ( x y -- )
    2dup dist flow @ 1000 */ e +!
    y ! x ! ." G1 X" x @ mm. ."  Y" y @ mm. ."  E" e @ mm. cr ;
: lineby ( x y -- ) y @ + swap x @ + swap lineto ;
: syncup ." G1 Z" z @ mm. cr reextrude ;
: up z +! syncup ;

: boilerplate
   nonfan 60 bed 200 temperature home
   5000 speed 0 0 5000 absolute cr
   200 temperatured
   millimeters absocoordinate absodistinate cr
   reextrude 9000 speed 0 0 300 absolute
   2400 speed reextrude
   600 speed ;
: terminus 9000 speed nonfan 0 temperature home nonmotor cr ;
: box dup 0 lineby dup 0 swap lineby negate dup 0 lineby 0 swap lineby ;

: inverse ( n -- n ) negate ;
: union ( a b -- n ) min ;
: intersection ( a b -- n ) max ;
: difference ( a b -- n ) inverse intersection ;
: sphere ( r -- n ) x @ square y @ square + z @ square + sqrt swap - ;
: prism ( x y z -- n )
   z @ abs swap - swap y @ abs swap - max swap x @ abs swap - max ;
: cylinder ( h r -- n )
   x @ square y @ square + sqrt swap - swap z @ abs swap - max ;

: translate ( x y z -- ) z -! y -! x -! ;
: scale ( x y z -- )
   z @ swap 1000 */ z !
   y @ swap 1000 */ y !
   x @ swap 1000 */ x ! ;
: uscale ( s -- ) dup dup scale ;

create geostack 300 cells allot
variable geosp  geostack geosp !
: gunipush geosp @ !   cell geosp +! ;
: gunipop cell geosp -!   geosp @ @ ;
: gpush x @ gunipush y @ gunipush z @ gunipush ;
: gpop gunipop z ! gunipop y ! gunipop x ! ;

5 cm constant sweepspan
sweepspan negate constant -sweepspan
300 um constant sweepstep
sweepstep negate constant -sweepstep
: sweeprange sweepspan -sweepspan ;
: sweepvrange sweepspan 2* 600 um ;
sweeprange - sweepstep / 1+ constant slicelength
slicelength square constant slicearea
: posify sweepstep * sweepspan - ;

create slice slicearea allot
: clearslice slice slicearea 0 fill ;
: pointify slice - slicelength /mod posify swap posify ;

create floodstack slicelength 100 * cells allot
variable floodsp   floodstack floodsp !
: flooddepth floodsp @ floodstack - cell / ;
: unflooded floodsp @ floodstack <> ;
: floodpush floodsp @ ! cell floodsp +! ;
: floodpop cell floodsp -! floodsp @ @ ;

variable sweepmodel
variable samplepos
: addsample samplepos @ c! 1 samplepos +! ;
: probemodel gpush sweepmodel @ execute 0< gpop addsample ;
: sampleyslice sweeprange do i x ! probemodel sweepstep +loop ;
: sampleslice
   clearslice slice samplepos !
   sweeprange do i y ! sampleyslice sweepstep +loop ;

variable floodorder
: toggleorder floodorder @ invert floodorder ! ;

: horiflood ( pos -- pos )
   dup 1- floodpush
   dup 1+ floodpush
   dup slicelength - floodpush
   dup slicelength + floodpush ;
: vertiflood ( pos -- pos )
   dup slicelength - floodpush
   dup slicelength + floodpush
   dup 1- floodpush
   dup 1+ floodpush ;

: flooddrain
   begin unflooded while
     floodpop
     dup c@ if
       0 over c!
       floodorder @ if horiflood else vertiflood then
       pointify
       2dup dist 500 um > if 9000 speed moveto 600 speed else lineto then
     else drop then 
   repeat
;
: floodpoint dup c@ if floodpush flooddrain else drop then ;
: floodslice slicearea 0 do i slice + floodpoint loop ;
: doslice gpush sampleslice gpop toggleorder floodslice ;
: sweep sweepvrange do i z ! syncup doslice sweepstep +loop ;

: generate
   boilerplate
   ( surround box ) -sweepspan -sweepspan moveto    sweepspan 2* box
   300 um z ! syncup doslice
   fan
   sweep
   terminus
;

: model
   gpush
       6 mm -7500 um 1500 um - 1 mm translate
       4 mm 3 mm 1 mm prism
   gpop
   gpush
       -6 mm -7500 um 1500 um - 1 mm translate
       4 mm 3 mm 1 mm prism
   gpop
   union
   gpush
       0 1500 um 4 mm translate
       6500 um 9 mm 4 mm prism
   gpop
   union
   gpush
       0 7500 um 10 mm - 5 mm translate
       5 mm 3500 um cylinder
   gpop
   difference
;

' model sweepmodel !
generate

bye
