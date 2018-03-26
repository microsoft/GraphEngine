using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.IdentityModel.Tokens;
using Trinity.Utilities;

namespace GraphEngine.TSL.Utf8
{
    public class EncoderTask : Microsoft.Build.Utilities.Task
    {
        private static string Encode(string input)
            => new Regex("\"([^\"]*)\"").Replace(input, match => "UTF8Encoded_" + Base64UrlEncoder.Encode(match.Groups[1].Value).Replace('-', '_') + "_UTF8Encoded");

        [Required]
        public string Input { get; set; }
        [Output]
        public string Output { get; set; }

        public override bool Execute()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Output));
                File.WriteAllText(Output, Encode(File.ReadAllText(Input)));
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
