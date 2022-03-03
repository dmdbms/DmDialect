using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Dialect;

public class DmDialect : Dialect
{
	public override int? MaxNumberOfParameters => 2048;

	public override string AddColumnString => "add column";

	public override string CurrentTimestampSelectString => "select sysdate";

	public override string CurrentTimestampSQLFunctionName => "sysdate";

	public override bool DropConstraints => false;

	public override string CascadeConstraintsString => " cascade constraints";

	public override string QuerySequencesString => "select sequence_name from user_sequences";

	public override string CreateTemporaryTableString => "create global temporary table";

	public override string CreateTemporaryTablePostfix => "on commit delete rows";

	public override bool IsCurrentTimestampSelectStringCallable => false;

	[Obsolete("Use UsesColumnsWithForUpdateOf instead")]
	public override bool ForUpdateOfColumns => true;

	public override bool SupportsUnionAll => true;

	public override bool SupportsCommentOn => true;

	public override bool SupportsTemporaryTables => true;

	public override bool SupportsCurrentTimestampSelection => true;

	public override string SelectGUIDString => "select rawtohex(sys_guid()) from dual";

	public override bool SupportsSequences => true;

	public override bool SupportsLimit => true;

	public override bool UseMaxForLimit => false;

	public override long TimestampResolutionInTicks => 10000000L;

	public override bool SupportsConcurrentWritingConnectionsInSameTransaction => false;

	public override bool SupportsDistributedTransactions => false;

	public override bool SupportsEmptyInList => false;

	public override bool SupportsExistsInSelect => false;

