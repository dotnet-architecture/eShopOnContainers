using static Microsoft.EntityFrameworkCore.ExecutionStrategyExtensions;
using ArgumentNullException = System.ArgumentNullException;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using Task = System.Threading.Tasks.Task;


namespace Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Utilities
{
	using FuncOfTask = System.Func<Task>;

	public class ResilientTransaction
	{
		private DbContext _context;
		private ResilientTransaction(DbContext context) =>
		    _context = context ?? throw new ArgumentNullException(nameof(context));

		public static ResilientTransaction New(DbContext context) =>
		    new ResilientTransaction(context);

		public async Task ExecuteAsync(FuncOfTask action)
		{
			//Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
			//See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
			var strategy = _context.Database.CreateExecutionStrategy();
			await strategy.ExecuteAsync(async () =>
			{
				using (var transaction = _context.Database.BeginTransaction())
				{
					await action();
					transaction.Commit();
				}
			});
		}
	}
}
