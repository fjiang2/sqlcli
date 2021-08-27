using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Orders
		: IEntityRow, IEquatable<Orders>
	{
		public int OrderID { get; set; }
		public string CustomerID { get; set; }
		public int EmployeeID { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime RequiredDate { get; set; }
		public DateTime ShippedDate { get; set; }
		public int ShipVia { get; set; }
		public decimal Freight { get; set; }
		public string ShipName { get; set; }
		public string ShipAddress { get; set; }
		public string ShipCity { get; set; }
		public string ShipRegion { get; set; }
		public string ShipPostalCode { get; set; }
		public string ShipCountry { get; set; }
		
		public Orders()
		{
		}
		
		public Orders(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.OrderID = row.GetField<int>(_ORDERID);
			this.CustomerID = row.GetField<string>(_CUSTOMERID);
			this.EmployeeID = row.GetField<int>(_EMPLOYEEID);
			this.OrderDate = row.GetField<DateTime>(_ORDERDATE);
			this.RequiredDate = row.GetField<DateTime>(_REQUIREDDATE);
			this.ShippedDate = row.GetField<DateTime>(_SHIPPEDDATE);
			this.ShipVia = row.GetField<int>(_SHIPVIA);
			this.Freight = row.GetField<decimal>(_FREIGHT);
			this.ShipName = row.GetField<string>(_SHIPNAME);
			this.ShipAddress = row.GetField<string>(_SHIPADDRESS);
			this.ShipCity = row.GetField<string>(_SHIPCITY);
			this.ShipRegion = row.GetField<string>(_SHIPREGION);
			this.ShipPostalCode = row.GetField<string>(_SHIPPOSTALCODE);
			this.ShipCountry = row.GetField<string>(_SHIPCOUNTRY);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_ORDERID, this.OrderID);
			row.SetField(_CUSTOMERID, this.CustomerID);
			row.SetField(_EMPLOYEEID, this.EmployeeID);
			row.SetField(_ORDERDATE, this.OrderDate);
			row.SetField(_REQUIREDDATE, this.RequiredDate);
			row.SetField(_SHIPPEDDATE, this.ShippedDate);
			row.SetField(_SHIPVIA, this.ShipVia);
			row.SetField(_FREIGHT, this.Freight);
			row.SetField(_SHIPNAME, this.ShipName);
			row.SetField(_SHIPADDRESS, this.ShipAddress);
			row.SetField(_SHIPCITY, this.ShipCity);
			row.SetField(_SHIPREGION, this.ShipRegion);
			row.SetField(_SHIPPOSTALCODE, this.ShipPostalCode);
			row.SetField(_SHIPCOUNTRY, this.ShipCountry);
		}
		
		public void CopyTo(Orders obj)
		{
			obj.OrderID = this.OrderID;
			obj.CustomerID = this.CustomerID;
			obj.EmployeeID = this.EmployeeID;
			obj.OrderDate = this.OrderDate;
			obj.RequiredDate = this.RequiredDate;
			obj.ShippedDate = this.ShippedDate;
			obj.ShipVia = this.ShipVia;
			obj.Freight = this.Freight;
			obj.ShipName = this.ShipName;
			obj.ShipAddress = this.ShipAddress;
			obj.ShipCity = this.ShipCity;
			obj.ShipRegion = this.ShipRegion;
			obj.ShipPostalCode = this.ShipPostalCode;
			obj.ShipCountry = this.ShipCountry;
		}
		
		public bool Equals(Orders obj)
		{
			return this.OrderID == obj.OrderID
			&& this.CustomerID == obj.CustomerID
			&& this.EmployeeID == obj.EmployeeID
			&& this.OrderDate == obj.OrderDate
			&& this.RequiredDate == obj.RequiredDate
			&& this.ShippedDate == obj.ShippedDate
			&& this.ShipVia == obj.ShipVia
			&& this.Freight == obj.Freight
			&& this.ShipName == obj.ShipName
			&& this.ShipAddress == obj.ShipAddress
			&& this.ShipCity == obj.ShipCity
			&& this.ShipRegion == obj.ShipRegion
			&& this.ShipPostalCode == obj.ShipPostalCode
			&& this.ShipCountry == obj.ShipCountry;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_ORDERID, typeof(int)));
			dt.Columns.Add(new DataColumn(_CUSTOMERID, typeof(string)));
			dt.Columns.Add(new DataColumn(_EMPLOYEEID, typeof(int)));
			dt.Columns.Add(new DataColumn(_ORDERDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_REQUIREDDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_SHIPPEDDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_SHIPVIA, typeof(int)));
			dt.Columns.Add(new DataColumn(_FREIGHT, typeof(decimal)));
			dt.Columns.Add(new DataColumn(_SHIPNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_SHIPADDRESS, typeof(string)));
			dt.Columns.Add(new DataColumn(_SHIPCITY, typeof(string)));
			dt.Columns.Add(new DataColumn(_SHIPREGION, typeof(string)));
			dt.Columns.Add(new DataColumn(_SHIPPOSTALCODE, typeof(string)));
			dt.Columns.Add(new DataColumn(_SHIPCOUNTRY, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_ORDERID] = this.OrderID,
				[_CUSTOMERID] = this.CustomerID,
				[_EMPLOYEEID] = this.EmployeeID,
				[_ORDERDATE] = this.OrderDate,
				[_REQUIREDDATE] = this.RequiredDate,
				[_SHIPPEDDATE] = this.ShippedDate,
				[_SHIPVIA] = this.ShipVia,
				[_FREIGHT] = this.Freight,
				[_SHIPNAME] = this.ShipName,
				[_SHIPADDRESS] = this.ShipAddress,
				[_SHIPCITY] = this.ShipCity,
				[_SHIPREGION] = this.ShipRegion,
				[_SHIPPOSTALCODE] = this.ShipPostalCode,
				[_SHIPCOUNTRY] = this.ShipCountry
			};
		}
		
		public Orders(IDictionary<string, object> dict)
		{
			this.OrderID = (int)dict[_ORDERID];
			this.CustomerID = (string)dict[_CUSTOMERID];
			this.EmployeeID = (int)dict[_EMPLOYEEID];
			this.OrderDate = (DateTime)dict[_ORDERDATE];
			this.RequiredDate = (DateTime)dict[_REQUIREDDATE];
			this.ShippedDate = (DateTime)dict[_SHIPPEDDATE];
			this.ShipVia = (int)dict[_SHIPVIA];
			this.Freight = (decimal)dict[_FREIGHT];
			this.ShipName = (string)dict[_SHIPNAME];
			this.ShipAddress = (string)dict[_SHIPADDRESS];
			this.ShipCity = (string)dict[_SHIPCITY];
			this.ShipRegion = (string)dict[_SHIPREGION];
			this.ShipPostalCode = (string)dict[_SHIPPOSTALCODE];
			this.ShipCountry = (string)dict[_SHIPCOUNTRY];
		}
		
		public OrdersAssociation GetAssociation()
		{
			return GetAssociation(new Orders[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<OrdersAssociation> GetAssociation(IEnumerable<Orders> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<OrdersAssociation>();
			
			var _Order_Details = reader.Read<Order_Details>();
			var _Customer = reader.Read<Customers>();
			var _Employee = reader.Read<Employees>();
			var _Shipper = reader.Read<Shippers>();
			
			foreach (var entity in entities)
			{
				var association = new OrdersAssociation
				{
					Order_Details = new EntitySet<Order_Details>(_Order_Details.Where(row => row.OrderID == entity.OrderID)),
					Customer = new EntityRef<Customers>(_Customer.FirstOrDefault(row => row.CustomerID == entity.CustomerID)),
					Employee = new EntityRef<Employees>(_Employee.FirstOrDefault(row => row.EmployeeID == entity.EmployeeID)),
					Shipper = new EntityRef<Shippers>(_Shipper.FirstOrDefault(row => row.ShipperID == entity.ShipVia)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{OrderID:{0}, CustomerID:{1}, EmployeeID:{2}, OrderDate:{3}, RequiredDate:{4}, ShippedDate:{5}, ShipVia:{6}, Freight:{7}, ShipName:{8}, ShipAddress:{9}, ShipCity:{10}, ShipRegion:{11}, ShipPostalCode:{12}, ShipCountry:{13}}}", 
			OrderID, 
			CustomerID, 
			EmployeeID, 
			OrderDate, 
			RequiredDate, 
			ShippedDate, 
			ShipVia, 
			Freight, 
			ShipName, 
			ShipAddress, 
			ShipCity, 
			ShipRegion, 
			ShipPostalCode, 
			ShipCountry);
		}
		
		public const string TableName = "Orders";
		public static readonly string[] Keys = new string[] { _ORDERID };
		public static readonly string[] Identity = new string[] { _ORDERID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Order_Details>
			{
				ThisKey = _ORDERID,
				OtherKey = Order_Details._ORDERID,
				OneToMany = true
			},
			new Constraint<Customers>
			{
				Name = "FK_Orders_Customers",
				ThisKey = _CUSTOMERID,
				OtherKey = Customers._CUSTOMERID,
				IsForeignKey = true
			},
			new Constraint<Employees>
			{
				Name = "FK_Orders_Employees",
				ThisKey = _EMPLOYEEID,
				OtherKey = Employees._EMPLOYEEID,
				IsForeignKey = true
			},
			new Constraint<Shippers>
			{
				Name = "FK_Orders_Shippers",
				ThisKey = _SHIPVIA,
				OtherKey = Shippers._SHIPPERID,
				IsForeignKey = true
			}
		};
		
		public const string _ORDERID = "OrderID";
		public const string _CUSTOMERID = "CustomerID";
		public const string _EMPLOYEEID = "EmployeeID";
		public const string _ORDERDATE = "OrderDate";
		public const string _REQUIREDDATE = "RequiredDate";
		public const string _SHIPPEDDATE = "ShippedDate";
		public const string _SHIPVIA = "ShipVia";
		public const string _FREIGHT = "Freight";
		public const string _SHIPNAME = "ShipName";
		public const string _SHIPADDRESS = "ShipAddress";
		public const string _SHIPCITY = "ShipCity";
		public const string _SHIPREGION = "ShipRegion";
		public const string _SHIPPOSTALCODE = "ShipPostalCode";
		public const string _SHIPCOUNTRY = "ShipCountry";
	}
	
	public class OrdersAssociation
	{
		public EntitySet<Order_Details> Order_Details { get; set; }
		public EntityRef<Customers> Customer { get; set; }
		public EntityRef<Employees> Employee { get; set; }
		public EntityRef<Shippers> Shipper { get; set; }
	}
}