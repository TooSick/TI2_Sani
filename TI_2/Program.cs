using System;
using System.Collections.Generic;
using System.Linq;

namespace RSA_ElGamal
{
    static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("List of prime numbers:\n" +
                              "2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,\n" +
                              "101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197,\n" +
                              "199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271\n");
            Console.WriteLine("Write prime number p:");
            long p = long.Parse(Console.ReadLine() ?? string.Empty);
            Console.WriteLine("Write prime number q:");
            long q = long.Parse(Console.ReadLine() ?? string.Empty);
            Console.WriteLine("Write encrypted text:");

            string text = Console.ReadLine()?.ToUpper();
            text = text?.Replace("\\s", "");

            long r = p * q;
            long f = (p - 1) * (q - 1);
            long e = GetE(f);

            TempValuesGcd temp = GetExtendGcd(f, e);
            long d = temp.Y;
            if (d < 0)
            {
                d += f;
            }

            var encryptMessage = RsaEncrypt(text, e, r);
            Console.WriteLine("Encryption: " + string.Join(" ", encryptMessage.ToArray()));
            var decryptMessage = RsaDecrypt(encryptMessage, d, r);
            Console.WriteLine("Decryption: " + decryptMessage);
        }

        private static List<string> RsaEncrypt(string s, long e, long r)
        {
            return (from t in s select (int) t into index select Power(index, e, r) into res select res.ToString())
                .ToList();
        }

        private static string RsaDecrypt(IEnumerable<string> input, long d, long r)
        {
            return input.Select(long.Parse)
                .Select(b => (int) Power(b, d, r))
                .Aggregate("", (current, index) => current + (char) (index));
        }

        private static long GetE(long f)
        {
            var valArr = new List<long>();
            var e = f - 1;
            for (var i = 2; i < f; i++)
            {
                if (IsPrime(e) && IsMutuallySimple(e, f))
                {
                    valArr.Add(e);
                }

                e--;
            }

            Random random = new();
            var index = random.Next(valArr.Count);
            return valArr[index];
        }

        private static TempValuesGcd GetExtendGcd(long a, long b)
        {
            if (b == 0)
            {
                return new TempValuesGcd(a, 1, 0);
            }
            else
            {
                var tmp = GetExtendGcd(b, a % b);
                var d = tmp.D;
                var y = tmp.X - tmp.Y * (a / b);
                var x = tmp.Y;
                return new TempValuesGcd(d, x, y);
            }
        }

        private static long Power(long x, long y, long n)
        {
            if (y == 0)
            {
                return 1;
            }

            var z = Power(x, y / 2, n);

            if (y % 2 == 0) return (z * z) % n;

            return (x * z * z) % n;
        }

        private static bool IsMutuallySimple(long a, long b)
        {
            if (a == b)
            {
                return a == 1;
            }
            else
            {
                if (a > b)
                {
                    return IsMutuallySimple(a - b, b);
                }
                else
                {
                    return IsMutuallySimple(b - a, a);
                }
            }
        }

        private static bool IsPrime(long a)
        {
            for (long i = 2; i <= Math.Sqrt(a); i++)
            {
                if (a % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private class TempValuesGcd
        {
            public TempValuesGcd(long d, long x, long y)
            {
                D = d;
                X = x;
                Y = y;
            }

            public long D { get; }
            public long X { get; }
            public long Y { get; }
        }
    }
}