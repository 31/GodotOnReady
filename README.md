# GodotOnReady

**GodotOnReady** is a [C# Source Generator] that adds convenient `onready`-like
features to your C# scripts in Godot Mono without any reflection.

* [`[OnReadyGet]`](#onreadyget) - Load a `Node` or `Resource` subclass into a
  field or property. Automatically exports a property so you can use the Godot
  editor to configure the path it loads.
* [`[OnReady]`](#OnReady) - Execute any 0-argument method during `_Ready`.

Bonus feature:

* [`[GenerateDataSelectorEnum]`](docs/GenerateDataSelectorEnum.md) - Generate an enum
  where each value is strongly associated with custom data.

## Prerequisites

* Godot 3.2.3 or greater (Mono version)  
  <https://godotengine.org/download>

* .NET SDK 5.0 or greater  
  <https://dotnet.microsoft.com/download>

* Set Godot to use the `dotnet CLI` Build Tool  
  <details><summary>Screenshot showing this editor setting (click me!)</summary>

  ![](docs/img/EditorSettings-BuildTool-cli.png)

  </details>

## Project setup

`GodotOnReady` is a NuGet package that adds two components: a C# library with a
few annotations, and a source generator that detects those annotations and
generates code. Just add the `GodotOnReady` package to your project.

Your Godot project's `.csproj` file should look like this when you're done:

```xml
<Project Sdk="Godot.NET.Sdk/3.2.3">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <!-- Workaround for using .NET 5 in Godot 3.2.3: https://github.com/godotengine/godot/issues/43717#issuecomment-739422982 -->
    <GodotUseNETFrameworkRefAssemblies>true</GodotUseNETFrameworkRefAssemblies>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="GodotOnReady" Version="1.0.0" />
  </ItemGroup>
</Project>
```

You may need to restart your IDE to navigate to generated sources and for the
generated code to show up in code completion/intellisense.

For advanced alternatives, see
[/docs/advanced-setup.md](/docs/advanced-setup.md).

## Usage

### `[OnReadyGet]`

Instead of writing this repetitive code:

```cs
public class MyControl : Control
{
  [Export] public NodePath ButtonPath { get; set; }
  private Button _button;

  public override void _Ready()
  {
    _button = GetNode<Button>(ButtonPath);
  }
}
```

Write this code instead to do the same thing:

```cs
public partial class MyControl : Control
{
  [OnReadyGet] private Button _button;
}
```

The source generator figures out that the exported property should be called
`ButtonPath` by taking `_button`, trimming the leading `_` character, and
capitalizing the first letter.

The `[OnReadyGet]` source generator works for fields and properties. It also
works for `Resource` subclasses like `PackedScene` and `Texture`.

### `[OnReadyGet(...)]`

If you pass a path, that path is always used, and `ButtonPath` won't show up in
the Godot editor. This is useful for a dependency you know will always be at
that path, and avoids cluttering the Godot editor with script properties that
you never modify:

```cs
[OnReadyGet("My/Button/Somewhere")] private Button _button;
```

If you know the dependency will usually be at one path, but it may need to be
tweaked to point somewhere else sometimes, set `Export = true` to use the path
as the default and also export `ButtonPath` for tweaking:

```cs
[OnReadyGet("My/Button/Somewhere", Export = true)] private Button _button;
```

To make an optional dependency, use `OrNull = true`. It allows the path to be
`null` or empty without errors, and if the path exists, it uses
`GetNodeOrNull<T>` to allow the path to be invalid. The `_button` member will
then be `null`, so be sure to check.

```cs
[OnReadyGet("My/Button/Maybe", OrNull = true)] private Button _button;
```

If your property is a `Resource` rather than a `Node`, pass a resource path
instead of a node path.

---

### `[OnReady]`

Using `OnReadyGet` generates a `public override void _Ready()` method. This
means you can't define it yourself. To run your own code during `_Ready`, mark
any zero-argument method with `[OnReady]`:

```cs
[OnReadyGet] private Button _button;
[OnReady] private void ConnectButtonOnReady()
{
  _button.Connect("pressed", this, nameof(ButtonPressed));
}
```

The generated `_Ready()` method will then be:

```cs
public override void _Ready()
{
  _button = GetNode<Button>(ButtonPath);
  ConnectButtonOnReady();
}
```

### `[OnReady(...)]` arguments:

* `Order = 0` defines a custom order for methods to be called in the generated
  `_Ready` method. Default is `0`.
  * Calls are sorted with this priority:
    * (1) `Order`, from low to high.
    * (2) Declaration order in the class file.
  * `OnReadyGet` members are all initialized between the last `Order=-1` method
    and the first `Order=0` method, regardless of declaration order.

---

# Troubleshooting

### error CS0111: Type '***' already defines a member called '_Ready' with the same parameter types

When you use `[OnReadyGet]`, you can't write `public override void _Ready()` in
your own code, because GodotOnReady generated it already. To run your own code
in the generated `_Ready()` method, use [`[OnReady]`](#OnReady).

### error CS0260: Missing partial modifier on declaration of type '***'; another partial declaration of this type exists

Your class is most likely missing the `partial` modifier. The declaration
should look like this:

```cs
public partial class MyNode : Node
```

### It isn't working! My variables are null, and no new properties show up in Godot.

First, hit the `Build` button at the top-right of the Godot editor to make sure
your build is up to date. That might make the properties show up.

Then, make sure you are using [.NET SDK version 5.0](#Prerequisites) or newer.
If you run `dotnet --version` in a console window, it will show you what version
of the SDK you have.

If that doesn't work, please file an issue. Include the output of `dotnet
--info`, the Godot build log, and if possible, the project.

# License

GodotOnReady is licensed under the [MIT license](LICENSE).


[C# Source Generator]: https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/
