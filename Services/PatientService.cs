using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using testAPI_A.Models;
using testAPI_A.Models.DTOS;

public class PatientService : IPatient
{
    private readonly Cassandra.ISession _session;

    public PatientService(Cassandra.ISession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        var result = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM laboratorio.patients"));
        var list = new List<Patient>();

        foreach (var row in result)
        {
            list.Add(MapRow(row));
        }

        return list;
    }

    public async Task<Patient> GetByIdAsync(int id)
    {
        var result = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM laboratorio.patients WHERE patient_id = ?", id));
        var row = result.FirstOrDefault();
        return row != null ? MapRow(row) : null;
    }

    public async Task<Patient> CreateAsync(DTOCreatePatient dto)
    {
        int id = new Random().Next(1000, 999999); // Cambia si quieres un generador más robusto

        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt;

        // Convierte DateTime a LocalDate para Cassandra
        var localDate = new LocalDate(dto.Date_Of_Birth.Year, dto.Date_Of_Birth.Month, dto.Date_Of_Birth.Day);

        var query = "INSERT INTO laboratorio.patients (patient_id, first_name, last_name, date_of_birth, gender, email, phone, doctor_id, created_at, updated_at) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        await _session.ExecuteAsync(new SimpleStatement(query,
            id, dto.First_Name, dto.Last_Name, localDate,
            dto.Gender, dto.Email, dto.Phone, dto.Doctor_Id, createdAt, updatedAt
        ));

        return new Patient
        {
            Patient_Id = id,
            First_Name = dto.First_Name,
            Last_Name = dto.Last_Name,
            Date_Of_Birth = dto.Date_Of_Birth,
            Gender = dto.Gender,
            Email = dto.Email,
            Phone = dto.Phone,
            Doctor_Id = dto.Doctor_Id,
            Created_At = createdAt,
            Updated_At = updatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, DTOUpdatePatient dto)
    {
        var updatedAt = DateTime.UtcNow;
        var localDate = new LocalDate(dto.Date_Of_Birth.Year, dto.Date_Of_Birth.Month, dto.Date_Of_Birth.Day);

        var query = "UPDATE laboratorio.patients SET first_name = ?, last_name = ?, date_of_birth = ?, gender = ?, email = ?, phone = ?, updated_at = ? WHERE patient_id = ?";

        await _session.ExecuteAsync(new SimpleStatement(query,
            dto.First_Name, dto.Last_Name, localDate, dto.Gender,
            dto.Email, dto.Phone, updatedAt, id
        ));

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "DELETE FROM laboratorio.patients WHERE patient_id = ?";
        await _session.ExecuteAsync(new SimpleStatement(query, id));
        return true;
    }

    private Patient MapRow(Row row)
    {
        var localDate = row.GetValue<LocalDate>("date_of_birth");
        DateTime dateOfBirth = new DateTime(localDate.Year, localDate.Month, localDate.Day);

        return new Patient
        {
            Patient_Id = row.GetValue<int>("patient_id"),
            First_Name = row.GetValue<string>("first_name"),
            Last_Name = row.GetValue<string>("last_name"),
            Date_Of_Birth = dateOfBirth,
            Gender = row.GetValue<string>("gender"),
            Email = row.GetValue<string>("email"),
            Phone = row.GetValue<string>("phone"),
            Doctor_Id = row.GetValue<int>("doctor_id"),
            Created_At = row.GetValue<DateTime>("created_at"),
            Updated_At = row.GetValue<DateTime>("updated_at")
        };
    }

}
