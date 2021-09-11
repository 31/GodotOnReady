### `[GenerateDataSelectorEnum]`

## The problem:

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

> You can shrink this down from three to two times if you use a Dictionary
> instead of a switch. Down to only one time, if you use an attribute on each
> enum member (although attributes limit the data types you can set). There are
> theoretically some performance differences, with `switch` *maybe* being the
> best, but these differences probably aren't important for most usage. The
> source generator uses `switch` rather than a dictionary somewhat arbitrarily.

## The feature!

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
