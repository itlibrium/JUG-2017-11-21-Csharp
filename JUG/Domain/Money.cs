using System;
using System.Globalization;

namespace JUG.Domain
{
    public struct Money
    {
        public static Money Zero => new Money(0);
        public static Money FromDecimal(decimal value) => new Money(value);
        
        private readonly decimal _value;

        public Money(decimal value)
        {
            if (value < 0) throw new ArgumentException(nameof(value));
            _value = value;
        }

        public bool Equals(Money other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Money money && Equals(money);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString("C", CultureInfo.GetCultureInfo("pl-PL"));
        }

        public static Money operator+ (Money x, Money y) => new Money(x._value + y._value);
        public static Money operator- (Money x, Money y) => new Money(x._value - y._value);
        
        public static Money operator* (Money x, double y) => new Money(x._value * (decimal)y);
        public static Money operator/ (Money x, double y) => new Money(x._value / (decimal)y);
        
        public static bool operator== (Money x, Money y) => x._value == y._value;
        public static bool operator!= (Money x, Money y) => x._value != y._value;
        
        public static bool operator> (Money x, Money y) => x._value > y._value;
        public static bool operator< (Money x, Money y) => x._value < y._value;
        
        public static bool operator>= (Money x, Money y) => x._value >= y._value;
        public static bool operator<= (Money x, Money y) => x._value <= y._value;
    }
}