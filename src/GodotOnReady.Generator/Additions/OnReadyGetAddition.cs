namespace GodotOnReady.Generator.Additions
{
	public abstract class OnReadyGetAddition : PartialClassAddition
	{
		public MemberSymbol Member { get; }

		/// <summary>
		/// The name of the export property, without any suffix that differentiates it from the main
		/// member (such as Resource or Path).
		/// </summary>
		public string SuffixlessExportPropertyName { get; }

		public string? Path { get; }
		public bool Export { get; }
		public bool OrNull { get; }
		public string? Property { get; }

		public OnReadyGetAddition(MemberAttributeSite memberSite)
			: base(memberSite.AttributeSite.Class)
		{
			Member = memberSite.Member;

			// Handle field name convention: _ prefix with lowercase name.
			string pathName = Member.Name.TrimStart('_');
			pathName =
				pathName[0].ToString().ToUpperInvariant() +
				pathName.Substring(1);

			SuffixlessExportPropertyName = pathName;

			foreach (var constructorArg in memberSite.AttributeSite.Attribute.ConstructorArguments)
			{
				if (constructorArg.Value is string path)
				{
					Path = path;
				}
			}

			foreach (var namedArg in memberSite.AttributeSite.Attribute.NamedArguments)
			{
				switch (namedArg.Key)
				{
					case "Path" when namedArg.Value.Value is string s:
						Path = s;
						break;
					case "Export" when namedArg.Value.Value is bool b:
						Export = b;
						break;
					case "OrNull" when namedArg.Value.Value is bool b:
						OrNull = b;
						break;
					case "Property" when namedArg.Value.Value is string s:
						Property = s;
						break;
				}
			}
		}

		protected void WriteMemberNullCheck(SourceStringBuilder g, string exportMemberName)
		{
			g.Line("if (", Member.Name, " == null)");
			g.BlockBrace(() =>
			{
				g.Line(
					"throw new NullReferenceException($\"",
					"'{((Resource)GetScript()).ResourcePath}' member '",
					Member.Name,
					"' is unexpectedly null in '{GetPath()}', '{this}'. Ensure '",
					exportMemberName,
					"' is set correctly, or set [OnReadyGet(OrNull = true)] to allow null.\");");
			});
		}
	}
}
