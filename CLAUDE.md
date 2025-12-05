# CLAUDE.md

This file provides guidance to Claude Code when working with the Tsonic.JSRuntime project.

## Critical Guidelines

### NEVER ACT WITHOUT EXPLICIT USER APPROVAL

**YOU MUST ALWAYS ASK FOR PERMISSION BEFORE:**

- Making architectural decisions or changes
- Implementing new features or functionality
- Modifying runtime semantics or JavaScript behavior implementations
- Changing bindings format or adding new globals
- Adding new dependencies or packages
- Making changes to type coercion, operators, or structural typing

**ONLY make changes AFTER the user explicitly approves.** When you identify issues or potential improvements, explain them clearly and wait for the user's decision. Do NOT assume what the user wants or make "helpful" changes without permission.

### ANSWER QUESTIONS AND STOP

**CRITICAL RULE**: If the user asks you a question - whether as part of a larger text or just the question itself - you MUST:

1. **Answer ONLY that question**
2. **STOP your response completely**
3. **DO NOT continue with any other tasks or implementation**
4. **DO NOT proceed with previous tasks**
5. **Wait for the user's next instruction**

This applies to ANY question, even if it seems like part of a larger task or discussion.

### NEVER USE AUTOMATED SCRIPTS FOR FIXES

**🚨 CRITICAL RULE: NEVER EVER attempt automated fixes via scripts or mass updates. 🚨**

- **NEVER** create scripts to automate replacements (PowerShell, bash, Python, etc.)
- **NEVER** use sed, awk, grep, or other text processing tools for bulk changes
- **NEVER** write code that modifies multiple files automatically
- **ALWAYS** make changes manually using the Edit tool
- **Even if there are hundreds of similar changes, do them ONE BY ONE**

Automated scripts break syntax in unpredictable ways and destroy codebases.

### WORKING DIRECTORIES

**IMPORTANT**: Never create temporary files in the project root or src directories. Use dedicated gitignored directories for different purposes.

#### .tests/ Directory (Test Output Capture)

**Purpose:** Save test run output for analysis without re-running tests

**Usage:**
```bash
# Create directory (gitignored)
mkdir -p .tests

# Run tests with tee - shows output AND saves to file
dotnet test | tee .tests/run-$(date +%s).txt

# Analyze saved output later without re-running:
grep "Failed" .tests/run-*.txt
tail -50 .tests/run-*.txt
grep -A10 "specific test name" .tests/run-*.txt
```

**Benefits:**
- See test output in real-time (unlike `>` redirection)
- Analyze failures without expensive re-runs
- Keep historical test results for comparison
- Search across multiple test runs

**Key Rule:** ALWAYS use `tee` for test output, NEVER plain redirection (`>` or `2>&1`)

#### .analysis/ Directory (Research & Documentation)

**Purpose:** Keep analysis artifacts separate from source code

**Usage:**
```bash
# Create directory (gitignored)
mkdir -p .analysis

# Use for:
# - JavaScript semantics research and comparisons
# - Type coercion behavior analysis
# - Performance benchmarking results
# - Runtime behavior verification scripts
# - Architecture diagrams and documentation
# - Temporary debugging scripts
```

**Benefits:**
- Keeps analysis work separate from source code
- Allows iterative analysis without cluttering repository
- Safe place for temporary debugging scripts
- Gitignored - no risk of committing debug artifacts

#### .todos/ Directory (Persistent Task Tracking)

**Purpose:** Track multi-step tasks across conversation sessions

**Usage:**
```bash
# Create task file: YYYY-MM-DD-task-name.md
# Example: 2025-11-03-array-semantics.md

# Task file must include:
# - Task overview and objectives
# - Current status (completed work)
# - Detailed remaining work list
# - Important decisions made
# - Files affected (C# implementations, bindings)
# - Testing requirements
# - JavaScript behavior verification notes
```

**Benefits:**
- Resume complex tasks across sessions with full context
- No loss of progress or decisions
- Gitignored for persistence

#### .temp/ Directory (Temporary Scripts & Debugging)

**Purpose:** Store temporary scripts and one-off debugging files

**Usage:**
```bash
# Create directory (gitignored)
mkdir -p .temp

# Use for:
# - Quick test scripts
# - Debug output files
# - One-off data transformations
# - Temporary C#/JavaScript for testing

# NEVER use /tmp or system temp directories
# .temp keeps files visible and within the project
```

**Key Rule:** ALWAYS use `.temp/` instead of `/tmp/` or system temp directories. This keeps temporary work visible and accessible within the project.

**Note:** All four directories (`.tests/`, `.analysis/`, `.todos/`, `.temp/`) should be added to `.gitignore`

