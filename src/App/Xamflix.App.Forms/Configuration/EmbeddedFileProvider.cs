// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Xamflix.App.Forms.Configuration
{
    /// <summary>
    ///     Looks up files using embedded resources in the specified assembly.
    ///     This file provider is case sensitive.
    /// </summary>
    public class EmbeddedFileProvider : IFileProvider
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars()
                                                                  .Where(c => c != '/' && c != '\\').ToArray();

        private readonly Assembly _assembly;
        private readonly string _baseNamespace;
        private readonly DateTimeOffset _lastModified;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbeddedFileProvider" /> class using the specified
        ///     assembly with the base namespace defaulting to the assembly name.
        /// </summary>
        /// <param name="assembly">The assembly that contains the embedded resources.</param>
        public EmbeddedFileProvider(Assembly assembly)
            : this(assembly, assembly?.GetName()?.Name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbeddedFileProvider" /> class using the specified
        ///     assembly and base namespace.
        /// </summary>
        /// <param name="assembly">The assembly that contains the embedded resources.</param>
        /// <param name="baseNamespace">The base namespace that contains the embedded resources.</param>
        public EmbeddedFileProvider(Assembly assembly, string baseNamespace)
        {
            if(assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            _baseNamespace = string.IsNullOrEmpty(baseNamespace) ? string.Empty : baseNamespace + ".";
            _assembly = assembly;

            _lastModified = DateTimeOffset.UtcNow;

            if(!string.IsNullOrEmpty(_assembly.Location))
            {
                try
                {
                    _lastModified = File.GetLastWriteTimeUtc(_assembly.Location);
                }
                catch(PathTooLongException)
                {
                }
                catch(UnauthorizedAccessException)
                {
                }
            }
        }

        /// <summary>
        ///     Locates a file at the given path.
        /// </summary>
        /// <param name="subPath">The path that identifies the file. </param>
        /// <returns>
        ///     The file information. Caller must check Exists property. A <see cref="NotFoundFileInfo" /> if the file could
        ///     not be found.
        /// </returns>
        public IFileInfo GetFileInfo(string subPath)
        {
            if(string.IsNullOrEmpty(subPath))
            {
                return new NotFoundFileInfo(subPath);
            }

            var builder = new StringBuilder(_baseNamespace.Length + subPath.Length);
            builder.Append(_baseNamespace);

            // Relative paths starting with a leading slash okay
            if(subPath.StartsWith("/", StringComparison.Ordinal))
            {
                builder.Append(subPath, 1, subPath.Length - 1);
            }
            else
            {
                builder.Append(subPath);
            }

            for(int i = _baseNamespace.Length; i < builder.Length; i++)
            {
                if(builder[i] == '/' || builder[i] == '\\')
                {
                    builder[i] = '.';
                }
            }

            var resourcePath = builder.ToString();
            if(HasInvalidPathChars(resourcePath))
            {
                return new NotFoundFileInfo(resourcePath);
            }

            string name = Path.GetFileName(subPath);
            //CAUTIONS: This is the only bit that's different from the Microsoft provided source code 
            //Even though embedded resources work just fine, but GetManifestResourceInfo doesn't work on UWP
            //https://github.com/dotnet/standard/issues/1782
            //if(_assembly.GetManifestResourceInfo(resourcePath) == null)
            if (_assembly.GetManifestResourceStream(resourcePath) == null)
            {
                return new NotFoundFileInfo(name);
            }

            return new EmbeddedResourceFileInfo(_assembly, resourcePath, name, _lastModified);
        }

        /// <summary>
        ///     Enumerate a directory at the given path, if any.
        ///     This file provider uses a flat directory structure. Everything under the base namespace is considered to be one
        ///     directory.
        /// </summary>
        /// <param name="subPath">The path that identifies the directory</param>
        /// <returns>
        ///     Contents of the directory. Caller must check Exists property. A <see cref="NotFoundDirectoryContents" /> if no
        ///     resources were found that match <paramref name="subPath" />
        /// </returns>
        public IDirectoryContents GetDirectoryContents(string subPath)
        {
            // The file name is assumed to be the remainder of the resource name.
            if(subPath == null)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            // EmbeddedFileProvider only supports a flat file structure at the base namespace.
            if(subPath.Length != 0 && !string.Equals(subPath, "/", StringComparison.Ordinal))
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var entries = new List<IFileInfo>();

            // TODO: The list of resources in an assembly isn't going to change. Consider caching.
            string[] resources = _assembly.GetManifestResourceNames();
            foreach(string resourceName in resources)
            {
                if(resourceName.StartsWith(_baseNamespace, StringComparison.Ordinal))
                {
                    entries.Add(new EmbeddedResourceFileInfo(
                                                             _assembly,
                                                             resourceName,
                                                             resourceName.Substring(_baseNamespace.Length),
                                                             _lastModified));
                }
            }

            return new EnumerableDirectoryContents(entries);
        }

        /// <summary>
        ///     Embedded files do not change.
        /// </summary>
        /// <param name="pattern">This parameter is ignored</param>
        /// <returns>A <see cref="NullChangeToken" /></returns>
        public IChangeToken Watch(string pattern)
        {
            return NullChangeToken.Singleton;
        }

        private static bool HasInvalidPathChars(string path)
        {
            return path.IndexOfAny(InvalidFileNameChars) != -1;
        }
    }
}