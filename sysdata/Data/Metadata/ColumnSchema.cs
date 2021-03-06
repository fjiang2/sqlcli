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
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sys.Data
{



    public class ColumnSchema : PersistentObject, IColumn, IDataPath
    {

        [Column("ColumnName", CType.NVarChar, Primary = true)]
        public string ColumnName { get; set; }

        [Column("DataType", CType.NVarChar)]
        public string DataType { get; set; }

        [Column("Length", CType.Int)]
        public short Length { get; set; }    //length return from SQL Server

        [Column("Nullable", CType.Bit)]
        public bool Nullable { get; set; }

        [Column("precision", CType.TinyInt)]
        public byte Precision { get; set; }

        [Column("scale", CType.TinyInt)]
        public byte Scale { get; set; }

        [Column("IsPrimary", CType.Bit)]
        public bool IsPrimary { get; set; }

        [Column("IsIdentity", CType.Bit)]
        public bool IsIdentity { get; set; }

        [Column("IsComputed", CType.Bit)]
        public bool IsComputed { get; set; }

        [Column("definition", CType.NVarChar)]
        public string Definition { get; set; }

        [Column("PKContraintName", CType.NVarChar)]
        public string PkContraintName { get; set; }

        [Column("PK_Schema", CType.NVarChar)]
        public string PK_Schema { get; set; }

        [Column("PK_Table", CType.NVarChar)]
        public string PK_Table { get; set; }

        [Column("PK_Column", CType.NVarChar)]
        public string PK_Column { get; set; }

        [Column("FKContraintName", CType.NVarChar)]
        public string FkContraintName { get; set; }

        [Column("ColumnID", CType.Int)]
        public int ColumnID { get; set; }    //column_id is from column dictionary

        [Column("label", CType.NVarChar)]
        public string label { get; set; }    //label used as caption to support internationalization


        private CType ctype;
        private IForeignKey foreignKey;

        public ColumnSchema(DataRow dataRow)
            : base(dataRow)
        {
            this.ctype = this.DataType.GetCType();
        }

        internal ColumnSchema(ColumnAttribute attr)
        {

            this.ColumnName = attr.ColumnName;
            SetCType(attr.CType);

            this.Nullable = attr.Nullable;
            this.Precision = attr.Precision;
            this.Scale = attr.Scale;
            this.IsIdentity = attr.Identity;
            this.IsComputed = attr.Computed;
            this.IsPrimary = attr.Primary;

            this.ColumnID = -1; //useless here
            this.label = attr.Caption;
        }

        public override void FillObject(DataRow dataRow)
        {
            ColumnName = (string)dataRow["ColumnName"];
            DataType = (string)dataRow["DataType"];
            Length = (short)dataRow["Length"];

            Nullable = (bool)dataRow["Nullable"];
            Precision = (byte)dataRow["precision"];
            Scale = (byte)dataRow["scale"];

            IsPrimary = (bool)dataRow["IsPrimary"];
            IsIdentity = (bool)dataRow["IsIdentity"];
            IsComputed = (bool)dataRow["IsComputed"];
            Definition = getField<string>(dataRow, "definition");

            PkContraintName = getField<string>(dataRow, "PKContraintName");
            PK_Schema = getField<string>(dataRow, "PK_Schema");
            PK_Table = getField<string>(dataRow, "PK_Table");
            PK_Column = getField<string>(dataRow, "PK_Column");
            FkContraintName = getField<string>(dataRow, "FKContraintName");

            ColumnID = getField<int>(dataRow, "ColumnID");
            label = getField<string>(dataRow, "label");
        }

        private T getField<T>(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return default(T);

            var obj = row[columnName];
            if (obj == DBNull.Value)
                return default(T);
            else
                return (T)obj;
        }

        public string Path => ColumnName;
        public string Caption
        {
            get
            {
                if (string.IsNullOrEmpty(this.label))
                    return ColumnName;
                else
                    return this.label;
            }
        }

        public CType CType
        {
            get { return this.ctype; }
        }

        private void SetCType(CType value)
        {
            this.ctype = value;
            this.DataType = value.ToString();
        }


        public bool IsForeignKey => FkContraintName != null;


        public IForeignKey ForeignKey => this.foreignKey;

        public void SetForeignKey(IForeignKey value)
        {
            this.foreignKey = value;
        }


        public override int GetHashCode()
        {
            return this.ColumnName.GetHashCode();
        }

        public override bool Equals(object obj)
        {

            if (!(obj is ColumnSchema it))
                return false;

            return this.ColumnName.Equals(it.ColumnName)
            && this.CType.Equals(it.CType)
            && this.Nullable.Equals(it.Nullable)
            && this.Precision.Equals(it.Precision)
            && this.Scale.Equals(it.Scale)
            && this.IsIdentity.Equals(it.IsIdentity)
            && this.IsComputed.Equals(it.IsComputed)
            && this.IsPrimary.Equals(it.IsPrimary);
        }

        public override string ToString()
        {
            return string.Format("Column={0}(Type={1}, Null={2}, Length={3})", ColumnName, DataType, Nullable, Length);
        }




        public object Parse(string val)
        {
            if (this.Nullable && (val == "" || val.ToUpper() == "NULL"))
                return null;

            switch (ctype)
            {
                case CType.VarChar:
                case CType.NVarChar:
                case CType.Char:
                case CType.NChar:
                    if (this.Oversize(val))
                        throw new MessageException("Column Name={0}, length of value \"{1}\" {2} > {3}", ColumnName, val, val.Length, this.AdjuestedLength());
                    else
                        return val;

                case CType.VarBinary:
                case CType.Binary:
                    throw new NotImplementedException(string.Format("cannot convert {0} into type of {1}", val, CType.Binary));


                case CType.Date:
                case CType.DateTime:
                    if (val.IndexOf("-") > 0)    //2011-10-30
                    {
                        string[] date = val.Split('-');
                        return new DateTime(Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(date[2]));
                    }
                    else if (val.Length == 8)    //20111030
                    {
                        int month = Convert.ToInt32(val.Substring(4, 2));
                        int day = Convert.ToInt32(val.Substring(6, 2));
                        if (month == 0)
                            month = 1;

                        if (day == 0)
                            day = 1;

                        return new DateTime(Convert.ToInt32(val.Substring(0, 4)), month, day);
                    }
                    else
                    {
                        return Convert.ToDateTime(val);
                    }

                case CType.Time:
                    {
                        string[] time = val.Split(':');
                        return new TimeSpan(Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), Convert.ToInt32(time[2]));
                    }

                case CType.Float:
                    return Convert.ToDouble(val);

                case CType.Real:
                    return Convert.ToSingle(val);


                case CType.Bit:
                    if (val == "0")
                        return false;
                    else if (val == "1")
                        return true;
                    else
                        return Convert.ToBoolean(val);

                case CType.Decimal:
                    return Convert.ToDecimal(val);

                case CType.TinyInt:
                    return Convert.ToByte(val);

                case CType.SmallInt:
                    return Convert.ToInt16(val);

                case CType.Int:
                    return Convert.ToInt32(val);

                case CType.BigInt:
                    return Convert.ToInt64(val);

                case CType.Xml:
                    return XElement.Parse(val);

                default:
                    throw new NotImplementedException(string.Format("cannot convert {0} into type of {1}", val, ctype));
            }

        }
    }
}
