using Cassandra;
using Microsoft.Extensions.Configuration;

namespace ResultsService.Data
{
    public class CassandraConnector
    {
        private readonly IConfiguration _configuration;
        private Cassandra.ISession? _session;

        public CassandraConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Cassandra.ISession Connect()
        {
            if (_session == null)
            {
                var contactPoint = _configuration["Cassandra:ContactPoint"];
                var keyspace = _configuration["Cassandra:Keyspace"];
                var cluster = Cluster.Builder().AddContactPoint(contactPoint).Build();
                _session = cluster.Connect(keyspace);
            }
            return _session!;
        }
    }
}
