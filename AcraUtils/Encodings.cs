using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AcraUtils
{
    public static class Encodings
    {
        public static string GetUTF8string(string isoMessage)
        {
            var enc1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            return Encoding.UTF8.GetString(enc1252.GetBytes(isoMessage));
        }

        public static string GetCP1252string(string utfMessage)
        {
            try
            {
                var enc1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
                Console.WriteLine(enc1252);
                var utf8 = Encoding.UTF8;
                Console.WriteLine(utf8);
                return enc1252.GetString(utf8.GetBytes(utfMessage));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string NewGetCP1252string(string utfMessage)
        {
            try
            {
                byte[] bytes = new byte[utfMessage.Length * sizeof(char)];
                var utf8 = Encoding.UTF8;
                System.Buffer.BlockCopy(utfMessage.ToCharArray(), 0, bytes, 0, bytes.Length);
                Encoding w1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
                byte[] output = Encoding.Convert(utf8, w1252, bytes);
               return w1252.GetString(output);
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string SkipControlCharactersFromString(string message)
        {
            string result = Regex.Replace(message, @"\\u001d|\\u001c|\\n", "");

            return new string(result.Where(c => !char.IsControl(c)).ToArray());
        }
    }
}
