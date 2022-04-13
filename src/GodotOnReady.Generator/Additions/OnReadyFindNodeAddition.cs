using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text;

namespace GodotOnReady.Generator.Additions
{
    public class OnReadyFindNodeAddition : PartialClassAddition
    {
        public MemberSymbol Member { get; }
        public string PropertyName { get; }

        public string? Name { get; }
        public bool Recursive { get; } = true;
        public bool Owned { get; } = true;
        public OnReadyFindNodeAddition(MemberAttributeSite memberSite) : base(memberSite.AttributeSite.Class)
        {
            Member = memberSite.Member;
            string pathName = Member.Name.TrimStart('_');

            PropertyName = pathName;

            foreach (var constructorArg in memberSite.AttributeSite.Attribute.ConstructorArguments)
            {
                if (constructorArg.Value is string name)
                {
                    Name = name;
                }
            }

            foreach (var namedArg in memberSite.AttributeSite.Attribute.NamedArguments)
            {
                switch (namedArg.Key)
                {
                    case "Path" when namedArg.Value.Value is string s:
                        Name = s;
                        break;
                    case "Recursive" when namedArg.Value.Value is bool b:
                        Recursive = b;
                        break;
                    case "Owned" when namedArg.Value.Value is bool b:
                        Owned = b;
                        break;
                }
            }
        }

        public override Action<SourceStringBuilder>? DeclarationWriter => g =>
        {
            g.Line();
            g.Line($"public {Member.Type.ToFullDisplayString()} ", PropertyName, " { get; set; }");
        };

        public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
        {
            g.Line();
            g.Line($"{PropertyName} = ({Member.Type.ToFullDisplayString()})FindNode(\"{Name}\", {Recursive.ToString().ToLower()}, {Owned.ToString().ToLower()});");
        };

    }
}
