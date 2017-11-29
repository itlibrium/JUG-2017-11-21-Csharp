namespace JUG.Domain
{
    public struct SparePartsCostLimit
    {
        public Money Initial { get; }
        public Money Used { get; }
        public Money Available => Initial - Used;
        
        public static SparePartsCostLimit CreateInitial(Money value) => new SparePartsCostLimit(value);

        private SparePartsCostLimit(Money initial)
        {
            Initial = initial;
            Used = Money.Zero;
        }
        
        public SparePartsCostLimit Use(Money value) => new SparePartsCostLimit(Initial, Used + value);

        private SparePartsCostLimit(Money initial, Money used)
        {
            Initial = initial;
            Used = used;
        }
    }
}