# Omega Spiral Game Senior Game Developer Overview

[AGENTS.md](./../AGENTS.md) is your guide to working on Omega Spiral - Chapter Zero.

## Correct Disposable Implementation

When implementing `IDisposable`, ensure you follow best practices to avoid resource leaks and ensure proper cleanup.

// ✅ CORRECT
void IDisposable.Dispose() { }  // Explicit interface, safe
protected override void Dispose(bool) { }  // Properly overrides base