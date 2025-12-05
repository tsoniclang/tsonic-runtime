# Tsonic.JSRuntime

JavaScript/TypeScript runtime implementation for the Tsonic compiler - provides exact JavaScript semantics in C#.

## Overview

Tsonic.JSRuntime is a C# library that implements JavaScript semantics, enabling TypeScript code compiled by Tsonic to behave exactly like it would in a JavaScript runtime. This includes:

- **JavaScript Arrays** - Sparse arrays with JS semantics (length, holes, etc.)
- **String manipulation** - JS string methods and behavior
- **Type coercion** - Automatic type conversions matching JS rules
- **Structural typing** - Duck typing and object shape compatibility
- **Console API** - `console.log`, `console.error`, etc.
- **JSON support** - `JSON.stringify`, `JSON.parse`
- **Math** - JavaScript Math object (abs, floor, ceil, round, random, etc.)
- **Global functions** - `parseInt`, `parseFloat`, `encodeURIComponent`, etc.
- **Map/Set** - ES6 Map and Set collections
- **WeakMap/WeakSet** - Weak reference collections
- **ArrayBuffer** - Fixed-length binary data buffer
- **Typed Arrays** - Int8Array, Uint8Array, Uint8ClampedArray, Int16Array, Uint16Array, Int32Array, Uint32Array, Float32Array, Float64Array
- **Date** - Date object with timezone support
- **RegExp** - Regular expression support with JavaScript flags

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

All 500+ tests verify that the runtime matches JavaScript behavior exactly.

## NativeAOT Compatibility

This library is fully compatible with .NET NativeAOT, enabling TypeScript code to be compiled to native executables with zero runtime dependencies and fast startup times.

## Package

Published as `Tsonic.JSRuntime` on NuGet.

## License

MIT License - see LICENSE file for details.
