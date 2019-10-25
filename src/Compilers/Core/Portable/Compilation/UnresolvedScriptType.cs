// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nullable enable

using System;

namespace Microsoft.CodeAnalysis
{
    internal sealed class UnresolvedScriptType
    {
        public readonly Type? ReflectionType;
        public readonly ITypeSymbol? TypeSymbol;
        public readonly string? TypeName;
        public readonly Func<Compilation, UnresolvedScriptType>? TypeFactory;

        public static UnresolvedScriptType? Create(Type? reflectionType)
            => reflectionType == null ? null : new UnresolvedScriptType(reflectionType);

        private UnresolvedScriptType(Type reflectionType)
            => ReflectionType = reflectionType;

        public static UnresolvedScriptType? Create(ITypeSymbol? typeSymbol)
            => typeSymbol == null ? null : new UnresolvedScriptType(typeSymbol);

        private UnresolvedScriptType(ITypeSymbol typeSymbol)
            => TypeSymbol = typeSymbol;

        public static UnresolvedScriptType? Create(string? typeName)
            => typeName == null ? null : new UnresolvedScriptType(typeName);

        private UnresolvedScriptType(string typeName)
            => TypeName = typeName;

        public static UnresolvedScriptType? Create(Func<Compilation, UnresolvedScriptType>? typeFactory)
            => typeFactory == null ? null : new UnresolvedScriptType(typeFactory);

        private UnresolvedScriptType(Func<Compilation, UnresolvedScriptType>? typeFactory)
            => TypeFactory = typeFactory;
    }
}
