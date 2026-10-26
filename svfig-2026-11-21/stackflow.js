"use strict";

function ProcessOne(container) {
  var steps = container.dataset.steps.split(" ");
  if (container.dataset.steps == "") {
    steps = [];
  }
  var labels = container.dataset.labels.split(" ");

  const svgNS = "http://www.w3.org/2000/svg";

  const svg = document.createElementNS(svgNS, "svg");
  svg.setAttribute("width", 100 + 80 * steps.length);
  svg.setAttribute("height", 100 + labels.length * 80);
  svg.setAttribute("viewBox", "0 0 " + (100 + 80 * steps.length) + " " + (100 + labels.length * 80));
  svg.style.fontFamily = "sans-serif";

  const style = document.createElementNS(svgNS, 'style');
  style.textContent = `@import url('stackflow.css');`;
  svg.appendChild(style);

  for (var i = 0; i < labels.length; ++i) {
    const text = document.createElementNS(svgNS, "text");
    text.setAttribute("x", 20);
    text.setAttribute("y", 80 * (i + 1));
    text.setAttribute("class", "label-text");
    text.textContent = i == 0 ? "0 Top" : "" + i;
    svg.append(text);
  }

  for (var i = 0; i < labels.length; ++i) {
    const line = document.createElementNS(svgNS, "line");
    line.setAttribute("x1", 60);
    line.setAttribute("y1", 80 * (i + 1));
    line.setAttribute("x2", 120 + 80 * steps.length);
    line.setAttribute("y2", 80 * (i + 1));
    line.setAttribute("class", "track");
    svg.append(line);
  }

  var old_labels = [];
  for (var j = 0; j < steps.length + 1; ++j) {
    for (var i = 0; i < labels.length; ++i) {
      if (labels[i] == old_labels[i]) {
        continue;
      }
      old_labels[i] = labels[i];
      const text = document.createElementNS(svgNS, "text");
      text.setAttribute("x", 80 + j * 80);
      text.setAttribute("y", 70 + 80 * i);
      text.setAttribute("class", "value-text");
      text.textContent = labels[i];
      svg.append(text);
    }
    if (j >= steps.length) { continue; }

    var op = steps[j].substr(0, steps[j].length - 1);
    var index = parseInt(steps[j].substr(steps[j].length - 1, 1));

    var height = 120;
    if (op == "SWAP") {
      var t = labels[index];
      old_labels[index] = "";
      old_labels[index + 1] = "";
      labels[index] = labels[index + 1];
      labels[index + 1] = t;
    } else if (op == "DUP") {
      for (var i = labels.length - 1; i > index; --i) {
        old_labels[i - 1] = "";
        old_labels[i] = "";
        labels[i] = labels[i - 1];
      }
      height = 40 + 80 * (labels.length - index - 1);
    } else if (op == "DROP") {
      for (var i = index; i < labels.length - 1; ++i) {
        old_labels[i] = "";
        old_labels[i + 1] = "";
        labels[i] = labels[i + 1];
      }
      labels[labels.length - 1] = "?";
      height = 40 + 80 * (labels.length - index - 1);
    }

    const box = document.createElementNS(svgNS, "g");
    box.setAttribute("transform", "translate(" + (100 + 80 * j) + ", " + (60 + 80 * index) + ")");
    const rect = document.createElementNS(svgNS, "rect");
    rect.setAttribute("width", 50);
    rect.setAttribute("height", height);
    var colorMap = {"SWAP": "node-blue", "DUP": "node-gold", "DROP": "node-red"};
    rect.setAttribute("class", colorMap[op]);
    box.append(rect);
    if (op == "SWAP") {
      var line = document.createElementNS(svgNS, "line");
      line.setAttribute("x1", 0);
      line.setAttribute("y1", 20);
      line.setAttribute("x2", 50);
      line.setAttribute("y2", 100);
      line.setAttribute("class", "inside-track");
      box.append(line);
      var line = document.createElementNS(svgNS, "line");
      line.setAttribute("x1", 50);
      line.setAttribute("y1", 20);
      line.setAttribute("x2", 0);
      line.setAttribute("y2", 100);
      line.setAttribute("class", "inside-track");
      box.append(line);
    } else if (op == "DUP") {
      for (var i = 0; i < labels.length - index; ++i) {
        var line = document.createElementNS(svgNS, "line");
        line.setAttribute("x1", 0);
        line.setAttribute("y1", i == 0 ? 20 : i * 80 - 60);
        line.setAttribute("x2", 50);
        line.setAttribute("y2", i * 80 + 20);
        line.setAttribute("class", "inside-track");
        box.append(line);
      }
    } else if (op == "DROP") {
      for (var i = 0; i < labels.length - index - 1; ++i) {
        var line = document.createElementNS(svgNS, "line");
        line.setAttribute("x1", 0);
        line.setAttribute("y1", (i + 1) * 80 + 20);
        line.setAttribute("x2", 50);
        line.setAttribute("y2", i * 80 + 20);
        line.setAttribute("class", "inside-track");
        box.append(line);
      }
    }
    const text = document.createElementNS(svgNS, "text");
    text.setAttribute("x", 25);
    text.setAttribute("y", height / 2);
    text.setAttribute("class", "node-text");
    text.textContent = op;
    const subscript = document.createElementNS(svgNS, "tspan");
    subscript.setAttribute("class", "subscript");
    subscript.textContent = index;
    text.append(subscript);
    box.append(text);
    svg.append(box);
  }

  container.appendChild(svg);
}

function ProcessAll() {
  var diagrams = document.getElementsByClassName("stackflow");
  for (var i = 0; i < diagrams.length; ++i) {
    ProcessOne(diagrams[i]);
  }
}

ProcessAll();
