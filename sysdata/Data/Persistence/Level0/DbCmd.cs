//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading;

using DataProviderHandle = System.Int32;

namespace Sys.Data
{
	public abstract class DbCmd : BaseDbCmd
	{
		protected string script;
		protected DbProvider dbProvider;
		protected ConnectionProvider provider;

		public DbCmd(ConnectionProvider provider, string script)
		{
			this.script = script
						  .Replace("$DB_SYSTEM", Const.DB_SYSTEM)
						  .Replace("$DB_APPLICATION", Const.DB_APPLICATION);

			this.provider = provider;
			this.dbProvider = provider.CreateDbProvider(script);
		}

		protected DbCommand command
		{
			get
			{
				return this.dbProvider.DbCommand;
			}
		}

		protected DbConnection connection
		{
			get
			{
				return dbProvider.DbConnection;
			}
		}


		public virtual void ChangeConnection(ConnectionProvider provider)
		{
			if (this.connection.State != ConnectionState.Closed)
				this.connection.Close();

			this.dbProvider = provider.CreateDbProvider(this.script);
			this.command.Connection = provider.NewDbConnection;
		}



		public void ChangeDatabase(string database)
		{
			this.connection.ChangeDatabase(database);
		}


		public event EventHandler<SqlExceptionEventArgs> Error;

		public void OnError(SqlExceptionEventArgs e)
		{
			if (Error != null)
				Error(this, e);
			else
				throw e.Exception;
		}

		public abstract DataTable FillDataTable(DataSet dataSet, string tableName);
		public abstract DataTable FillDataTable(DataTable table);

		public override object ExecuteScalar()
		{

			try
			{
				connection.Open();
				return command.ExecuteScalar();
			}
			catch (Exception ex)
			{
				OnError(new SqlExceptionEventArgs(command, ex));
			}
			finally
			{
				connection.Close();
			}

			return null;
		}

		public override int ExecuteNonQuery()
		{
			int n = 0;
			try
			{
				//
				//Transaction on INSERT/UPDATE/DELETE
				//these commands use ExecuteNonQuery()
				//
				if (command.Transaction == null)
					connection.Open();

				n = command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				OnError(new SqlExceptionEventArgs(command, ex));
			}
			finally
			{
				if (command.Transaction == null)
					connection.Close();
			}

			return n;
		}


		/// <summary>
		/// Get stored procedure's return value
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public object GetReturnValue(string parameterName)
		{
			return command.Parameters[parameterName].Value;
		}




		public EnumerableRowCollection<DataRow> AsEnumerable()
		{
			return FillDataTable()?.AsEnumerable();
		}



		public void Read(Action<DbDataReader> action)
		{
			using (connection)
			{
				connection.Open();
				DbDataReader reader = command.ExecuteReader();

				try
				{
					action(reader);
				}
				finally
				{
					if (reader != null)
						reader.Close();
				}
			}
		}

		public void Read(CancellationToken cancellationToken, IProgress<DataRow> progress)
		{
			Action<DbDataReader> action = reader =>
			{
				try
				{
					var x = new DbReader(reader);
					x.ReadTable(cancellationToken, progress);
				}
				catch (Exception ex)
				{
					OnError(new SqlExceptionEventArgs(command, ex));
				}
			};

			Read(action);

		}


		/// <summary>
		/// Fill data table by DbDataReader
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <param name="progress"></param>
		/// <returns></returns>
		public DataTable ReadDataTable(CancellationToken cancellationToken, IProgress<int> progress)
		{
			DataTable table = null;

			Action<DbDataReader> action = reader =>
			{
				try
				{
					table = new DbReader(reader).ReadTable(cancellationToken, progress);
				}
				catch (Exception ex)
				{
					OnError(new SqlExceptionEventArgs(command, ex));
				}
			};

			Read(action);

			return table;
		}

		public string ToSql()
		{
			string text = this.command.CommandText;

			foreach (DbParameter parameter in command.Parameters)
			{
				text = text.Replace(parameter.ParameterName, new SqlValue(parameter.Value).ToScript(DbAgentStyle.SqlServer));
			}

			return text;
		}

		public override string ToString()
		{
			return this.script;
		}



	}
}
