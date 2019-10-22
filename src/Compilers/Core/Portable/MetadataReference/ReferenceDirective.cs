// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Represents the value of #r reference along with its source location.
    /// </summary>
    internal struct ReferenceDirective
    {
        public readonly string File;
        public readonly Location Location;
        public readonly bool ScanForScriptGlobals;

        public ReferenceDirective(string file, Location location, bool scanForScriptGlobals)
        {
            Debug.Assert(file != null);
            Debug.Assert(location != null);

            File = file;
            Location = location;
            ScanForScriptGlobals = scanForScriptGlobals;
        }
    }
}