## Session Startup

### First Steps When Starting a Session

When you begin working on this project, you MUST:

1. **Read this entire CLAUDE.md file** to understand the project conventions
2. **Review the bindings manifest** (`src/Tsonic.JSRuntime/types/Tsonic.JSRuntime.bindings.json`)
3. **Check JavaScript specification** as source of truth for runtime behavior
4. **Review existing implementations** (Array.cs, String.cs, etc.) to understand patterns

Only after reading these documents should you proceed with implementation tasks.

## Project Overview

**Tsonic.JSRuntime** is a C# implementation of JavaScript/TypeScript runtime semantics for the Tsonic compiler. It enables TypeScript code compiled to C# to behave exactly like it would in a JavaScript runtime. This includes:

- **JavaScript Arrays** - Sparse arrays with JS semantics (length, holes, etc.)
- **String manipulation** - JS string methods and behavior
- **Type coercion** - Automatic type conversions matching JS rules
- **Structural typing** - Duck typing and object shape compatibility
- **Console API** - `console.log`, `console.error`, etc.
- **JSON support** - `JSON.stringify`, `JSON.parse`
- **Math** - JavaScript Math object
- **Global functions** - `parseInt`, `parseFloat`, `isNaN`, `isFinite`, etc.

## Architecture

### Bindings Manifest

The runtime uses a single bindings file at `src/Tsonic.JSRuntime/types/Tsonic.JSRuntime.bindings.json` that maps JavaScript globals to C# types:

```json
{
  "bindings": {
    "console": {
      "kind": "global",
      "assembly": "Tsonic.JSRuntime",
      "type": "Tsonic.JSRuntime.console"
    },
    "Math": {
      "kind": "global",
      "assembly": "Tsonic.JSRuntime",
      "type": "Tsonic.JSRuntime.Math"
    }
  }
}
```

### Code Organization

```
src/Tsonic.JSRuntime/              # C# implementation
├── Array.cs                        # JavaScript Array semantics
├── String.cs                       # JavaScript String methods
├── console.cs                      # Console API (static class)
├── Math.cs                         # Math object (static class)
├── JSON.cs                         # JSON.stringify/parse (static class)
├── Operators.cs                    # Type coercion and operators
├── Structural.cs                   # Structural typing support
└── types/
    └── Tsonic.JSRuntime.bindings.json  # Global bindings

tests/Tsonic.JSRuntime.Tests/      # xUnit tests
├── ArrayTests.cs
├── StringTests.cs
├── ConsoleTests.cs
├── MathTests.cs
├── JSONTests.cs
├── OperatorsTests.cs
└── GlobalsTests.cs
```

## Implementation Guidelines

### C# Code Style

1. **Static classes for globals**: `console`, `Math`, `JSON` are static classes
2. **Lowercase names when appropriate**: Match JavaScript conventions (e.g., `console`, not `Console`)
3. **Suppress CS8981**: Warning for lowercase names (done in Directory.Build.props or .csproj)
4. **XML documentation**: Required for all public members
5. **Nullable annotations**: Use `string?` for nullable values
6. **Platform-specific**: Use appropriate .NET APIs for cross-platform behavior
7. **NativeAOT compatible**: No reflection, trim-safe code only

### JavaScript Semantics

**CRITICAL**: All implementations must match JavaScript behavior EXACTLY, including edge cases:

1. **Type coercion**: Follow ECMAScript specification for implicit conversions
2. **Array semantics**:
   - Sparse arrays (holes vs undefined)
   - Length property can be set
   - Negative indices don't wrap
3. **String behavior**:
   - Immutable
   - UTF-16 code units (not Unicode code points)
   - Index out of bounds returns undefined behavior
4. **Math functions**: Match JavaScript Math object exactly
5. **JSON**: Handle cycles, undefined, functions as JavaScript does

### Testing

1. **Use xUnit**: Standard .NET testing framework
2. **Verify JavaScript behavior**: Test against actual JavaScript output
3. **Test edge cases**: null, undefined behavior, empty arrays, NaN, Infinity
4. **500+ tests**: Comprehensive coverage of all runtime behaviors
5. **Cross-platform**: Ensure tests pass on Windows, Linux, macOS

## Common Tasks

### Adding a New Runtime Feature

1. Research JavaScript specification and behavior
2. Create or modify C# implementation in `src/Tsonic.JSRuntime/<feature>.cs`
3. Add binding to `src/Tsonic.JSRuntime/types/Tsonic.JSRuntime.bindings.json` if it's a global
4. Create comprehensive tests in `tests/Tsonic.JSRuntime.Tests/<Feature>Tests.cs`
5. Verify behavior matches JavaScript exactly (use Node.js REPL to test)
6. Update README.md if adding major functionality

