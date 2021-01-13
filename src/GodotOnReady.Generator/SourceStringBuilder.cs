using System;
using System.Text;

namespace GodotOnReady.Generator
{
	public class SourceStringBuilder
	{
		private readonly StringBuilder _sourceBuilder = new();
		private readonly StringBuilder _indentPrefix = new();

		public void Line(params string[] parts)
		{
			if (parts.Length != 0)
			{
				_sourceBuilder.Append(_indentPrefix);

				foreach (var s in parts)
				{
					_sourceBuilder.Append(s);
				}
			}

			_sourceBuilder.AppendLine();
		}

		public void BlockTab(Action writeInner)
		{
			BlockPrefix("\t", writeInner);
		}

		public void BlockPrefix(string delimiter, Action writeInner)
		{
			_indentPrefix.Append(delimiter);
			writeInner();
			_indentPrefix.Remove(_indentPrefix.Length - delimiter.Length, delimiter.Length);
		}

		public void BlockBrace(Action writeInner)
		{
			Line("{");
			BlockTab(writeInner);
			Line("}");
		}

		public void BlockDecl(Action writeInner)
		{
			Line("{");
			BlockTab(writeInner);
			Line("};");
		}

		public void NamespaceBlockBraceIfExists(string? ns, Action writeInner)
		{
			if (ns is { Length: > 0 })
			{
				Line("namespace ", ns);
				BlockBrace(writeInner);
			}
			else
			{
				BlockPrefix("", writeInner);
			}
		}

		public override string ToString()
		{
			return _sourceBuilder + Environment.NewLine +
				"/*" + Environment.NewLine +
				"---- END OF GENERATED SOURCE TEXT ----" + Environment.NewLine +
				"Roslyn doesn't clear the file when writing debug output for" + Environment.NewLine +
				"EmitCompilerGeneratedFiles, so I'm writing this message to" + Environment.NewLine +
				"make it more obvious what's going on when that happens." + Environment.NewLine +
				"*/" + Environment.NewLine;
		}
	}
}
