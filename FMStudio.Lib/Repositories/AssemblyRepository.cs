﻿using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public class AssemblyRepository : IAssemblyRepository
    {
        public Task<AssemblyName> GetReferenceByName(Assembly assembly, string referenceName)
        {
            return Task.Run(() =>
            {
                return assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Equals(referenceName, StringComparison.OrdinalIgnoreCase));
            });
        }

        public Task<Assembly> LoadFromArchive(byte[] bytes)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    using (var stream = new MemoryStream(bytes))
                    using (var zf = ZipFile.Read(stream))
                    {
                        foreach (var entry in zf)
                        {
                            if (!entry.IsDirectory && entry.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                && !entry.FileName.EndsWith("FluentMigrator.dll", StringComparison.OrdinalIgnoreCase)
                                && !entry.FileName.EndsWith("FluentMigrator.Runner.dll", StringComparison.OrdinalIgnoreCase))
                            {
                                Debug.WriteLine(string.Format("Found dll '{0}', trying to load...", Path.GetFileName(entry.FileName)));

                                using (var ms = new MemoryStream())
                                {
                                    entry.Extract(ms);
                                    var entryBytes = ms.ToArray();

                                    var assembly = await LoadFromBytes(entryBytes);

                                    if (assembly != null)
                                    {
                                        stopwatch.Stop();

                                        Debug.WriteLine(string.Format("Loaded assembly from an archive in {0}ms", stopwatch.ElapsedMilliseconds));

                                        return assembly;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }

                return null;
            });
        }

        public Task<Assembly> LoadFromBytes(byte[] bytes)
        {
            return Task.Run(() =>
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.Load(bytes);

                    if (assembly.GetReferencedAssemblies().Any(r => r.Name.Equals("FluentMigrator", StringComparison.OrdinalIgnoreCase)))
                    {
                        return assembly;
                    }
                }
                catch { }

                return null;
            });
        }

        public Task<Assembly> LoadFromFile(string path)
        {
            return Task.Run(() =>
            {
                Debug.WriteLine(string.Format("Loading file '{0}'...", path));

                var bytes = File.ReadAllBytes(path);

                // Dll's can be loaded directly
                if (Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("File is a dll, attempting to load...");

                    return LoadFromBytes(bytes);
                }

                // Try to unpack the file if it is not a dll
                Debug.WriteLine("Could not load the file directly, trying to unpack as an archive...");

                var assembly = LoadFromArchive(bytes);
                if (assembly != null)
                    return assembly;

                // We could add more types here
                return null;
            });
        }
    }
}