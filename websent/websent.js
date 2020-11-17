'use strict';

var FOREGROUND = '#000';
var BACKGROUND = '#fff';
var USABLE = 0.75;
var LINE_SPACING = 1.4;
var FONT = 'dejavu sans, roboto, ubuntu';
var WebSent;

(function() {

var screen;
var context;
var pages = [];
var activeSlide = 0;

function Load(data) {
  var lines = data.split('\n');
  var page = [];
  for (var i = 0; i < lines.length; ++i) {
    if (lines[i] == '') {
      pages.push(page);
      page = [];
    } else {
      page.push(lines[i]);
    }
  }
}

function Resize() {
  screen.width = window.innerWidth;
  screen.height = window.innerHeight;
  if (context === undefined) {
    context = screen.getContext('2d');
  }
  Goto(activeSlide);
}

function PickSize(width, height, page) {
  var line_frac = (LINE_SPACING * (page.length - 1)) + 1;
  var size = Math.floor(height / line_frac);
  var w = 0;
  for (var i = 0; i < page.length; ++i) {
    for (;;) {
      context.font = size + 'px ' + FONT;
      var rw = context.measureText(page[i]).width;
      if (rw <= width) {
        break;
      }
      --size; 
    }
    w = Math.max(w, rw);
  }
  var rsize = size * LINE_SPACING;
  return [w, size * line_frac, rsize];
}

function Goto(n) {
  activeSlide = n;

  context.fillStyle = BACKGROUND;
  context.fillRect(0, 0, screen.width, screen.height);

  context.fillStyle = FOREGROUND;
  context.textBaseline = 'top'
  var width = screen.width * USABLE;
  var height = screen.height * USABLE;
  var page = pages[activeSlide];
  var limit = Math.max(width, height);
  var dimensions = PickSize(width, height, page);
  dimensions[0] = (screen.width - dimensions[0]) / 2;
  dimensions[1] = (screen.height - dimensions[1]) / 2;
  for (var i = 0; i < page.length; ++i) {
    context.fillText(page[i], dimensions[0], dimensions[1]);
    dimensions[1] += dimensions[2];
  }
}

function Main(data) {
  Load(data);
  Resize();
}
WebSent = Main;

window.addEventListener('load', function() {
  screen = document.createElement('canvas');
  screen.style.position = 'absolute';
  document.body.appendChild(screen);
  document.body.style.border = '0';
  document.body.style.margin = '0';
  document.body.style.padding = '0';

  var pre = document.getElementsByTagName('pre');
  if (pre.length) {
    var data = pre[0].innerHTML;
    if (data[0] === '\n') {
      data = data.substr(1);
    }
    Main(data);
    pre[0].parentNode.removeChild(pre[0]);
  }
});

window.addEventListener('keydown', function(e) {
  if (e.keyCode === 37) {
    Goto(Math.max(0, activeSlide - 1));
  } else if (e.keyCode == 39) {
    Goto(Math.min(pages.length - 1, activeSlide + 1));
  }
});

window.addEventListener('resize', Resize);

})();
