using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Huffman
{
    public static class EncoderHuffman
    {
        private static Dictionary<char, string> _resultTable; // Словарь символ-двоичная строка
        private static List<string> _codes; // Двоичные коды символов
        private static double _codingPrice; // Цена кодирования

        /// <summary>
        /// Возвращает таблицу с закодированными символами
        /// </summary>
        /// <returns></returns>
        public static Dictionary<char, string> GetResultTable()
        {
            return _resultTable;
        }

        /// <summary>
        /// Возвращает цену кодирования
        /// </summary>
        /// <returns></returns>
        public static double GetCodingPrice()
        {
            return _codingPrice;
        }

        /// <summary>
        /// Считает цену кодирования
        /// </summary>
        /// <param name="probabilities"></param>
        private static void CalculateCodingPrice(Dictionary<char, double> probabilities)
        {
            _codingPrice = 0;
            foreach (var item in probabilities)
            {
                _codingPrice += item.Value * _resultTable[item.Key].Length;
            }
        }

        /// <summary>
        /// Считает вероятности символов
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Создает результативную таблицу
        /// </summary>
        /// <param name="p"></param>
        private static void CreateTable(List<double> p)
        {
            List<int> indexes = new List<int>();

            // Пока не останется 2 вероятности
            while (p.Count != 2)
            {
                // Складываем две последние вероятности
                p[p.Count - 2] += p[p.Count - 1];
                // Запоминаем сумму
                double value = p[p.Count - 2];
                // Удаляем последний элемент
                p.RemoveAt(p.Count - 1);
                // Сортируем получившийся массив по убыванию вероятностей
                p.Sort();
                p.Reverse();

                // Добавляем в массив индексов индекс получившейся суммы
                indexes.Add(p.IndexOf(value));
            }

            // Добавляем к коду 1 и 0
            indexes.Reverse();
            _codes.Add("1");
            _codes.Add("0");

            // Для каждого кода с индексом из массива индексов
            foreach (var index in indexes)
            {
                // Заменяем текущий код на два кода с 1 и 0 в конце
                _codes.Add(_codes[index] + "1");
                _codes.Add(_codes[index] + "0");
                _codes.RemoveAt(index);
            }
        }

        /// <summary>
        /// Записывает результат в файл
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encode"></param>
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

        /// <summary>
        /// Кодирует входную строку
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string Encode(string inputStr)
        {
            // Инициализируем таблицу с результатом
            _resultTable = new Dictionary<char, string>();
            _codes = new List<string>();

            // Считаем словарь с вероятностями и сортируем по убыванию вероятностей
            Dictionary<char, double> probabilities = GetCharsProbability(inputStr);
            probabilities = probabilities.OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            // Создаем результативную таблицу
            CreateTable(probabilities.Values.ToList());

            // Записываем коды символов в результат
            for (int i = 0; i < probabilities.Count; i++)
            {
                char character = probabilities.Keys.ToArray()[i];

                if (!_resultTable.ContainsKey(character))
                {
                    _resultTable.Add(character, _codes[i]);
                }
            }

            // Считаем цену кодирования
            CalculateCodingPrice(probabilities);

            // С помощью получившегося словаря кодов кодирует входное сообщение
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

            // Записываем результат в файл
            WriteResultToFile(inputStr, encode);
            return encode;
        }
    }
}