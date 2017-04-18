using System;
using System.IO;
using System.Reflection;

namespace Trinity.Tools
{
    class TemplateGen
    {
        public static void Write(string entry, string root)
        {
            var test_name = Path.GetFileName(root);
            var target = Path.Combine(root, Path.GetFileName(entry)).Replace("test_name", test_name);
            var txt = File.ReadAllText(entry).Replace("test_name", test_name);
            File.WriteAllText(target, txt);
        }

        public static void Main(string[] args)
        {
            try
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var template = Path.Combine(dir, "template");
                var target = args[0];

                Directory.CreateDirectory(target);
                foreach(var dirent in Directory.GetFileSystemEntries(template))
                {
                    if (File.Exists(dirent))
                    {
                        Write(dirent, target);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Failed to create template: {ex.Message} \n {ex.StackTrace}"); }

        }
    }
}