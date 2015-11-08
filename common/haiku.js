/***************************************/
/* Inject Haikus                       */
/***************************************/

function fixup(i) {
  var url = 'https://forthsalon.appspot.com/haiku-bare/';
  url += i.getAttribute('data-haiku');
  var zoom = parseFloat(i.parentElement.parentElement.style.zoom);
  url += '?width=' + Math.ceil(i.clientWidth * zoom) +
    '&height=' + Math.ceil(i.clientHeight * zoom);
  if (i.src !== url) {
    i.src = url;
  }
}

setInterval(function() {
  var haikus = document.getElementsByName('haiku');
  for (var i = 0; i < haikus.length; i++) {
    fixup(haikus[i]);
  }
}, 500);


/***************************************/
/* Attempt to support touch pad        */
/***************************************/

var haiku_touch_port = null;
var haiku_touch_buffer = '';

function connect_touch() {
  try {
    haiku_touch_port = chrome.runtime.connect(
        'gjpkfjbomndhibbiiakfjpgjcaeggbic');
    haiku_touch_port.onMessage.addListener(function(m) {
      haiku_touch_buffer += m.data.replace(/[\r]/g, '');
      var parts = haiku_touch_buffer.split('\n');
      if (parts.length > 1) {
        for (var i = 0; i < parts.length - 1; i++) {
          if (window.onstroke !== undefined &&
              parts[i].length !== 0) {
            console.log('stroke: ' + parts[i]);
            try {
              window.onstroke(parts[i]);
            } catch(e) {
            }
          }
        }
        haiku_touch_buffer = parts[parts.length - 1];
      }
    });
  } catch(e) {
  }
}

function send_to_touch(s) {
  if (haiku_touch_port === null) {
    return;
  }
  try {
    haiku_touch_port.postMessage({'data': s});
  } catch(e) {
  }
}

window.addEventListener('load', connect_touch);


/***************************************/
/* Attempt to support touch pad        */
/***************************************/
window.onstroke = function(s) {
  if (s === 'R') {
    Reveal.prev();
  } else if (s === 'S') {
    Reveal.next();
  } else if (s === 'F') {
    Reveal.left();
  } else if (s === 'T') {
    Reveal.right();
  } else if (s === 'L') {
    Reveal.up();
  } else if (s === 'P') {
    Reveal.down();
  }
};
