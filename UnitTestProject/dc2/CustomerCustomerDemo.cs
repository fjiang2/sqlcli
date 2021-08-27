using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

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
		
		public CustomerCustomerDemoAssociation GetAssociation()
		{
			return GetAssociation(new CustomerCustomerDemo[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<CustomerCustomerDemoAssociation> GetAssociation(IEnumerable<CustomerCustomerDemo> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<CustomerCustomerDemoAssociation>();
			
			var _Customer = reader.Read<Customers>();
			var _CustomerDemographic = reader.Read<CustomerDemographics>();
			
			foreach (var entity in entities)
			{
				var association = new CustomerCustomerDemoAssociation
				{
					Customer = new EntityRef<Customers>(_Customer.FirstOrDefault(row => row.CustomerID == entity.CustomerID)),
					CustomerDemographic = new EntityRef<CustomerDemographics>(_CustomerDemographic.FirstOrDefault(row => row.CustomerTypeID == entity.CustomerTypeID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{CustomerID:{0}, CustomerTypeID:{1}}}", 
			CustomerID, 
			CustomerTypeID);
		}
		
		public const string TableName = "CustomerCustomerDemo";
		public static readonly string[] Keys = new string[] { _CUSTOMERID, _CUSTOMERTYPEID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Customers>
			{
				Name = "FK_CustomerCustomerDemo_Customers",
				ThisKey = _CUSTOMERID,
				OtherKey = Customers._CUSTOMERID,
				IsForeignKey = true
			},
			new Constraint<CustomerDemographics>
			{
				Name = "FK_CustomerCustomerDemo",
				ThisKey = _CUSTOMERTYPEID,
				OtherKey = CustomerDemographics._CUSTOMERTYPEID,
				IsForeignKey = true
			}
		};
		
		public const string _CUSTOMERID = "CustomerID";
		public const string _CUSTOMERTYPEID = "CustomerTypeID";
	}
	
	public class CustomerCustomerDemoAssociation
	{
		public EntityRef<Customers> Customer { get; set; }
		public EntityRef<CustomerDemographics> CustomerDemographic { get; set; }
	}
}