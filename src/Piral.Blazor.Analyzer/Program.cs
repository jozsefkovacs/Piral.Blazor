﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Piral.Blazor.Analyzer
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            var options = new Options(args);

            SetupLoader(options.Dir);

            var types = Assembly
                .LoadFrom(options.DllPath)
                .GetTypes();

            var routes = ExtractRoutes(types);
            var extensions = ExtractExtensions(types);
            Console.WriteLine(JsonSerializer.Serialize(new { routes, extensions }));
        }

        private static Dictionary<string, IReadOnlyCollection<string>> ExtractExtensions(Type[] types)
        {
            var extensions = types.MapAttributeValuesFor("PiralExtensionAttribute");
            return extensions;
        }

        private static IEnumerable<string> ExtractRoutes(Type[] types)
        {
            return types.SelectMany(t => t.GetFirstAttributeValue("RouteAttribute"));
        }

        private static void SetupLoader(string directory)
        {
            Loader.LoadFromDirectoryName = directory;
            AppDomain.CurrentDomain.AssemblyResolve += Loader.LoadDependency;
            AppDomain.CurrentDomain.TypeResolve += Loader.LoadDependency;
        }
    }
}
