namespace JUG.Model
{
    public class Contract
    {
        public int Id { get; set; }
        public int FreeServices { get; set; }
        public int FreeServicesUsed { get; set; }

        public decimal SparePartsCostLimit { get; set; }
        public decimal SparePartsCostLimitUsed { get; set; }

        //...
    }
}