using Cassandra;
using FieldsService.Models;

namespace FieldsService.Services
{
    public class FieldService
    {
        private readonly Cassandra.ISession _session;
        public FieldService(Cassandra.ISession session) => _session = session;

        public async Task<IEnumerable<Field>> GetAllFieldsAsync()
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM fields"));
            return rs.Select(row => new Field
            {
                TestTypeId = row.GetValue<int>("test_type_id"),
                FieldId = row.GetValue<int>("field_id"),
                FieldName = row.GetValue<string>("field_name"),
                ReferenceRange = row.GetValue<string>("reference_range"),
                Unit = row.GetValue<string>("unit"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            });
        }

        public async Task<IEnumerable<Field>> GetFieldsByTestTypeAsync(int testTypeId)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM fields WHERE test_type_id = ?", testTypeId));
            return rs.Select(row => new Field
            {
                TestTypeId = row.GetValue<int>("test_type_id"),
                FieldId = row.GetValue<int>("field_id"),
                FieldName = row.GetValue<string>("field_name"),
                ReferenceRange = row.GetValue<string>("reference_range"),
                Unit = row.GetValue<string>("unit"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            });
        }

        public async Task<Field?> GetFieldAsync(int testTypeId, int fieldId)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM fields WHERE test_type_id = ? AND field_id = ?", testTypeId, fieldId));
            var row = rs.FirstOrDefault();
            if (row == null) return null;
            return new Field
            {
                TestTypeId = row.GetValue<int>("test_type_id"),
                FieldId = row.GetValue<int>("field_id"),
                FieldName = row.GetValue<string>("field_name"),
                ReferenceRange = row.GetValue<string>("reference_range"),
                Unit = row.GetValue<string>("unit"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            };
        }

        public async Task<int> AddFieldAsync(Field field)
        {
            // Buscar el máximo field_id existente para ese testTypeId
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT field_id FROM fields WHERE test_type_id = ?", field.TestTypeId));
            var fieldIds = rs.Select(r => r.GetValue<int>("field_id")).ToList();
            field.FieldId = fieldIds.Count > 0 ? fieldIds.Max() + 1 : 1;

            var now = DateTime.UtcNow;
            field.CreatedAt = now;
            field.UpdatedAt = now;

            await _session.ExecuteAsync(new SimpleStatement(
                "INSERT INTO fields (test_type_id, field_id, field_name, reference_range, unit, created_at, updated_at) VALUES (?, ?, ?, ?, ?, ?, ?)",
                field.TestTypeId, field.FieldId, field.FieldName, field.ReferenceRange, field.Unit, field.CreatedAt, field.UpdatedAt));
            return field.FieldId;
        }

        public async Task UpdateFieldAsync(int testTypeId, int fieldId, Field updated)
        {
            var now = DateTime.UtcNow;
            await _session.ExecuteAsync(new SimpleStatement(
                "UPDATE fields SET field_name = ?, reference_range = ?, unit = ?, updated_at = ? WHERE test_type_id = ? AND field_id = ?",
                updated.FieldName, updated.ReferenceRange, updated.Unit, now, testTypeId, fieldId));
        }

        public async Task DeleteFieldAsync(int testTypeId, int fieldId)
        {
            await _session.ExecuteAsync(new SimpleStatement(
                "DELETE FROM fields WHERE test_type_id = ? AND field_id = ?", testTypeId, fieldId));
        }
    }
}
