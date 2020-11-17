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
var image;
var pages = [];
var activeSlide = 0 | location.hash.match(/\d+/);
var touchStartX = 0;
var touchLastX = 0;

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
  document.title = pages[0][0];
}

function AddMeta(name, content) {
  var meta = document.createElement('meta');
  meta.name = name;
  meta.content = content;
  document.head.appendChild(meta);
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
  window.location.hash = activeSlide;

  var width = screen.width * USABLE;
  var height = screen.height * USABLE;

  context.fillStyle = BACKGROUND;
  context.fillRect(0, 0, screen.width, screen.height);

  var page = pages[activeSlide];
  if (page.length && page[0][0] == '@') {
    image.src = page[0].substr(1);
    var altw = image.width * height / image.height;
    var alth = image.height * width / image.width;
    var w = width, h = height;
    if (altw * height < width * alth) {
      w = altw;
    } else {
      h = alth;
    }
    var x = (screen.width - w) / 2;
    var y = (screen.height - h) / 2;
    image.onload = function() {
      context.drawImage(image, x, y, w, h);
    };
  } else {
    context.fillStyle = FOREGROUND;
    context.textBaseline = 'top'
    var limit = Math.max(width, height);
    var dimensions = PickSize(width, height, page);
    dimensions[0] = (screen.width - dimensions[0]) / 2;
    dimensions[1] = (screen.height - dimensions[1]) / 2;
    for (var i = 0; i < page.length; ++i) {
      context.fillText(page[i], dimensions[0], dimensions[1]);
      dimensions[1] += dimensions[2];
    }
  }
}

function Main(data) {
  Load(data);
  Resize();
}
WebSent = Main;

window.addEventListener('load', function() {
  AddMeta('apple-mobile-web-app-capable', 'yes');
  AddMeta('apple-mobile-web-app-status-bar-style', 'black-translucent');
  AddMeta('viewport', 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no, minimal-ui');

  screen = document.createElement('canvas');
  screen.style.position = 'absolute';
  document.body.appendChild(screen);
  document.body.style.border = '0';
  document.body.style.margin = '0';
  document.body.style.padding = '0';

  image = document.createElement('img');
  image.style.display = 'none';

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

function Back() {
  Goto(Math.max(0, activeSlide - 1));
}

function Next() {
  Goto(Math.min(pages.length - 1, activeSlide + 1));
}

window.addEventListener('keydown', function(e) {
  if ([36].indexOf(e.keyCode) >= 0) {  // home
    Goto(0);
  } else if ([35].indexOf(e.keyCode) >= 0) {  // end
    Goto(pages.length - 1);
  } else if ([37, 72, 75, 37, 38, 80, 33, 8].indexOf(e.keyCode) >= 0) {
    Back();
  } else if ([39, 13, 40, 74, 76, 34, 78, 32].indexOf(e.keyCode) >= 0) {
    Next();
  } else if ([70].indexOf(e.keyCode) >= 0) {  // fullscreen
    screen.requestFullscreen();
  }
});

window.addEventListener('mousedown', function(e) {
  if (event.which === 1) {
    Next();
  } else {
    Back();
  }
});

window.addEventListener('touchstart', function(e) {
  touchStartX = e.targetTouches[0].screenX;
  touchLastX = touchStartX;
});

window.addEventListener('touchmove', function(e) {
  touchLastX = e.targetTouches[0].screenX;
});

window.addEventListener('touchend', function(e) {
  var delta = touchLastX - touchStartX;
  if (delta < -screen.width / 8) {
    Next();
  } else if (delta > screen.width / 8) {
    Back(); 
  }
});

window.addEventListener('popstate', function(e) {
  Goto(0 | location.hash.match(/\d+/));
});

window.addEventListener('resize', Resize);

})();
