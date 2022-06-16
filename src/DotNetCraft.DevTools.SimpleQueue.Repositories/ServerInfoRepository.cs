using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.SimpleQueue.Core.Entities;
using DotNetCraft.DevTools.SimpleQueue.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.SimpleQueue.Repositories
{
    public class ServerInfoRepository : BaseSimpleQueueRepository<ServerInfo>, IServerInfoRepository
    {
        private readonly SimpleQueueContext _dbContext;
        private readonly ILogger<ServerInfoRepository> _logger;

        public ServerInfoRepository(SimpleQueueContext dbContext, ILogger<ServerInfoRepository> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task DeleteOldServersAsync(CancellationToken cancellationToken = default)
        {
            using (var conn = _dbContext.Database.GetDbConnection())
            {
                await conn.OpenAsync(cancellationToken);
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "Delete from [ServerInformation] where [ServerExpiredTime] <= GETUTCDATE()";
                    var result = await command.ExecuteNonQueryAsync(cancellationToken);
                    _logger.LogTrace($"{result} servers was/were deleted");
                }
                conn.Close();
            }
        }
    }
}
