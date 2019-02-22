: cons ( a b -- c ) noname create swap , , latestxt ;
: car ( c -- a ) execute @ ;
: cdr ( c -- b ) execute cell+ @ ;

