using NHibernate.Engine;

namespace NHibernate.AdoNet;

public class DmBatchingBatcherFactory : IBatcherFactory
{
	public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
	{
		return new DmBatchingBatcher(connectionManager, interceptor);
	}
}
