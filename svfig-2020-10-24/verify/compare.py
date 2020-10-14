#! /usr/bin/env python

pi1 = open('pi_ref.txt').read().replace('\n', '')
pi2 = open('pi_me.txt').read().replace('\n', '')

print('base 16')
print('-------')
print('%d (ref) vs %d (ref)' % (len(pi1), len(pi2)))
m = min(len(pi1), len(pi2))
print('match=%s' % str(pi1[:m] == pi2[:m]))

print('')

pi1 = open('pi_ref10.txt').read().replace('\n', '')
pi2 = open('pi_me10.txt').read().replace('\n', '')

print('base 10')
print('-------')
print('%d (ref) vs %d (ref)' % (len(pi1), len(pi2)))
m = min(len(pi1), len(pi2))
print('match=%s' % str(pi1[:m] == pi2[:m]))
