#!/usr/bin/env racket
#lang racket/base

(define (forth source)
  (define (literal value)
    (lambda (sp rp k) (k (cons value sp) rp)))

  (define (find dictionary word)
    (cond ((null? dictionary) (literal word))
          ((eq? word (caar dictionary)) (cdar dictionary))
          (else (find (cdr dictionary) word))))

  (define (uniop op)
    (lambda (sp rp k) (k (cons (op (car sp)) (cdr sp)) rp)))
  (define (binop op)
    (lambda (sp rp k) (k (cons (op (cadr sp) (car sp)) (cddr sp)) rp)))

  (define (repeat n op sp rp k)
    (if (= n 0)
        (k sp rp)
        (op sp rp (lambda (nsp nrp) (repeat (- n 1) op nsp nrp k)))))

  (define root-dictionary (list
    (cons 'dup (lambda (sp rp k) (k (cons (car sp) sp) rp)))
    (cons 'drop (lambda (sp rp k) (k (cdr sp) rp)))
    (cons 'swap (lambda (sp rp k) (k (cons (cadr sp) (cons (car sp) (cddr sp))) rp)))
    (cons '>r (lambda (sp rp k) (k (cdr sp) (cons (car sp) rp))))
    (cons 'r> (lambda (sp rp k) (k (cons (car rp) sp) (cdr rp))))
    (cons '+ (binop +))
    (cons '- (binop -))
    (cons '* (binop *))
    (cons '/ (binop /))
    (cons 'and (binop bitwise-and))
    (cons 'or (binop bitwise-ior))
    (cons 'xor (binop bitwise-xor))
    (cons 'negate (uniop -))
    (cons 'execute (lambda (sp rp k) ((car sp) sp rp (lambda (nsp nrp) (k nsp nrp)))))
    (cons 'repeat (lambda (sp rp k) (repeat (cadr sp) (car sp) (cddr sp) rp k)))
    (cons 'print (lambda (sp rp k) (display (car sp)) (display " ") (k (cdr sp) rp)))
    (cons 'cr (lambda (sp rp k) (newline) (k sp rp)))
    (cons 'bye (lambda (sp rp k) (exit) (k sp rp)))
  ))

  (define (nop sp rp k) (k sp rp))
  (define (link before after)
    (lambda (sp rp k)
      (before sp rp (lambda (nsp nrp)
        (after nsp nrp k)))))

  (define (interpret dictionary source sp rp)
    (define (compile definition)
      (define (iter code words)
        (cond ((null? words) code)
              ((pair? (car words)) (iter (link code (literal (compile (car words)))) (cdr words)))
              (else (iter (link code (find dictionary (car words))) (cdr words)))))
      (iter nop definition))
    (define (colon name definition)
      (cons (cons name (compile definition)) dictionary))
    (cond ((null? source) '())
          ((pair? (car source)) (interpret (colon (caar source) (cadar source)) (cdr source) sp rp))
          (else ((find dictionary (car source)) sp rp (lambda (sp rp) (interpret dictionary (cdr source) sp rp))))))
  (interpret root-dictionary source '() '())
)

(forth '(
  (square ( dup * ))
  (test1 ( 0 11 ( dup print dup square print cr 1 + ) repeat drop ))
  test1
  bye
))

(exit)
