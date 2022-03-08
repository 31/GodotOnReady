### `[GenerateDataSelectorEnum]`

Use this attribute to create an `enum` where each element is associated with an instance of a class that conatins data and potentially behaviors.

When you have an enum field or property in Godot with `[Export]` applied to it, Godot shows a nice dropdown that you can use in the editor.
This can be useful to select options when designing a level, or creating some other game logic.

The unfortunate thing is that an `enum` value is just that: a simple value. An integer with names.
You still need to write game logic that handles each possibility, and associates it with some value or some behavior.

Instead, you can use `GenerateDataSelectorEnum` to define each enum entry along with some data and behavior associated with each entry:

```cs
[GenerateDataSelectorEnum("SleepPreference")]
public partial class SleepPreferenceData
{
  private static readonly SleepPreferenceData
    Bed = new() { Comfort = 10, Feeling = "This mattress is comfortable." },
    Floor = new() { Comfort = 2, Feeling = "I regret this. There's a bed right over there!"},
    Microgravity = new() { Comfort = 4, Feeling = "Floating is... ok." };

  public string Feeling { get; set; }
  public int Comfort { get; set; }
  
  public void Exclaim() => GD.Print($"Time to sleep. {Feeling} I would rate that a {Comfort} out of 10.");
}
```

```cs
public class SpaceWorker : Node
{
  [Export] public SleepPreference Pref { get; set; }
  
  public override void _Ready()
  {
    Pref.GetData().Exclaim();
  }
}
```

When you create a `SpaceWorker` in a scene, you get a dropdown: 

![image](https://user-images.githubusercontent.com/331300/157167723-184d7699-e091-4198-a284-ddd3f26ec0cd.png)

And when launching the scene: `Time to sleep. I regret this. There's a bed right over there! I would rate that a 2 out of 10.`

## Why a source generator?

This is very possible to do without a source generator.
The generator simply makes it (much) more concise.

Here is one way, with a `switch` statement:

```cs
public enum SleepPreference { Bed, Floor, Microgravity }

public class SleepPreferenceData
{
  private static readonly SleepPreferenceData
    Bed = new() { Comfort = 1f },
    Floor = new() { Comfort = 0.6f },
    Microgravity = new() { Comfort = 0.6f };

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

In fact, that's essentially what the source generator generates!

Note that every name in `SleepPreference`, like `Microgravity`, is written four times!
To add a new enum value, every place needs to be updated in sync.
That can be a lot to keep track of.

You *can* make this more concise without using a source generator.
If you use a Dictionary instead of a switch, you cut out one repetition.
If you use reflection and add an attribute on each enum member, you cut out another.
However, attributes significantly limit the data types you can set.
There are some also some (most likely minor) performance penalties you pay with these approaches, and the source generators avoids these.

## Why is this called a "bonus" feature?

This isn't nearly as useful as `[OnReadyGet]` and the rest of the GodotOnReady functionality.
It hard to justify why it exists it in a brief readme, so I wrote a long explanation and put it here, instead!
