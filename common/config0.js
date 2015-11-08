Reveal.initialize({
  controls: true,
  progress: true,
  history: true,
  center: true,

  transition: Reveal.getQueryHash().transition || 'default',

  // Optional reveal.js plugins
  dependencies: [
  { src: reveal_js + '/lib/js/classList.js', condition: function() { return !document.body.classList; } },
  { src: reveal_js + '/plugin/markdown/marked.js', condition: function() { return !!document.querySelector( '[data-markdown]' ); } },
  { src: reveal_js + '/plugin/markdown/markdown.js', condition: function() { return !!document.querySelector( '[data-markdown]' ); } },
  { src: reveal_js + '/plugin/highlight/highlight.js', async: true, condition: function() { return !!document.querySelector( 'pre code' ); }, callback: function() { hljs.initHighlightingOnLoad(); } },
  { src: reveal_js + '/plugin/zoom-js/zoom.js', async: true },
  { src: reveal_js + '/plugin/notes/notes.js', async: true }
  ]
});


