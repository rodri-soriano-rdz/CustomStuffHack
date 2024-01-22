using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DuckGame.CustomStuffHack.Utility
{
    internal static class AssemblyLoader
    {
        /// <summary>
        /// Attempts to load assembly from content/dlls.
        /// </summary>
        /// <returns>Loaded assembly. Or null, if no assembly could be loaded.</returns>
        public static Assembly TryLoadAssembly(string assemblyName)
        {
            int commaPos = assemblyName.IndexOf(",", StringComparison.Ordinal);

            if (commaPos > -1)
            {
                assemblyName = assemblyName.Substring(0, commaPos);
            }

            // Try to check if assembly was already loaded.
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in loadedAssemblies)
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return assembly;
                }
            }

            // Get assembly from contents folder.
            string assemblyPath = CSH_Mod.GetContentPath($"dlls/{assemblyName}.dll");

            // Return null if assembly does not exist.
            if (!File.Exists(assemblyPath))
            {
                return null;
            }

            Assembly loadedAssembly;

            // Try to load assembly.
            try
            {
                loadedAssembly = Assembly.LoadFrom(assemblyPath);
            }
            catch
            {
                loadedAssembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            }

            // Return null if path does not exist.
            if (loadedAssembly == null)
            {
                return null;
            }

            return loadedAssembly;
        }
    }
}