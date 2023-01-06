using Godot;

public class Provider : Control
{
	public class Provided1 { }
	public class Provided2 : Provided1 { }
	public class Provided3 : Provided2 { }
	public class Provided4 : Provided3 { }

	public string StringField = "string in a field";
	public int IntProperty { get; set; } = 123;

	// Inaccessible by injection attribute: Supersuperclass is also Provided2.
	public Provided2 Superclass = new Provided2();

	public Provided4 Supersuperclass = new Provided4();
}
