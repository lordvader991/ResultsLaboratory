using System;
namespace OrderService.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int TestTypeId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "pendiente";
        public string Notes { get; set; } = "";
    }
}
