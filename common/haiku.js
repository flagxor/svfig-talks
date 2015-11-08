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
