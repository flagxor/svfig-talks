'use strict';

var FOREGROUND = '#000';
var BACKGROUND = '#fff';
var USABLE = 0.75;
var LINE_SPACING = 1.4;
var FONT = 'dejavu sans, roboto, ubuntu';
var WebSent;

(function() {

var slides = [];
var activeSlide = -1;
var activeElement = null;
var holder = null;
var touchStartX = 0;
var touchLastX = 0;
var refresh = false;

function Load(data) {
  var lines = data.split('\n');
  var pages = [];
  var page = [];
  for (var i = 0; i < lines.length; ++i) {
    if (lines[i] == '') {
      pages.push(page);
      page = [];
    } else {
      page.push(lines[i]);
    }
  }
  if (page.length) {
    pages.push(page);
  }
  // Setup overall document.
  document.title = pages[0][0];
  document.body.style.backgroundColor = BACKGROUND;
  holder = document.createElement('div');
  holder.style.backgroundColor = BACKGROUND;
  holder.style.color = FOREGROUND;
  holder.style.fontFamily = FONT;
  holder.style.margin = '0';
  holder.style.border = '0';
  holder.style.padding = '0';
  holder.width = '100%';
  holder.height = '100%';
  document.body.appendChild(holder);
  // Process into slides.
  for (var i = 0; i < pages.length; ++i) {
    if (pages[i].length && pages[i][0][0] == '@') {
      var element = document.createElement('img');
      element.src = pages[i][0].substr(1);
      element.style.imageRendering = 'crisp-edges';
    } else {
      var element = document.createElement('pre');
      element.innerHTML = pages[i].join('\n');
      element.style.lineHeight = LINE_SPACING * 100 + '%';
    }
    element.style.position = 'absolute';
    element.style.display = 'inline';
    element.style.margin = '0';
    element.style.border = '0';
    element.style.padding = '0';
    slides.push(element);
  }
}

function AddMeta(name, content) {
  var meta = document.createElement('meta');
  meta.name = name;
  meta.content = content;
  document.head.appendChild(meta);
}

function Resize() {
  refresh = true;
  Goto(activeSlide);
}

function Goto(n) {
  n = Math.max(0, Math.min(n, slides.length - 1));
  if (n === activeSlide && !refresh) {
    return;
  }
  activeSlide = n;
  refresh = false;

  window.location.hash = n;
  var width = window.innerWidth * USABLE;
  var height = window.innerHeight * USABLE;

  if (activeElement === null) {
    holder.appendChild(slides[n]);
  } else {
    holder.replaceChild(slides[n], activeElement);
  }
  activeElement = slides[n];
  var e = activeElement;

  var min = 1;
  var max = window.innerHeight;
  for (;;) {
    if (min >= max) {
      break;
    }
    var mid = Math.floor((min + max + 1) / 2);
    e.style.fontSize = mid + 'px';
    e.height = mid;
    if (e.offsetWidth < width && e.offsetHeight < height) {
      min = mid;
    } else {
      max = mid - 1;
    }
  }
  --min;
  e.style.fontSize = min + 'px';
  e.height = mid;
  e.style.left = (window.innerWidth - e.offsetWidth)/ 2 + 'px';
  e.style.top = (window.innerHeight - e.offsetHeight)/ 2 + 'px';
}

window.addEventListener('load', function() {
  AddMeta('apple-mobile-web-app-capable', 'yes');
  AddMeta('apple-mobile-web-app-status-bar-style', 'black-translucent');
  AddMeta('viewport', 'width=device-width, initial-scale=1.0, ' +
                      'maximum-scale=1.0, user-scalable=no, minimal-ui');
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
  if (event.ctrlKey || event.shiftKey || event.altKey || event.metaKey) {
    return;
  }
  if ([36].indexOf(e.keyCode) >= 0) {  // home
    Goto(0);
  } else if ([35].indexOf(e.keyCode) >= 0) {  // end
    Goto(slides.length - 1);
  } else if ([37, 72, 75, 37, 38, 80, 33, 8].indexOf(e.keyCode) >= 0) {
    Goto(activeSlide - 1);
  } else if ([39, 13, 40, 74, 76, 34, 78, 32].indexOf(e.keyCode) >= 0) {
    Goto(activeSlide + 1);
  } else if ([70].indexOf(e.keyCode) >= 0) {  // fullscreen
    holder.requestFullscreen();
  }
  e.preventDefault();
});

window.addEventListener('touchstart', function(e) {
  if (e.targetTouches.length > 1) {
    holder.requestFullscreen();
  }
  touchStartX = e.targetTouches[0].screenX;
  touchLastX = touchStartX;
  e.preventDefault();
});

window.addEventListener('touchmove', function(e) {
  touchLastX = e.targetTouches[0].screenX;
  e.preventDefault();
});

window.addEventListener('touchend', function(e) {
  var delta = touchLastX - touchStartX;
  if (delta < -screen.width / 8) {
    Goto(activeSlide + 1);
  } else if (delta > screen.width / 8) {
    Goto(activeSlide - 1);
  }
  e.preventDefault();
});

function GotoHash() {
  Goto(0 | location.hash.match(/\d+/));
}

window.addEventListener('popstate', function(e) {
  GotoHash();
});

window.addEventListener('resize', Resize);

function Main(data) {
  Load(data);
  GotoHash();
}
WebSent = Main;

})();
