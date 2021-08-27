using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Products
		: IEntityRow, IEquatable<Products>
	{
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public int SupplierID { get; set; }
		public int CategoryID { get; set; }
		public string QuantityPerUnit { get; set; }
		public decimal UnitPrice { get; set; }
		public short UnitsInStock { get; set; }
		public short UnitsOnOrder { get; set; }
		public short ReorderLevel { get; set; }
		public bool Discontinued { get; set; }
		
		public Products()
		{
		}
		
		public Products(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.ProductID = row.GetField<int>(_PRODUCTID);
			this.ProductName = row.GetField<string>(_PRODUCTNAME);
			this.SupplierID = row.GetField<int>(_SUPPLIERID);
			this.CategoryID = row.GetField<int>(_CATEGORYID);
			this.QuantityPerUnit = row.GetField<string>(_QUANTITYPERUNIT);
			this.UnitPrice = row.GetField<decimal>(_UNITPRICE);
			this.UnitsInStock = row.GetField<short>(_UNITSINSTOCK);
			this.UnitsOnOrder = row.GetField<short>(_UNITSONORDER);
			this.ReorderLevel = row.GetField<short>(_REORDERLEVEL);
			this.Discontinued = row.GetField<bool>(_DISCONTINUED);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_PRODUCTID, this.ProductID);
			row.SetField(_PRODUCTNAME, this.ProductName);
			row.SetField(_SUPPLIERID, this.SupplierID);
			row.SetField(_CATEGORYID, this.CategoryID);
			row.SetField(_QUANTITYPERUNIT, this.QuantityPerUnit);
			row.SetField(_UNITPRICE, this.UnitPrice);
			row.SetField(_UNITSINSTOCK, this.UnitsInStock);
			row.SetField(_UNITSONORDER, this.UnitsOnOrder);
			row.SetField(_REORDERLEVEL, this.ReorderLevel);
			row.SetField(_DISCONTINUED, this.Discontinued);
		}
		
		public void CopyTo(Products obj)
		{
			obj.ProductID = this.ProductID;
			obj.ProductName = this.ProductName;
			obj.SupplierID = this.SupplierID;
			obj.CategoryID = this.CategoryID;
			obj.QuantityPerUnit = this.QuantityPerUnit;
			obj.UnitPrice = this.UnitPrice;
			obj.UnitsInStock = this.UnitsInStock;
			obj.UnitsOnOrder = this.UnitsOnOrder;
			obj.ReorderLevel = this.ReorderLevel;
			obj.Discontinued = this.Discontinued;
		}
		
		public bool Equals(Products obj)
		{
			return this.ProductID == obj.ProductID
			&& this.ProductName == obj.ProductName
			&& this.SupplierID == obj.SupplierID
			&& this.CategoryID == obj.CategoryID
			&& this.QuantityPerUnit == obj.QuantityPerUnit
			&& this.UnitPrice == obj.UnitPrice
			&& this.UnitsInStock == obj.UnitsInStock
			&& this.UnitsOnOrder == obj.UnitsOnOrder
			&& this.ReorderLevel == obj.ReorderLevel
			&& this.Discontinued == obj.Discontinued;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_PRODUCTID, typeof(int)));
			dt.Columns.Add(new DataColumn(_PRODUCTNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_SUPPLIERID, typeof(int)));
			dt.Columns.Add(new DataColumn(_CATEGORYID, typeof(int)));
			dt.Columns.Add(new DataColumn(_QUANTITYPERUNIT, typeof(string)));
			dt.Columns.Add(new DataColumn(_UNITPRICE, typeof(decimal)));
			dt.Columns.Add(new DataColumn(_UNITSINSTOCK, typeof(short)));
			dt.Columns.Add(new DataColumn(_UNITSONORDER, typeof(short)));
			dt.Columns.Add(new DataColumn(_REORDERLEVEL, typeof(short)));
			dt.Columns.Add(new DataColumn(_DISCONTINUED, typeof(bool)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_PRODUCTID] = this.ProductID,
				[_PRODUCTNAME] = this.ProductName,
				[_SUPPLIERID] = this.SupplierID,
				[_CATEGORYID] = this.CategoryID,
				[_QUANTITYPERUNIT] = this.QuantityPerUnit,
				[_UNITPRICE] = this.UnitPrice,
				[_UNITSINSTOCK] = this.UnitsInStock,
				[_UNITSONORDER] = this.UnitsOnOrder,
				[_REORDERLEVEL] = this.ReorderLevel,
				[_DISCONTINUED] = this.Discontinued
			};
		}
		
		public Products(IDictionary<string, object> dict)
		{
			this.ProductID = (int)dict[_PRODUCTID];
			this.ProductName = (string)dict[_PRODUCTNAME];
			this.SupplierID = (int)dict[_SUPPLIERID];
			this.CategoryID = (int)dict[_CATEGORYID];
			this.QuantityPerUnit = (string)dict[_QUANTITYPERUNIT];
			this.UnitPrice = (decimal)dict[_UNITPRICE];
			this.UnitsInStock = (short)dict[_UNITSINSTOCK];
			this.UnitsOnOrder = (short)dict[_UNITSONORDER];
			this.ReorderLevel = (short)dict[_REORDERLEVEL];
			this.Discontinued = (bool)dict[_DISCONTINUED];
		}
		
		public ProductsAssociation GetAssociation()
		{
			return GetAssociation(new Products[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<ProductsAssociation> GetAssociation(IEnumerable<Products> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<ProductsAssociation>();
			
			var _Order_Details = reader.Read<Order_Details>();
			var _Supplier = reader.Read<Suppliers>();
			var _Category = reader.Read<Categories>();
			
			foreach (var entity in entities)
			{
				var association = new ProductsAssociation
				{
					Order_Details = new EntitySet<Order_Details>(_Order_Details.Where(row => row.ProductID == entity.ProductID)),
					Supplier = new EntityRef<Suppliers>(_Supplier.FirstOrDefault(row => row.SupplierID == entity.SupplierID)),
					Category = new EntityRef<Categories>(_Category.FirstOrDefault(row => row.CategoryID == entity.CategoryID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{ProductID:{0}, ProductName:{1}, SupplierID:{2}, CategoryID:{3}, QuantityPerUnit:{4}, UnitPrice:{5}, UnitsInStock:{6}, UnitsOnOrder:{7}, ReorderLevel:{8}, Discontinued:{9}}}", 
			ProductID, 
			ProductName, 
			SupplierID, 
			CategoryID, 
			QuantityPerUnit, 
			UnitPrice, 
			UnitsInStock, 
			UnitsOnOrder, 
			ReorderLevel, 
			Discontinued);
		}
		
		public const string TableName = "Products";
		public static readonly string[] Keys = new string[] { _PRODUCTID };
		public static readonly string[] Identity = new string[] { _PRODUCTID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Order_Details>
			{
				ThisKey = _PRODUCTID,
				OtherKey = Order_Details._PRODUCTID,
				OneToMany = true
			},
			new Constraint<Suppliers>
			{
				Name = "FK_Products_Suppliers",
				ThisKey = _SUPPLIERID,
				OtherKey = Suppliers._SUPPLIERID,
				IsForeignKey = true
			},
			new Constraint<Categories>
			{
				Name = "FK_Products_Categories",
				ThisKey = _CATEGORYID,
				OtherKey = Categories._CATEGORYID,
				IsForeignKey = true
			}
		};
		
		public const string _PRODUCTID = "ProductID";
		public const string _PRODUCTNAME = "ProductName";
		public const string _SUPPLIERID = "SupplierID";
		public const string _CATEGORYID = "CategoryID";
		public const string _QUANTITYPERUNIT = "QuantityPerUnit";
		public const string _UNITPRICE = "UnitPrice";
		public const string _UNITSINSTOCK = "UnitsInStock";
		public const string _UNITSONORDER = "UnitsOnOrder";
		public const string _REORDERLEVEL = "ReorderLevel";
		public const string _DISCONTINUED = "Discontinued";
	}
	
	public class ProductsAssociation
	{
		public EntitySet<Order_Details> Order_Details { get; set; }
		public EntityRef<Suppliers> Supplier { get; set; }
		public EntityRef<Categories> Category { get; set; }
	}
}