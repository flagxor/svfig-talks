'use strict';

const VOICE_URL = 'https://voice.flagxor.com/voiceforth/voicecheck';
//const VOICE_URL = 'http://localhost:8000/voicecheck';

function httpPost(url) {
  return new Promise((resolve, reject) => {
    const request = new XMLHttpRequest();
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
    request.send();
  });
}

function voiceCheck() {
  setTimeout(voiceCheck, 500);
  httpPost(VOICE_URL).then((data) => {
    if (data == 'p') {
      Reveal.prev();
    } else if (data == 'n') {
      Reveal.next();
    } else if (data.substr(0, 1) == 'g') {
      Reveal.slide(parseInt(data.substr(1)));
    }
  });
}

voiceCheck();

