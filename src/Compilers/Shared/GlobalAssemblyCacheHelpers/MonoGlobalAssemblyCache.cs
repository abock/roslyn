// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Provides APIs to enumerate and look up assemblies stored in the Global Assembly Cache.
    /// </summary>
    internal sealed class MonoGlobalAssemblyCache : GlobalAssemblyCache
    {
        private static readonly string CorlibDirectory;
        private static readonly string GacDirectory;

        static MonoGlobalAssemblyCache()
        {
            var corlibAssemblyFile = typeof(object).GetTypeInfo().Assembly.Location;
            CorlibDirectory = Path.GetDirectoryName(corlibAssemblyFile);

            var systemAssemblyFile = typeof(Uri).GetTypeInfo().Assembly.Location;
            GacDirectory = Directory.GetParent(Path.GetDirectoryName(systemAssemblyFile)).Parent.FullName;
        }

        private static IEnumerable<string> GetGacAssemblyPaths(string gacPath, string name, Version version, byte[] publicKeyTokenBytes)
        {
            var fileName = name + ".dll";

            // First check to see if the assembly lives alongside mscorlib.dll.
            var corlibFriendPath = Path.Combine(CorlibDirectory, fileName);
            if (!File.Exists(corlibFriendPath))
            {
                // If not, check the Facades directory (e.g. this is where netstandard.dll will live)
                corlibFriendPath = Path.Combine(CorlibDirectory, "Facades", fileName);
            }

            // Yield and bail early if we find anything - it'll either be a Facade assembly or a
            // symlink into the GAC so we can avoid the more exhaustive work below.
            if (File.Exists(corlibFriendPath))
            {
                yield return corlibFriendPath;
                yield break;
            }

            var publicKeyToken = ToHexString(publicKeyTokenBytes);

            // Another bail fast attempt to peek directly into the GAC if we have version and public key
            if (version != null && publicKeyToken != null)
            {
                yield return Path.Combine(gacPath, name, version + "__" + publicKeyToken, fileName);
                yield break;
            }

            // Otherwise we need to iterate the GAC in the file system to find a match
            var gacAssemblyRootDir = new DirectoryInfo(Path.Combine(gacPath, name));
            if (!gacAssemblyRootDir.Exists)
            {
                yield break;
            }

            foreach (var assemblyDir in gacAssemblyRootDir.GetDirectories())
            {
                if (version != null && !assemblyDir.Name.StartsWith(version.ToString(), StringComparison.Ordinal))
                {
                    continue;
                }

                if (publicKeyToken != null && !assemblyDir.Name.EndsWith(publicKeyToken, StringComparison.Ordinal))
                {
                    continue;
                }

                var assemblyPath = Path.Combine(assemblyDir.ToString(), fileName);
                if (File.Exists(assemblyPath))
                {
                    yield return assemblyPath;
                }
            }
        }

        private static IEnumerable<(AssemblyIdentity Identity, string Path)> GetAssemblyIdentitiesAndPaths(AssemblyName name, ImmutableArray<ProcessorArchitecture> architectureFilter)
        {
            if (name == null)
            {
                return GetAssemblyIdentitiesAndPaths(null, null, null, architectureFilter);
            }

            return GetAssemblyIdentitiesAndPaths(name.Name, name.Version, name.GetPublicKeyToken(), architectureFilter);
        }

        private static IEnumerable<(AssemblyIdentity Identity, string Path)> GetAssemblyIdentitiesAndPaths(string name, Version version, byte[] publicKeyToken, ImmutableArray<ProcessorArchitecture> architectureFilter)
        {
            var assemblyPaths = GetGacAssemblyPaths(GacDirectory, name, version, publicKeyToken);

            foreach (var assemblyPath in assemblyPaths)
            {
                if (!File.Exists(assemblyPath))
                {
                    continue;
                }

                var gacAssemblyName = new AssemblyName(assemblyPath);

                if (gacAssemblyName.ProcessorArchitecture != ProcessorArchitecture.None &&
                    architectureFilter != default(ImmutableArray<ProcessorArchitecture>) &&
                    architectureFilter.Length > 0 &&
                    !architectureFilter.Contains(gacAssemblyName.ProcessorArchitecture))
                {
                    continue;
                }

                var assemblyIdentity = new AssemblyIdentity(
                    gacAssemblyName.Name,
                    gacAssemblyName.Version,
                    gacAssemblyName.CultureName,
                    ImmutableArray.Create(gacAssemblyName.GetPublicKeyToken()));

                yield return (assemblyIdentity, assemblyPath);
            }
        }

        public override IEnumerable<AssemblyIdentity> GetAssemblyIdentities(AssemblyName partialName, ImmutableArray<ProcessorArchitecture> architectureFilter = default(ImmutableArray<ProcessorArchitecture>))
        {
            return GetAssemblyIdentitiesAndPaths(partialName, architectureFilter).Select(identityAndPath => identityAndPath.Item1);
        }

        public override IEnumerable<AssemblyIdentity> GetAssemblyIdentities(string partialName = null, ImmutableArray<ProcessorArchitecture> architectureFilter = default(ImmutableArray<ProcessorArchitecture>))
        {
            AssemblyName name;
            try
            {
                name = (partialName == null) ? null : new AssemblyName(partialName);
            }
            catch
            {
                return SpecializedCollections.EmptyEnumerable<AssemblyIdentity>();
            }

            return GetAssemblyIdentities(name, architectureFilter);
        }

        public override IEnumerable<string> GetAssemblySimpleNames(ImmutableArray<ProcessorArchitecture> architectureFilter = default(ImmutableArray<ProcessorArchitecture>))
        {
            return GetAssemblyIdentitiesAndPaths(name: null, version: null, publicKeyToken: null, architectureFilter: architectureFilter).
                Select(identityAndPath => identityAndPath.Identity.Name).Distinct();
        }

        public override AssemblyIdentity ResolvePartialName(
            string displayName,
            out string location,
            ImmutableArray<ProcessorArchitecture> architectureFilter,
            CultureInfo preferredCulture)
        {
            if (displayName == null)
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            string cultureName = (preferredCulture != null && !preferredCulture.IsNeutralCulture) ? preferredCulture.Name : null;

            var assemblyName = new AssemblyName(displayName);
            AssemblyIdentity assemblyIdentity = null;

            location = null;
            bool isBestMatch = false;

            foreach (var identityAndPath in GetAssemblyIdentitiesAndPaths(assemblyName, architectureFilter))
            {
                var assemblyPath = identityAndPath.Path;

                if (!File.Exists(assemblyPath))
                {
                    continue;
                }

                var gacAssemblyName = new AssemblyName(assemblyPath);

                isBestMatch = cultureName == null || gacAssemblyName.CultureName == cultureName;
                bool isBetterMatch = location == null || isBestMatch;

                if (isBetterMatch)
                {
                    location = assemblyPath;
                    assemblyIdentity = identityAndPath.Identity;
                }

                if (isBestMatch)
                {
                    break;
                }
            }

            return assemblyIdentity;
        }

        static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            var sb = PooledObjects.PooledStringBuilder.GetInstance();
            foreach (var b in bytes)
            {
                sb.Builder.Append(b.ToString("x2"));
            }

            return sb.ToStringAndFree();
        }
    }
}
