#! /usr/bin/env python

import re
import sys

if len(sys.argv) != 2:
  print 'Usage: ./ten.py <src>'
  sys.exit(1)

data = open(sys.argv[1]).read()

data = re.sub('[\n\r\t ]+', ' ', data)
words = [i for i in data.split(' ') if i != '']
tally = {}
for word in words:
  if word not in tally:
    tally[word] = 0
  tally[word] += 1

for word in tally.keys():
  print tally[word], word
