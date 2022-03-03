using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.AdoNet;

public class DmBatchingBatcher : AbstractBatcher
{
	private class BatchingCommandSet
	{
		private class BatchParameter
		{
			public ParameterDirection Direction { get; set; }

			public byte Precision { get; set; }

			public byte Scale { get; set; }

			public int Size { get; set; }

			public object Value { get; set; }
		}

		private readonly string _statementTerminator;

		private readonly DmBatchingBatcher _batcher;

		private readonly SqlStringBuilder _sql = new SqlStringBuilder();

		private readonly List<SqlType> _sqlTypes = new List<SqlType>();

		private readonly List<BatchParameter> _parameters = new List<BatchParameter>();

		private CommandType _commandType;

		public int CountOfCommands { get; private set; }

		public int CountOfParameters { get; private set; }

		public BatchingCommandSet(DmBatchingBatcher batcher, char statementTerminator)
		{
			_batcher = batcher;
			_statementTerminator = statementTerminator.ToString();
		}

		public void Append(DbParameterCollection parameters)
		{
			if (CountOfCommands > 0)
			{
				_sql.Add(_statementTerminator);
			}
			else
			{
				_commandType = _batcher.CurrentCommand.CommandType;
			}
			_sql.Add(_batcher.CurrentCommandSql.Copy());
			_sqlTypes.AddRange(_batcher.CurrentCommandParameterTypes);
			foreach (DbParameter parameter in parameters)
			{
				_parameters.Add(new BatchParameter
				{
					Direction = parameter.Direction,
					Precision = parameter.Precision,
					Scale = parameter.Scale,
					Size = parameter.Size,
					Value = parameter.Value
				});
			}
			CountOfCommands++;
			CountOfParameters += parameters.Count;
		}

		public int ExecuteNonQuery()
		{
			if (CountOfCommands == 0)
			{
				return 0;
			}
			DbCommand dbCommand = _batcher.Driver.GenerateCommand(_commandType, _sql.ToSqlString(), _sqlTypes.ToArray());
			for (int i = 0; i < _parameters.Count; i++)
			{
				BatchParameter batchParameter = _parameters[i];
				DbParameter dbParameter = dbCommand.Parameters[i];
				dbParameter.Value = batchParameter.Value;
				dbParameter.Direction = batchParameter.Direction;
				dbParameter.Precision = batchParameter.Precision;
				dbParameter.Scale = batchParameter.Scale;
				dbParameter.Size = batchParameter.Size;
			}
			_batcher.Prepare(dbCommand);
			return dbCommand.ExecuteNonQuery();
		}

		public void Clear()
		{
			CountOfParameters = 0;
			CountOfCommands = 0;
			_sql.Clear();
			_sqlTypes.Clear();
			_parameters.Clear();
		}

