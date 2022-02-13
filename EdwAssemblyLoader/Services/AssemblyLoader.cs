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
    /// <summary>
    /// Assembly Loader Interface
    /// </summary>
    public interface IAssemblyLoaderService
    {
        List<string> GetAllComponents(string targetDirectory, string execluded = "");
        void CreateObjects(List<string> components, string execluded = "", string instances = "");
    }
    /// <summary>
    /// Assembly Loader Class
    /// </summary>
    internal class AssemblyLoaderService : IAssemblyLoaderService
    {
        /// <summary>
        /// private props
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _helper;
        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="serviceProvider">using to get ILogger // we can also pass Ilogger to the constructor directly</param>
        /// <param name="configuration"></param>
        public AssemblyLoaderService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _helper = _configuration.GetSection("Helper");
        }
       
        /// <summary>
        /// get allowed .dlls to load
        /// </summary>
        /// <param name="targetDirectory">location path for the .dlls</param>
        /// <param name="execluded">execluded components will not be hit</param>
        /// <returns></returns>
        public List<string> GetAllComponents(string targetDirectory, string execluded = "")
        {
            execluded = execluded ?? "";
            List<string> fileEntries = Directory.GetFiles(targetDirectory).ToList();
            List<string> components = fileEntries.Where(file => file.Contains(".dll") && (execluded != "" ? !file.Contains(execluded) : true)).ToList();
            return components;
        }

        /// <summary>
        /// Creating instances from all required classes
        /// </summary>
        /// <param name="components">all allowed .dll components</param>
        /// <param name="execluded">execluded classes will not be hit</param>
        /// <param name="instances">number of instances for created objects</param>
        public void CreateObjects(List<string> components, string execluded = "", string instances = "")
        {
            execluded = execluded ?? "";
            instances = instances ?? "";
            foreach (var component in components)
            {
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveCallback;
                var DLL = Assembly.LoadFrom(component);
                var attribute = DLL.CustomAttributes;
                var qualified = attribute.Any(attr => attr.AttributeType.Name == _helper.GetSection("Attribute").Value);
                if (qualified)
                {
                    foreach (Type type in DLL.GetExportedTypes())
                    {
                        if (type.Name != execluded && type.GetInterfaces().Any(inte => inte.Name == _helper.GetSection("Interface").Value))
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
                            objs[i] = _configuration;
                        }
                        else if (typeof(ILoggerFactory).IsAssignableFrom(parameters[i].ParameterType))
                        {
                            objs[i] = _serviceProvider.GetService(typeof(ILoggerFactory));
                        }
                    }
                    instance = Activator.CreateInstance(type, objs);
                }
                else
                {
                    instance = Activator.CreateInstance(type);
                }
            }
            //we can execute more than one method, just we have to add it into Methods list in appsettings.json
            var methods = _helper.GetSection("Methods").GetChildren().ToList();
            foreach (var method in methods)
            {
                type.InvokeMember(method.Value, BindingFlags.InvokeMethod, null, instance, null);
            }
        }

        private Assembly AssemblyResolveCallback(object sender, ResolveEventArgs args) => Assembly.GetExecutingAssembly();
    }
}
