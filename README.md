# Audacia.Typescript

### Write typescript and generate typescript modules from C# assemblies.

[![Build Status](https://dev.azure.com/audacia/Audacia/_apis/build/status/Audacia.Typescript?branchName=master)](https://dev.azure.com/audacia/Audacia/_build/latest?definitionId=272&branchName=master)
![Release Status](https://vsrm.dev.azure.com/audacia/_apis/public/Release/badge/8f54bcdc-d88d-46d7-9918-1bf635097bd4/17/17)

This solution consists of two main projects:

- [Audacia.Typescript](https://github.com/audaciaconsulting/Audacia.Typescript/tree/master/Audacia.Typescript)
- [Audacia.Typescript.Transpiler](https://github.com/audaciaconsulting/Audacia.Typescript/tree/master/Audacia.Typescript.Transpiler)

The base project contains classes for generating typescript programatically in C#, in a consistent and clean way.
The transpiler project contains classes and tooling for reflecting C# assemblies at build time and generating typescript modules based on them.
Please follow their respective links above for more information on each.

_Please note: this project is still in development and not all CLR features are supported yet._
