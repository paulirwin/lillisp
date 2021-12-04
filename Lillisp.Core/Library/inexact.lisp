
(define (exp z) (Math/Pow Math/E z))
(define (sin z) (Math/Sin z))
(define (cos z) (Math/Cos z))
(define (tan z) (Math/Tan z))
(define (asin z) (Math/Asin z))
(define (acos z) (Math/Acos z))
(define atan (lambda z (if (= (length z) 2) (Math/Atan2 (car z) (cadr z)) (Math/Atan (car z)))))