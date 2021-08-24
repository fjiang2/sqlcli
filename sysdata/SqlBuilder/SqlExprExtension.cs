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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Data
{
    public static class SqlExprExtension 
    {
        #region SqlExpr/SqlClause: ColumName/ParameterName/AddParameter

    
        public static Expression Assign(this string name, object value)
        {
          return Expression.Assign(name, value);
        }
        public static Expression Equal(this string name, object value)
        {
            return Expression.Equal(name, value);
        }

        /// <summary>
        /// "name" -> "[name]"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Expression AsColumn(this string name)
        {
            return Expression.ColumnName(name, null);
        }

        public static Expression AsColumn(this string name, string dbo)
        {
            return Expression.ColumnName(name, dbo);
        }

        public static Expression AsColumn(this string[] names)
        {
            var L = names.Select(column => column.AsColumn()).ToArray();
            return Expression.Join(L);
        }


      
        /// <summary>
        /// "name" -> "@name"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Expression AsParameter(this string name)
        {
            return Expression.ParameterName(name);
        }

        #endregion

        public static Expression AND(this Expression exp1, Expression exp2)
        {
            return Expression.OPR(exp1, "AND", exp2);
        }

        public static Expression AND(this IEnumerable<Expression> expl)
        {
            if(expl.Count() >1)
                return Expression.OPR(expl.First(), "AND", expl.Skip(1).ToArray());
            else
                return expl.First();
        }


        public static Expression OR(this Expression exp1, Expression exp2)
        {
            return Expression.OPR(exp1, "OR", exp2);
        }

        public static Expression OR(this IEnumerable<Expression> expl)
        {
            if (expl.Count() > 1)
                return Expression.OPR(expl.First(), "OR", expl.Skip(1).ToArray());
            else
                return expl.First();
        }


      
    }
}
