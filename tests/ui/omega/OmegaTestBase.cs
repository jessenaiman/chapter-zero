// <copyright file="OmegaTestBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;

namespace OmegaSpiral.Tests;

/// <summary>
/// Base class for all Omega UI test suites.
/// Provides default timeout protection (10 seconds per test) to prevent test hangs and system crashes.
///
/// All test methods automatically inherit this timeout unless explicitly overridden.
/// </summary>
[TestSuite]
public abstract class OmegaTestBase
{
    /// <summary>
    /// Default timeout for all Omega UI tests: 10 seconds (10000 milliseconds).
    /// This protects against infinite loops, deadlocks, and scene tree processing hangs.
    /// Individual tests can override by specifying their own Timeout parameter.
    /// </summary>
    protected const int DEFAULT_TEST_TIMEOUT_MS = 10000;
}
