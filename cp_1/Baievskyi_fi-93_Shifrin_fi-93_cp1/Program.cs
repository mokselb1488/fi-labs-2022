using System.Text.RegularExpressions;
using System.Text;

namespace CP_1_SymCrypto
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var path = @"D:\Download\KPI\S6\Симметрическая криптография\Лабораторные\КП1\CP_1_SymCrypto\Files\";
            var file_name = "OpenText.txt";

            var monograms_with_space = CountMonogram(path, file_name, true);
            var bigrams_with_space = CountBigram(path, file_name, true);
            var bigram_with_intersection_with_space = CountBigramOrdered(path, file_name, true);

            var monograms = CountMonogram(path, file_name, false);
            var bigrams = CountBigram(path, file_name, false);
            var bigram_with_intersection = CountBigramOrdered(path, file_name, false);


            var monogram_with_space_entropy = CalculateEntropy(monograms_with_space);
            var monogram_without_space_entropy = CalculateEntropy(monograms);
            
            var bigram_with_space_entropy = CalculateEntropy(bigrams_with_space);
            var bigram_without_space_entropy = CalculateEntropy(bigrams);
            
            var ordered_bigram_with_space_entropy = CalculateEntropy(bigram_with_intersection_with_space);
            var ordered_bigram_without_space_entropy = CalculateEntropy(bigram_with_intersection);


            PrintResult(path, @"monograms_with_space.txt", monograms_with_space);
            PrintResult(path, @"bigrams_with_space.txt", bigrams_with_space);
            PrintResult(path, @"bigram_with_intersection_with_space.txt", bigram_with_intersection_with_space);
            PrintResult(path, @"monograms.txt", monograms);
            PrintResult(path, @"bigrams.txt", bigrams);
            PrintResult(path, @"bigram_with_intersection.txt", bigram_with_intersection);


            PrintResult(path, @"monogram_with_space_entropy.txt", monogram_with_space_entropy);
            PrintResult(path, @"monogram_without_space_entropy.txt", monogram_without_space_entropy);
            PrintResult(path, @"bigram_with_space_entropy.txt", bigram_with_space_entropy);
            PrintResult(path, @"bigram_without_space_entropy.txt", bigram_without_space_entropy);
            PrintResult(path, @"ordered_bigram_with_space_entropy.txt", ordered_bigram_with_space_entropy);
            PrintResult(path, @"ordered_bigram_without_space_entropy.txt", ordered_bigram_without_space_entropy);

            Console.WriteLine(String.Format("Энтропия монограммы с пробелом равна {0}", monogram_with_space_entropy));
            Console.WriteLine(String.Format("Энтропия монограммы без пробела  равна {0}", monogram_without_space_entropy));
            Console.WriteLine(String.Format("Энтропия биграммы с пробелом равна {0}", bigram_with_space_entropy));
            Console.WriteLine(String.Format("Энтропия биграммы без пробела равна {0}", bigram_without_space_entropy));
            Console.WriteLine(String.Format("Энтропия биграммы с нахлёстом с пробелами равна {0}", ordered_bigram_with_space_entropy));
            Console.WriteLine(String.Format("Энтропия биграммы с нахлёстом без пробелов равна {0}", ordered_bigram_without_space_entropy));
        }

        static void PrintResult(string path, string file_name, SortedDictionary<String, Double> dictionary)
        {
            var result = new StringBuilder();
            
            foreach (var n_gram in dictionary)
                result.Append(String.Format("| {0} | {1:N5} |\n", n_gram.Key, n_gram.Value));

            File.WriteAllText(path + file_name, result.ToString());

            Console.WriteLine("Данные успешно записаны.");
        }

        static void PrintResult(string path, string file_name, Double entropy)
        {
            File.WriteAllText(path + file_name, String.Format("Энтропия равна {0}", entropy));
            Console.WriteLine("Данные успешно записаны.");
        }

        static string CheckText(string path, string file_name, Boolean space)
        {
            var text = File.ReadAllText(path + file_name);

            if (new Regex(@"[а-еж-щы-я ]").IsMatch(text))
            {
                text = text.ToLower();
                text = Regex.Replace(text, "ё", "е");
                text = Regex.Replace(text, "ъ", "ь");
                text = Regex.Replace(text, @"[^а-еж-щы-я ]", " ");
                text = Regex.Replace(text, @"\s+", space ? " " : "");
            }

            File.WriteAllText(path + "OpetText_Cleaned.txt", text, System.Text.Encoding.UTF8);
            
            return text;
        }
        
        static SortedDictionary<String, Double> CountMonogram(string path, string file_name, Boolean space)
        {
            var open_text = CheckText(path, file_name, space);
            //var array = new Double[31 + (space ? 1 : 0)];
            var dictionary = new SortedDictionary<String, Double>();

            for (int q = 0; q < open_text.Length; q++)
            {
                var monogram = open_text[q].ToString();

                if (monogram != " ")
                    dictionary[monogram] = dictionary.Keys.Contains(monogram) ? ++dictionary[monogram] : 1;
            }

            var monograms = dictionary.Keys.ToArray();

            foreach (var monogram in monograms)
                dictionary[monogram] = dictionary[monogram] / open_text.Length;

            return dictionary;
        }
        static SortedDictionary<String, Double> CountBigram(string path, string file_name, bool space)
        {
            //привет = пр ив ет

            var open_text = CheckText(path, file_name, space);
            var dictionary = new SortedDictionary<String, Double>();

            if ((open_text.Length % 2) == 0)
                open_text = open_text + "о";

            for (int q = 0; q < open_text.Length - 1; q = q + 2)
            {
                var bigram = open_text.Substring(q, 2);
                dictionary[bigram] = dictionary.ContainsKey(bigram) ? ++dictionary[bigram] : 1;
            }

            var bigrams = dictionary.Keys.ToArray();
            Int32 bigrams_amount = open_text.Length / 2;

            foreach (var bigram in bigrams)
                dictionary[bigram] = dictionary[bigram] / bigrams_amount;
            
            return dictionary;
        }
        static SortedDictionary<String, Double> CountBigramOrdered(string path, string file_name, bool space)
        {
            // привет = пр ри ив ве ет

            var open_text = CheckText(path, file_name, space);
            var dictionary = new SortedDictionary<String, Double>();

            for (int q = 0; q < open_text.Length - 1; q++)
            {
                var bigram = open_text.Substring(q, 2);
                dictionary[bigram] = dictionary.ContainsKey(bigram) ? ++dictionary[bigram] : 1;
            }

            var bigrams = dictionary.Keys.ToArray();
            Int32 bigrams_amount = open_text.Length - 1;

            foreach (var bigram in bigrams)
                dictionary[bigram] = dictionary[bigram] / bigrams_amount;

            return dictionary;
        }
        
        static Double CalculateEntropy(Double[] monograms)
        {
            Double entropy = 0;

            foreach (var monogram in monograms)
                entropy += monogram * Math.Log2(monogram);

            return -entropy;
        }
        static Double CalculateEntropy(SortedDictionary<String, Double> dictionary)
        {
            Double entropy = 0;

            foreach (var bigram in dictionary.Keys)
                entropy += dictionary[bigram] * Math.Log2(dictionary[bigram]);

            return -entropy;
        }
    }
}