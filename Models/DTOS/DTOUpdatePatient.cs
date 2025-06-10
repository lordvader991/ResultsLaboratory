using System;

namespace testAPI_A.Models.DTOS
{
    public class DTOUpdatePatient
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime Date_Of_Birth { get; set; } // <- Esta es la propiedad que faltaba
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
