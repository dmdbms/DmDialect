using System;
using System.Data;

namespace NHibernate.Dialect.Schema;

public class DmForeignKeyMetadata : AbstractForeignKeyMetadata
{
	public DmForeignKeyMetadata(DataRow rs)
		: base(rs)
	{
		base.Name = Convert.ToString(rs["FOREIGN_KEY_CONSTRAINT_NAME"]);
	}
}
