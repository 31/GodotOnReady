# Advanced setup

The [GodotOnReady NuGet package](https://www.nuget.org/packages/GodotOnReady) is the easiest way to set up and use GodotOnReady in your project. However, if that doesn't work for you, this doc lists a few alternate ways to set up GodotOnReady.

## How do I remove `GodotOnReady.Attributes.dll` from the game's output folder?

The `GodotOnReady` package includes the source generator, which is never shipped with your game, and `GodotOnReady.Attributes.dll`, which is included with your game. This DLL contains the attribute classes, like `OnReadyGetAttribute`. If `GodotOnReady.Attributes.dll` is a problem, you can remove it by following these steps:

1. Open your `.csproj` file.
1. Look for `PackageReference Include="GodotOnReady"`. Replace `GodotOnReady` with `GodotOnReady.Generator`.
1. Copy this file into your Godot project to define the attributes:  
   [/src/GodotOnReady.Attributes/Attributes.cs](/src/GodotOnReady.Attributes/Attributes.cs)

Now the generator will automatically find the annotations that you have defined in your own game's `.dll` file, rather than using the external `GodotOnReady.Attributes.dll`.

## How do I build from source?

If you want to make changes to the GodotOnReady source code, or for any other reason you want to build GodotOnReady from source rather than using a NuGet package, you need to edit your game's `.csproj` file with a special `ProjectReference`:

```xml
<Project Sdk="Godot.NET.Sdk/3.2.3">
  <!-- ... -->
  <ItemGroup>
    <ProjectReference Include="..\sub\GodotOnReady\src\GodotOnReady.Attributes\GodotOnReady.Attributes.csproj" />
    <ProjectReference Include="..\sub\GodotOnReady\src\GodotOnReady.Generator\GodotOnReady.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
  <!-- ... -->
</Project>
```

This is how the development project [`GodotOnReadyDev`](/godot) is built.

I suggest arranging your Git repository this way:

* Keep your project in `godot/` (so your game is `godot/project.godot`).
* Use a Git submodule, Git subtree, or something similar to put the whole GodotOnReady repository in `sub/GodotOnReady`.

This should make the paths in the above `ProjectReference` elements work.