	public DmDialect()
	{
		RegisterKeywords();
		RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
		RegisterColumnType(DbType.AnsiStringFixedLength, 8188, "CHAR($l)");
		RegisterColumnType(DbType.AnsiStringFixedLength, "CHARACTER(255)");
		RegisterColumnType(DbType.AnsiStringFixedLength, 8188, "CHARACTER($l)");
		RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
		RegisterColumnType(DbType.AnsiString, 8188, "VARCHAR($l)");
		RegisterColumnType(DbType.AnsiString, "VARCHAR2(255)");
		RegisterColumnType(DbType.AnsiString, 8188, "VARCHAR2($l)");
		RegisterColumnType(DbType.AnsiString, int.MaxValue, "CLOB");
		RegisterColumnType(DbType.Binary, int.MaxValue, "BLOB");
		RegisterColumnType(DbType.AnsiString, int.MaxValue, "TEXT");
		RegisterColumnType(DbType.Binary, int.MaxValue, "IMAGE");
		RegisterColumnType(DbType.AnsiString, int.MaxValue, "LONGVARCHAR");
		RegisterColumnType(DbType.Binary, int.MaxValue, "LONGVARBINARY");
		RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
		RegisterColumnType(DbType.StringFixedLength, 8188, "CHAR($l)");
		RegisterColumnType(DbType.StringFixedLength, "CHARACTER(255)");
		RegisterColumnType(DbType.StringFixedLength, 8188, "CHARACTER($l)");
		RegisterColumnType(DbType.String, "VARCHAR(255)");
		RegisterColumnType(DbType.String, 8188, "VARCHAR($l)");
		RegisterColumnType(DbType.String, "VARCHAR2(255)");
		RegisterColumnType(DbType.String, 8188, "VARCHAR2($l)");
		RegisterColumnType(DbType.String, int.MaxValue, "CLOB");
		RegisterColumnType(DbType.String, int.MaxValue, "TEXT");
		RegisterColumnType(DbType.String, int.MaxValue, "LONGVARCHAR");
		RegisterColumnType(DbType.Boolean, "BIT");
		RegisterColumnType(DbType.Byte, "BYTE");
		RegisterColumnType(DbType.Byte, "TINYINT");
		RegisterColumnType(DbType.Int16, "SMALLINT");
		RegisterColumnType(DbType.Int32, "INTEGER");
		RegisterColumnType(DbType.Int32, "INT");
		RegisterColumnType(DbType.Int64, "BIGINT");
		RegisterColumnType(DbType.Currency, "DECIMAL(16,4)");
		RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
		RegisterColumnType(DbType.Decimal, 29, "DECIMAL($p, $s)");
		RegisterColumnType(DbType.Currency, "NUMERIC(16,4)");
		RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
		RegisterColumnType(DbType.Decimal, 29, "NUMERIC($p, $s)");
		RegisterColumnType(DbType.Currency, "DEC(16,4)");
		RegisterColumnType(DbType.Decimal, "DEC(19,5)");
		RegisterColumnType(DbType.Decimal, 29, "DEC($p, $s)");
		RegisterColumnType(DbType.Currency, "NUMBER(16,4)");
		RegisterColumnType(DbType.Decimal, "NUMBER(19,5)");
		RegisterColumnType(DbType.Decimal, 29, "NUMBER($p, $s)");
		RegisterColumnType(DbType.Single, "REAL");
		RegisterColumnType(DbType.Double, "FLOAT");
		RegisterColumnType(DbType.Double, "DOUBLE");
		RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
		RegisterColumnType(DbType.Binary, "BINARY(8188)");
		RegisterColumnType(DbType.Binary, 8188, "BINARY($l)");
		RegisterColumnType(DbType.Binary, "VARBINARY(8188)");
		RegisterColumnType(DbType.Binary, 8188, "VARBINARY($l)");
		RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
		RegisterColumnType(DbType.Date, "DATE");
		RegisterColumnType(DbType.DateTime, "TIMESTAMP");
		RegisterColumnType(DbType.DateTime, "DATETIME");
		RegisterColumnType(DbType.Time, "TIME");
		RegisterColumnType(DbType.Guid, "BINARY(16)");
		RegisterFunction("abs", new StandardSQLFunction("abs"));
		RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
		RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
		RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
		RegisterFunction("ceil", new StandardSQLFunction("ceil"));
		RegisterFunction("ceiling", new StandardSQLFunction("ceil"));
		RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
		RegisterFunction("cosh", new StandardSQLFunction("cosh", NHibernateUtil.Double));
		RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
		RegisterFunction("degrees", new StandardSQLFunction("degrees", NHibernateUtil.Double));
		RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
		RegisterFunction("floor", new StandardSQLFunction("floor"));
		RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));
		RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
		RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
		RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));
		RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Single));
		RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
		RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
		RegisterFunction("round", new StandardSQLFunction("round"));
		RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
		RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
		RegisterFunction("sinh", new StandardSQLFunction("sinh", NHibernateUtil.Double));
		RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
		RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
		RegisterFunction("tanh", new StandardSQLFunction("tanh", NHibernateUtil.Double));
		RegisterFunction("trunc", new StandardSQLFunction("trunc"));
		RegisterFunction("truncate", new StandardSQLFunction("trunc"));
		RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
		RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "vsize(?1)*8"));
		RegisterFunction("char", new StandardSQLFunction("chr", NHibernateUtil.Character));
		RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
		RegisterFunction("initcap", new StandardSQLFunction("initcap"));
		RegisterFunction("instr", new StandardSQLFunction("instr", NHibernateUtil.Int32));
		RegisterFunction("instrb", new StandardSQLFunction("instrb", NHibernateUtil.Int32));
		RegisterFunction("lcase", new StandardSQLFunction("lcase"));
		RegisterFunction("left", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1, 1, ?2)"));
		RegisterFunction("leftstr", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1, 1, ?2)"));
		RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int64));
		RegisterFunction("lower", new StandardSQLFunction("lower"));
		RegisterFunction("lpad", new StandardSQLFunction("lpad", NHibernateUtil.String));
		RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
		RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
		RegisterFunction("right", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1, -?2)"));
		RegisterFunction("rpad", new StandardSQLFunction("rpad", NHibernateUtil.String));
		RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
		RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));
		RegisterFunction("substr", new StandardSQLFunction("substr", NHibernateUtil.String));
		RegisterFunction("substrb", new StandardSQLFunction("substrb", NHibernateUtil.String));
		RegisterFunction("translate", new StandardSQLFunction("translate", NHibernateUtil.String));
		RegisterFunction("ucase", new StandardSQLFunction("ucase"));
		RegisterFunction("lcase", new StandardSQLFunction("lcase"));
		RegisterFunction("str", new StandardSQLFunction("to_char", NHibernateUtil.String));
		RegisterFunction("curdate", new NoArgSQLFunction("current_date", NHibernateUtil.Date, hasParenthesesIfNoArguments: false));
		RegisterFunction("curtime", new NoArgSQLFunction("current_timestamp", NHibernateUtil.Time, hasParenthesesIfNoArguments: false));
		RegisterFunction("current_date", new NoArgSQLFunction("current_date", NHibernateUtil.Date, hasParenthesesIfNoArguments: false));
		RegisterFunction("current_time", new NoArgSQLFunction("current_timestamp", NHibernateUtil.Time, hasParenthesesIfNoArguments: false));
		RegisterFunction("dayname", new StandardSQLFunction("dayname", NHibernateUtil.String));
		RegisterFunction("dayofweek", new StandardSQLFunction("dayofweek", NHibernateUtil.Int32));
		RegisterFunction("dayofyear", new StandardSQLFunction("dayofyear", NHibernateUtil.Int32));
		RegisterFunction("hour", new StandardSQLFunction("hour", NHibernateUtil.Int32));
		RegisterFunction("last_day", new StandardSQLFunction("last_day", NHibernateUtil.Date));
		RegisterFunction("minute", new StandardSQLFunction("minute", NHibernateUtil.Int32));
		RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
		RegisterFunction("monthname", new StandardSQLFunction("monthname", NHibernateUtil.String));
		RegisterFunction("months_between", new StandardSQLFunction("months_between", NHibernateUtil.Single));
		RegisterFunction("next_day", new StandardSQLFunction("next_day", NHibernateUtil.Date));
		RegisterFunction("quarter", new StandardSQLFunction("quarter", NHibernateUtil.Int32));
		RegisterFunction("second", new StandardSQLFunction("second", NHibernateUtil.Int32));
		RegisterFunction("sysdate", new NoArgSQLFunction("sysdate", NHibernateUtil.Date, hasParenthesesIfNoArguments: false));
		RegisterFunction("week", new StandardSQLFunction("week", NHibernateUtil.Int32));
		RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));
		RegisterFunction("localtime", new NoArgSQLFunction("current_timestamp", NHibernateUtil.Time, hasParenthesesIfNoArguments: false));
		RegisterFunction("date", new StandardSQLFunction("trunc", NHibernateUtil.Date));
		RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(second from cast(?1 as timestamp))"));
		RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(minute from cast(?1 as timestamp))"));
		RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(hour from cast(?1 as timestamp))"));
		RegisterFunction("nvl", new StandardSQLFunction("nvl"));
		base.DefaultProperties["connection.driver_class"] = "NHibernate.Driver.DmDriver";
	}

	public virtual string GetBasicSelectClauseNullString(SqlType sqlType)
	{
		return base.GetSelectClauseNullString(sqlType);
	}

	public override string GetSelectClauseNullString(SqlType sqlType)
	{
		switch (sqlType.DbType)
		{
		case DbType.AnsiString:
		case DbType.String:
		case DbType.AnsiStringFixedLength:
		case DbType.StringFixedLength:
			return "to_char(null)";
		case DbType.Date:
		case DbType.DateTime:
		case DbType.Time:
			return "to_date(null)";
		default:
			return "to_number(null)";
		}
	}

	public override string GenerateTemporaryTableName(string baseTableName)
	{
		string text = base.GenerateTemporaryTableName(baseTableName);
		if (text.Length <= 30)
		{
			return text;
		}
		return text.Substring(1, 29);
	}

	public override bool DropTemporaryTableAfterUse()
	{
		return false;
	}

	public override string GetForUpdateString(string aliases)
	{
		return ForUpdateString + " of " + aliases;
	}

	public override string GetForUpdateNowaitString(string aliases)
	{
		return ForUpdateString + " of " + aliases + " nowait";
	}

	public override string GetSequenceNextValString(string sequenceName)
	{
		return "select " + GetSelectSequenceNextValString(sequenceName);
	}

	public override SqlString AddIdentifierOutParameterToInsert(SqlString insertString, string identifierColumnName, string parameterName)
	{
		return insertString.Append(" returning " + identifierColumnName + " into :" + parameterName);
	}

	public override string GetSelectSequenceNextValString(string sequenceName)
	{
		return sequenceName + ".nextval";
	}

	public override string GetCreateSequenceString(string sequenceName)
	{
		return "create sequence " + sequenceName;
	}

	public override string GetDropSequenceString(string sequenceName)
	{
		return "drop sequence " + sequenceName;
	}

	public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
	{
		SqlStringBuilder sqlStringBuilder = new SqlStringBuilder(queryString);
		sqlStringBuilder.Add(" limit ");
		if (offset != null)
		{
			sqlStringBuilder.Add(offset);
			sqlStringBuilder.Add(", ");
		}
		if (limit != null)
		{
			sqlStringBuilder.Add(limit);
		}
		else
		{
			sqlStringBuilder.Add(int.MaxValue.ToString());
		}
		return sqlStringBuilder.ToSqlString();
	}

	private string ExtractColumnOrAliasNames(SqlString select)
	{
		ExtractColumnOrAliasNames(select, out var columnsOrAliases, out var _, out var _);
		return StringHelper.Join(",", columnsOrAliases);
	}

	internal static void ExtractColumnOrAliasNames(SqlString select, out List<SqlString> columnsOrAliases, out Dictionary<SqlString, SqlString> aliasToColumn, out Dictionary<SqlString, SqlString> columnToAlias)
	{
		columnsOrAliases = new List<SqlString>();
		aliasToColumn = new Dictionary<SqlString, SqlString>();
		columnToAlias = new Dictionary<SqlString, SqlString>();
		IList<SqlString> tokens = new QuotedAndParenthesisStringTokenizer(select).GetTokens();
		int num = 0;
		while (num < tokens.Count)
		{
			SqlString sqlString = tokens[num];
			int num2 = ++num;
			if (sqlString.EqualsCaseInsensitive("select") || sqlString.EqualsCaseInsensitive("distinct") || sqlString.EqualsCaseInsensitive(","))
			{
				continue;
			}
			if (sqlString.EqualsCaseInsensitive("from"))
			{
				break;
			}
			while (num2 < tokens.Count && !tokens[num2].EqualsCaseInsensitive("as") && !tokens[num2].EqualsCaseInsensitive("from") && !tokens[num2].EqualsCaseInsensitive(","))
			{
				SqlString sql = tokens[num2];
				sqlString = sqlString.Append(sql);
				num2 = ++num;
			}
			SqlString sqlString2 = sqlString;
			if (sqlString.IndexOfCaseInsensitive("'") < 0 && sqlString.IndexOfCaseInsensitive("(") < 0)
			{
				int num3 = sqlString.IndexOfCaseInsensitive(".");
				if (num3 != -1)
				{
					sqlString2 = sqlString.Substring(num3 + 1);
				}
			}
			if (num2 + 1 < tokens.Count && tokens[num2].IndexOfCaseInsensitive("as") >= 0)
			{
				sqlString2 = tokens[num2 + 1];
				num += 2;
			}
			columnsOrAliases.Add(sqlString2);
			aliasToColumn[sqlString2] = sqlString;
			columnToAlias[sqlString] = sqlString2;
		}
	}

	public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
	{
		return new DmDataBaseSchema(connection);
	}

	protected virtual void RegisterKeywords()
	{
		string[] keywords = new string[661]
		{
			"abort", "absolute", "across", "action", "add", "advanced", "after", "all", "alter", "and",
			"any", "arraylen", "archivedir", "archivelog", "archivestyle", "as", "asc", "assign", "at", "attach",
			"audit", "authid", "authorization", "auto", "avg", "backup", "backupdir", "backupinfo", "backupset", "maxpiecesize",
			"device", "delimited", "parms", "trace", "file", "bakfile", "parallel", "before", "begin", "between",
			"bigdatediff", "bigint", "binary", "bit", "bitmap", "blob", "block", "bool", "boolean", "both",
			"bstring", "btree", "by", "byte", "cache", "calculate", "call", "cascade", "cascaded", "case",
			"cast", "catalog", "chain", "char", "character", "cumulative", "nchar", "ncharacter", "national", "check",
			"cipher", "clob", "close", "cluster", "column", "commit", "committed", "commitwork", "comment", "compile",
			"dump", "job", "compress", "compressed", "connect", "connect_by_isleaf", "connect_by_iscycle", "connect_by_root", "constant", "constraint",
			"constraints", "constructor", "contains", "context", "dictionary", "privilege", "replay", "backed", "buffer", "lexer",
			"convert", "copy", "corresponding", "count", "create", "cross", "crypto", "ctlfile", "current", "current_user",
			"current_schema", "cursor", "cycle", "database", "data", "datafile", "date", "dateadd", "datediff", "datetime",
			"day", "datepart", "dbfile", "debug", "ddl", "dec", "decimal", "declare", "decode", "default",
			"deferrable", "deferred", "definer", "delete", "deleting", "delta", "desc", "detach", "deterministic", "directory",
			"disconnect", "disable", "distinct", "distributed", "double", "down", "drop", "each", "else", "elseif",
			"elsif", "enable", "encrypt", "encryption", "end", "escape", "events", "except", "exception", "exclusive",
			"exec", "execute", "exists", "exit", "explain", "external", "extract", "fetch", "final", "fillfactor",
			"filegroup", "first", "float", "for", "force", "foreign", "frequence", "from", "full", "fully",
			"fields", "function", "global", "globally", "goto", "grant", "group", "hash", "having", "hextoraw",
			"hour", "identified", "identity_insert", "identity", "if", "image", "immediate", "in", "increase", "increment",
			"index", "initial", "initially", "inner", "innerid", "insert", "inserting", "instantiable", "overriding", "instead",
			"int", "pls_integer", "integer", "intent", "intersect", "interval", "into", "is", "isolation", "join",
			"key", "last", "leading", "left", "level", "like", "link", "local", "locally", "location",
			"lock", "unlock", "account", "locked", "logfile", "login", "logon", "longvarbinary", "longvarchar", "loop",
			"manual", "map", "match", "matched", "max", "maxsize", "maxvalue", "member", "merge", "min",
			"minextents", "minus", "minute", "minvalue", "mirror", "mode", "modify", "money", "month", "mount",
			"materialized", "movement", "mapped", "refresh", "fast", "complete", "demand", "never", "build", "purge",
			"synchronous", "asynchronous", "natural", "new", "next", "no", "noarchivelog", "noaudit", "norowdependencies", "not",
			"nocache", "nocycle", "nomaxvalue", "nominvalue", "noorder", "nowait", "null", "nulls", "number", "numeric",
			"nosort", "object", "of", "off", "offline", "offset", "old", "on", "once", "online",
			"only", "open", "optimize", "option", "or", "order", "out", "outer", "overlaps", "package",
			"page", "partial", "partition", "partitions", "pendant", "percent", "precision", "preserve", "primary", "print",
			"prior", "privileges", "procedure", "public", "raise", "randomly", "range", "rawtohex", "read", "real",
			"rebuild", "record", "records", "ref", "references", "reference", "referencing", "relative", "rename", "repeat",
			"repeatable", "replace", "resize", "restore", "restrict", "result", "return", "reverse", "revoke", "right",
			"role", "rollback", "rollfile", "root", "row", "rowcount", "rowdependencies", "rownum", "rows", "salt",
			"savepoint", "schema", "second", "select", "self", "sequence", "serializable", "server", "set", "share",
			"siblings", "size", "smallint", "snapshot", "some", "sound", "spatial", "split", "sql", "statement",
			"style", "storage", "subpartition", "subpartitions", "substring", "successful", "sum", "switch", "sync", "synonym",
			"sys_connect_by_path", "tablespace", "table", "template", "temporary", "text", "then", "ties", "time", "timer",
			"timestamp", "timestampadd", "timestampdiff", "tinyint", "to", "top", "trailing", "transaction", "trigger", "triggers",
			"trim", "truncate", "truncsize", "type", "uncommitted", "under", "union", "unique", "until", "up",
			"update", "updating", "user", "using", "use_hash", "use_merge", "use_nl", "use_nl_with_index", "value", "values",
			"varbinary", "varchar", "varchar2", "varray", "varying", "variance", "since", "skip", "stddev", "view",
			"vsize", "wait", "week", "when", "whenever", "where", "while", "with", "work", "write",
			"year", "analyze", "sererr", "suspend", "logout", "logoff", "related", "limit", "unlimited", "externally",
			"session_per_user", "connect_idle_time", "failed_login_attemps", "password_life_time", "password_reuse_time", "password_reuse_max", "password_lock_time", "password_grace_time", "cpu_per_call", "cpu_per_session",
			"mem_space", "read_per_call", "read_per_session", "rule", "startup", "label", "shutdown", "times", "allow_ip", "not_allow_ip",
			"allow_datetime", "not_allow_datetime", "password_policy", "eventinfo", "diskspace", "deref", "dangling", "returning", "scope", "string",
			"sbyte", "short", "ushort", "uint", "long", "ulong", "void", "const", "do", "break",
			"continue", "throw", "finally", "try", "catch", "protected", "internal", "private", "abstract", "sealed",
			"static", "readonly", "virtual", "visible", "override", "extends", "extern", "volatile", "java", "class",
			"base", "struct", "get", "sizeof", "typeof", "admin", "replicate", "verify", "zone", "vertical",
			"log", "none", "lob", "error", "less", "than", "equ", "exchange", "store", "nobranch",
			"branch", "clusterbtr", "list", "normal", "standby", "transactional", "array", "rollup", "cube", "grouping",
			"over", "rowid", "section", "stat", "unbounded", "preceding", "following", "autoextend", "sets", "wrapped",
			"connect_time", "trxid", "versions", "versions_starttime", "versions_endtime", "versions_starttrxid", "versions_endtrxid", "versions_operation", "huge", "path",
			"filesize", "session", "query_rewrite_integrity", "pragma", "autonomous_transaction", "exception_init", "subtype", "bulk", "collect", "forall",
			"indices", "save", "exceptions", "domain", "usage", "collation", "collate", "time_zone", "overlay", "placing",
			"large", "without", "diagnostics", "characteristics", "simple", "pad", "space", "sensitive", "asensitive", "insensitive",
			"scroll", "hold", "pipelined", "pipe", "keep", "dense_rank", "counter", "within", "system", "spfile",
			"memory", "accessed", "initialized", "logging", "nologging", "lnnvl", "mod", "columns", "sample", "xml",
			"pivot", "unpivot", "seed", "parallel_enable", "aggregate", "nocopy", "including", "excluding", "indexes", "invisible",
			"unusable", "ddl_clone", "archive", "inline", "typedef", "include", "exclude", "lsn", "input", "result_cache",
			"monitoring", "nomonitoring", "corrupt", "strict", "lax", "format", "json", "keys", "json_query", "json_value",
			"ascii", "pretty", "wrapper", "conditional", "unconditional", "empty", "task", "thread", "errors", "badfile",
			"browse"
		};
		RegisterKeywords(keywords);
	}
}
