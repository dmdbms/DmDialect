using System;
using System.Data;

namespace NHibernate.Dialect.Schema;

public class DmIndexMetadata : AbstractIndexMetadata
{
	public DmIndexMetadata(DataRow rs)
		: base(rs)
	{
		base.Name = Convert.ToString(rs["INDEX_NAME"]);
	}
}
