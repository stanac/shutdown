using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ShutDown.Data
{
    public class Log : DataBase
    {
        private static readonly object _sync = new object();

        public static void LogErrorAndDisplayMessageBox(string message, Exception ex = null)
        {
            string title = "Application error";
            var guid = Error(message, ex);
            string id = guid.ToString();
            if (guid != default(Guid))
            {
                string content = title + " occurred and it was logged on you disk with following id: " + id;
                MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string content = "Error occurred and were not able to log it, error message: " + (message ?? "message-not-provided") +
                    "Error details: " + GetExceptionText(ex);
                MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static Guid Error(string message, Exception ex = null)
        {
            lock (_sync)
            {
                try
                {
                    Guid id = Guid.NewGuid();
                    var file = GetFileName();
                    using (Stream fs = new FileStream(file, FileMode.Append))
                    using (TextWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        writer.WriteLine();
                        writer.WriteLine("---------------------------------------------------- " + id);
                        writer.WriteLine("Error - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss - " + (message ?? "no-message-provided")));
                        writer.WriteLine(GetExceptionText(ex));
                        writer.Flush();
                    }
                    return id;
                }
                catch { }
                return Guid.Empty;
            }
        }

        private static string GetFileName()
        {
            var now = DateTime.Now;
            return Path.Combine(FolderPath, $"ShutDown-{now.Year.ToString("0000")}-{now.Month.ToString("00")}-{now.Day.ToString("00")}.log");
        }

        private static string GetExceptionText(Exception ex, int exceptionLevel = 1)
        {
            string depth = string.Join(" ", Enumerable.Range(0, exceptionLevel).Select(x => " "));
            string data = depth;
            if (ex != null)
            {
                data += ex.GetType().FullName + Environment.NewLine;
                if (ex.Message != null)
                {
                    data += depth + "Message: " + ex.Message + Environment.NewLine; 
                }
                if (ex.InnerException != null)
                {
                    data += "Inner exception: " + Environment.NewLine;
                    data += GetExceptionText(ex.InnerException, exceptionLevel + 1);
                }
            }
            return data;
        }
    }
}
