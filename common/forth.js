(function() {

function ForthLanguage(hljs) {
  var WORD_RE = '[^ \t\r\n]+';
  return {
    name: 'Forth',
    aliases: ['forth'],
    lexemes: WORD_RE,
    case_insensitive: true,
    contains: [
      hljs.COMMENT(/\( /, /\)/, {relevance: 0}),
      hljs.COMMENT(/\\/, /$/, {relevance: 0}),
      {
        className: 'variable',
        begin: /(constant|variable|create|value)/,
        end: /\s[^\s]+/,
      },
      {
        className: 'function',
        begin: /[^ \t\n\r]*: /,
        end: /\s/,
      },
      {
        className: 'function',
        begin: ':noname',
        end: /\s/,
      },
      {
        className: 'params',
        begin: /[{]/,
        end: /[}]/,
      },
      {
        className: 'string',
        begin: /\[char\]\s/,
        end: /\s/,
      },
      {
        className: 'string',
        begin: /[^\s]*["] /,
        end: '["\n\r]',
      },
    ],
  };
}

document.addEventListener('DOMContentLoaded', (event) => {
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
    highlight: {
      highlightOnLoad: false,
    },
  });

  var hljs = Reveal.getPlugin('highlight').hljs;

  if (window.RevealHighlight !== undefined) {
    hljs.registerLanguage('Forth', ForthLanguage);
  }
  document.querySelectorAll('pre code').forEach((block) => {
    for (var i = 0; i < block.classList.length; ++i) {
      if (block.classList.item(i).startsWith('language-') ||
          block.classList.item(i).startsWith('lang-') ||
          block.classList.item(i).startsWith('python')) {
        break;
      }
    }
    if (i == block.classList.length) {
      block.classList.add('language-forth');
    }
    hljs.highlightBlock(block);
  });
});

})();
