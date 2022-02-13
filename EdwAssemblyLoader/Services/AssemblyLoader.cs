using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdwAssemblyLoader.Services
{
    public interface IAssemblyLoaderService
    {
        void CreateObjects(List<string> components, string execluded = "", string instances = "");
        List<string> GetAllComponents(string targetDirectory, string execluded = "");
    }
    internal class AssemblyLoaderService : IAssemblyLoaderService
    {
        public readonly IServiceProvider _serviceProvider;
        public readonly IConfiguration _configuration;
        public AssemblyLoaderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void CreateObjects(List<string> components, string execluded = "", string instances = "")
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

        public List<string> GetAllComponents(string targetDirectory, string execluded = "")
        {
            execluded = execluded ?? "";
            List<string> fileEntries = Directory.GetFiles(targetDirectory).ToList();
            List<string> components = fileEntries.Where(file => file.Contains(".dll") && (execluded != "" ? !file.Contains(execluded) : true)).ToList();
            return components;
        }

        private void CreateInstance(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            object instance = null;
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Count() > 0)
                {
                    object[] objs = new object[parameters.Count()];
                    for (int i = 0; i < parameters.Count(); i++)
                    {
                        if (typeof(ILogger).IsAssignableFrom(parameters[i].ParameterType))
                        {
                            objs[i] = _serviceProvider.GetService(typeof(ILogger<dynamic>)) as ILogger<dynamic>;
                        }
                        else if (typeof(IConfiguration).IsAssignableFrom(parameters[i].ParameterType))
                        {
                            objs[i] = _serviceProvider.GetService<IConfiguration>();
                        }
                    }
                    instance = Activator.CreateInstance(type, objs);
                }
                else
                {
                    instance = Activator.CreateInstance(type);
                }
            }
            type.InvokeMember("Report", BindingFlags.InvokeMethod, null, instance, null);
        }

        private Assembly AssemblyResolveCallback(object sender, ResolveEventArgs args) => Assembly.GetExecutingAssembly();
    }
}
