# Samples

- [x] EF Core Console Sample
- [x] EF Core with DI Console Sample
- [x] Minimal Api Sample

# Refactorings
- [x] rename DefaultQueryTookit to QueryTookit with Default(...) static factory method

# Optimizations
- [x] Add ConfigureAwait(false) to every Bodde.Query.core awaited call

# Tests
- [x] Refactor everywhere with Moq dependencies
- [x] Best code coverage

# Clean code
- [x] Remove all unnecessary usings

# AspNetCore support
- [x] Use AsParameters attribute to bind QueryString to QueryCriteriaParameters

# Comments
- [x] Add comments for public interfaces, classes, methods, props

# README.md
- [x] Create full document

# Make the libraries compatible with most versions of .NET
- [x] Convert to .NET Standard 2.0: Bodde.Query.Abstractions
- [x] Convert to .NET Standard 2.0: Bodde.Query.Core
- [x] Add .NET 8 compatibility to Bodde.Query.EntityFrameworkCore

# Read
- [ ] [Migrate to MTP mode of dotnet test](https://learn.microsoft.com/en-gb/dotnet/core/testing/unit-testing-with-dotnet-test#migrate-to-mtp-mode-of-dotnet-test)

# Move common extension methods to Bodde.Common.Extension
- [ ] Tokenize
- [ ] ToDisplayTable -> FormatAsTable

# Nuget packages
- [ ] Move ToDisplayTable extension to Bodde.Common.Extensions package
- [ ] Abstractions
  - [ ] Configure NuGet package
  - [ ] Ensure that README.md is taken from root
  - [ ] Create GitHub Action
  - [ ] Create Tag/Release
  - [ ] Publish
- [ ] Core
  - [ ] Configure NuGet package
  - [ ] Ensure that Abstraction dependency comes from NuGet package
  - [ ] Ensure that README.md is taken from root
  - [ ] Create GitHub Action
  - [ ] Create Tag/Release
  - [ ] Publish
- [ ] EntityFrameworkCore
  - [ ] Configure NuGet package
  - [ ] Ensure that Abstraction anc Core dependencies come from NuGet package
  - [ ] Ensure that README.md is taken from root
  - [ ] Create GitHub Action
  - [ ] Create Tag/Release
  - [ ] Publish
- Add Code coverage badges with [Shields](https://shields.io)

