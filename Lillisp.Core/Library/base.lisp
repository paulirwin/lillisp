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

(define member
	(lambda (obj list . compare)
		(let ((predicate (if (> (length compare) 0) (car compare) equal?)))
			(if (eq? list '()) #f
				(if (predicate obj (car list)) list (member obj (cdr list) predicate))))))
(define memq (lambda (obj list) (member obj list eq?)))
(define memv (lambda (obj list) (member obj list eqv?)))

(define assoc
	(lambda (obj alist . compare)
		(let ((predicate (if (> (length compare) 0) (car compare) equal?)))
			(if (eq? alist '()) #f
				(if (predicate obj (caar alist)) (car alist) (assoc obj (cdr alist) predicate))))))
(define assq (lambda (obj alist) (assoc obj alist eq?)))
(define assv (lambda (obj alist) (assoc obj alist eqv?)))

#| this doesn't currently work...
(define-syntax case-lambda
	(syntax-rules ()
		((case-lambda (params body0 ...) ...)
			(lambda args
				(let ((len (length args)))
					(letrec-syntax
						((cl (syntax-rules ::: ()
							((cl)
							 (error "no matching clause"))
							((cl ((p :::) . body) . rest)
							 (if (= len (length '(p :::)))
								(apply (lambda (p :::)
											. body)
										args)
								(cl . rest)))
						((cl ((p ::: . tail) . body)
							. rest)
						 (if (>= len (length '(p :::)))
							(apply
							 (lambda (p ::: . tail)
								. body)
							 args)
							(cl . rest))))))
						(cl (params body0 ...) ...))))))) |#