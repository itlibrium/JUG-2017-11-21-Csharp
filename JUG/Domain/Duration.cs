using System;

namespace JUG.Domain
{
    public struct Duration
    {
        public static Duration FromHours(double hours) => new Duration(new TimeSpan((int)hours, (int) ((hours - (int)hours) * 60), 0));
        
        private readonly TimeSpan _value;

        public double Hours => _value.TotalHours;
        
        private Duration(TimeSpan value)
        {
            if (value.TotalHours < 0) throw new ArgumentException(nameof(value));
            _value = value;
        }
        
        public static Duration operator+ (Duration x, Duration y) 
            => new Duration(x._value + y._value);
        
        public static Duration operator- (Duration x, Duration y) 
            => new Duration(x._value < y._value ? TimeSpan.Zero : x._value - y._value);
    }
}