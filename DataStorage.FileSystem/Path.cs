using System;

namespace DataStorage.FileSystem
{
    public class PathProvider
    {
        public string UserPath { get; }  = Environment.GetEnvironmentVariable("AppData") + "\\" + "BankUser";
    }
}