### Verifying JavaScript Behavior

Before implementing any feature, verify exact JavaScript behavior:

```bash
# Test in Node.js REPL
node
> // Test the behavior you're implementing
> [1,2,3].length = 10
> [1,2,3].length
> // etc.
```

Document unusual behaviors in comments.

### Running Tests

**IMPORTANT:** Always use `tee` to capture test output for later analysis:

```bash
# Run all tests with output capture
dotnet test | tee .tests/run-$(date +%s).txt

# Run specific test class with output capture
dotnet test --filter "ClassName=ArrayTests" | tee .tests/array-$(date +%s).txt

# Run with verbose output and capture
dotnet test --logger "console;verbosity=detailed" | tee .tests/verbose-$(date +%s).txt

# Analyze previous test runs without re-running
grep "Failed" .tests/run-*.txt
tail -100 .tests/run-*.txt
```

### Building and Packaging

```bash
dotnet build                            # Build solution
dotnet test                             # Run all tests
dotnet pack -c Release                  # Create NuGet package
```

## Reference Projects

- **tsonic-node**: Node.js API bindings at `../tsonic-node`
  - Follow same patterns for bindings manifest
  - Reference for module binding approach
- **ECMAScript Specification**: https://tc39.es/ecma262/
  - Source of truth for JavaScript semantics
  - Reference for all runtime behavior

## Current Status

### Implemented ✅
- **Array**: JavaScript array semantics with sparse array support
- **String**: Core string manipulation methods
- **console**: log, error, warn, info, dir
- **Math**: Complete Math object (abs, floor, ceil, round, random, etc.)
- **JSON**: stringify and parse
- **Operators**: Type coercion and comparison operators
- **Structural**: Structural typing support
- **Globals**: parseInt, parseFloat, isNaN, isFinite
- **Map/Set**: ES6 Map and Set collections
- **WeakMap/WeakSet**: Weak reference collections
- **ArrayBuffer**: Fixed-length binary data buffer
- **Typed Arrays**: Int8Array, Uint8Array, Uint8ClampedArray, Int16Array, Uint16Array, Int32Array, Uint32Array, Float32Array, Float64Array
- **Date**: Date object with timezone support
- **RegExp**: Regular expression support with JavaScript flags (g, i, m, s, u, y)

### Not Implemented ⏳
- **Promise**: Async/await runtime support
- **Proxy**: Meta-programming features
- **Symbol**: Symbol primitive type
- **Error types**: Specific error types (TypeError, RangeError, etc.)

## Known Issues / Limitations

1. **Array performance**: Sparse array implementation may be slower than native C# arrays
2. **String encoding**: C# strings are UTF-16, matching JavaScript
3. **Number precision**: Uses C# double (64-bit IEEE 754), matching JavaScript
4. **No async support yet**: Promise/async/await not implemented

## Best Practices

1. **Match JavaScript exactly**: Even weird edge cases matter
2. **Test edge cases thoroughly**: null, undefined, NaN, Infinity, empty strings, etc.
3. **Document differences**: Note any deviations from JavaScript in XML comments
4. **NativeAOT compatible**: No reflection, trim-safe code only
5. **Performance matters**: Runtime is on hot path, optimize carefully
6. **Read ECMAScript spec**: Always reference official specification

## Debugging Tips

1. **Check bindings**: Verify global name maps to correct C# type
2. **Test in Node.js**: Compare actual JavaScript behavior
3. **Use Node.js REPL**: Quick way to verify edge cases
4. **Check ECMAScript spec**: For authoritative behavior definition
5. **Build errors**: Check `TreatWarningsAsErrors` - all warnings must be fixed
6. **Test failures**: Use `--logger "console;verbosity=detailed"` for full output

## Performance Notes

- Array operations should be O(1) for typical cases
- String operations follow .NET string performance characteristics
- JSON operations use System.Text.Json for performance
- Type coercion should be fast (inline where possible)
- Console operations are I/O bound

## Security Considerations

- No eval or dynamic code execution
- JSON parsing has depth limits
- No file system access (that's in tsonic-node)
- No network access
- Pure computational semantics only

## Future Enhancements

Priority order if expanding:

1. **Promise**: Basic promise implementation for async/await
2. **Symbol**: Symbol primitive type
3. **Proxy/Reflect**: Meta-programming features
4. **Error types**: Specific error types (TypeError, RangeError, etc.)
5. **Intl**: Internationalization APIs
