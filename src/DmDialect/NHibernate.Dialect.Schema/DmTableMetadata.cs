using System;
using System.Data;

namespace NHibernate.Dialect.Schema;

public class DmTableMetadata : AbstractTableMetadata
{
	public DmTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras)
		: base(rs, meta, extras)
	{
	}

	protected override void ParseTableInfo(DataRow rs)
	{
		base.Catalog = null;
		base.Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
		if (string.IsNullOrEmpty(base.Schema))
		{
			base.Schema = null;
		}
		base.Name = Convert.ToString(rs["TABLE_NAME"]);
	}

	protected override string GetConstraintName(DataRow rs)
	{
		return Convert.ToString(rs["FOREIGN_KEY_CONSTRAINT_NAME"]);
	}

	protected override string GetColumnName(DataRow rs)
	{
		return Convert.ToString(rs["COLUMN_NAME"]);
	}

	protected override string GetIndexName(DataRow rs)
	{
		return Convert.ToString(rs["INDEX_NAME"]);
	}

	protected override IColumnMetadata GetColumnMetadata(DataRow rs)
	{
		return new DmColumnMetadata(rs);
	}

	protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
	{
		return new DmForeignKeyMetadata(rs);
	}

	protected override IIndexMetadata GetIndexMetadata(DataRow rs)
	{
		return new DmIndexMetadata(rs);
	}
}
