using System;
using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;

namespace NHibernate.Driver;

public class DmDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
{
	private static readonly SqlType GuidSqlType = new SqlType(DbType.Binary, 16);

	public override bool UseNamedPrefixInSql => true;

	public override bool UseNamedPrefixInParameter => false;

	public override string NamedPrefix => ":";

	System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(DmBatchingBatcherFactory);

	public DmDriver()
		: base("Dm", "DmProvider, Version=1.1.0.0, Culture=neutral, PublicKeyToken=7a2d44aa446c6d01", "Dm.DmConnection", "Dm.DmCommand")
	{
	}

	protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
	{
		if (sqlType.DbType == DbType.Guid)
		{
			base.InitializeParameter(dbParam, name, GuidSqlType);
		}
		else
		{
			base.InitializeParameter(dbParam, name, sqlType);
		}
	}

	protected override void OnBeforePrepare(DbCommand command)
	{
		base.OnBeforePrepare(command);
		if (!CallableParser.Parse(command.CommandText).IsCallable)
		{
			return;
		}
		throw new NotImplementedException(GetType().Name + " does not support CallableStatement syntax (stored procedures). Consider using OracleDataClientDriver instead.");
	}
}
