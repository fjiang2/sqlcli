﻿//--------------------------------------------------------------------------------------------------//
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
using System.Text;
using System.Data;
using Sys.Data.Linq;

namespace Sys.Data
{
    public class ComputedColumns
    {
        private readonly string[] columnNames;

        public ComputedColumns()
        {
            this.columnNames = new string[0];
        }

        public ComputedColumns(string[] columns)
        {
            this.columnNames = columns;
        }

        public ComputedColumns(ColumnCollection columns)
        {
            this.columnNames = columns.Where(column => column.IsComputed).Select(column => column.ColumnName).ToArray();
        }

        internal ComputedColumns(TableName tname)
        { 
            string SQL = $@"
            USE [{tname.DatabaseName.Name}]
            SELECT c.name
            FROM sys.tables t 
	            JOIN sys.columns c ON t.object_id = c.object_id 
            WHERE t.name = '{tname.Name}' AND c.is_computed = 1";

            this.columnNames = tname.FillDataTable(SQL).ToArray<string>(0);
        
        }

        public string[] ColumnNames
        {
            get
            {
                return this.columnNames;
            }
        }

        public int Length { get { return this.ColumnNames.Length; } }

        public override string ToString()
        {
            return string.Join(" , ", columnNames);
        }

    }
}
