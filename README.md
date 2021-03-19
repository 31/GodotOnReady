# GodotOnReady

**GodotOnReady** is a [C# Source Generator] that adds convenient `onready`-like
features to your C# scripts in Godot Mono without any reflection.

* [`[OnReadyGet]`](#onreadyget) - Load a strongly typed `Node` or `Resource`
  subclass into a field or property and automatically export a property for
  configurability in your Godot scene.
* [`[OnReady]`](#OnReady) - Execute any 0-argument method during `_Ready`.

Bonus feature:

* [`[GenerateDataSelectorEnum]`](#GenerateDataSelectorEnum) - Generate an enum
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
    <PackageReference Include="GodotOnReady" Version="0.0.6" />
  </ItemGroup>
</Project>
```

You may need to restart your IDE to navigate to generated sources and for the
generated code to affect code completion.

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

The source generator turns `_button` into `ButtonPath` by trimming all leading
`_` characters and capitalizing the first letter. The `[OnReadyGet]` generator
works for fields and properties.

It also works for `Resource` subclasses:

```cs
[OnReadyGet("res://Scene.tscn")] private PackedScene _scene;
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

### `[GenerateDataSelectorEnum]`

This utility allows you to concisely add some custom data to each member of an
`enum`.

An `enum` works nicely to show a choice in the Godot editor while authoring a
scene:

```cs
public enum SleepPreference { Bed, Floor, Microgravity }

public partial class Person : Node2D
{
  [Export] public SleepPreference Pref { get; set; }
  [OnReady] private void PrintPref()
  {
    GD.Print(Pref);
  }
}
```

Let's add a `float Comfort` factor to each enum member. Here's a way that uses
`switch` and provides a convenient `.GetData` method that can be called on the
enum:

```cs
public enum SleepPreference { Bed, Floor, Microgravity }

public class SleepPreferenceData
{
  private static readonly SleepPreferenceData
    Bed = new SleepPreferenceData { Comfort = 1f },
    Floor = new SleepPreferenceData { Comfort = 0.6f },
    Microgravity = new SleepPreferenceData { Comfort = 0.6f };

  public static SleepPreferenceData Get(SleepPreference p)
  {
    switch(p)
    {
      case SleepPreference.Bed: return Bed;
      case SleepPreference.Floor: return Floor;
      case SleepPreference.Microgravity: return Microgravity;
    }
    throw new ArgumentOutOfRangeException("key");
  }

  public float Comfort { get; set; }
}

public static class SleepPreferenceExtensions {
  public static SleepPreferenceData GetData(this SleepPreference p) => SleepPreferenceData.Get(p);
}
```

Usage:

```cs
public partial class Person : Node2D
{
  [Export] public SleepPreference Pref { get; set; }

  [OnReady] private void PrintPref()
  {
    GD.Print(Pref);
    GD.Print(Pref.GetData().Comfort);
  }
}
```

That's fine, but every value name in `SleepPreference` is written four times, in
a few separate sections of the code. To add a new enum value, every place needs
to be updated in sync.

> You can shrink this down to two times if you use a Dictionary instead of a
> switch. Down to only one time, if you use an attribute on each enum member
> (although attributes limit the data types you can set). There are
> theoretically some performance differences, with `switch` being the best, but
> these differences probably aren't important for most usage. The source
> generator uses `switch` only because it might as well use the theoretical best
> when generating code for people.

Instead, use `GenerateDataSelectorEnum` to generate all of that code based on
the data object field names:

```cs
[GenerateDataSelectorEnum("SleepPreference")]
public partial class SleepPreferenceData
{
  private static readonly SleepPreferenceData
    Bed = new SleepPreferenceData { Comfort = 1f },
    Floor = new SleepPreferenceData { Comfort = 0.6f },
    Microgravity = new SleepPreferenceData { Comfort = 0.6f };

  public float Comfort { get; set; }
}
```

To add a new enum value, just add another field to `SleepPreferenceData`.

If you use C# 9.0, you can also use *target-typed new* to shrink this:  
`Bed = new SleepPreferenceData { Comfort = 1f }`  
down to this:  
`Bed = new() { Comfort = 1f }`.

---

# License

GodotOnReady is licensed under the [MIT license](LICENSE).


[C# Source Generator]: https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/
