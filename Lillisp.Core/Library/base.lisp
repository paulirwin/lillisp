(define (caar x) (car (car x)))
(define (cadr x) (car (cdr x)))
(define (cdar x) (cdr (car x)))
(define (cddr x) (cdr (cdr x)))

(define (odd? x) (= (% x 2) 1))
(define (even? x) (= (% x 2) 0))
(define (positive? x) (> x 0))
(define (negative? x) (< x 0))
(define (zero? x) (= x 0))
(define (square x) (* x x))

(define list-tail
	(lambda (x k)
		(if (zero? k)
		x
		(list-tail (cdr x) (- k 1)))))
(define list-ref
	(lambda (x k) (car (list-tail x k))))