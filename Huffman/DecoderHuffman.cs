using System.Collections.Generic;

namespace Huffman
{
    public static class DecoderHuffman
    {
        /// <summary>
        /// Декодирует входную строку с помощью словаря
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string Decode(string inputStr, Dictionary<string, char> table)
        {
            string decode = "";

            string letterCode = "";
            // Проходим по каждому числу в закодированной строке
            for (int i = 0; i < inputStr.Length; i++)
            {
                // Прибавляем число к коду
                letterCode += inputStr[i];
                // Если словарь содержит данный код
                if (table.ContainsKey(letterCode))
                {
                    // Добавляем символ с таким кодом в результат
                    decode += table[letterCode];
                    letterCode = "";
                }
            }

            decode = decode.Replace("&", " ");

            return decode;
        }
    }
}