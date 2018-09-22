﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Collections;

namespace Audacia.Typescript
{
    public class Class : Element, IEnumerable<IMemberOf<Class>>
    {
        public string Name { get; }

        public IList<string> Implements { get; } = new List<string>();

        public string Extends { get; set; }

        public Class(string name)
        {
            Name = name;
        }

        public ClassMemberList Members { get; } = new ClassMemberList();

        public IEnumerable<Property> Properties => Members.OfType<Property>()
            .Where(p => !p.HasGetter && !p.HasSetter)
            .ToArray();

        public IEnumerable<Property> PropertyAccessors => Members.OfType<Property>()
            .Where(p => p.HasGetter || p.HasSetter)
            .ToArray();

        public IEnumerable<Constructor> Constructors => Members.OfType<Constructor>().ToArray();

        public IEnumerable<Function> Functions => Members.OfType<Function>()
            .Where(m => !(m is Constructor))
            .ToArray();
        
        public IList<IModifier<Class>> Modifiers { get; } = new List<IModifier<Class>>(); 
        
        public void Add(IMemberOf<Class> member)
        {
            Members.Add(member);
        }

        public IEnumerator<IMemberOf<Class>> GetEnumerator() => Members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            var memberWritten = false;
            return builder
                .If(!string.IsNullOrWhiteSpace(Comment), b => b
                    .Append(new Comment(Comment), this).NewLine())
                .Join(Modifiers.Distinct().Select(m => m.ToString()), " ")
                .If(Modifiers.Any(), b => b.Append(' '))
                .Append("class ")
                .Append(Name)
                .If(!string.IsNullOrWhiteSpace(Extends), b => b
                    .Append(" extends ")
                    .Append(Extends))
                .If(Implements.Any(), b => b
                    .Append(" implements ")
                    .Join(Implements.Distinct(), ", "))
                .Append(" {")
                .Indent()
                .NewLine()
                .If(Properties.Any(), b =>
                {
                    b.Join(Properties, this, Environment.NewLine + builder.Indentation);

                    memberWritten = true;
                })
                .If(Constructors.Any(), b =>
                {
                    b.If(memberWritten, x => x.NewLine().NewLine())
                        .Join(Constructors, this, Environment.NewLine);

                    memberWritten = true;
                })
                .If(Functions.Any(), b =>
                {
                    b.If(memberWritten, x => x.NewLine().NewLine())
                        .Join(Functions, this, Environment.NewLine + Environment.NewLine + builder.Indentation);    

                    memberWritten = true;
                })
                .If(PropertyAccessors.Any(), b =>
                {
                    b.If(memberWritten, x => x.NewLine().NewLine())
                        .Join(PropertyAccessors, this, Environment.NewLine + builder.Indentation);
                })
                .Unindent()
                .NewLine()
                .Append("}");
        }
    }
}
