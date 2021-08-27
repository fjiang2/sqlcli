using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Shippers
		: IEntityRow, IEquatable<Shippers>
	{
		public int ShipperID { get; set; }
		public string CompanyName { get; set; }
		public string Phone { get; set; }
		
		public Shippers()
		{
		}
		
		public Shippers(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.ShipperID = row.GetField<int>(_SHIPPERID);
			this.CompanyName = row.GetField<string>(_COMPANYNAME);
			this.Phone = row.GetField<string>(_PHONE);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_SHIPPERID, this.ShipperID);
			row.SetField(_COMPANYNAME, this.CompanyName);
			row.SetField(_PHONE, this.Phone);
		}
		
		public void CopyTo(Shippers obj)
		{
			obj.ShipperID = this.ShipperID;
			obj.CompanyName = this.CompanyName;
			obj.Phone = this.Phone;
		}
		
		public bool Equals(Shippers obj)
		{
			return this.ShipperID == obj.ShipperID
			&& this.CompanyName == obj.CompanyName
			&& this.Phone == obj.Phone;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_SHIPPERID, typeof(int)));
			dt.Columns.Add(new DataColumn(_COMPANYNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_PHONE, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_SHIPPERID] = this.ShipperID,
				[_COMPANYNAME] = this.CompanyName,
				[_PHONE] = this.Phone
			};
		}
		
		public Shippers(IDictionary<string, object> dict)
		{
			this.ShipperID = (int)dict[_SHIPPERID];
			this.CompanyName = (string)dict[_COMPANYNAME];
			this.Phone = (string)dict[_PHONE];
		}
		
		public ShippersAssociation GetAssociation()
		{
			return GetAssociation(new Shippers[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<ShippersAssociation> GetAssociation(IEnumerable<Shippers> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<ShippersAssociation>();
			
			var _Orders = reader.Read<Orders>();
			
			foreach (var entity in entities)
			{
				var association = new ShippersAssociation
				{
					Orders = new EntitySet<Orders>(_Orders.Where(row => row.ShipVia == entity.ShipperID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{ShipperID:{0}, CompanyName:{1}, Phone:{2}}}", 
			ShipperID, 
			CompanyName, 
			Phone);
		}
		
		public const string TableName = "Shippers";
		public static readonly string[] Keys = new string[] { _SHIPPERID };
		public static readonly string[] Identity = new string[] { _SHIPPERID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Orders>
			{
				ThisKey = _SHIPPERID,
				OtherKey = Orders._SHIPVIA,
				OneToMany = true
			}
		};
		
		public const string _SHIPPERID = "ShipperID";
		public const string _COMPANYNAME = "CompanyName";
		public const string _PHONE = "Phone";
	}
	
	public class ShippersAssociation
	{
		public EntitySet<Orders> Orders { get; set; }
	}
}