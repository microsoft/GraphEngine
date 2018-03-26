using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.IdentityModel.Tokens;
using Trinity.Utilities;

namespace GraphEngine.TSL.Utf8
{
    public class DecoderTask : Microsoft.Build.Utilities.Task
    {
        private static string Decode(string input)
            => new Regex("UTF8Encoded_(.*?)_UTF8Encoded").Replace(input, match => Base64UrlEncoder.Decode(match.Groups[1].Value.Replace('_', '-')));

        [Required]
        public string SourceFile { get; set; }

        public override bool Execute()
        {
            try
            {
                File.WriteAllText(SourceFile, Decode(File.ReadAllText(SourceFile)));
                return true;
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
