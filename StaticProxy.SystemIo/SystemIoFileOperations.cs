using System.IO;
using StaticProxy.Abstractions;

namespace StaticProxy.SystemIo
{
    public class SystemIoFileOperations : IFileOperations
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
        public string[] ReadFromFile(string path)
        {
            return File.ReadAllLines(path);
        }

        public void WriteToFile(string path, string[] content)
        {
            File.WriteAllLines(path, content);
        }

    }
}
