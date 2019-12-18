# Audacia.Typescript

### Write typescript and generate typescript modules from C# assemblies.

![Build Status](https://dev.azure.com/audacia/Audacia/_apis/build/status/Audacia.Typescript?branchName=master)
[![CodeFactor](https://www.codefactor.io/repository/github/audaciaconsulting/audacia.typescript/badge)](https://www.codefactor.io/repository/github/audaciaconsulting/audacia.typescript)

| NuGet                         |               |
| ----------------------------- |:-------------:|
| Audacia.Typescript            | [![NuGet](https://img.shields.io/nuget/v/Audacia.Typescript.svg)](https://www.nuget.org/packages/Audacia.Typescript) |
| Audacia.Typescript.Transpiler | [![NuGet](https://img.shields.io/nuget/v/Audacia.Typescript.Transpiler.svg)](https://www.nuget.org/packages/Audacia.Typescript.Transpiler) |

This solution consists of two main projects:

- [Audacia.Typescript](https://github.com/audaciaconsulting/Audacia.Typescript/tree/master/Audacia.Typescript)
- [Audacia.Typescript.Transpiler](https://github.com/audaciaconsulting/Audacia.Typescript/tree/master/Audacia.Typescript.Transpiler)

The base project contains classes for generating typescript programatically in C#, in a consistent and clean way.
The transpiler project contains classes and tooling for reflecting C# assemblies at build time and generating typescript modules based on them.
Please follow their respective links above for more information on each.

_Please note: this project is still in development and not all CLR features are supported yet._
