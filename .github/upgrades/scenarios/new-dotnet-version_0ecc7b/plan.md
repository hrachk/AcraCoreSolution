# .NET Version Upgrade Plan

## Table of contents

- [1 Executive Summary](#executive-summary)
- [2 Migration Strategy](#migration-strategy)
- [3 Detailed Dependency Analysis](#detailed-dependency-analysis)
- [4 Project-by-Project Plans](#project-by-project-plans)
- [5 Package Update Reference](#package-update-reference)
- [6 Breaking Changes Catalog](#breaking-changes-catalog)
- [7 Testing Strategy](#testing-strategy)
- [8 Risk Management](#risk-management)
- [9 Complexity & Effort Assessment](#complexity--effort-assessment)
- [10 Source Control Strategy](#source-control-strategy)
- [11 Success Criteria](#success-criteria)
- [12 Appendix & References](#appendix--references)

---

## 1 Executive Summary

- **Scope**: Upgrade entire solution `AcraCoreSolution.sln` (13 projects) from current targets to `net11.0` (preview) as proposed by assessment.
- **Key findings from assessment**: multiple projects have API compatibility issues (binary/source/behavioral), several NuGet packages need upgrades including security fixes and deprecations, and all projects require target framework updates.
- **Selected Strategy**: All-At-Once Strategy — upgrade all projects simultaneously in a single coordinated operation.

**Rationale for All-At-Once**:
- **Solution size**: 13 projects (medium). Dependency structure is relatively shallow (topological order depth is moderate) and assessment shows cross-cutting package updates and framework changes that are easier to handle in a unified pass.
- **Risk tradeoffs**: Assessment found multiple package updates and API changes; team must accept higher short-term risk to reduce total migration time and avoid multi-state complexity.

---

## 2 Migration Strategy

**Selected Strategy**: All-At-Once Strategy — All projects upgraded simultaneously in a single atomic operation.

**Key points**:
- Update `TargetFramework` to `net11.0` for all projects (or append to existing multitarget framework lists where assessment indicates multi-targeting).
- Apply all NuGet package updates suggested by the assessment across all projects in the same operation.
- Ensure SDK prerequisites are addressed (developer machines and CI must have .NET 11 preview SDK installed) and `global.json` updated if present.
- Restore and build the complete solution, resolve compile-time errors caused by framework and package changes in the same atomic pass.
- Run all test projects and fix test failures as part of the same upgrade operation.
- **Source control**: make a single consolidated commit/PR containing all changes for the atomic upgrade.

Dependency-based ordering rationale (applies to testing and validation within the atomic upgrade):
- Foundation libraries `AcraUtils` and `AcraData` provide core APIs used across the solution. Although the upgrade is atomic, verify their compilation and test coverage first as part of the validation steps after the atomic change.
- Leaf-first reasoning is used to identify critical components and tests to run early in post-upgrade validation; it does not change that changes are applied across all projects simultaneously.

Parallelization guidance:
- Because the operation is atomic, code changes can be prepared in parallel by multiple engineers but merged in a single branch/PR for the upgrade.
- Test execution and fixes can be parallelized across independent test suites but must complete before merging the atomic PR.

Rollback guidance:
- If the atomic upgrade fails in CI or produces regressions, revert the single upgrade commit/PR and iterate on fixes in a feature branch; do not attempt piecemeal rollbacks across projects.

**Constraints and cautions**:
- .NET 11 is preview; consider whether preview is acceptable for production. If not, the target should be changed to a supported LTS version (e.g., net10.0) — this plan is written for `net11.0` as requested.
- The upgrade may introduce API-breaking changes; plan includes a Breaking Changes Catalog section to prepare implementers.

---

## 3 Detailed Dependency Analysis

Summary of projects in dependency topological order (leaf nodes first):

1. `AcraUtils` (leaf)
2. `AcraData`
3. `AcraIDServices` (depends on AcraData, AcraUtils)
4. `PekWebService` (depends on AcraUtils)
5. `PackUpService` (CheckUpService) (depends on AcraData, AcraUtils)
6. `AcraIDGenerator` (depends on AcraIDServices, AcraData, AcraUtils)
7. `PekBackService` (depends on AcraIDServices, PekWebService, AcraData, AcraUtils)
8. `CheckUpBackEndService` (depends on CheckUpService, AcraData)
9. `AcraIdentityFE` (depends on AcraUtils)
10. `AcraValidatorWebService` (depends on AcraIDServices, AcraData, AcraUtils)
11. `EkengWebService` (depends on AcraIDServices, AcraUtils)
12. `PackUpWebService` (CheckUpWebService) (depends on CheckUpService, AcraData, AcraUtils)
13. `AcraIdentityServer` (depends on AcraData, AcraUtils)

Notes:
- `AcraUtils` and `AcraData` are foundation libraries used throughout the solution and must be considered central to the upgrade.
- Several projects reference older ASP.NET/Core packages or `Microsoft.AspNetCore.App`. These need coordination because framework references and implicit package behavior changed in later .NET versions.
- WCF/System.ServiceModel usage observed in `PekWebService` and `PekBackService` suggests evaluation of `CoreWCF` or alternative approaches.

---

## 4 Project-by-Project Plans

Below are per-project stubs. Each stub lists current state observed in assessment and required target state. Detailed migration steps, package lists and breaking changes are provided as follow-up items in this plan.

### Project: `AcraUtils`
**Current State**: referenced packages: AutoMapper 16.0.0, Newtonsoft.Json 12.0.1, RabbitMQ.Client 5.1.0, others. TargetFramework: older (requires change).
**Target State**: `net11.0`
**Migration Steps**:
- [To be filled]
**Risk**: Medium
**Notes**: Foundation library; prioritize build and unit test validation.

### Project: `AcraData`
**Current State**: EF Core 5.x, Pomelo.MySql 5.0.0, Newtonsoft.Json 12.0.1.
**Target State**: `net11.0`, EF Core updated to assessment-suggested version.
**Migration Steps**:
- [To be filled]
**Risk**: High (database and EF migration compatibility)

### Project: `AcraIDServices`
**Current State**: Depends on `AcraData` and `AcraUtils`. Uses AutoMapper 16.0.0.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `PekWebService`
**Current State**: Uses WCF / System.ServiceModel packages. Mixed package versions.
**Target State**: `net11.0` (evaluate CoreWCF for WCF server-side functionality)
**Migration Steps**: [To be filled]
**Risk**: High (WCF migration)

### Project: `PackUpService (CheckUpService)`
**Current State**: Uses EPPlus.Core, ASP.NET Core Http Features 5.x.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `AcraIDGenerator`
**Current State**: EF Core 5.x, depends on data/services/util libs.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `PekBackService`
**Current State**: Mixed ServiceModel and EF packages. Depends on many projects.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: High

### Project: `CheckUpBackEndService`
**Current State**: Depends on `CheckUpService` and `AcraData`.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `AcraIdentityFE`
**Current State**: Razor Pages front-end project (uses Microsoft.AspNetCore.App 2.2 and EF Core 5.x).
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: High (front-end behavioral changes and Identity packages)

### Project: `AcraValidatorWebService`
**Current State**: Uses EF Core 10.x in some packages (note: mixed versions present), depends on ID services.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `EkengWebService`
**Current State**: Similar stack to other web services (EF Core 5.x, ASP.NET packages).
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `PackUpWebService (CheckUpWebService)`
**Current State**: Depends on CheckUpService and uses IdentityServer4.AccessTokenValidation package.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: Medium

### Project: `AcraIdentityServer`
**Current State**: Uses IdentityServer4 (4.1.2) and EF Relational 5.x. IdentityServer4 may not be supported in .NET 11; migration to Duende IdentityServer or alternative may be required.
**Target State**: `net11.0`
**Migration Steps**: [To be filled]
**Risk**: High (identity/security components)

---

## 5 Package Update Reference

This section groups package updates by scope. Exact current and suggested target versions were listed in the assessment; implementers must apply the versions from `assessment.md` when performing changes.

### Common package groups
- Microsoft.AspNetCore.* and Microsoft.Extensions.* packages: update to versions compatible with `net11.0`. These are framework-aligned packages and many become implicit framework references in newer SDKs. Replace explicit package references with `FrameworkReference` to `Microsoft.AspNetCore.App` only if assessment recommends it.
- Entity Framework Core family: update EF Core packages from 5.x to versions compatible with `net11.0` (assessment shows mixed 5.x and 10.x versions present). Use EF Core 11 packages where available, or EF Core 10 if EF 11 is not released.
- Security-sensitive packages (packages flagged with `NuGet.0004` in assessment): treat as high priority. Examples include older versions of Newtonsoft.Json, IdentityServer4-related packages, and others — see assessment.
- WCF / ServiceModel packages: evaluate `System.Private.ServiceModel` and `System.ServiceModel.*` references; consider migration to CoreWCF or updated package versions compatible with `net11.0`.
- IdentityServer4: IdentityServer4 is not supported on later .NET versions in its OSS form; plan migration to Duende IdentityServer (commercial) or another supported identity provider.

### Project-specific package notes
- `AcraData`: Pomelo.EntityFrameworkCore.MySql update required (assessment lists 5.0.0 or 9.x in some projects). Align MySQL provider to EF Core version chosen.
- `PekWebService`/`PekBackService`: System.ServiceModel packages need verification for binary compatibility; consider CoreWCF or alternate approach.
- `AcraIdentityFE` / `AcraIdentityServer` / `PackUpWebService`: Identity-related packages require special attention for token validation and authentication middleware.

⚠️ All packages marked as deprecated or containing security vulnerabilities in `assessment.md` must be updated or replaced as part of this upgrade.

---

## 6 Breaking Changes Catalog

This catalog lists expected categories of breaking changes to prepare for during the atomic upgrade.

### API compatibility
- Binary and source incompatible APIs reported by assessment (`Api.0001` and `Api.0002`) must be resolved by code changes found during compilation. Common cases: removed or changed method overloads, moved types, or changed assembly internals.

### Behavioral changes
- Behavioral change warnings (`Api.0003`) may alter runtime behavior; review authentication flow, JSON serialization defaults, HttpClient behavior, and configuration binding.

### ASP.NET Core / Razor Pages
- Razor Pages startup patterns may require updating `Program.cs` to the minimal hosting model if using latest templates. Review middleware registration and endpoint routing.
- `Microsoft.AspNetCore.App` implicit references: many previously explicit packages are now included; remove duplicate package references where necessary.

### Entity Framework
- EF Core major version upgrades can include breaking surface area changes (query translation, change tracking, migrations). Verify database migrations and update to the corresponding EF Core CLI tools.

### WCF / ServiceModel
- System.ServiceModel packages may not be fully supported; migrating to CoreWCF or gRPC is a recommended path.

### IdentityServer4
- IdentityServer4 has no community-supported upgrade path to newer .NET versions; consider Duende IdentityServer or external identity providers (Azure AD, Auth0). Migration includes configuration and token validation changes.

---

## 7 Testing Strategy

Testing phases (applied after atomic upgrade completes):

- Foundation verification: build and run unit tests for `AcraUtils` and `AcraData` first.
- Component tests: run unit and integration tests for project groups that depend on updated libraries.
- Full-solution tests: run all test projects and integration tests.

Validation checklist for each project after upgrade:
- [ ] Project builds without errors
- [ ] No unresolved package conflicts
- [ ] Unit tests pass
- [ ] Integration tests that exercise DB or external services pass or are marked for follow-up
- [ ] No security vulnerabilities remain in updated packages

Automated test execution:
- Use existing CI pipelines; update runners to include .NET 11 preview SDK where required.
- Parallelize test execution where possible, but ensure ordering constraints if integration tests depend on shared test data.

---

## 8 Risk Management

High-risk items and mitigations:
- WCF and System.ServiceModel usage (`PekWebService`, `PekBackService`): High risk. Mitigation: evaluate CoreWCF or plan a migration to gRPC; dedicate specialists to this work and run integration tests against compatible endpoints.
- IdentityServer4 and authentication (`AcraIdentityServer`, `AcraIdentityFE`, `PackUpWebService`): High risk. Mitigation: consider migration plan to Duende IdentityServer or external provider; create a compatibility shim layer for token validation.
- EF Core and DB compatibility (`AcraData`, `AcraIDGenerator`): High risk. Mitigation: run database migration tests, validate SQL generated, and update Pomelo/MySql provider to a version compatible with chosen EF Core.

Medium-risk items:
- Package upgrades with deprecations: update carefully and run unit tests.
- Razor Pages and middleware changes: verify `Program.cs` and `Startup` patterns.

Rollback strategies:
- Revert the single atomic commit/PR if CI failure or severe regressions occur.
- For critical fixes, create hotfix branches that revert only the problematic changes if partial rollback is necessary.

---

## 9 Complexity & Effort Assessment

Use relative complexity ratings (Low/Medium/High):
- High: `AcraData`, `PekWebService`, `PekBackService`, `AcraIdentityFE`, `AcraIdentityServer`
- Medium: `AcraIDGenerator`, `AcraIDServices`, `AcraValidatorWebService`, `CheckUpBackEndService`, `PackUpWebService`, `EkengWebService`, `PackUpService`
- Low: `AcraUtils`

Notes:
- Complexity considers LOC, package update counts, database and identity dependencies, and presence of WCF or other legacy tech.

---

## 10 Source Control Strategy

- **Branching**: create an upgrade branch for the atomic upgrade (branch name recommended by assessment tools was used earlier). If no Git repo exists, apply plan changes locally and create a branch/PR once repository is available.
- **Commit strategy**: single atomic commit containing all project file changes, package reference changes, and code modifications required to restore build.
- **PR checklist**: build success, all unit/integration tests pass, review for package security vulnerabilities addressed.

---

## 11 Success Criteria

The migration is complete when:
- All projects target `net11.0` as specified in project files
- All packages flagged by assessment are updated or replaced
- Full solution builds with 0 errors and no package dependency conflicts
- All unit and integration tests pass
- No remaining critical security vulnerabilities in package inventory

---

## 12 Appendix & References

- Assessment: `F:\acra4-services2\.github\upgrades\scenarios\new-dotnet-version_0ecc7b\assessment.md`
- Solution: `F:\acra4-services2\AcraCoreSolution.sln`

[End of file]
