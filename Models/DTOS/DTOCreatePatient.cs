// DTOCreatePatient.cs
using System;

namespace testAPI_A.Models.DTOS
{
    public class DTOCreatePatient
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Doctor_Id { get; set; }
    }
}
