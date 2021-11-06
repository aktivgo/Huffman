using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Huffman
{
    public static class EncoderHuffman
    {
        private static Dictionary<char, string> _resultTable;
        private static List<string> _codes;
        private static double _codingPrice;

        public static Dictionary<char, string> GetResultTable()
        {
            return _resultTable;
        }

        public static double GetCodingPrice()
        {
            return _codingPrice;
        }

        private static void CalculateCodingPrice(Dictionary<char, double> probabilities)
        {
            _codingPrice = 0;
            foreach (var item in probabilities)
            {
                _codingPrice += item.Value * _resultTable[item.Key].Length;
            }
        }

        private static Dictionary<char, double> GetCharsProbability(string str)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            foreach (var character in str)
            {
                if (char.IsWhiteSpace(character))
                {
                    if (!result.ContainsKey('&'))
                    {
                        result.Add('&', 1);
                    }
                    else
                    {
                        result['&']++;
                    }
                    continue;
                }

                if (result.ContainsKey(character))
                {
                    result[character]++;
                }
                else
                {
                    result.Add(character, 1);
                }
            }

            foreach (char key in result.Keys.ToArray())
            {
                result[key] /= str.Length;
            }

            return result;
        }

        private static void CreateTable(List<double> p)
        {
            List<int> indexes = new List<int>();

            while (p.Count != 2)
            {
                p[p.Count - 2] += p[p.Count - 1];
                p.RemoveAt(p.Count - 1);
                p.Sort();
                p.Reverse();
                
                indexes.Add(p.IndexOf(p[p.Count - 2]));
            }

            indexes.Reverse();
            _codes.Add("1");
            _codes.Add("0");

            foreach (var index in indexes)
            {
                _codes.Add(_codes[index] + "1");
                _codes.Add(_codes[index] + "0");
                _codes.RemoveAt(index);
            }
        }

        private static void WriteResultToFile(string str, string encode)
        {
            string path = "decode/";
            string fileName = "test_" + str.Substring(0, 6) + ".txt";

            using (FileStream fstream = new FileStream(path + fileName, FileMode.OpenOrCreate))
            {
                byte[] array = Encoding.Default.GetBytes(encode + "\r\n");
                fstream.Write(array, 0, array.Length);
                string table = "";
                foreach (var item in _resultTable)
                {
                    table += item.Key + " " + item.Value + "\r\n";
                }

                table = table.Remove(table.Length - 2);
                array = Encoding.Default.GetBytes(table);
                fstream.Write(array, 0, array.Length);
            }

            Console.WriteLine("Результат сохранен в файл " + fileName);
        }

        public static string Encode(string inputStr)
        {
            _resultTable = new Dictionary<char, string>();
            _codes = new List<string>();

            Dictionary<char, double> probabilities = GetCharsProbability(inputStr);
            probabilities = probabilities.OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            CreateTable(probabilities.Values.ToList());

            for (int i = 0; i < probabilities.Count; i++)
            {
                char character = probabilities.Keys.ToArray()[i];

                if (!_resultTable.ContainsKey(character))
                {
                    _resultTable.Add(character, _codes[i]);
                }
            }

            CalculateCodingPrice(probabilities);

            string encode = "";
            foreach (var character in inputStr)
            {
                if (char.IsWhiteSpace(character))
                {
                    encode += _codes[probabilities.Keys.ToList().IndexOf('&')];
                    continue;
                }

                encode += _codes[probabilities.Keys.ToList().IndexOf(character)];
            }

            WriteResultToFile(inputStr, encode);
            return encode;
        }
    }
}