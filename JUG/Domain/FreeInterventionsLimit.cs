namespace JUG.Domain
{
    public struct FreeInterventionsLimit
    {
        public int Initial { get; }
        public int Used { get; }

        public bool UsedInCurrentIntervention => Used > 0;
        public bool CanUse => Used < Initial;

        public static FreeInterventionsLimit CreateInitial(int initial) => new FreeInterventionsLimit(initial);
        
        private FreeInterventionsLimit(int initial)
        {
            Initial = initial;
            Used = 0;
        }
        
        public FreeInterventionsLimit Use() => new FreeInterventionsLimit(Initial, Used + 1);

        private FreeInterventionsLimit(int initial, int used)
        {
            Initial = initial;
            Used = used;
        }
    }
}