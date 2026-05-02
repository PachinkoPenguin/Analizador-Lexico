# Analizador Lexico - Proyecto de Compiladores

Este repositorio contiene dos implementaciones de analizador lexico para un lenguaje tipo Python/Triton:

1. Analizador basado en expresiones regulares (Regex).
2. Analizador manual basado en automata finito (maquina de estados).

Ademas incluye un archivo ZIP de entrega con el codigo fuente requerido.

## Objetivo

Comprender y aplicar la fase de analisis lexico dentro del proceso de traduccion de lenguajes, incluyendo:

- Reconocimiento de lexemas y generacion de tokens.
- Construccion de tabla de simbolos.
- Deteccion de errores lexicos comunes.

## Estructura Relevante

- `Lexical/Lexical/Program.cs`: Analizador lexico 1 (Regex).
- `Lexical/LexicalManual/Program.cs`: Analizador lexico 2 (manual por estados).
- `Lexical/LexicalManual/LexicalManual.csproj`: Proyecto del analizador manual.
- `Entrega_AnalizadoresLexicos.zip`: Entrega empaquetada con ambos analizadores.

## Que Se Hizo

### 1) Analizador Regex (Lexical/Lexical)

Implementacion usando una expresion regular compuesta con grupos nombrados.

Funciones principales:

- Definicion de patrones de token en una lista (`tokenPatterns`).
- Construccion de un regex global para escanear la entrada completa.
- Clasificacion de tokens por orden de prioridad.
- Reconocimiento de keywords (`if`, `else`, `while`, `def`, etc.).
- Generacion de tabla de simbolos para `NAME`, `NUMBER`, `STRING` y errores.

Tokens destacados:

- Literales: `STRING`, `NUMBER` (incluye cientifica).
- Identificadores: `ID` y conversion a `KEYWORD` cuando aplica.
- Operadores: `+ - * / %`, `//`, `**`, `==`, `!=`, `<=`, `>=`, `=`.
- Delimitadores: `() {} [] , : ;`.
- Error generico: `ERROR`.

Salida:

- Bloque `=== TOKENS ===`.
- Bloque `=== SYMBOL TABLE ===`.

### 2) Analizador Manual (Lexical/LexicalManual)

Implementacion sin regex, caracter por caracter, simulando un automata.

Funciones principales:

- `Analyze()`: ciclo principal de escaneo.
- `ScanString()`: lectura de cadenas con validacion de cierre.
- `ScanNumber()`: enteros, decimales, cientifica y deteccion de identificador invalido tipo `2bad`.
- `ScanIdentifierOrKeyword()`: diferencia `NAME` vs keyword.
- `ScanMultiCharOperator()`: operadores dobles (`==`, `<=`, `//`, `**`, etc.).
- `ScanSingleCharOperator()`: operadores y delimitadores simples.
- `PrintTokens()` y `PrintSymbolTable()` para salida legible.

Salida:

- Bloque `=== TOKENS ===` con linea/columna por token.
- Bloque `=== SYMBOL TABLE ===`.

## Como Correr Los Analizadores

## Requisitos

- .NET SDK instalado (proyectos en `net9.0`).
- En equipos con runtime mas nuevo (por ejemplo .NET 10), se recomienda usar `DOTNET_ROLL_FORWARD=Major`.

## Opcion A: Analizador Regex

Desde PowerShell, en la raiz del repositorio:

```powershell
cd "Lexical/Lexical"
dotnet build
$env:DOTNET_ROLL_FORWARD="Major"
"if x<=10: y=x**2;" | dotnet run
```

## Opcion B: Analizador Manual

Desde PowerShell, en la raiz del repositorio:

```powershell
cd "Lexical/LexicalManual"
dotnet build
$env:DOTNET_ROLL_FORWARD="Major"
"2bad = 7" | dotnet run
```

Tambien puedes ejecutar y escribir la entrada manualmente cuando el programa la solicite.

## Casos De Prueba Recomendados

1. Entrada valida con operadores compuestos:

```text
if x<=10: y=x**2;
```

2. Identificador invalido:

```text
2bad = 7
```

3. Cadena no terminada:

```text
msg = "hola
```

4. Simbolo desconocido:

```text
x = 10 @ 2
```

## Compilacion Sin Warnings

Ambos proyectos principales se dejaron compilando sin errores ni warnings:

- `Lexical/Lexical`
- `Lexical/LexicalManual`

## Entrega

El archivo de entrega preparado es:

- `Entrega_AnalizadoresLexicos.zip`

Incluye el codigo fuente de ambos analizadores lexicos.
