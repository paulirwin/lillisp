(define (call-with-input-file file proc) (call-with-port (open-input-file file) proc))
(define (call-with-output-file file proc) (call-with-port (open-output-file file) proc))