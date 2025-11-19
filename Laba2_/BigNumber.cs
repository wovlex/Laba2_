using System;
using System.Collections.Generic;
using System.Text;

namespace Laba2_
{
    public class BigNumber
    {
        private List<int> digits;
        private const int BASE = 1000;

        public BigNumber(string number)
        {
            digits = new List<int>();
            ParseString(number);
        }

        private BigNumber(List<int> digits)
        {
            this.digits = digits;
            Normalize();
        }

        private void ParseString(string number)
        {
            number = number.Replace(" ", "").Trim();

            for (int i = number.Length; i > 0; i -= 3)
            {
                int start = Math.Max(0, i - 3);
                int length = Math.Min(3, i - start);
                string digitStr = number.Substring(start, length);

                if (int.TryParse(digitStr, out int digit))
                {
                    digits.Add(digit);
                }
            }

            Normalize();
        }

        private void Normalize()
        {
           
            for (int i = digits.Count - 1; i > 0; i--)
            {
                if (digits[i] == 0)
                    digits.RemoveAt(i);
                else
                    break;
            }
        }

        public BigNumber Add(BigNumber other)
        {
            List<int> result = new List<int>();
            int carry = 0;
            int maxLength = Math.Max(digits.Count, other.digits.Count);

            for (int i = 0; i < maxLength || carry > 0; i++)
            {
                int sum = carry;
                if (i < digits.Count) sum += digits[i];
                if (i < other.digits.Count) sum += other.digits[i];

                result.Add(sum % BASE);
                carry = sum / BASE;
            }

            return new BigNumber(result);
        }

        public BigNumber Subtract(BigNumber other)
        {
            if (this < other)
                return new BigNumber("0");

            List<int> result = new List<int>();
            int borrow = 0;

            for (int i = 0; i < digits.Count; i++)
            {
                int current = digits[i] - borrow;
                int otherDigit = i < other.digits.Count ? other.digits[i] : 0;

                if (current < otherDigit)
                {
                    current += BASE;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.Add(current - otherDigit);
            }

            return new BigNumber(result);
        }

        public BigNumber Multiply(BigNumber other)
        {
            List<int> result = new List<int>(new int[digits.Count + other.digits.Count]);

            for (int i = 0; i < digits.Count; i++)
            {
                int carry = 0;
                for (int j = 0; j < other.digits.Count || carry > 0; j++)
                {
                    long product = result[i + j] + (long)digits[i] * (j < other.digits.Count ? other.digits[j] : 0) + carry;
                    result[i + j] = (int)(product % BASE);
                    carry = (int)(product / BASE);
                }
            }

            return new BigNumber(result);
        }

        public bool LessThan(BigNumber other)
        {
            if (digits.Count != other.digits.Count)
                return digits.Count < other.digits.Count;

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                if (digits[i] != other.digits[i])
                    return digits[i] < other.digits[i];
            }

            return false;
        }

        public bool GreaterThanOrEqual(BigNumber other)
        {
            return !LessThan(other);
        }

        
        public static BigNumber operator +(BigNumber a, BigNumber b) => a.Add(b);
        public static BigNumber operator -(BigNumber a, BigNumber b) => a.Subtract(b);
        public static BigNumber operator *(BigNumber a, BigNumber b) => a.Multiply(b);
        public static bool operator <(BigNumber a, BigNumber b) => a.LessThan(b);
        public static bool operator >(BigNumber a, BigNumber b) => b.LessThan(a);
        public static bool operator <=(BigNumber a, BigNumber b) => !(a > b);
        public static bool operator >=(BigNumber a, BigNumber b) => !(a < b);

        public override string ToString()
        {
            if (digits.Count == 0) return "0";

            StringBuilder sb = new StringBuilder();
            for (int i = digits.Count - 1; i >= 0; i--)
            {
                if (i == digits.Count - 1)
                    sb.Append(digits[i]);
                else
                    sb.Append(digits[i].ToString("D3"));
            }

            return sb.ToString();
        }
        public double ToDouble()
        {
            try
            {
                
                return double.Parse(this.ToString());
            }
            catch
            {
                return 0;
            }
        }
    }
}