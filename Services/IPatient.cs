using System.Collections.Generic;
using System.Threading.Tasks;
using testAPI_A.Models;
using testAPI_A.Models.DTOS;

public interface IPatient
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient> GetByIdAsync(int id);
    Task<Patient> CreateAsync(DTOCreatePatient dto);
    Task<bool> UpdateAsync(int id, DTOUpdatePatient dto);
    Task<bool> DeleteAsync(int id);
}
