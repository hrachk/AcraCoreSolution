# AcraCoreSolution .NET 11 Upgrade Tasks

## Overview

This document tracks the execution of the upgrade of `AcraCoreSolution.sln` to `net11.0` across all projects in a single coordinated operation, followed by automated testing and a final consolidated commit. Tasks follow the plan's prerequisites, atomic upgrade, testing, and commit ordering.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §2 Migration Strategy, Plan §10 Source Control Strategy

- [ ] (1) Verify required .NET 11 preview SDK is installed on developer machines and CI runners per Plan §2
- [ ] (2) Update or create `global.json` (if present) to lock the required SDK version per Plan §2
- [ ] (3) Verify CI runner images and required toolchain components are available/updated per Plan §2 (e.g., MSBuild, dotnet CLI) (**Verify**)
- [ ] (4) Check compatibility of repository-level MSBuild files (`Directory.Build.props`, `Directory.Packages.props`) and other shared build configuration files per Plan §5 (if present) (**Verify**)

### [ ] TASK-002: Atomic framework and package upgrade with compilation fixes
**References**: Plan §2 Migration Strategy, Plan §3 Detailed Dependency Analysis, Plan §4 Project-by-Project Plans, Plan §5 Package Update Reference, Plan §6 Breaking Changes Catalog

- [ ] (1) Update `TargetFramework` to `net11.0` (or append to existing multitarget values where specified) in all projects listed in Plan §3 and per Plan §4
- [ ] (2) Update all NuGet package references across the solution per Plan §5 (apply versions from assessment.md referenced in plan)
- [ ] (3) Restore all dependencies (dotnet restore) and ensure all packages resolve successfully (**Verify**)
- [ ] (4) Build the full solution and fix all compilation errors introduced by framework/package changes, addressing items from Plan §6 Breaking Changes Catalog
- [ ] (5) Rebuild and confirm the solution builds with 0 errors (**Verify**)

### [ ] TASK-003: Run test suites and validate upgrade
**References**: Plan §7 Testing Strategy, Plan §3 Detailed Dependency Analysis, Plan §6 Breaking Changes Catalog

- [ ] (1) Run unit tests for foundation projects `AcraUtils` and `AcraData` first per Plan §7 (foundation verification)
- [ ] (2) Fix any test failures (reference Plan §6 for likely breaking-change fixes)
- [ ] (3) Run component-level and full-solution test projects per Plan §7 (parallelize where safe)
- [ ] (4) All tests pass with 0 failures (**Verify**)

### [ ] TASK-004: Final commit
**References**: Plan §10 Source Control Strategy

- [ ] (1) Commit all remaining changes with message: "TASK-004: Complete atomic upgrade to net11.0"
