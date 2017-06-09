﻿/*
    Copyright (C) 2014-2017 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace dnSpy.Debugger.DotNet.Metadata.Impl {
	sealed class DmdPointerType : DmdTypeBase {
		public override DmdTypeSignatureKind TypeSignatureKind => DmdTypeSignatureKind.Pointer;
		public override DmdTypeScope TypeScope => SkipElementTypes().TypeScope;
		public override DmdModule Module => SkipElementTypes().Module;
		public override string Namespace => SkipElementTypes().Namespace;
		public override DmdType BaseType => null;
		public override StructLayoutAttribute StructLayoutAttribute => null;
		public override DmdTypeAttributes Attributes => DmdTypeAttributes.NotPublic | DmdTypeAttributes.AutoLayout | DmdTypeAttributes.Class | DmdTypeAttributes.AnsiClass;
		public override string Name => DmdMemberFormatter.FormatName(this);
		public override DmdType DeclaringType => null;
		public override int MetadataToken => 0x02000000;
		public override bool IsMetadataReference { get; }

		readonly DmdTypeBase elementType;

		public DmdPointerType(DmdTypeBase elementType, IList<DmdCustomModifier> customModifiers) : base(customModifiers) {
			this.elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
			IsMetadataReference = elementType.IsMetadataReference;
			IsFullyResolved = elementType.IsFullyResolved;
		}

		public override DmdType WithCustomModifiers(IList<DmdCustomModifier> customModifiers) => AppDomain.MakePointerType(elementType, customModifiers);
		public override DmdType WithoutCustomModifiers() => GetCustomModifiers().Count == 0 ? this : AppDomain.MakePointerType(elementType, null);
		public override DmdType GetElementType() => elementType;

		protected override DmdType ResolveNoThrowCore() {
			if (!IsMetadataReference)
				return this;
			var newElementType = elementType.ResolveNoThrow();
			if ((object)newElementType != null)
				return AppDomain.MakePointerType(newElementType, GetCustomModifiers());
			return null;
		}

		public override bool IsFullyResolved { get; }
		public override DmdTypeBase FullResolve() {
			if (IsFullyResolved)
				return this;
			var et = elementType.FullResolve();
			if ((object)et != null)
				return (DmdTypeBase)AppDomain.MakePointerType(et, GetCustomModifiers());
			return null;
		}

		protected override IList<DmdType> ReadDeclaredInterfaces() => null;
	}
}
