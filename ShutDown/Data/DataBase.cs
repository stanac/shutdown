using System;
using System.IO;

namespace ShutDown.Data
{
    public abstract class DataBase
    {
        protected static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ShutDown_cJ51KAf2ckeBsFCJ2wP3pQ"); // to avoid collisions

        static DataBase()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }
    }
}
