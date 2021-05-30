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
using System.Text;
using System.Data;

namespace Sys.Data
{
    public static class DataExtension
    {

  

        /// <summary>
        /// Adjuested Length
        /// </summary>
        public static int AdjuestedLength(this IColumn column)
        {
            if (column.Length == -1)
                return -1;

            switch (column.CType)
            {
                case CType.NChar:
                case CType.NVarChar:
                    return column.Length / 2;
            }

            return column.Length;
        }




        public static TableName TableName(this Type dpoType)
        {
            TableAttribute[] A = dpoType.GetAttributes<TableAttribute>();
            if (A.Length > 0)
                return A[0].TableName;
            else
                return null;
        }



        public static DPList<T> ToDPList<T>(this TableReader<T> reader) where T : class, IDPObject, new()
        {
            return new DPList<T>(reader);
        }

        public static DPCollection<T> ToDPCollection<T>(this DPList<T> list) where T : class, IDPObject, new()
        {
            return new DPCollection<T>(list.Table);
        }

    }

}