		public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (CountOfCommands == 0)
			{
				return 0;
			}
			DbCommand batcherCommand = _batcher.Driver.GenerateCommand(_commandType, _sql.ToSqlString(), _sqlTypes.ToArray());
			for (int i = 0; i < _parameters.Count; i++)
			{
				BatchParameter batchParameter = _parameters[i];
				DbParameter dbParameter = batcherCommand.Parameters[i];
				dbParameter.Value = batchParameter.Value;
				dbParameter.Direction = batchParameter.Direction;
				dbParameter.Precision = batchParameter.Precision;
				dbParameter.Scale = batchParameter.Scale;
				dbParameter.Size = batchParameter.Size;
			}
			await _batcher.PrepareAsync(batcherCommand, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return await batcherCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private readonly int? _maxNumberOfParameters;

	private readonly BatchingCommandSet _currentBatch;

	private int _totalExpectedRowsAffected;

	private StringBuilder _currentBatchCommandsLog;

	public sealed override int BatchSize { get; set; }

	protected override int CountOfStatementsInCurrentBatch => _currentBatch.CountOfCommands;

	public DmBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		: base(connectionManager, interceptor)
	{
		BatchSize = base.Factory.Settings.AdoBatchSize;
		_currentBatch = new BatchingCommandSet(this, base.Factory.Dialect.StatementTerminator);
		_maxNumberOfParameters = base.Factory.Dialect.MaxNumberOfParameters;
		_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
	}

	public override void AddToBatch(IExpectation expectation)
	{
		DbCommand currentCommand = base.CurrentCommand;
		int? maxNumberOfParameters = _maxNumberOfParameters;
		if (maxNumberOfParameters.HasValue && _currentBatch.CountOfParameters + currentCommand.Parameters.Count > _maxNumberOfParameters)
		{
			ExecuteBatchWithTiming(currentCommand);
		}
		_totalExpectedRowsAffected += expectation.ExpectedRowCount;
		base.Driver.AdjustCommand(currentCommand);
		LogBatchCommand(currentCommand);
		_currentBatch.Append(currentCommand.Parameters);
		if (_currentBatch.CountOfCommands >= BatchSize)
		{
			ExecuteBatchWithTiming(currentCommand);
		}
	}

	protected override void DoExecuteBatch(DbCommand ps)
	{
		if (_currentBatch.CountOfCommands == 0)
		{
			Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, 0);
			return;
		}
		try
		{
			AbstractBatcher.Log.Debug("Executing batch");
			CheckReaders();
			if (base.Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				base.Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
			}
			try
			{
				_currentBatch.ExecuteNonQuery();
			}
			catch (DbException sqlException)
			{
				throw ADOExceptionHelper.Convert(base.Factory.SQLExceptionConverter, sqlException, "could not execute batch command.");
			}
		}
		finally
		{
			ClearCurrentBatch();
		}
	}

	private void LogBatchCommand(DbCommand batchCommand)
	{
		string text = null;
		SqlStatementLogger sqlStatementLogger = base.Factory.Settings.SqlStatementLogger;
		if (sqlStatementLogger.IsDebugEnabled || AbstractBatcher.Log.IsDebugEnabled())
		{
			text = sqlStatementLogger.GetCommandLineWithParameters(batchCommand);
			text = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic).Formatter.Format(text);
			_currentBatchCommandsLog.Append("command ").Append(_currentBatch.CountOfCommands).Append(":")
				.AppendLine(text);
		}
		if (AbstractBatcher.Log.IsDebugEnabled())
		{
			AbstractBatcher.Log.Debug("Adding to batch:{0}", text);
		}
	}

	private void ClearCurrentBatch()
	{
		_currentBatch.Clear();
		_totalExpectedRowsAffected = 0;
		if (base.Factory.Settings.SqlStatementLogger.IsDebugEnabled)
		{
			_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
		}
	}

	public override void CloseCommands()
	{
		base.CloseCommands();
		ClearCurrentBatch();
	}

	protected override void Dispose(bool isDisposing)
	{
		base.Dispose(isDisposing);
		_currentBatch.Clear();
	}

	public override async Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		DbCommand batchCommand = base.CurrentCommand;
		int? maxNumberOfParameters = _maxNumberOfParameters;
		if (maxNumberOfParameters.HasValue && _currentBatch.CountOfParameters + batchCommand.Parameters.Count > _maxNumberOfParameters)
		{
			await ExecuteBatchWithTimingAsync(batchCommand, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		_totalExpectedRowsAffected += expectation.ExpectedRowCount;
		base.Driver.AdjustCommand(batchCommand);
		LogBatchCommand(batchCommand);
		_currentBatch.Append(batchCommand.Parameters);
		if (_currentBatch.CountOfCommands >= BatchSize)
		{
			await ExecuteBatchWithTimingAsync(batchCommand, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	protected override async Task DoExecuteBatchAsync(DbCommand ps, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_currentBatch.CountOfCommands == 0)
		{
			Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, 0);
			return;
		}
		try
		{
			AbstractBatcher.Log.Debug("Executing batch");
			await CheckReadersAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (base.Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				base.Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
			}
			try
			{
				await _currentBatch.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (DbException sqlException)
			{
				throw ADOExceptionHelper.Convert(base.Factory.SQLExceptionConverter, sqlException, "could not execute batch command.");
			}
		}
		finally
		{
			ClearCurrentBatch();
		}
	}
}
