using Cassandra;
using Microsoft.Extensions.Configuration;
using ISession = Cassandra.ISession;
namespace OrderService.Cassandra
{
    public class CassandraConnector
    {
        private static ISession _session;

        public static ISession Connect(IConfiguration configuration)
        {
            if (_session == null)
            {
                var cassandraSettings = configuration.GetSection("Cassandra");
                var contactPoints = cassandraSettings.GetSection("ContactPoints").Get<string[]>();
                var port = cassandraSettings.GetValue<int>("Port");
                var keyspace = cassandraSettings.GetValue<string>("Keyspace");

                var cluster = Cluster.Builder()
                    .AddContactPoints(contactPoints)
                    .WithPort(port)
                    .Build();

                _session = cluster.Connect(keyspace);
            }

            return _session;
        }
    }
}
