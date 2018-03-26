using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace Trinity.TSL.Utf8.Encoder
{
    class Program
    {
        static string Encode(string input)
            => new Regex("\"([^\"]*)\"").Replace(input, match => "UTF8Encoded_" + Base64UrlEncoder.Encode(match.Groups[1].Value).Replace('-', '_') + "_UTF8Encoded");

        static string Decode(string input)
            => new Regex("UTF8Encoded_(.*?)_UTF8Encoded").Replace(input, match => Base64UrlEncoder.Decode(match.Groups[1].Value.Replace('_', '-')));

        static void VisitAllFiles(string rootPath, Action<string> action)
        {
            DirectoryInfo root = new DirectoryInfo(rootPath);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                VisitAllFiles(d.FullName, action);
            }
            foreach (FileInfo f in root.GetFiles())
            {
                action(f.FullName);
            }
        }

        static void Main(string[] args)
        {
            string type = args[0];
            string inputPath = args[1];
            if (type == "0")
            {
                File.WriteAllText(inputPath + ".tsl", Encode(File.ReadAllText(inputPath)));
            }
            else if (type == "1")
            {
                VisitAllFiles(inputPath.TrimEnd('\\', '/', '"'), path => File.WriteAllText(path, Decode(File.ReadAllText(path))));
            }
        }
    }
}
