using System;
using System.Data;

namespace NHibernate.Dialect.Schema;

public class DmColumnMetadata : AbstractColumnMetaData
{
	public DmColumnMetadata(DataRow rs)
		: base(rs)
	{
		base.Name = Convert.ToString(rs["COLUMN_NAME"]);
		SetColumnSize(rs["COLUMN_SIZE"]);
		SetNumericalPrecision(rs["COLUMN_SIZE"]);
		base.Nullable = Convert.ToString(rs["NULLABLE"]);
		base.TypeName = Convert.ToString(rs["COLUMN_TYPE"]);
	}
}
