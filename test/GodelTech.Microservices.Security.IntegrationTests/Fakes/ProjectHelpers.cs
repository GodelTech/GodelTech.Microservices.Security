using System.IO;
using System.Reflection;

namespace GodelTech.Microservices.Security.IntegrationTests.Fakes
{
    // todo: move to separate lib with tests??? (https://github.com/Aurochses/Aurochses.Extensions.Configuration/blob/master/src/Aurochses.Extensions.Configuration/ProjectHelpers.cs)
    // toto: move this lib to GodelTech
    public static class ProjectHelpers
    {
        public static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            // Get name of the target project which we want to test
            var projectName = startupAssembly.GetName().Name;

            // Get currently executing test project path
            var applicationBasePath = System.AppContext.BaseDirectory;

            // Find the path to the target project
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent;

                if (directoryInfo == null) break;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new DirectoryNotFoundException($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}