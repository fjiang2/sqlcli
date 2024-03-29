﻿using System.Data;

namespace Sys.Data
{
    public class DataTableSchemaName
    {
        private readonly DataTable dt;
        public DataTableSchemaName(DataTable dt)
        {
            this.dt = dt;
        }

        public void SetSchemaAndTableName(TableName tname)
        {
            dt.TableName = tname.Name;
            UpdateSchemaName(tname.SchemaName);

            if (dt.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
            }

            dt.DataSet.DataSetName = tname.DatabaseName.Name;
        }

        public bool IsDbo
        {
            get
            {
                if (string.IsNullOrEmpty(dt.Prefix))
                    return true;

                return dt.Prefix == Data.SchemaName.dbo;
            }
        }
        
        public void UpdateSchemaName(string schemaName)
        {
            if (schemaName != Data.SchemaName.dbo)
                dt.Prefix = schemaName;
        }

        public string SchemaName
        {
            get
            {
                if (string.IsNullOrEmpty(dt.Prefix))
                    return Data.SchemaName.dbo;
                else
                    return dt.Prefix;
            }
        }

        public override string ToString()
        {
            return SchemaName;
        }
    }
}
