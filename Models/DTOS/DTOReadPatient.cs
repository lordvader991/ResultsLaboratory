namespace testAPI_A.Models.DTOS
{
    public class DTOReadPatient
    {
        public int Patient_Id { get; set; }  // Cambiar a Guid
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int DoctorId { get; set; }
    }
}
