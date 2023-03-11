namespace StaticProxy.Abstractions
{
    public interface IFileOperations
    {
        string[] ReadFromFile(string path);
        void WriteToFile(string path, string[] content);
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
        void CreateDirectory(string path);
    }





}
