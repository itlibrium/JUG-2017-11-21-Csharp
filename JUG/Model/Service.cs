using System.Collections.Generic;

namespace JUG.Model
{
    public class Service
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public ServiceStatus Status { get; set; }
        public double Duration { get; set; }
        public List<SparePart> SpareParts { get; set; }
        public decimal Price { get; set; }

        public bool IsWarranty { get; set; }

        //...
    }
}