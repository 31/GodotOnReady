﻿using Microsoft.CodeAnalysis;
using System;

namespace GodotOnReady.Generator.Additions
{
	public abstract class PartialClassAddition
	{
		protected PartialClassAddition(INamedTypeSymbol @class)
		{
			Class = @class;
		}

		public INamedTypeSymbol Class { get; }

		public int Order { get; set; }

		public virtual Action<SourceStringBuilder>? DeclarationWriter => null;
		public virtual Action<SourceStringBuilder>? ConstructorStatementWriter => null;
		public virtual Action<SourceStringBuilder>? OnReadyStatementWriter => null;

		public virtual Action<SourceStringBuilder>? OutsideClassStatementWriter => null;
	}
}
