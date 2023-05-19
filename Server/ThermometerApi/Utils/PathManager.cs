using System.Reflection;

namespace Utils
{
    public sealed class PathManager
    {
        public static string StartupDirectory => CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        public static string AboveStartupDirectory => CreateDirectory(Directory.GetParent(StartupDirectory)?.FullName);
        public static string LogDirectory => CreateDirectory(Path.Combine(AboveStartupDirectory, "Logs"));
        public static string ConfigurationDirectory => CreateDirectory(Path.Combine(AboveStartupDirectory, "Configurations"));

        private PathManager() { }

        private static string CreateDirectory(string? path)
        {
            if (path == null) {  throw new ArgumentNullException(nameof(path)); }

            if (Directory.Exists(path)) return path;
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
