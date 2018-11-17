'use strict';

const VOICE_URL = 'https://voice.flagxor.com/voiceforth/voicecheck';
//const VOICE_URL = 'http://localhost:8000/voicecheck';

function httpPost(url, args) {
  return new Promise((resolve, reject) => {
    const request = new XMLHttpRequest();
    request.timeout = 30000;
    request.onreadystatechange = function() {
      if (request.readyState === 4) {
        if (request.status === 200) {
          resolve(request.responseText);
        } else {
          reject();
        }
      }
    };
    request.open('POST', url);
    request.send(args);
  });
}

function voiceCheck() {
  httpPost(VOICE_URL, 'passwd=' + passwd).then((data) => {
    var cc1 = document.getElementById('cc1');
    var cc2 = document.getElementById('cc2');
    if (data == 'p') {
      cc1.innerText = '';
      cc2.innerText = '';
      Reveal.prev();
    } else if (data == 'n') {
      cc1.innerText = '';
      cc2.innerText = '';
      Reveal.next();
    } else if (data.substr(0, 1) == 'g') {
      cc1.innerText = '';
      cc2.innerText = '';
      Reveal.slide(parseInt(data.substr(1)));
    } else if (data.substr(0, 1) == 'i') {
      cc1.innerText = data.substr(1);
    } else if (data.substr(0, 1) == 'o') {
      cc2.innerText = data.substr(1);
    }
    voiceCheck();
  }).catch(() => {
    voiceCheck();
  });
}

voiceCheck();

