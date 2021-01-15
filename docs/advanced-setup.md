# Advanced setup

There are a few alternate ways to set up GodotOnReady that might fit your
requirements better.

## Remove dependency on `GodotOnReady.Attributes.dll`

The `GodotOnReady` package includes the source generator plus a separate DLL
with the attribute classes. If the extra DLL is a problem, you can depend on the
generator directly and define the attributes in your Godot project:

1. Remove your `csproj`'s dependency on the `GodotOnReady` package.
1. Add a direct dependency on the `GodotOnReady.Generator` package.
1. Copy this file into your code directly to define the attributes:  
   [/src/GodotOnReady.Attributes/Attributes.cs](/src/GodotOnReady.Attributes/Attributes.cs)

Now the generator will automatically use your project's annotations.

## Clone the source and use `ProjectReference`

If you want to make changes to the GodotOnReady source code, or for any other
reason want to reference `GodotOnReady.Generator.csproj` rather than using the
nuget package or building a nuget package yourself, special `ProjectReferences`
are necessary:

```xml
<Project Sdk="Godot.NET.Sdk/3.2.3">
  <!-- ... -->
  <ItemGroup>
    <ProjectReference Include="..\src\GodotOnReady.Attributes\GodotOnReady.Attributes.csproj" />
    <ProjectReference Include="..\src\GodotOnReady.Generator\GodotOnReady.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
  <!-- ... -->
</Project>
```

This is how the development project [`GodotOnReadyDev`](/godot) is built.
