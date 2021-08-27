using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Sys;
using Sys.Data;
using Sys.Stdio;
using Sys.Data.Coding;

namespace sqlcli
{
	class TableOut
	{
		private readonly ApplicationCommand cmd;
		private readonly TableName tname;

		private UniqueTable uniqueTable = null;

		public TableOut(ApplicationCommand cmd, TableName tableName)
		{
			this.cmd = cmd;
			this.tname = tableName;
		}

		public TableName TableName => this.tname;

		public UniqueTable Table => this.uniqueTable;

		public bool HasPhysloc
		{
			get
			{
				if (this.uniqueTable == null)
					return false;

				return uniqueTable.HasPhysloc;
			}
		}


		private Locator LikeExpr(string wildcard, string[] columns)
		{
			if (columns.Length == 0)
			{
				var schema = new TableSchema(tname);
				List<string> L = new List<string>();
				foreach (var c in schema.Columns)
				{
					if (c.CType == CType.NVarChar || c.CType == CType.NChar || c.CType == CType.NText)
						L.Add(c.ColumnName);
				}
				columns = L.ToArray();
			}

			return new Locator(wildcard, columns);
		}

		private static void DisplayTable(ApplicationCommand cmd, UniqueTable udt, bool more)
		{
			DataTable table = udt.Table;

			if (table == null)
				return;

			if (cmd.Has("json"))
			{
				cout.WriteLine(table.WriteJson(JsonStyle.Normal, excludeTableName: false));
				return;
			}

#if WINDOWS
            if (cmd.Has("edit"))
            {
                var editor = new Windows.TableEditor(udt);
                editor.ShowDialog();
                return;
            }
#endif

			int maxColumnWidth = Config.console.table.grid.MaxColumnWidth;

			table.ToConsole(vertical: cmd.IsVertical, more: more, outputDbNull: true, maxColumnWidth);
		}


		private bool Display(SqlBuilder builder, int top)
		{
			try
			{
				DataTable table = builder.SqlCmd(tname.Provider).FillDataTable();
				table.SetSchemaAndTableName(tname);
				ShellHistory.SetLastResult(table);

				return Display(table, top);
			}
			catch (Exception ex)
			{
				cerr.WriteLine(ex.Message);
				return false;
			}
		}

		private bool Display(DataTable table, int top)
		{
			try
			{
				uniqueTable = new UniqueTable(tname, table);
				DisplayTable(cmd, uniqueTable, top > 0 && table.Rows.Count == top);
			}
			catch (Exception ex)
			{
				cerr.WriteLine(ex.Message);
				return false;
			}

			return true;
		}


		public string[] ROWID(bool has)
		{
			if (has)
				return new string[]
				{
					$"{UniqueTable._PHYSLOC} AS [{UniqueTable._PHYSLOC}]",
					$"0 AS [{UniqueTable._ROWID}]"
				};
			else
				return new string[] { };
		}


		public bool Display()
		{
			SqlBuilder builder;
			int top = cmd.Top;
			string[] columns = cmd.Columns;

			if (cmd.Wildcard != null)
			{
				Locator where = LikeExpr(cmd.Wildcard, cmd.Columns);
				builder = new SqlBuilder().SELECT().COLUMNS(ROWID(cmd.HasRowId).Concat(new string[] { "*" })).FROM(tname).WHERE(where);
			}
			else if (cmd.Where != null)
			{
				var locator = new Locator(cmd.Where);
				builder = new SqlBuilder().SELECT().TOP(top).COLUMNS(ROWID(cmd.HasRowId).Concat(columns)).FROM(tname).WHERE(locator);
			}
			else if (cmd.Has("dup"))
			{
				DuplicatedTable dup = new DuplicatedTable(tname, columns);
				if (dup.group.Rows.Count == 0)
				{
					cout.WriteLine("no duplicated record found");
					return true;
				}

				if (cmd.IsSchema)
				{
					Display(dup.group, 0);
				}
				else
				{
					dup.Dispaly(dt => Display(dt, 0));
				}

				return true;
			}
			else
				builder = new SqlBuilder().SELECT().TOP(top).COLUMNS(ROWID(cmd.HasRowId).Concat(columns)).FROM(tname);

			return Display(builder, top);
		}


		public bool Display(string columns, Locator locator)
		{
			SqlBuilder builder;
			if (cmd.Wildcard == null)
			{
				builder = new SqlBuilder().SELECT().TOP(cmd.Top).COLUMNS(columns).FROM(tname);
				if (locator != null)
					builder.WHERE(locator);
			}
			else
			{
				Locator where = LikeExpr(cmd.Wildcard, cmd.Columns);
				if (locator != null)
					where = locator.And(where);

				builder = new SqlBuilder().SELECT().COLUMNS(columns).FROM(tname).WHERE(where);
			}

			return Display(builder, cmd.Top);
		}


	}
}
