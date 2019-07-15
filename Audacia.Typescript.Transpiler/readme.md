# Audacia.Typescript.Transpiler

### Generate typescript modules from .NET assemblies.

## Usage

With the `Audacia.Typescript.Transpiler` package installed in your web project, building the solution will add a `transpiler.config` file at the project root. The build will then fail because the configuration file will be invalid- its up to you to now specify what is to be transpiled, and where.

An example `transpiler.config` would be as follows:

```xml

<?xml version="1.0" encoding="utf-8"?>
<Settings>
  <Transpile path="">
    <Assembly name="bin/Debug/netcoreapp2.0/Example1.dll" />
    <Assembly name="bin/Debug/netcoreapp2.0/Example2.dll">
      <Properties initialize="true"/>
      <Namespace name="Example2.Models"/>
    </Assembly>
  </Transpile>
</Settings>

```

The `Transpile` element's `path` attribute can be used to specify what filepath the typescript files should be written to, relative to the project root. This element represents a single transpilation job.

Each `Assembly` element specifies an assembly which should be transpiled. In this example, `Example1.dll` will produce a file called `example1.ts`.

The `Properties` element can optionally be used to specify whether each typescript type's properties should be initialized to match their .NET counterparts. Defaults to true.

The `Namespace` element can optionally used to provide specific namespaces to transpile. If not present, all namespaces are included.


## Developing

an [example configuration file](https://github.com/audaciaconsulting/Audacia.Typescript/blob/master/Audacia.Typescript.Transpiler/example.config) is included in the repository. To debug the transpilier against it, first set `Audacia.Transcript.Transpiler` as the startup project. 

Ensure the working directory is the root directory of the project, and in the debug settings for the project set `example.config` as the single argument in the `Application Arguments` field.

Running or debugging the application now will produce example transpiled versions of the two libraries themselves. In your source folder you will some new typescript files, including `audacia.typescript.ts` and `audacia.typescript.transpiler.ts`.
