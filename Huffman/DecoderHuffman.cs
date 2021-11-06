using System.Collections.Generic;

namespace Huffman
{
    public static class DecoderHuffman
    {
        public static string Decode(string inputStr, Dictionary<string, char> table)
        {
            string decode = "";

            string letterCode = "";
            for (int i = 0; i < inputStr.Length; i++)
            {
                letterCode += inputStr[i];
                if (table.ContainsKey(letterCode))
                {
                    decode += table[letterCode];
                    letterCode = "";
                }
            }

            decode = decode.Replace("&", " ");

            return decode;
        }
    }
}