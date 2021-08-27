using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Suppliers
		: IEntityRow, IEquatable<Suppliers>
	{
		public int SupplierID { get; set; }
		public string CompanyName { get; set; }
		public string ContactName { get; set; }
		public string ContactTitle { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string HomePage { get; set; }
		
		public Suppliers()
		{
		}
		
		public Suppliers(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.SupplierID = row.GetField<int>(_SUPPLIERID);
			this.CompanyName = row.GetField<string>(_COMPANYNAME);
			this.ContactName = row.GetField<string>(_CONTACTNAME);
			this.ContactTitle = row.GetField<string>(_CONTACTTITLE);
			this.Address = row.GetField<string>(_ADDRESS);
			this.City = row.GetField<string>(_CITY);
			this.Region = row.GetField<string>(_REGION);
			this.PostalCode = row.GetField<string>(_POSTALCODE);
			this.Country = row.GetField<string>(_COUNTRY);
			this.Phone = row.GetField<string>(_PHONE);
			this.Fax = row.GetField<string>(_FAX);
			this.HomePage = row.GetField<string>(_HOMEPAGE);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_SUPPLIERID, this.SupplierID);
			row.SetField(_COMPANYNAME, this.CompanyName);
			row.SetField(_CONTACTNAME, this.ContactName);
			row.SetField(_CONTACTTITLE, this.ContactTitle);
			row.SetField(_ADDRESS, this.Address);
			row.SetField(_CITY, this.City);
			row.SetField(_REGION, this.Region);
			row.SetField(_POSTALCODE, this.PostalCode);
			row.SetField(_COUNTRY, this.Country);
			row.SetField(_PHONE, this.Phone);
			row.SetField(_FAX, this.Fax);
			row.SetField(_HOMEPAGE, this.HomePage);
		}
		
		public void CopyTo(Suppliers obj)
		{
			obj.SupplierID = this.SupplierID;
			obj.CompanyName = this.CompanyName;
			obj.ContactName = this.ContactName;
			obj.ContactTitle = this.ContactTitle;
			obj.Address = this.Address;
			obj.City = this.City;
			obj.Region = this.Region;
			obj.PostalCode = this.PostalCode;
			obj.Country = this.Country;
			obj.Phone = this.Phone;
			obj.Fax = this.Fax;
			obj.HomePage = this.HomePage;
		}
		
		public bool Equals(Suppliers obj)
		{
			return this.SupplierID == obj.SupplierID
			&& this.CompanyName == obj.CompanyName
			&& this.ContactName == obj.ContactName
			&& this.ContactTitle == obj.ContactTitle
			&& this.Address == obj.Address
			&& this.City == obj.City
			&& this.Region == obj.Region
			&& this.PostalCode == obj.PostalCode
			&& this.Country == obj.Country
			&& this.Phone == obj.Phone
			&& this.Fax == obj.Fax
			&& this.HomePage == obj.HomePage;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_SUPPLIERID, typeof(int)));
			dt.Columns.Add(new DataColumn(_COMPANYNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_CONTACTNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_CONTACTTITLE, typeof(string)));
			dt.Columns.Add(new DataColumn(_ADDRESS, typeof(string)));
			dt.Columns.Add(new DataColumn(_CITY, typeof(string)));
			dt.Columns.Add(new DataColumn(_REGION, typeof(string)));
			dt.Columns.Add(new DataColumn(_POSTALCODE, typeof(string)));
			dt.Columns.Add(new DataColumn(_COUNTRY, typeof(string)));
			dt.Columns.Add(new DataColumn(_PHONE, typeof(string)));
			dt.Columns.Add(new DataColumn(_FAX, typeof(string)));
			dt.Columns.Add(new DataColumn(_HOMEPAGE, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_SUPPLIERID] = this.SupplierID,
				[_COMPANYNAME] = this.CompanyName,
				[_CONTACTNAME] = this.ContactName,
				[_CONTACTTITLE] = this.ContactTitle,
				[_ADDRESS] = this.Address,
				[_CITY] = this.City,
				[_REGION] = this.Region,
				[_POSTALCODE] = this.PostalCode,
				[_COUNTRY] = this.Country,
				[_PHONE] = this.Phone,
				[_FAX] = this.Fax,
				[_HOMEPAGE] = this.HomePage
			};
		}
		
		public Suppliers(IDictionary<string, object> dict)
		{
			this.SupplierID = (int)dict[_SUPPLIERID];
			this.CompanyName = (string)dict[_COMPANYNAME];
			this.ContactName = (string)dict[_CONTACTNAME];
			this.ContactTitle = (string)dict[_CONTACTTITLE];
			this.Address = (string)dict[_ADDRESS];
			this.City = (string)dict[_CITY];
			this.Region = (string)dict[_REGION];
			this.PostalCode = (string)dict[_POSTALCODE];
			this.Country = (string)dict[_COUNTRY];
			this.Phone = (string)dict[_PHONE];
			this.Fax = (string)dict[_FAX];
			this.HomePage = (string)dict[_HOMEPAGE];
		}
		
		public SuppliersAssociation GetAssociation()
		{
			return GetAssociation(new Suppliers[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<SuppliersAssociation> GetAssociation(IEnumerable<Suppliers> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<SuppliersAssociation>();
			
			var _Products = reader.Read<Products>();
			
			foreach (var entity in entities)
			{
				var association = new SuppliersAssociation
				{
					Products = new EntitySet<Products>(_Products.Where(row => row.SupplierID == entity.SupplierID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{SupplierID:{0}, CompanyName:{1}, ContactName:{2}, ContactTitle:{3}, Address:{4}, City:{5}, Region:{6}, PostalCode:{7}, Country:{8}, Phone:{9}, Fax:{10}, HomePage:{11}}}", 
			SupplierID, 
			CompanyName, 
			ContactName, 
			ContactTitle, 
			Address, 
			City, 
			Region, 
			PostalCode, 
			Country, 
			Phone, 
			Fax, 
			HomePage);
		}
		
		public const string TableName = "Suppliers";
		public static readonly string[] Keys = new string[] { _SUPPLIERID };
		public static readonly string[] Identity = new string[] { _SUPPLIERID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Products>
			{
				ThisKey = _SUPPLIERID,
				OtherKey = Products._SUPPLIERID,
				OneToMany = true
			}
		};
		
		public const string _SUPPLIERID = "SupplierID";
		public const string _COMPANYNAME = "CompanyName";
		public const string _CONTACTNAME = "ContactName";
		public const string _CONTACTTITLE = "ContactTitle";
		public const string _ADDRESS = "Address";
		public const string _CITY = "City";
		public const string _REGION = "Region";
		public const string _POSTALCODE = "PostalCode";
		public const string _COUNTRY = "Country";
		public const string _PHONE = "Phone";
		public const string _FAX = "Fax";
		public const string _HOMEPAGE = "HomePage";
	}
	
	public class SuppliersAssociation
	{
		public EntitySet<Products> Products { get; set; }
	}
}