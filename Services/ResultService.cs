using Cassandra;
using ResultsService.Models;

namespace ResultsService.Services
{
    public class ResultService
    {
        private readonly Cassandra.ISession _session;
        public ResultService(Cassandra.ISession session) => _session = session;

        public async Task<IEnumerable<Result>> GetAllResultsAsync()
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM results"));
            return rs.Select(row => new Result
            {
                ResultId = row.GetValue<int>("result_id"),
                OrderId = row.GetValue<int>("order_id"),
                PatientId = row.GetValue<int>("patient_id"),
                TestTypeId = row.GetValue<int>("test_type_id"),
                Status = row.GetValue<string>("status"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            });
        }

        public async Task<Result?> GetResultAsync(int resultId)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM results WHERE result_id = ?", resultId));
            var row = rs.FirstOrDefault();
            if (row == null) return null;
            return new Result
            {
                ResultId = row.GetValue<int>("result_id"),
                OrderId = row.GetValue<int>("order_id"),
                PatientId = row.GetValue<int>("patient_id"),
                TestTypeId = row.GetValue<int>("test_type_id"),
                Status = row.GetValue<string>("status"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            };
        }


        public async Task<int> AddResultAsync(Result result)
        {
            // Autoincrementar el ResultId
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT result_id FROM results"));
            var resultIds = rs.Select(r => r.GetValue<int>("result_id")).ToList();
            result.ResultId = resultIds.Count > 0 ? resultIds.Max() + 1 : 0;

            var now = DateTime.UtcNow;
            result.Status = "Pendiente";
            result.CreatedAt = now;
            result.UpdatedAt = now;

            await _session.ExecuteAsync(new SimpleStatement(
                "INSERT INTO results (result_id, order_id, patient_id, test_type_id, status, created_at, updated_at) VALUES (?, ?, ?, ?, ?, ?, ?)",
                result.ResultId, result.OrderId, result.PatientId, result.TestTypeId, result.Status, result.CreatedAt, result.UpdatedAt));
            return result.ResultId;
        }

        public async Task UpdateResultAsync(int resultId, Result updated)
        {
            var now = DateTime.UtcNow;
            await _session.ExecuteAsync(new SimpleStatement(
                "UPDATE results SET order_id = ?, patient_id = ?, test_type_id = ?, status = ?, updated_at = ? WHERE result_id = ?",
                updated.OrderId, updated.PatientId, updated.TestTypeId, updated.Status ?? "Pendiente", now, resultId));
        }

        public async Task DeleteResultAsync(int resultId)
        {
            await _session.ExecuteAsync(new SimpleStatement(
                "DELETE FROM results WHERE result_id = ?", resultId));
        }
    }
}
