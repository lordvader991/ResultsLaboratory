using Cassandra;
using TestTypeService.Models;

namespace TestTypeService.Services
{
    public class TestTypeService
    {
        private readonly Cassandra.ISession _session;
        public TestTypeService(Cassandra.ISession session) => _session = session;

        public async Task<IEnumerable<TestType>> GetAllAsync()
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM test_types"));
            return rs.Select(row => new TestType
            {
                TestTypeId = row.GetValue<int>("test_type_id"),
                Name = row.GetValue<string>("name"),
                Description = row.GetValue<string>("description"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            });
        }

        public async Task<TestType?> GetByIdAsync(int testTypeId)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM test_types WHERE test_type_id = ?", testTypeId));
            var row = rs.FirstOrDefault();
            if (row == null) return null;
            return new TestType
            {
                TestTypeId = row.GetValue<int>("test_type_id"),
                Name = row.GetValue<string>("name"),
                Description = row.GetValue<string>("description"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            };
        }

        public async Task<int> AddAsync(TestType testType)
        {
            var rs = await _session.ExecuteAsync(new SimpleStatement("SELECT test_type_id FROM test_types"));
            var ids = rs.Select(r => r.GetValue<int>("test_type_id")).ToList();
            testType.TestTypeId = ids.Count > 0 ? ids.Max() + 1 : 1;

            var now = DateTime.UtcNow;
            testType.CreatedAt = now;
            testType.UpdatedAt = now;

            await _session.ExecuteAsync(new SimpleStatement(
                "INSERT INTO test_types (test_type_id, name, description, created_at, updated_at) VALUES (?, ?, ?, ?, ?)",
                testType.TestTypeId, testType.Name, testType.Description, testType.CreatedAt, testType.UpdatedAt));
            return testType.TestTypeId;
        }

        public async Task UpdateAsync(int testTypeId, TestType updated)
        {
            var now = DateTime.UtcNow;
            await _session.ExecuteAsync(new SimpleStatement(
                "UPDATE test_types SET name = ?, description = ?, updated_at = ? WHERE test_type_id = ?",
                updated.Name, updated.Description, now, testTypeId));
        }

        public async Task DeleteAsync(int testTypeId)
        {
            await _session.ExecuteAsync(new SimpleStatement(
                "DELETE FROM test_types WHERE test_type_id = ?", testTypeId));
        }
    }
}
