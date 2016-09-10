using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace wayne_questions
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test Question 1
            int[] myIntArray = { 1, 2, 3, 4, 5, 100, 102, 111 };
            Question1 q1 = new Question1(myIntArray);
            Console.WriteLine("The solution of Question 1 is: {0}", q1.Solution());

            // Test Question 2
            string myString1 = "I ate oatmeal for breakfast";
            Console.WriteLine("The new string of Question 2 is: {0}", Question2.ReverseWords(myString1));

            // Test Question 3
            string myString2 = "The sky is blue";
            Console.WriteLine("The new string of Question 3 is: {0}", Question3.RemoveVowels(myString2));


            // Test Design 1
            int n = 6;
            double[] p = { 0.01, 0.19, 0.25, 0.35, 0.049, 0.151 };
            NSidedDie die = new NSidedDie(n, p);
            int[] results = new int[6];
            Console.WriteLine("\nLet's roll die 10000 times.");

            for (var i = 0; i < 10000; i++)
            {
                results[die.RollDie() - 1]++;
            }

            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine("The number of side {0} (p={1}) is: {2}", i + 1, p[i], results[i]);
            }

            // Test Design 2
            string myBarcode = "9781430231714";
            Console.WriteLine("\nParse the EAN-13 barcode: {0}", myBarcode);
            BarcodeParser myBarcodeParser = new BarcodeParser(myBarcode);
            myBarcodeParser.Parse();
            Console.WriteLine("Country Code = {0}, Manufacturer Code = {1}, Product Code = {2}",
                myBarcodeParser.CountryCode, myBarcodeParser.ManufacturerCode, myBarcodeParser.ProductCode);
            //
            string myBarcode2 = "4001430231718";
            Console.WriteLine("\nParse the EAN-13 barcode: {0}", myBarcode2);
            BarcodeParser myBarcodeParser2 = new BarcodeParser(myBarcode2);
            myBarcodeParser2.Parse();
            Console.WriteLine("Country Code = {0}, Manufacturer Code = {1}, Product Code = {2}",
                myBarcodeParser2.CountryCode, myBarcodeParser2.ManufacturerCode, myBarcodeParser2.ProductCode);

            // wait for input before exiting
            Console.WriteLine("\nPress enter to finish");
            Console.ReadLine();
        }
    }

    public class Question1
    {
        private int[] intArray;

        public Question1(int[] intArray)
        {
            this.intArray = intArray;
        }

        public int Solution()
        {
            if (intArray == null)
            {
                throw new Exception();
            }
            int maxOdd = int.MinValue;
            int minEven = int.MaxValue;

            foreach (var num in intArray)
            {
                if (IsEven(num))
                {
                    minEven = minEven < num ? minEven : num;
                }
                else
                {
                    maxOdd = maxOdd > num ? maxOdd : num;
                }
            }

            return maxOdd - minEven;
        }

        private bool IsEven(int num)
        {
            return num % 2 == 0;
        }
    }

    public class Question2
    {
        public Question2()
        {

        }

        public static string ReverseWords(string myString)
        {
            var words = myString.Split(' ');
            StringBuilder sb = new StringBuilder();
            for (int i = words.Length - 1; i >= 0; i--)
            {
                sb.Append(words[i]);
                sb.Append(' ');
            }
            return sb.ToString().TrimEnd();
        }

    }

    public class Question3
    {
        public Question3()
        {
        }

        public static string RemoveVowels(string myString)
        {
            string vowels = "aeiou";
            StringBuilder sb = new StringBuilder();
            foreach (char c in myString)
            {
                if (vowels.Contains(c)) continue;
                sb.Append(c);
            }

            return sb.ToString();
        }
    }

    public class NSidedDie
    {
        private int n;                  // number of sides
        private double[] pArray;        // probabilty of each side
        private double[] boundsArray;
        private Random rng;

        public NSidedDie(int n, double[] p)
        {
            if (p.Length != n) throw new Exception();

            this.n = n;
            this.pArray = p;
            boundsArray = new Double[n];

            for (var i = 0; i < n; i++)
            {
                boundsArray[i] = i == 0 ? p[0] : boundsArray[i - 1] + p[i];
            }

            rng = new Random();
        }

        public int RollDie()
        {
            var val = rng.NextDouble() * boundsArray[n - 1];  // scale it to be safe, avoid exception
            if (val <= boundsArray[0]) return 1;
            else
            {
                for (var i = 1; i < n; i++)
                {
                    if (val > boundsArray[i - 1] && val <= boundsArray[i])
                        return i + 1;
                }
            }

            throw new Exception();
        }
    }

    // EAN-13
    public class BarcodeParser
    {
        private string barcode;
        private BarcodeParserState state;
        private int cursor;
        private int manufacturerCodeLength;
        private int productCodeLength;

        public int CountryCode { set; get; }
        public int ManufacturerCode { set; get; }
        public int ProductCode { set; get; }

        public BarcodeParser(string barcode)
        {
            Regex pattern = new Regex(@"^\d{13}$");

            if (!pattern.IsMatch(barcode))
            {
                throw new Exception();
            }

            this.barcode = barcode;
        }

        public void Parse()
        {
            bool done = false;
            StringBuilder sb = new StringBuilder();
            cursor = 0;
            int tmpInt;
            int count = 0;
            state = BarcodeParserState.CountryCode;

            while (!done)
            {
                char c = GetChar();
                switch (state)
                {
                    case BarcodeParserState.CountryCode:
                        sb.Append(c);
                        count++;
                        tmpInt = Convert.ToInt32(sb.ToString());
                        if (IsSpecialCountryCode(tmpInt))
                        {
                            CountryCode = Convert.ToInt32(sb.ToString());
                            state = BarcodeParserState.ManufacturerCode;
                            sb.Clear();
                            count = 0;
                        }
                        else if (count == 3)
                        {
                            CountryCode = Convert.ToInt32(sb.ToString());
                            manufacturerCodeLength = 5;
                            productCodeLength = 4;
                            state = BarcodeParserState.ManufacturerCode;
                            sb.Clear();
                            count = 0;
                        }

                        break;
                    case BarcodeParserState.ManufacturerCode:
                        sb.Append(c);
                        count++;
                        if (count == manufacturerCodeLength)
                        {
                            ManufacturerCode = Convert.ToInt32(sb.ToString());
                            sb.Clear();
                            count = 0;
                            state = BarcodeParserState.ProductCode;
                        }
                        break;
                    case BarcodeParserState.ProductCode:
                        sb.Append(c);
                        count++;
                        if (count == productCodeLength)
                        {
                            ProductCode = Convert.ToInt32(sb.ToString());
                            sb.Clear();
                            count = 0;
                            state = BarcodeParserState.Checksum;
                        }
                        break;
                    case BarcodeParserState.Checksum:
                        // verify checksum
                        int checksum = c - '0';
                        int sum = 0;
                        for (int i = 0; i < 12; i++)
                        {
                            sum += IsEven(i) ? (barcode[i] - '0') : (barcode[i] - '0') * 3;
                        }

                        if ((checksum + sum) % 10 != 0) throw new Exception();
                        done = true;
                        break;
                }
            }
        }

        private bool IsSpecialCountryCode(int code)
        {
            bool result = false;
            switch (code)
            {
                case 40:
                case 41:
                case 42:
                    manufacturerCodeLength = 5;
                    productCodeLength = 5;
                    result = true;
                    break;
                default:
                    break;
            }
            return result;
        }

        private char GetChar()
        {
            char c;
            if (cursor < barcode.Length)
            {
                c = barcode[cursor];
                cursor++;
                return c;
            }
            throw new Exception();
        }

        private bool IsEven(int num)
        {
            return num % 2 == 0;
        }
    }

    public enum BarcodeParserState
    {
        CountryCode,
        ManufacturerCode,
        ProductCode,
        Checksum,
    }

}
