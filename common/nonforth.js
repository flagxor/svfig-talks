(function() {
  var plugins = [];
  if (window.RevealHighlight !== undefined) {
    plugins.push(window.RevealHighlight);
  }
  if (window.RevealMath !== undefined) {
    plugins.push(window.RevealMath);
  }
  Reveal.initialize({
    history: true,
    plugins: plugins,
  });
})();
