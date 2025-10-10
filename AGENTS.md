# C# Documentation and Async Standards

## XML Documentation Requirements

### Public Members
- Public members must be documented with XML comments
- Internal members should be documented if complex or not self-explanatory

### XML Tag Usage
- Use `<summary>` for method/class descriptions (brief overview)
- Use `<param>` for method parameters
- Use `<paramref>` to reference parameters in documentation
- Use `<returns>` for method return values
- Use `<remarks>` for additional information (implementation details, usage notes)
- Use `<example>` for usage examples with `<code language="csharp">` blocks
- Use `<exception>` to document thrown exceptions
- Use `<see langword>` for keywords (`null`, `true`, `false`, `int`, `bool`)
- Use `<see cref>` for inline type/member references
- Use `<seealso>` for "See also" section references
- Use `<inheritdoc/>` to inherit base documentation (document differences if behavior changes)
- Use `<typeparam>` and `<typeparamref>` for generic type parameters
- Use `<c>` for inline code, `<code>` for blocks

## Async Programming Standards

### Naming Conventions
- [ ] Use 'Async' suffix for all async methods
- [ ] Match async method names with synchronous counterparts (e.g., `GetDataAsync()` for `GetData()`)

### Return Types
- [ ] Return `Task<T>` when method returns a value
- [ ] Return `Task` when method doesn't return a value  
- [ ] Consider `ValueTask<T>` for high-performance scenarios
- [ ] Avoid returning `void` (except for event handlers)

### Exception Handling
- [ ] Use try/catch around await expressions
- [ ] Avoid swallowing exceptions
- [ ] Use `ConfigureAwait(false)` in library code when appropriate
- [ ] Propagate exceptions with `Task.FromException()`

### Performance & Best Practices
- [ ] Use `Task.WhenAll()` for parallel execution
- [ ] Use `Task.WhenAny()` for timeouts/first completion
- [ ] Avoid unnecessary async/await when passing through results
- [ ] Use cancellation tokens for long-running operations

### Prohibited Practices
- [ ] Never use `.Wait()`, `.Result()`, or `.GetAwaiter().GetResult()`
- [ ] Don't create async void methods (except event handlers)
- [ ] Always await Task-returning methods
- [ ] Don't mix blocking and async code