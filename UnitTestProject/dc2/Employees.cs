using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Employees
		: IEntityRow, IEquatable<Employees>
	{
		public int EmployeeID { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string Title { get; set; }
		public string TitleOfCourtesy { get; set; }
		public DateTime BirthDate { get; set; }
		public DateTime HireDate { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public string HomePhone { get; set; }
		public string Extension { get; set; }
		public byte[] Photo { get; set; }
		public string Notes { get; set; }
		public int ReportsTo { get; set; }
		public string PhotoPath { get; set; }
		
		public Employees()
		{
		}
		
		public Employees(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.EmployeeID = row.GetField<int>(_EMPLOYEEID);
			this.LastName = row.GetField<string>(_LASTNAME);
			this.FirstName = row.GetField<string>(_FIRSTNAME);
			this.Title = row.GetField<string>(_TITLE);
			this.TitleOfCourtesy = row.GetField<string>(_TITLEOFCOURTESY);
			this.BirthDate = row.GetField<DateTime>(_BIRTHDATE);
			this.HireDate = row.GetField<DateTime>(_HIREDATE);
			this.Address = row.GetField<string>(_ADDRESS);
			this.City = row.GetField<string>(_CITY);
			this.Region = row.GetField<string>(_REGION);
			this.PostalCode = row.GetField<string>(_POSTALCODE);
			this.Country = row.GetField<string>(_COUNTRY);
			this.HomePhone = row.GetField<string>(_HOMEPHONE);
			this.Extension = row.GetField<string>(_EXTENSION);
			this.Photo = row.GetField<byte[]>(_PHOTO);
			this.Notes = row.GetField<string>(_NOTES);
			this.ReportsTo = row.GetField<int>(_REPORTSTO);
			this.PhotoPath = row.GetField<string>(_PHOTOPATH);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_EMPLOYEEID, this.EmployeeID);
			row.SetField(_LASTNAME, this.LastName);
			row.SetField(_FIRSTNAME, this.FirstName);
			row.SetField(_TITLE, this.Title);
			row.SetField(_TITLEOFCOURTESY, this.TitleOfCourtesy);
			row.SetField(_BIRTHDATE, this.BirthDate);
			row.SetField(_HIREDATE, this.HireDate);
			row.SetField(_ADDRESS, this.Address);
			row.SetField(_CITY, this.City);
			row.SetField(_REGION, this.Region);
			row.SetField(_POSTALCODE, this.PostalCode);
			row.SetField(_COUNTRY, this.Country);
			row.SetField(_HOMEPHONE, this.HomePhone);
			row.SetField(_EXTENSION, this.Extension);
			row.SetField(_PHOTO, this.Photo);
			row.SetField(_NOTES, this.Notes);
			row.SetField(_REPORTSTO, this.ReportsTo);
			row.SetField(_PHOTOPATH, this.PhotoPath);
		}
		
		public void CopyTo(Employees obj)
		{
			obj.EmployeeID = this.EmployeeID;
			obj.LastName = this.LastName;
			obj.FirstName = this.FirstName;
			obj.Title = this.Title;
			obj.TitleOfCourtesy = this.TitleOfCourtesy;
			obj.BirthDate = this.BirthDate;
			obj.HireDate = this.HireDate;
			obj.Address = this.Address;
			obj.City = this.City;
			obj.Region = this.Region;
			obj.PostalCode = this.PostalCode;
			obj.Country = this.Country;
			obj.HomePhone = this.HomePhone;
			obj.Extension = this.Extension;
			obj.Photo = this.Photo;
			obj.Notes = this.Notes;
			obj.ReportsTo = this.ReportsTo;
			obj.PhotoPath = this.PhotoPath;
		}
		
		public bool Equals(Employees obj)
		{
			return this.EmployeeID == obj.EmployeeID
			&& this.LastName == obj.LastName
			&& this.FirstName == obj.FirstName
			&& this.Title == obj.Title
			&& this.TitleOfCourtesy == obj.TitleOfCourtesy
			&& this.BirthDate == obj.BirthDate
			&& this.HireDate == obj.HireDate
			&& this.Address == obj.Address
			&& this.City == obj.City
			&& this.Region == obj.Region
			&& this.PostalCode == obj.PostalCode
			&& this.Country == obj.Country
			&& this.HomePhone == obj.HomePhone
			&& this.Extension == obj.Extension
			&& this.Photo == obj.Photo
			&& this.Notes == obj.Notes
			&& this.ReportsTo == obj.ReportsTo
			&& this.PhotoPath == obj.PhotoPath;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_EMPLOYEEID, typeof(int)));
			dt.Columns.Add(new DataColumn(_LASTNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_FIRSTNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_TITLE, typeof(string)));
			dt.Columns.Add(new DataColumn(_TITLEOFCOURTESY, typeof(string)));
			dt.Columns.Add(new DataColumn(_BIRTHDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_HIREDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_ADDRESS, typeof(string)));
			dt.Columns.Add(new DataColumn(_CITY, typeof(string)));
			dt.Columns.Add(new DataColumn(_REGION, typeof(string)));
			dt.Columns.Add(new DataColumn(_POSTALCODE, typeof(string)));
			dt.Columns.Add(new DataColumn(_COUNTRY, typeof(string)));
			dt.Columns.Add(new DataColumn(_HOMEPHONE, typeof(string)));
			dt.Columns.Add(new DataColumn(_EXTENSION, typeof(string)));
			dt.Columns.Add(new DataColumn(_PHOTO, typeof(byte[])));
			dt.Columns.Add(new DataColumn(_NOTES, typeof(string)));
			dt.Columns.Add(new DataColumn(_REPORTSTO, typeof(int)));
			dt.Columns.Add(new DataColumn(_PHOTOPATH, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_EMPLOYEEID] = this.EmployeeID,
				[_LASTNAME] = this.LastName,
				[_FIRSTNAME] = this.FirstName,
				[_TITLE] = this.Title,
				[_TITLEOFCOURTESY] = this.TitleOfCourtesy,
				[_BIRTHDATE] = this.BirthDate,
				[_HIREDATE] = this.HireDate,
				[_ADDRESS] = this.Address,
				[_CITY] = this.City,
				[_REGION] = this.Region,
				[_POSTALCODE] = this.PostalCode,
				[_COUNTRY] = this.Country,
				[_HOMEPHONE] = this.HomePhone,
				[_EXTENSION] = this.Extension,
				[_PHOTO] = this.Photo,
				[_NOTES] = this.Notes,
				[_REPORTSTO] = this.ReportsTo,
				[_PHOTOPATH] = this.PhotoPath
			};
		}
		
		public Employees(IDictionary<string, object> dict)
		{
			this.EmployeeID = (int)dict[_EMPLOYEEID];
			this.LastName = (string)dict[_LASTNAME];
			this.FirstName = (string)dict[_FIRSTNAME];
			this.Title = (string)dict[_TITLE];
			this.TitleOfCourtesy = (string)dict[_TITLEOFCOURTESY];
			this.BirthDate = (DateTime)dict[_BIRTHDATE];
			this.HireDate = (DateTime)dict[_HIREDATE];
			this.Address = (string)dict[_ADDRESS];
			this.City = (string)dict[_CITY];
			this.Region = (string)dict[_REGION];
			this.PostalCode = (string)dict[_POSTALCODE];
			this.Country = (string)dict[_COUNTRY];
			this.HomePhone = (string)dict[_HOMEPHONE];
			this.Extension = (string)dict[_EXTENSION];
			this.Photo = (byte[])dict[_PHOTO];
			this.Notes = (string)dict[_NOTES];
			this.ReportsTo = (int)dict[_REPORTSTO];
			this.PhotoPath = (string)dict[_PHOTOPATH];
		}
		
		public EmployeesAssociation GetAssociation()
		{
			return GetAssociation(new Employees[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<EmployeesAssociation> GetAssociation(IEnumerable<Employees> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<EmployeesAssociation>();
			
			var _EmployeeTerritories = reader.Read<EmployeeTerritories>();
			var _Orders = reader.Read<Orders>();
			var _Employee = reader.Read<Employees>();
			
			foreach (var entity in entities)
			{
				var association = new EmployeesAssociation
				{
					EmployeeTerritories = new EntitySet<EmployeeTerritories>(_EmployeeTerritories.Where(row => row.EmployeeID == entity.EmployeeID)),
					Orders = new EntitySet<Orders>(_Orders.Where(row => row.EmployeeID == entity.EmployeeID)),
					Employee = new EntityRef<Employees>(_Employee.FirstOrDefault(row => row.EmployeeID == entity.ReportsTo)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{EmployeeID:{0}, LastName:{1}, FirstName:{2}, Title:{3}, TitleOfCourtesy:{4}, BirthDate:{5}, HireDate:{6}, Address:{7}, City:{8}, Region:{9}, PostalCode:{10}, Country:{11}, HomePhone:{12}, Extension:{13}, Photo:{14}, Notes:{15}, ReportsTo:{16}, PhotoPath:{17}}}", 
			EmployeeID, 
			LastName, 
			FirstName, 
			Title, 
			TitleOfCourtesy, 
			BirthDate, 
			HireDate, 
			Address, 
			City, 
			Region, 
			PostalCode, 
			Country, 
			HomePhone, 
			Extension, 
			Photo, 
			Notes, 
			ReportsTo, 
			PhotoPath);
		}
		
		public const string TableName = "Employees";
		public static readonly string[] Keys = new string[] { _EMPLOYEEID };
		public static readonly string[] Identity = new string[] { _EMPLOYEEID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<EmployeeTerritories>
			{
				ThisKey = _EMPLOYEEID,
				OtherKey = EmployeeTerritories._EMPLOYEEID,
				OneToMany = true
			},
			new Constraint<Orders>
			{
				ThisKey = _EMPLOYEEID,
				OtherKey = Orders._EMPLOYEEID,
				OneToMany = true
			},
			new Constraint<Employees>
			{
				Name = "FK_Employees_Employees",
				ThisKey = _REPORTSTO,
				OtherKey = Employees._EMPLOYEEID,
				IsForeignKey = true
			}
		};
		
		public const string _EMPLOYEEID = "EmployeeID";
		public const string _LASTNAME = "LastName";
		public const string _FIRSTNAME = "FirstName";
		public const string _TITLE = "Title";
		public const string _TITLEOFCOURTESY = "TitleOfCourtesy";
		public const string _BIRTHDATE = "BirthDate";
		public const string _HIREDATE = "HireDate";
		public const string _ADDRESS = "Address";
		public const string _CITY = "City";
		public const string _REGION = "Region";
		public const string _POSTALCODE = "PostalCode";
		public const string _COUNTRY = "Country";
		public const string _HOMEPHONE = "HomePhone";
		public const string _EXTENSION = "Extension";
		public const string _PHOTO = "Photo";
		public const string _NOTES = "Notes";
		public const string _REPORTSTO = "ReportsTo";
		public const string _PHOTOPATH = "PhotoPath";
	}
	
	public class EmployeesAssociation
	{
		public EntitySet<EmployeeTerritories> EmployeeTerritories { get; set; }
		public EntitySet<Orders> Orders { get; set; }
		public EntityRef<Employees> Employee { get; set; }
	}
}