using MelonLoader.Utils;

#if NET6_0
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
#endif

namespace MelonLoader.Fixes
{
    public static class AssemblyResolveSearchFix
    {
        private static string[] SearchableDirectories;

        public static void Install()
        {
#if NET6_0
            SearchableDirectories = new string[]
            {
                MelonEnvironment.ModulesDirectory,
                MelonEnvironment.UserLibsDirectory,
                MelonEnvironment.PluginsDirectory,
                MelonEnvironment.ModsDirectory,
                MelonEnvironment.MelonBaseDirectory,
                MelonEnvironment.GameRootDirectory
            };

            AssemblyLoadContext.Default.Resolving += OnResolve;
#endif
        }

#if NET6_0
        private static Assembly TryLoad(AssemblyLoadContext alc, string path)
        {
            if (File.Exists(path))
            {
                MelonDebug.Msg($"[{nameof(AssemblyResolveSearchFix)}] Loading from {path}...");
                return alc.LoadFromAssemblyPath(path);
            }

            return null;
        }

        private static Assembly OnResolve(AssemblyLoadContext alc, AssemblyName name)
        {
            var filename = name.Name + ".dll";

            Assembly ret = null;
            foreach (string folder in SearchableDirectories)
            {
                ret = TryLoad(alc, Path.Combine(folder, filename));
                if (ret != null)
                    return ret;

                foreach (var childFolder in Directory.GetDirectories(folder))
                {
                    ret = TryLoad(alc, Path.Combine(childFolder, filename));
                    if (ret != null)
                        return ret;
                }
            }

            if (ret == null)
                MelonDebug.Msg($"[{nameof(AssemblyResolveSearchFix)}] Failed to find {filename} in any of the known search directories");
            return ret;
        }
#endif
    }
}