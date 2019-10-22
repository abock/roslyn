// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed partial class ReferenceDirectiveTriviaSyntax
    {
        public ReferenceDirectiveTriviaSyntax Update(
            SyntaxToken hashToken,
            SyntaxToken referenceKeyword,
            SyntaxToken file,
            SyntaxToken endOfDirectiveToken,
            bool isActive)
            => Update(
                hashToken,
                referenceKeyword,
                default,
                file,
                endOfDirectiveToken,
                isActive);
    }
}

namespace Microsoft.CodeAnalysis.CSharp
{
    public static partial class SyntaxFactory
    {

        /// <summary>Creates a new ReferenceDirectiveTriviaSyntax instance.</summary>
        public static ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(
            SyntaxToken hashToken,
            SyntaxToken referenceKeyword,
            SyntaxToken file,
            SyntaxToken endOfDirectiveToken,
            bool isActive)
            => ReferenceDirectiveTrivia(
                hashToken,
                referenceKeyword,
                default,
                file,
                endOfDirectiveToken,
                isActive);
    }
}
