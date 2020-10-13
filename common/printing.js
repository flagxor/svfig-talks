var reveal_js = '../reveal.js';
var link = document.createElement('link');
link.rel = 'stylesheet';
link.type = 'text/css';
link.href = window.location.search.match( /print-pdf/gi ) ?
    reveal_js + '/css/print/pdf.css' : '../common/paper.css';
document.getElementsByTagName( 'head' )[0].appendChild( link );

