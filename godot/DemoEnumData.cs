using GodotOnReady.Attributes;

namespace GodotOnReadyDev
{
	[GenerateDataSelectorEnum("DemoEnum")]
	public partial class DemoEnumData
	{
		private static readonly DemoEnumData
			A = new DemoEnumData
			{
				Extended = "AaaaaAAaaAAAAA"
			},
			B = new DemoEnumData
			{
				Extended = "BbbbbBbbbbbb"
			};

		public string Extended { get; private set; }
	}
}
