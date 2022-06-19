using System.Text.RegularExpressions;

namespace CP_3_SymCrypto
{
    internal class EncryptionMachine
    {
        readonly String PathForFileToStatistic;
        
        Double _IndexOfCoincidence;
        Double _InverseIndexOfCoincidence;
        Double _MathematicalExpectationOfIndexOfCoincidence;
        public Double IndexOfCoincidence
        {
            private set { _IndexOfCoincidence = value; }
            get { return _IndexOfCoincidence; }
        }
        public Double InverseIndexOfCoincidence
        {
            private set { _InverseIndexOfCoincidence = value; }
            get { return _InverseIndexOfCoincidence; }
        }
        public Double MathematicalExpectationOfIndexOfCoincidence
        {
            private set { _MathematicalExpectationOfIndexOfCoincidence = value; }
            get { return _MathematicalExpectationOfIndexOfCoincidence; }
        }


        public readonly Dictionary<String, Int32> Monograms = new()
        {
            { "а", 0 },
            { "б", 1 },
            { "в", 2 },
            { "г", 3 },
            { "д", 4 },
            { "е", 5 },
            { "ж", 6 },
            { "з", 7 },
            { "и", 8 },
            { "й", 9 },
            { "к", 10 },
            { "л", 11 },
            { "м", 12 },
            { "н", 13 },
            { "о", 14 },
            { "п", 15 },
            { "р", 16 },
            { "с", 17 },
            { "т", 18 },
            { "у", 19 },
            { "ф", 20 },
            { "х", 21 },
            { "ц", 22 },
            { "ч", 23 },
            { "ш", 24 },
            { "щ", 25 },
            { "ь", 26 },
            { "ы", 27 },
            { "э", 28 },
            { "ю", 29 },
            { "я", 30 }
        };


        public SortedDictionary<Double, String> MonogramStatistics;
        public SortedDictionary<Double, String> BigramStatistics;

        public EncryptionMachine()
        {
            this.PathForFileToStatistic = @"D:\Download\KPI\S6\Симметрическая криптография\Лабораторные\КП3\SymCryptCP3\Files\ForStatistic.txt";
            var OText = File.ReadAllText(PathForFileToStatistic);

            BigramStatistics = CountBigram(OText, false);
        }

        #region bigrams statistics
        public string CheckText(string text, Boolean space = false)
        {
            if (new Regex(@"[а-еж-я ]").IsMatch(text))
            {
                text = text.ToLower();
                text = Regex.Replace(text, "ё", "е");
                text = Regex.Replace(text, "ъ", "ь");
                text = Regex.Replace(text, @"[^а-еж-я ]", " ");
                text = Regex.Replace(text, @"\s+", space ? " " : "");
            }

            return text;
        }

