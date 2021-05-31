using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sys.Data
{

    public interface IDbFill
    {
        DataSet FillDataSet();
        DataTable FillDataTable();
        DataRow FillDataRow(int row);
        DataRow FillDataRow();
        IEnumerable<T> FillDataColumn<T>(int column);
        IEnumerable<T> FillDataColumn<T>(string columnName);
        object FillObject();
        T FillObject<T>();
        List<T> ToList<T>(Func<DataRow, T> newObject);
    }
}
