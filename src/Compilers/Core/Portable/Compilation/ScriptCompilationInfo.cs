// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Microsoft.CodeAnalysis
{
    public abstract class ScriptCompilationInfo
    {
        internal UnresolvedScriptType UnresolvedScriptReturnType { get; }
        internal UnresolvedScriptType UnresolvedScriptGlobalsType { get; }

        public Type ReturnType => UnresolvedScriptReturnType?.ReflectionType ?? typeof(object);
        public Type GlobalsType => UnresolvedScriptGlobalsType?.ReflectionType;

        internal ScriptCompilationInfo(
            UnresolvedScriptType unresolvedScriptReturnType,
            UnresolvedScriptType unresolvedScriptGlobalsType)
        {
            UnresolvedScriptReturnType = unresolvedScriptReturnType;
            UnresolvedScriptGlobalsType = unresolvedScriptGlobalsType;
        }

        public Compilation PreviousScriptCompilation => CommonPreviousScriptCompilation;
        internal abstract Compilation CommonPreviousScriptCompilation { get; }

        public ScriptCompilationInfo WithPreviousScriptCompilation(Compilation compilation) => CommonWithPreviousScriptCompilation(compilation);
        internal abstract ScriptCompilationInfo CommonWithPreviousScriptCompilation(Compilation compilation);
    }
}
