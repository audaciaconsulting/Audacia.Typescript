[[_TOC_]]


#Intended use
- Download the `Audacia.Typescript.Transpiler` nuget package, building the solution should put a `transpiler.config` file at the project root.
- The contents of that config file are mapped to [`Settings.cs`](https://audacia.visualstudio.com/Audacia/_git/Audacia.Typescript?path=%2FAudacia.Typescript.Transpiler%2FConfiguration%2FSettings.cs&version=GBmaster). The `transpiler.config` is populated with what's found in `Setttings.Default`.

##OutputSettings
For each item here, a file will be created and placed in the path specified in `OutputSettings.Path`.

##InputSettings
Each output can have many of these. It's assembly-specific `C#` that will be transpiled. You can drill down further, as follows:
- _Namespaces_: give the `Input` tag a child of `NameSpace` to only transpile code within that namespace
- _TypeName_: give the `Namespace` tag a child of `TypeName` to only transpile **that type** within the namespace.

#Debugging

- Set `Audacia.Transcript.Transpiler` as the startup project
- Edit the above project's properties as follows:

|Name|Value|
|---|---|
|Application Arguments|example.config|
|Working Directory|{localRepoPath}\Audacia.Typescript.Transpiler

**NB** [`example.config`](https://audacia.visualstudio.com/Audacia/_git/Audacia.Typescript?path=%2FAudacia.Typescript.Transpiler%2Fexample.config&version=GBmaster) is checked in to source control for ease, so that there exists an example ready to test with.
- `F5` - the output file specified in the example config should be dumped to the working directory