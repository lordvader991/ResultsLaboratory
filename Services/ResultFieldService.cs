using Cassandra;
using ResultFieldsService.Models;

namespace ResultFieldsService.Services
{
    public class ResultFieldService
    {
        private readonly Cassandra.ISession _session;
        public ResultFieldService(Cassandra.ISession session) => _session = session;

        public async Task<IEnumerable<ResultField>> GetAllAsync()
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM result_fields"));
            return rs.Select(row => new ResultField
            {
                ResultFieldId = row.GetValue<int>("result_field_id"),
                ResultId = row.GetValue<int>("result_id"),
                TestTypeId = row.GetValue<int>("test_type_id"),
                FieldId = row.GetValue<int>("field_id"),
                Value = row.GetValue<string>("value"),
                Comment = row.GetValue<string>("comment"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            });
        }

        public async Task<ResultField?> GetByIdAsync(int resultFieldId)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM result_fields WHERE result_field_id = ?", resultFieldId));
            var row = rs.FirstOrDefault();
            if (row == null) return null;
            return new ResultField
            {
                ResultFieldId = row.GetValue<int>("result_field_id"),
                ResultId = row.GetValue<int>("result_id"),
                TestTypeId = row.GetValue<int>("test_type_id"),
                FieldId = row.GetValue<int>("field_id"),
                Value = row.GetValue<string>("value"),
                Comment = row.GetValue<string>("comment"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            };
        }

        public async Task<int> AddAsync(ResultField rf)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT result_field_id FROM result_fields"));
            var ids = rs.Select(r => r.GetValue<int>("result_field_id")).ToList();
            rf.ResultFieldId = ids.Count > 0 ? ids.Max() + 1 : 1;

            var now = DateTime.UtcNow;
            rf.CreatedAt = now;
            rf.UpdatedAt = now;

            await _session.ExecuteAsync(new SimpleStatement(
                "INSERT INTO result_fields (result_field_id, result_id, test_type_id, field_id, value, comment, created_at, updated_at) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                rf.ResultFieldId, rf.ResultId, rf.TestTypeId, rf.FieldId, rf.Value, rf.Comment, rf.CreatedAt, rf.UpdatedAt));
            return rf.ResultFieldId;
        }

        public async Task UpdateAsync(int resultFieldId, ResultField updated)
        {
            var now = DateTime.UtcNow;
            await _session.ExecuteAsync(new SimpleStatement(
                "UPDATE result_fields SET value = ?, comment = ?, updated_at = ? WHERE result_field_id = ?",
                updated.Value, updated.Comment, now, resultFieldId));
        }

        public async Task DeleteAsync(int resultFieldId)
        {
            await _session.ExecuteAsync(new SimpleStatement(
                "DELETE FROM result_fields WHERE result_field_id = ?", resultFieldId));
        }
    }
}
