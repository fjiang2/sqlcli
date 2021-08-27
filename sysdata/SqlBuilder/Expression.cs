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
    public sealed class Expression : SqlBuilderInfo
    {
        public static readonly Expression COUNT_STAR = new Expression().Append("COUNT(*)");

   

        private readonly StringBuilder script = new StringBuilder();

        private Expression()
        {
        }

        private Expression AppendValue(object value)
        {
            script.Append(new SqlValue(value));
            return this;
        }

        private Expression Append(object x)
        {
            script.Append(x);
            return this;
        }

        private Expression Append(string x)
        {
            script.Append(x);
            return this;
        }

        private Expression AppendSpace(string x) => Append(x).AppendSpace();
        private Expression WrapSpace(string x) => AppendSpace().Append(x).AppendSpace();
        private Expression AppendSpace() => Append(" ");


        internal static Expression Assign(string name, object value)
        {
            return ColumnName(name, null).WrapSpace("=").AppendValue(value);
        }

        internal static Expression Equal(string name, object value)
        {
            if (value == null || value == DBNull.Value)
                return ColumnName(name, null).WrapSpace("IS NULL");
            else
                return ColumnName(name, null).WrapSpace("=").AppendValue(value);
        }

        internal static Expression ColumnName(string name, string dbo)
        {
            Expression exp = new Expression();
            if (dbo != null)
                exp.Append(dbo)
                    .Append(".");

            exp.Append("[" + name + "]");

            return exp;
        }

        internal static Expression AllColumnNames(string dbo)
        {
            Expression exp = new Expression();
            if (dbo != null)
                exp.Append(dbo).Append(".");

            exp.Append("*");
            return exp;
        }

        internal static Expression ParameterName(string name)
        {
            Expression exp = new Expression().Append(name.SqlParameterName());
            exp.AddParam(name, null);
            return exp;
        }

        internal static Expression AddParameter(string columnName, string parameterName)
        {
            Expression exp = new Expression()
                .Append("[" + columnName + "]")
                .Append("=")
                .Append(parameterName.SqlParameterName());

            exp.AddParam(parameterName, columnName);

            return exp;
        }

        internal static Expression Join(Expression[] expl)
        {
            Expression exp = new Expression()
                .Append(string.Join<Expression>(",", expl));
            return exp;
        }

        internal static Expression Write(string any)
        {
            return new Expression().Append(any);
        }

#if USE
        public static explicit operator string(SqlExpr x)
        {
            return x.ToString();
        }

        public static explicit operator bool(SqlExpr x)
        {
            return x.expr == "1";
        }

        public static explicit operator char(SqlExpr x)
        {
            return x.ToString()[0];
        }

        public static explicit operator byte(SqlExpr x)
        {
            return Convert.ToByte(x.expr);
        }

        public static explicit operator sbyte(SqlExpr x)
        {
            return Convert.ToSByte(x.expr);
        }

        public static explicit operator short(SqlExpr x)
        {
            return Convert.ToInt16(x.expr);
        }

        public static explicit operator ushort(SqlExpr x)
        {
            return Convert.ToUInt16(x.expr);
        }

        public static explicit operator uint(SqlExpr x)
        {
            return Convert.ToUInt32(x.expr);
        }
        public static explicit operator long(SqlExpr x)
        {
            return Convert.ToInt64(x.expr);
        }

        public static explicit operator ulong(SqlExpr x)
        {
            return Convert.ToUInt64(x.expr);
        }

        public static explicit operator float(SqlExpr x)
        {
            return Convert.ToSingle(x.expr);
        }

        public static explicit operator DateTime(SqlExpr x)
        {
            return Convert.ToDateTime(x.expr);
        }

        public static explicit operator DBNull(SqlExpr x)
        {
            if (script.ToString() == "NULL")
                return System.DBNull.Value;
            else
                throw new SysException("cannot cast value {0} to System.DBNull", x);
        }

#endif

        #region implicit section
        public static implicit operator Expression(ident ident)
        {
            return new Expression().Append(ident);
        }


        public static implicit operator Expression(string value)
        {
            return new Expression().AppendValue(value);    // s= 'string'
        }

        public static implicit operator Expression(bool value)
        {
            return new Expression().AppendValue(value);    // b=1 or b=0
        }


        public static implicit operator Expression(char value)
        {
            return new Expression().AppendValue(value);    // ch= 'c'
        }

        public static implicit operator Expression(byte value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(byte[] value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(sbyte value)
        {
            return new Expression().AppendValue(value);
        }


        public static implicit operator Expression(int value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(short value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(ushort value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(uint value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(long value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(ulong value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(float value)
        {
            return new Expression().AppendValue(value);
        }

        public static implicit operator Expression(DateTime value)
        {
            return new Expression().AppendValue(value);    //dt = '10/20/2012'
        }

        public static implicit operator Expression(DBNull value)
        {
            return new Expression().AppendValue(value);    // NULL
        }

        public static implicit operator Expression(Enum value)
        {
            return new Expression().AppendValue(Convert.ToInt32(value));    // NULL
        }

        #endregion


        /// <summary>
        /// string s = (string)expr;
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static explicit operator string(Expression expr)
        {
            return expr.ToString();
        }

        public Expression AS(Expression alias)
        {
            this.WrapSpace("AS").Append(alias);
            return this;
        }


        public Expression this[Expression exp] => this.Append("[").Append(exp).Append("]");

        public Expression IN(SqlBuilder select)
        {
            this.WrapSpace($"IN ({select.Script})");
            this.Merge(select);
            return this;
        }

        public Expression IN(params Expression[] collection)
        {
            string values = string.Join(", ", collection.Select(x => x.ToString()));
            return this.WrapSpace($"IN ({values})");
        }

        public Expression IN<T>(IEnumerable<T> collection) => this.WrapSpace($"IN ({string.Join<T>(", ", collection)})");

        public Expression BETWEEN(Expression exp1, Expression exp2) => this.WrapSpace($"BETWEEN {exp1} AND {exp2}");

        public Expression IS() => this.WrapSpace("IS");
        public Expression IS_NULL() => this.WrapSpace("IS NULL");
        public Expression IS_NOT_NULL() => this.WrapSpace("IS NOT NULL");
        public Expression NOT() => this.WrapSpace("NOT");
        public Expression NULL() => this.WrapSpace("NULL");


        #region +-*/, compare, logical operation

        /// <summary>
        /// Compound expression
        /// </summary>
        private bool compound = false;

        private static string ExpToString(Expression exp)
        {
            if (exp.compound)
                return string.Format("({0})", exp);
            else
                return exp.ToString();
        }

        internal static Expression OPR(Expression exp1, string opr, Expression exp2)
        {
            Expression exp = new Expression()
                .Append(string.Format("{0} {1} {2}", ExpToString(exp1), opr, ExpToString(exp2)));

            exp.Merge(exp1).Merge(exp2);

            exp.compound = true;
            return exp;
        }

        // AND(A==1, B!=3, C>4) => "(A=1 AND B<>3 AND C>4)"
        internal static Expression OPR(Expression exp1, string opr, Expression[] exps)
        {
            Expression exp = new Expression();
            exp.Append("(")
               .Append(string.Format("{0}", ExpToString(exp1)));

            foreach (Expression exp2 in exps)
            {
                exp.Append(string.Format(" {0} {1}", opr, ExpToString(exp2)));
            }

            exp.compound = true;
            return exp.Append(")");
        }

        private static Expression OPR(string opr, Expression exp1)
        {
            Expression exp = new Expression()
                .Append(string.Format("{0} {1}", opr, ExpToString(exp1)));

            exp.Merge(exp1);
            return exp;
        }

        public static Expression operator -(Expression exp1)
        {
            return OPR("-", exp1);
        }

        public static Expression operator +(Expression exp1)
        {
            return OPR("+", exp1);
        }

        public static Expression operator +(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "+", exp2);
        }

        public static Expression operator -(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "-", exp2);
        }

        public static Expression operator *(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "*", exp2);
        }

        public static Expression operator /(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "/", exp2);
        }

        public static Expression operator %(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "%", exp2);
        }


        public static Expression operator ==(Expression exp1, Expression exp2)
        {
            if ((object)exp2 == null || exp2.ToString() == "NULL")
            {
                Expression exp = new Expression().Append(exp1).Append(" IS NULL");
                exp.Merge(exp1);
                return exp;
            }

            return OPR(exp1, "=", exp2);
        }


        public static Expression operator !=(Expression exp1, Expression exp2)
        {
            if ((object)exp2 == null || exp2.ToString() == "NULL")
            {
                Expression exp = new Expression().Append(exp1).Append(" IS NOT NULL");
                exp.Merge(exp1);
                return exp;
            }

            return OPR(exp1, "<>", exp2);
        }

        public static Expression operator >(Expression exp1, Expression exp2)
        {
            return OPR(exp1, ">", exp2);
        }

        public static Expression operator <(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "<", exp2);
        }

        public static Expression operator >=(Expression exp1, Expression exp2)
        {
            return OPR(exp1, ">=", exp2);
        }

        public static Expression operator <=(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "<=", exp2);
        }


        public static Expression operator &(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "AND", exp2);
        }

        public static Expression operator |(Expression exp1, Expression exp2)
        {
            return OPR(exp1, "OR", exp2);
        }

        public static Expression operator ~(Expression exp)
        {
            return OPR("NOT", exp);
        }

        #endregion


        #region SQL Function

        internal static Expression Func(string func, params Expression[] expl)
        {
            Expression exp = new Expression()
                .Append(func)
                .Append("(")
                .Append(string.Join<Expression>(",", expl))
                .Append(")");

            //exp.Merge(exp1);
            return exp;
        }


        #endregion


        public override bool Equals(object obj)
        {
            return script.Equals(((Expression)obj).script);
        }

        public override int GetHashCode()
        {
            return script.GetHashCode();
        }

        public override string ToString()
        {
            return script.ToString();
        }
    }
}
