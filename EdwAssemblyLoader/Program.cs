using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EdwAssemblyLoader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EdwAssemblyLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //setup our DI
            IConfiguration _configuration = SetupConfiguration(args);

            var _serviceProvider = new ServiceCollection()
                .AddLogging(b =>
                {
                    b.AddConsole();
                    b.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton(_configuration)
                .AddSingleton<IAssemblyLoaderService, AssemblyLoaderService>()
                .BuildServiceProvider();


            //creating service instance
            var _loader = _serviceProvider.GetService<IAssemblyLoaderService>();

            //getting all directory's components (dlls)
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //get command line arguments 
            Arguments CommandLine = new Arguments(args);

            //get the required components
            List<string> components = _loader.GetAllComponents(path, CommandLine["excludeAssembly"]);

            //load components
            _loader.CreateObjects(components, CommandLine["excludeClass"], CommandLine["instances"]);
            Console.ReadKey();
        }

        private static IConfiguration SetupConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }
    }
}
