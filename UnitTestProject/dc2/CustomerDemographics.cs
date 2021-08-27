using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class CustomerDemographics
		: IEntityRow, IEquatable<CustomerDemographics>
	{
		public string CustomerTypeID { get; set; }
		public string CustomerDesc { get; set; }
		
		public CustomerDemographics()
		{
		}
		
		public CustomerDemographics(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.CustomerTypeID = row.GetField<string>(_CUSTOMERTYPEID);
			this.CustomerDesc = row.GetField<string>(_CUSTOMERDESC);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_CUSTOMERTYPEID, this.CustomerTypeID);
			row.SetField(_CUSTOMERDESC, this.CustomerDesc);
		}
		
		public void CopyTo(CustomerDemographics obj)
		{
			obj.CustomerTypeID = this.CustomerTypeID;
			obj.CustomerDesc = this.CustomerDesc;
		}
		
		public bool Equals(CustomerDemographics obj)
		{
			return this.CustomerTypeID == obj.CustomerTypeID
			&& this.CustomerDesc == obj.CustomerDesc;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_CUSTOMERTYPEID, typeof(string)));
			dt.Columns.Add(new DataColumn(_CUSTOMERDESC, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_CUSTOMERTYPEID] = this.CustomerTypeID,
				[_CUSTOMERDESC] = this.CustomerDesc
			};
		}
		
		public CustomerDemographics(IDictionary<string, object> dict)
		{
			this.CustomerTypeID = (string)dict[_CUSTOMERTYPEID];
			this.CustomerDesc = (string)dict[_CUSTOMERDESC];
		}
		
		public CustomerDemographicsAssociation GetAssociation()
		{
			return GetAssociation(new CustomerDemographics[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<CustomerDemographicsAssociation> GetAssociation(IEnumerable<CustomerDemographics> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<CustomerDemographicsAssociation>();
			
			var _CustomerCustomerDemoes = reader.Read<CustomerCustomerDemo>();
			
			foreach (var entity in entities)
			{
				var association = new CustomerDemographicsAssociation
				{
					CustomerCustomerDemoes = new EntitySet<CustomerCustomerDemo>(_CustomerCustomerDemoes.Where(row => row.CustomerTypeID == entity.CustomerTypeID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{CustomerTypeID:{0}, CustomerDesc:{1}}}", 
			CustomerTypeID, 
			CustomerDesc);
		}
		
		public const string TableName = "CustomerDemographics";
		public static readonly string[] Keys = new string[] { _CUSTOMERTYPEID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<CustomerCustomerDemo>
			{
				ThisKey = _CUSTOMERTYPEID,
				OtherKey = CustomerCustomerDemo._CUSTOMERTYPEID,
				OneToMany = true
			}
		};
		
		public const string _CUSTOMERTYPEID = "CustomerTypeID";
		public const string _CUSTOMERDESC = "CustomerDesc";
	}
	
	public class CustomerDemographicsAssociation
	{
		public EntitySet<CustomerCustomerDemo> CustomerCustomerDemoes { get; set; }
	}
}