using Testcontainers.PostgreSql;

namespace Ambev.DeveloperEvaluation.Integration.Containers
{
    public class PostgreSqlContainerFactory : IAsyncDisposable
    {
        private static PostgreSqlContainer? _container;

        public async Task<string> StartAsync()
        {
            try
            {
                _container = new PostgreSqlBuilder()
                    .WithImage("postgres")
                    .WithCleanUp(true)
                    .WithName("ambev-postgres-integration-test")
                    .WithDatabase("developer_evaluation_test")
                    .WithUsername("postgres")
                    .WithPassword("postgres")
                    .WithPortBinding(5433, 5432)
                    .WithEnvironment("TZ", "UTC")
                    .Build();

                await _container.StartAsync();

                Console.WriteLine("PostgreSQL container started.");
                Console.WriteLine($"Container connection string: {_container.GetConnectionString()}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start PostgreSQL container: {ex.Message}");
                throw;
            }

            return _container.GetConnectionString();
        }

        public async ValueTask DisposeAsync()
        {
            if (_container is not null)
            {
                await _container.DisposeAsync();
            }
        }
    }
}
