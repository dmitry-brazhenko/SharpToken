using System;
using System.IO;


namespace SharpToken.Benchmark
{
    internal sealed class FileHelper
    {
        public static string ReadFile(string path)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, path));
        }

        public static T ReadJson<T>(string path)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(ReadFile(path));
        }

        public static string[] ReadFileLines(string path)
        {
            return File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, path));
        }
    }
}
