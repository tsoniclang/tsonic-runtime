# Tsonic.Runtime

JavaScript/TypeScript runtime implementation for the Tsonic compiler - provides exact JavaScript semantics in C#.

## Overview

Tsonic.Runtime is a C# library that implements JavaScript semantics, enabling TypeScript code compiled by Tsonic to behave exactly like it would in a JavaScript runtime. This includes:

- **JavaScript Arrays** - Sparse arrays with JS semantics (length, holes, etc.)
- **String manipulation** - JS string methods and behavior
- **Type coercion** - Automatic type conversions matching JS rules
- **Structural typing** - Duck typing and object shape compatibility
- **Console API** - `console.log`, `console.error`, etc.
- **JSON support** - `JSON.stringify`, `JSON.parse`
- **Global functions** - `parseInt`, `parseFloat`, `encodeURIComponent`, etc.

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

All 300+ tests verify that the runtime matches JavaScript behavior exactly.

## NativeAOT Compatibility

This library is fully compatible with .NET NativeAOT, enabling TypeScript code to be compiled to native executables with zero runtime dependencies and fast startup times.

## Package

Published as `Tsonic.Runtime` on NuGet.

## License

MIT License - see LICENSE file for details.
