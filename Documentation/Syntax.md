# Eslon Syntax
This document describes the Eslon syntax.

## Definitions
- Whitespace: (U+0020, U+00A0)
- Control character: (U+0000—U+001F, U+007F—U+009F)

## Element: Map
A collection of nodes.

### Syntax
1. A map is embraced by parentheses "()".
2. Nodes are separated by a comma ",".

### Postscript
1. Each key may only occur once.
2. Each key has a value attached.
3. Keys and values may not be empty.
4. Keys and values may not contain whitespace or control characters.
5. A trailing separator is prohibited.
6. Whitespace may surround any element.

### Figure 1
`(A=1,B=2,C=3)`

## Element: List
A collection of values.

### Syntax
1. A list is embraced by curly brackets "{}".
2. Values are separated by a comma ",".

### Postscript
1. A value may not be empty.
2. A value may not contain whitespace or control characters.
3. A trailing separator is prohibited.
4. Whitespace may surround any element.

### Figure 1
`{1,2,3}`

## Element: String
A sequence of characters.

### Syntax
1. (Embedded)
   1. An embedded string is delimited by double quotes (").
   2. Escape sequences are started by a backslash "\\".
2. (Verbatim)
   1. The string requires at least 1 character.
   2. The string may not contain whitespace or control characters.

### Postscript
1. (Embedded)
   1. Control characters, ("), and "\\" require an escape sequence.

### Figure 1: Embedded string
`"abc"`

### Figure 2: Verbatim string
`true`

### Figure 3: Escape sequences
```
\0 ........ U+0000 (NUL)
\a ........ U+0007 (BEL)
\b ........ U+0008 (BS)
\t ........ U+0009 (TAB)
\n ........ U+000A (LF)
\v ........ U+000B (VT)
\f ........ U+000C (FF)
\n ........ U+000D (CR)
\" ........ U+0022 (")
\\ ........ U+005C "\"
\x hh ..... (hexadecimal notation)
\u hhhh ... (hexadecimal notation)
```

## Element: Null
Flags the absence of an object, borrowed from runtime glossary.

### Syntax
1. A null value is denoted by a question mark "?".

### Figure 1
`?`