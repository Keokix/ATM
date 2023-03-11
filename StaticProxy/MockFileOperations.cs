using StaticProxy.Abstractions;

namespace StaticProxy
{
    public class MockFileOperation : IFileOperations
    {
        public string[] FileContent { get; set; }
        public string FileName { get; set; }

        public string[] AllFiles { get; set; }

        public void CreateDirectory(string path)
        {
            
        }

        public string[] GetDirectories(string path)
        {
            return AllFiles;
        }

        public string[] GetFiles(string path)
        {
            return AllFiles;
        }

        public string[] ReadFromFile(string path)
        {
            return FileContent;
        }

        public void WriteToFile(string path, string[] content)
        {
            FileContent = content;
            FileName = path;
        }
    }
}
