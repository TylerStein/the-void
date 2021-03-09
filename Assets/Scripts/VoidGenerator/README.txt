# Lexicon Rules

Lexicon rules are defined with strings to cover various scenarios

## Symbols

*		All
>		Forward transition check (If left side satisfied, then right side allowed)
<		Backward transition check (If left side satisfied, then right side allowed)
|		Or operator

## Examples
For example, let's say we have Beat defenitions with the handles A through Z

* < A > B	A can transition to B, anything can come before A
A > B		A can transition to B, anything can come before A (the * is implied by the lack of a < operator)
A > *		A can transition to anything, from anything
B < A > C	A can transition to C if it came from B
C > A | D	C can transition to A or D