        public SortedDictionary<Double, String> CountBigram(string text, bool space)
        {
            //привет = пр ив ет

            var text_ = CheckText(text);
            var dictionary = new SortedDictionary<String, Double>();

            if ((text_.Length % 2) == 0)
                text_ = text_ + "о";

            for (int q = 0; q < text_.Length - 1; q = q + 2)
            {
                var bigram = text_.Substring(q, 2);
                dictionary[bigram] = dictionary.ContainsKey(bigram) ? ++dictionary[bigram] : 1;
            }

            var bigrams = dictionary.Keys.ToArray();
            Int32 bigrams_amount = text_.Length / 2;

            foreach (var bigram in bigrams)
                dictionary[bigram] = dictionary[bigram] / bigrams_amount;


            var Sorted_By_Value_Bigram_Dictionary = new SortedDictionary<Double, String>();

            foreach (var monogram in dictionary.Keys.ToArray())
                if (!Sorted_By_Value_Bigram_Dictionary.ContainsKey(dictionary[monogram]))
                    Sorted_By_Value_Bigram_Dictionary.Add(dictionary[monogram], monogram);
                else
                {
                    bool switch_ = true;
                    Int32 q = 1;
                    while (switch_)
                    {
                        try
                        {
                            Sorted_By_Value_Bigram_Dictionary.Add(dictionary[monogram] + q++ * 1E-15, monogram);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        switch_ = false;
                    }
                }

            return Sorted_By_Value_Bigram_Dictionary;
        }
        #endregion

        #region Affine Cipher

        public String DecryptByAffineBigramCipher(String CText, Double a, Double b)
        {
            CText = CheckText(CText, false); 

            var module = Math.Pow(Monograms.Count, 2);
            var OText = "";
            var inverse_a = (GetInverseElementInFieldByExtendedEuclideanAlgorithm(a, module) + module) % module;
            
            for (int i = 0; i < CText.Length; i += 2)
            {
                var Y_number = Monograms[CText.Substring(i, 1)] * Monograms.Count + Monograms[CText.Substring(i + 1, 1)];
                var X_number = (inverse_a * (Y_number - b)) % module;

                while (X_number < 0)
                    X_number = (X_number + module) % module;

                var X_number_a = ((Monogram)(X_number / Monograms.Count)).ToString();
                var X_number_b = ((Monogram)(X_number % Monograms.Count)).ToString();

                OText += X_number_a + X_number_b;
            }

            return OText;
        }

        public Double[] GetAffineBigramCipherKey(String CText)
        {
            var module = (Int32)Math.Pow(Monograms.Count, 2);

            var currentCText_Ys = CountBigram(CText, false).Values.Reverse().ToArray();
            var statistics_Xs = BigramStatistics.Values.Reverse().ToArray();

            var Xs = new Int32[statistics_Xs.Length];
            var Ys = new Int32[currentCText_Ys.Length];

            for (int q = 0; q < currentCText_Ys.Length; q++)
                Ys[q] = Monograms[currentCText_Ys[q].Substring(0, 1)] * Monograms.Count + Monograms[currentCText_Ys[q].Substring(1, 1)];

            for (int q = 0; q < statistics_Xs.Length; q++)
                Xs[q] = (Monograms[statistics_Xs[q].Substring(0, 1)]) * Monograms.Count + (Monograms[statistics_Xs[q].Substring(1, 1)]);

            var possible_keys = new List<Double[]>();

            for (int q = 0; q < (Xs.Length > 100 ? 100 : Xs.Length); q++)
            {
                for (int t = 0; t < (Xs.Length > 100 ? 100 : Xs.Length); t++)
                {
                    if (q != t)
                    {
                        var u = 0;
                        var v = 0;

                        var gcd = GetExtendedEuclideanAlgorithmValues((Xs[q] - Xs[t] + module) % module, module, out u, out v);
                        if (gcd == 1)
                        {
                            var inv = (GetInverseElementInFieldByExtendedEuclideanAlgorithm((Xs[q] - Xs[t] + module) % module, module) + module) % module;
                            Console.WriteLine(inv);

                            while (inv < 0)
                                inv = (inv + module) % module;
                            

                            var a = (((Ys[q] - Ys[t] + module) % module) * inv) % module;

                            while (a < 0)
                                a = (a + module) % module;

                            var b = (Ys[q] - a * Xs[q]) % module;

                            while (b < 0)
                                b = (b + module) % module;

                            var b_1 = (Ys[t] - a * Xs[t] + module) % module;

                            while (b_1 < 0)
                                b_1 = (b_1 + module) % module;

                            possible_keys.Add(new Double[2] { a, b });
                            Console.WriteLine("a = {0}, b = {1}", a, b);
                        }

                    }
                }
            }

            for (int t = 0; t < possible_keys.Count; t += 100)
            {
                for (int q = t; q < t + 100; q++)
                { 
                    Console.WriteLine("a = {0}, b = {1}, OText = {2}", possible_keys[q][0], possible_keys[q][1], DecryptByAffineBigramCipher(CText.Substring(0, 100), possible_keys[q][0], possible_keys[q][1]));
                }

                Console.WriteLine("Have you found right key? If yes write a. In another case write zero.");
                var input = Console.ReadLine().Split(' ');

                var a = Int32.Parse(input[0]);
                var b = Int32.Parse(input[1]);

                if (a != 0)
                    for (int r = 0; r < possible_keys.Count; r++)
                        if (possible_keys[r][0] == a && possible_keys[r][1] == b)
                            return possible_keys[r];
            }

            return null;
        }
        #endregion

        #region Euclidean Algorithm

        public Double GetExtendedEuclideanAlgorithmValues(Int32 number_a, Int32 number_b, out Int32 s, out Int32 t)
        {
            if (number_a < number_b)
                return GetExtendedEuclideanAlgorithmValues(number_b, number_a, out s, out t);

            if (number_a % number_b == 0d)
            {
                s = 0;
                t = 1;
                return number_b;
            }

            var q0 = number_a / number_b;
            var s0 = 1;
            var t0 = -q0;
            var r0 = number_a - number_b * q0;

            if (number_b % r0 == 0)
            {
                s = s0;
                t = t0;

                return r0;
            }

            var q1 = number_b / r0;
            var s1 = -q1;
            var t1 = 1 + q0 * q1;
            var r1 = number_b - r0 * q1;

            var f = false;

            while (r0 % r1 != 0)
            {

                var q = r0 / r1;

                int s2 = s0 - s1 * q;
                int t2 = t0 - t1 * q;
                int r2 = r0 - r1 * q;

                s0 = s1;
                t0 = t1;
                r0 = r1;

                s1 = s2;
                t1 = t2;
                r1 = r2;
            }

            s = s1;
            t = t1;

            return r1;
        }

        #endregion

        #region Find Inverse element in Field

        public Double GetInverseElementInFieldByExtendedEuclideanAlgorithm(Double number_a, Double module)
        {
            var _ = GetExtendedEuclideanAlgorithmValues((Int32)number_a, (Int32)module, out Int32 u, out Int32 v);

            Console.WriteLine("Coefficients: {0} * u + {1} * v = 1", u, v);

            return v;
        }

        #endregion



    }
}
