using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EdwAssemblyLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<string> components = new();
            Arguments CommandLine = new Arguments(args);
            components = GetAllComponents(path, CommandLine["excludeAssembly"]);
            CreateObjects(components, CommandLine["excludeClass"], CommandLine["instances"]);
            Console.ReadKey();
        }

        private static void CreateObjects(List<string> components, string execluded = "", string instances = "")
        {
            execluded = execluded ?? "";
            instances = instances ?? "";
            foreach (var component in components)
            {
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveCallback;
                var DLL = Assembly.LoadFrom(component);
                var attribute = DLL.CustomAttributes;
                var qualified = attribute.Any(attr => attr.AttributeType.Name == "EDW_Challenge");
                if (qualified)
                {
                    foreach (Type type in DLL.GetExportedTypes())
                    {
                        if (type.Name != execluded && type.GetInterfaces().Any(inte => inte.Name == "IEDWChallenge"))
                        {
                            int loop = 1;
                            if (instances != "")
                                loop = int.Parse(instances);
                            for (int i = 0; i < loop; i++)
                            {
                                Task.Factory.StartNew(() => CreateInstance(type)).Wait(1000);
                            }
                        }
                    }
                }
            }
        }

        private static void CreateInstance(Type type)
        {
            var c = Activator.CreateInstance(type);
            type.InvokeMember("Report", BindingFlags.InvokeMethod, null, c, null);
        }

        static Assembly AssemblyResolveCallback(object sender, ResolveEventArgs args) => Assembly.GetExecutingAssembly();

        static List<string> GetAllComponents(string targetDirectory, string execluded = "")
        {
            execluded = execluded ?? "";
            List<string> fileEntries = Directory.GetFiles(targetDirectory).ToList();
            List<string> components = fileEntries.Where(file => file.Contains(".dll") && (execluded != "" ? !file.Contains(execluded) : true)).ToList();
            return components;
        }
    }
}
