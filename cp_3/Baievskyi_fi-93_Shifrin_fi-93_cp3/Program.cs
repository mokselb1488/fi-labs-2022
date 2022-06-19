using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace CP_3_SymCrypto
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;            
            var encryptionMachine = new EncryptionMachine();

            var path = @"D:\Download\KPI\S6\Симметрическая криптография\Лабораторные\КП3\SymCryptCP3\Files\CTexts_and_OText\02.txt";
            var CText = File.ReadAllText(path);


            var key = encryptionMachine.GetAffineBigramCipherKey(CText);
            var OText = encryptionMachine.DecryptByAffineBigramCipher(CText, key[0], key[1]);
            

            var path_back = @"D:\Download\KPI\S6\Симметрическая криптография\Лабораторные\КП3\SymCryptCP3\Files\CTexts_and_OText\02_OText.txt";
            File.WriteAllText(path_back, OText);
        }
    }
}                                                                                                                                       