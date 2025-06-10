using Cassandra;
using OrderService.Models;
using ISession = Cassandra.ISession;

namespace OrderService.Repositories
{
    public class CassandraOrderRepository
    {
        private readonly ISession _session;

        public CassandraOrderRepository(ISession session)
        {
            _session = session;
        }

        public async Task InsertOrderAsync(Order order)
        {
            var insertDoctor = _session.Prepare(
                "INSERT INTO orders_by_doctor (doctor_id, order_id, patient_id, test_type_id, order_date, status, notes) VALUES (?, ?, ?, ?, ?, ?, ?);"
            );

            var insertPatient = _session.Prepare(
                "INSERT INTO orders_by_patient (patient_id, order_id, doctor_id, test_type_id, order_date, status, notes) VALUES (?, ?, ?, ?, ?, ?, ?);"
            );

            var insertTestType = _session.Prepare(
                "INSERT INTO orders_by_test_type (test_type_id, order_id, doctor_id, patient_id, order_date, status, notes) VALUES (?, ?, ?, ?, ?, ?, ?);"
            );

            var insertByPatientAndType = _session.Prepare(
                "INSERT INTO orders_by_patient_and_test_type (patient_id, test_type_id, order_id, doctor_id, order_date, status, notes) VALUES (?, ?, ?, ?, ?, ?, ?);"
            );

            var tasks = new List<Task>
            {
                _session.ExecuteAsync(insertDoctor.Bind(
                    order.DoctorId, order.OrderId, order.PatientId,
                    order.TestTypeId, order.OrderDate, order.Status, order.Notes
                ).SetConsistencyLevel(ConsistencyLevel.One)),

                _session.ExecuteAsync(insertPatient.Bind(
                    order.PatientId, order.OrderId, order.DoctorId,
                    order.TestTypeId, order.OrderDate, order.Status, order.Notes
                ).SetConsistencyLevel(ConsistencyLevel.One)),

                _session.ExecuteAsync(insertTestType.Bind(
                    order.TestTypeId, order.OrderId, order.DoctorId,
                    order.PatientId, order.OrderDate, order.Status, order.Notes
                ).SetConsistencyLevel(ConsistencyLevel.One)),

                _session.ExecuteAsync(insertByPatientAndType.Bind(
                    order.PatientId, order.TestTypeId, order.OrderId,
                    order.DoctorId, order.OrderDate, order.Status, order.Notes
                ).SetConsistencyLevel(ConsistencyLevel.One))
            };

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error insertando orden en Cassandra: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByDoctorAsync(int doctorId)
        {
            var query = "SELECT * FROM orders_by_doctor WHERE doctor_id = ?";
            var statement = _session.Prepare(query)
                .Bind(doctorId)
                .SetConsistencyLevel(ConsistencyLevel.One);

            var result = await _session.ExecuteAsync(statement);
            return MapRows(result);
        }

        public async Task<IEnumerable<Order>> GetOrdersByPatientAsync(int patientId)
        {
            var query = "SELECT * FROM orders_by_patient WHERE patient_id = ?";
            var statement = _session.Prepare(query).Bind(patientId).SetConsistencyLevel(ConsistencyLevel.One);
            var result = await _session.ExecuteAsync(statement);
            return MapRows(result);
        }

        public async Task<IEnumerable<Order>> GetOrdersByTestTypeAsync(int testTypeId)
        {
            var query = "SELECT * FROM orders_by_test_type WHERE test_type_id = ?";
            var statement = _session.Prepare(query).Bind(testTypeId).SetConsistencyLevel(ConsistencyLevel.One);
            var result = await _session.ExecuteAsync(statement);
            return MapRows(result);
        }

        public async Task<IEnumerable<Order>> GetOrdersByPatientAndTypeAsync(int patientId, int testTypeId)
        {
            var query = "SELECT * FROM orders_by_patient_and_test_type WHERE patient_id = ? AND test_type_id = ?";
            var statement = _session.Prepare(query).Bind(patientId, testTypeId).SetConsistencyLevel(ConsistencyLevel.One);
            var result = await _session.ExecuteAsync(statement);
            return MapRows(result);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, int doctorId)
        {
            var query = "SELECT * FROM orders_by_doctor WHERE doctor_id = ? AND order_id = ?";
            var statement = _session.Prepare(query).Bind(doctorId, orderId).SetConsistencyLevel(ConsistencyLevel.One);
            var result = await _session.ExecuteAsync(statement);
            var row = result.FirstOrDefault();
            if (row == null) return null;

            return MapRow(row);
        }

        public async Task<Order?> GetOrderByIdWithFilteringAsync(int orderId)
        {
            var query = "SELECT * FROM orders_by_doctor WHERE order_id = ? ALLOW FILTERING";
            var statement = _session.Prepare(query).Bind(orderId).SetConsistencyLevel(ConsistencyLevel.One);
            var result = await _session.ExecuteAsync(statement);
            var row = result.FirstOrDefault();
            return row == null ? null : MapRow(row);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var query = "SELECT * FROM orders_by_doctor WHERE order_id = ?";
            var statement = _session.Prepare(query).Bind(orderId).SetConsistencyLevel(ConsistencyLevel.One);
            var result = await _session.ExecuteAsync(statement);
            var row = result.FirstOrDefault();
            return row == null ? null : MapRow(row);
        }


        public async Task UpdateOrderAsync(int orderId, int doctorId, int patientId, int testTypeId, string? status, string? notes)
        {
            var tasks = new List<Task>();

            if (status != null)
            {
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_doctor SET status = ? WHERE doctor_id = ? AND order_id = ?;")
                    .Bind(status, doctorId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_patient SET status = ? WHERE patient_id = ? AND order_id = ?;")
                    .Bind(status, patientId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_test_type SET status = ? WHERE test_type_id = ? AND order_id = ?;")
                    .Bind(status, testTypeId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_patient_and_test_type SET status = ? WHERE patient_id = ? AND test_type_id = ? AND order_id = ?;")
                    .Bind(status, patientId, testTypeId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
            }

            if (notes != null)
            {
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_doctor SET notes = ? WHERE doctor_id = ? AND order_id = ?;")
                    .Bind(notes, doctorId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_patient SET notes = ? WHERE patient_id = ? AND order_id = ?;")
                    .Bind(notes, patientId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_test_type SET notes = ? WHERE test_type_id = ? AND order_id = ?;")
                    .Bind(notes, testTypeId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
                tasks.Add(_session.ExecuteAsync(_session.Prepare("UPDATE orders_by_patient_and_test_type SET notes = ? WHERE patient_id = ? AND test_type_id = ? AND order_id = ?;")
                    .Bind(notes, patientId, testTypeId, orderId).SetConsistencyLevel(ConsistencyLevel.One)));
            }

            await Task.WhenAll(tasks);
        }

        public async Task DeleteOrderAsync(Order order)
        {
            var tasks = new List<Task>
            {
                _session.ExecuteAsync(_session.Prepare("DELETE FROM orders_by_doctor WHERE doctor_id = ? AND order_id = ?")
                    .Bind(order.DoctorId, order.OrderId)),
                _session.ExecuteAsync(_session.Prepare("DELETE FROM orders_by_patient WHERE patient_id = ? AND order_id = ?")
                    .Bind(order.PatientId, order.OrderId)),
                _session.ExecuteAsync(_session.Prepare("DELETE FROM orders_by_test_type WHERE test_type_id = ? AND order_id = ?")
                    .Bind(order.TestTypeId, order.OrderId)),
                _session.ExecuteAsync(_session.Prepare("DELETE FROM orders_by_patient_and_test_type WHERE patient_id = ? AND test_type_id = ? AND order_id = ?")
                    .Bind(order.PatientId, order.TestTypeId, order.OrderId))
            };

            await Task.WhenAll(tasks);
        }

        private IEnumerable<Order> MapRows(RowSet rows) =>
            rows.Select(MapRow);

        private Order MapRow(Row row) => new Order
        {
            DoctorId = row.GetValue<int>("doctor_id"),
            OrderId = row.GetValue<int>("order_id"),
            PatientId = row.GetValue<int>("patient_id"),
            TestTypeId = row.GetValue<int>("test_type_id"),
            OrderDate = row.GetValue<DateTime>("order_date"),
            Status = row.GetValue<string>("status"),
            Notes = row.GetValue<string>("notes")
        };
    }
}
