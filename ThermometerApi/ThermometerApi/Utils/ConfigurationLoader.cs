using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Utils
{
    public sealed class ConfigurationLoader
    {
        private ConfigurationLoader() { }

        public static T Get<T>(string fileName, T? @default = default)
        {
            string file = Path.Combine(PathManager.ConfigurationDirectory, fileName);

            if (!File.Exists(file))
                Set(file, @default);

            T? result = JsonConvert.DeserializeObject<T?>(File.ReadAllText(file));

            return result ?? @default ?? (T)Activator.CreateInstance(typeof(T))!;
        }

        private static void Set<T>(string file, T? configration = default)
        {
            if (configration is null || configration.Equals(default))
                File.WriteAllText(file, JsonConvert.SerializeObject(Activator.CreateInstance(typeof(T)), Formatting.Indented));
            else
                File.WriteAllText(file, JsonConvert.SerializeObject(configration, Formatting.Indented));
        }

    }
}
