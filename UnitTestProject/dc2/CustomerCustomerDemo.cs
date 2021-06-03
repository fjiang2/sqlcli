using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Linq;

namespace UnitTestProject.Northwind.dc2
{
	public partial class CustomerCustomerDemo
		: IEntityRow, IEquatable<CustomerCustomerDemo>
	{
		public string CustomerID { get; set; }
		public string CustomerTypeID { get; set; }
		
		public CustomerCustomerDemo()
		{
		}
		
		public CustomerCustomerDemo(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.CustomerID = row.GetField<string>(_CUSTOMERID);
			this.CustomerTypeID = row.GetField<string>(_CUSTOMERTYPEID);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_CUSTOMERID, this.CustomerID);
			row.SetField(_CUSTOMERTYPEID, this.CustomerTypeID);
		}
		
		public void CopyTo(CustomerCustomerDemo obj)
		{
			obj.CustomerID = this.CustomerID;
			obj.CustomerTypeID = this.CustomerTypeID;
		}
		
		public bool Equals(CustomerCustomerDemo obj)
		{
			return this.CustomerID == obj.CustomerID
			&& this.CustomerTypeID == obj.CustomerTypeID;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_CUSTOMERID, typeof(string)));
			dt.Columns.Add(new DataColumn(_CUSTOMERTYPEID, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_CUSTOMERID] = this.CustomerID,
				[_CUSTOMERTYPEID] = this.CustomerTypeID
			};
		}
		
		public CustomerCustomerDemo(IDictionary<string, object> dict)
		{
			this.CustomerID = (string)dict[_CUSTOMERID];
			this.CustomerTypeID = (string)dict[_CUSTOMERTYPEID];
		}
		
		public override string ToString()
		{
			return string.Format("{{CustomerID:{0}, CustomerTypeID:{1}}}", 
			CustomerID, 
			CustomerTypeID);
		}
		
		public const string TableName = "CustomerCustomerDemo";
		public static readonly string[] Keys = new string[] { _CUSTOMERID, _CUSTOMERTYPEID };
		
		public const string _CUSTOMERID = "CustomerID";
		public const string _CUSTOMERTYPEID = "CustomerTypeID";
	}
}