using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema;

public class DmDataBaseSchema : AbstractDataBaseSchema
{
	public override bool StoresUpperCaseIdentifiers => true;

	public DmDataBaseSchema(DbConnection connection)
		: base(connection)
	{
	}

	public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
	{
		return new DmTableMetadata(rs, this, extras);
	}

	public override DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
	{
		string text = (string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern);
		string[] restrictionValues = new string[2] { text, tableNamePattern };
		return base.Connection.GetSchema("Tables", restrictionValues);
	}

	public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
	{
		string text = (string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern);
		string[] restrictionValues = new string[3] { text, tableNamePattern, columnNamePattern };
		return base.Connection.GetSchema("Columns", restrictionValues);
	}

	public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
	{
		string text = (string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern);
		string[] restrictionValues = new string[3] { text, tableName, indexName };
		return base.Connection.GetSchema("IndexColumns", restrictionValues);
	}

	public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
	{
		string text = (string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern);
		string[] restrictionValues = new string[4] { text, null, null, tableName };
		return base.Connection.GetSchema("Indexes", restrictionValues);
	}

	public override DataTable GetForeignKeys(string catalog, string schema, string table)
	{
		string text = (string.IsNullOrEmpty(schema) ? null : schema);
		string[] restrictionValues = new string[3] { text, table, null };
		return base.Connection.GetSchema("ForeignKeys", restrictionValues);
	}
}
