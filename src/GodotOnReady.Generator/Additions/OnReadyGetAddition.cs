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

		public string? Default { get; }
		public bool OrNull { get; }
		public bool Private { get; }

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
				if (constructorArg.Value is string @default)
				{
					Default = @default;
				}
			}

			foreach (var namedArg in memberSite.AttributeSite.Attribute.NamedArguments)
			{
				switch (namedArg.Key)
				{
					case "Default" when namedArg.Value.Value is string s:
						Default = s;
						break;
					case "OrNull" when namedArg.Value.Value is bool b:
						OrNull = b;
						break;
					case "Private" when namedArg.Value.Value is bool b:
						Private = b;
						break;
				}
			}
		}
	}
}
