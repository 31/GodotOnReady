# GodotOnReady

**GodotOnReady** is a [C# Source Generator] that adds convenient `onready`-like
features to your C# scripts in Godot Mono without any reflection.

* [`[OnReadyGet]`](#onreadyget) - Load a strongly typed `Node` or `Resource`
  subclass into a field or property and automatically export a property for
  configurability in your Godot scene.
* [`[OnReady]`](#OnReady) - Execute any 0-argument method during `_Ready`.

## Prerequisites

* Godot 3.2.3 (Mono version)  
  <https://godotengine.org/download>

* .NET SDK 5.0.101  
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
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="GodotOnReady" Version="0.0.4" />
  </ItemGroup>
</Project>
```

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

The source generator turns `_button` into `ButtonPath` by trimming all leading
`_` characters and capitalizing the first letter. The `[OnReadyGet]` generator
works for fields and properties.

It also works for `Resource` subclasses:

```cs
[OnReadyGet("res://Scene.tscn")] public PackedScene _scene;
```

### `[OnReadyGet(...)]` arguments:

* `Default = "..."` specifies a default for `ButtonPath`.
* `OrNull = true` causes the `_Ready()` method to use `GetNodeOrNull<Button>`
  instead of `GetNode<Button>`. It also adds a not-null check on `ButtonPath` to
  make sure no exceptions are thrown. This can be used to make **optional**
  connections.
* `Private = true` removes the `[Export]` attribute from `ButtonPath`, hiding it
  from the editor.

---

### `[OnReady]`

Using `OnReadyGet` generates a `public override void _Ready()` method. This
means you can't define it yourself. To run your own code during `_Ready`, mark
any zero-argument method with `[OnReady]`:

```cs
[OnReadyGet] private Button _button;
[OnReady] public void ConnectButtonOnReady()
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


[C# Source Generator]: https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/
