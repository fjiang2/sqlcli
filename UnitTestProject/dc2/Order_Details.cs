using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Linq;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Order_Details
		: IEntityRow, IEquatable<Order_Details>
	{
		public int OrderID { get; set; }
		public int ProductID { get; set; }
		public decimal UnitPrice { get; set; }
		public short Quantity { get; set; }
		public float Discount { get; set; }
		
		public Order_Details()
		{
		}
		
		public Order_Details(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.OrderID = row.GetField<int>(_ORDERID);
			this.ProductID = row.GetField<int>(_PRODUCTID);
			this.UnitPrice = row.GetField<decimal>(_UNITPRICE);
			this.Quantity = row.GetField<short>(_QUANTITY);
			this.Discount = row.GetField<float>(_DISCOUNT);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_ORDERID, this.OrderID);
			row.SetField(_PRODUCTID, this.ProductID);
			row.SetField(_UNITPRICE, this.UnitPrice);
			row.SetField(_QUANTITY, this.Quantity);
			row.SetField(_DISCOUNT, this.Discount);
		}
		
		public void CopyTo(Order_Details obj)
		{
			obj.OrderID = this.OrderID;
			obj.ProductID = this.ProductID;
			obj.UnitPrice = this.UnitPrice;
			obj.Quantity = this.Quantity;
			obj.Discount = this.Discount;
		}
		
		public bool Equals(Order_Details obj)
		{
			return this.OrderID == obj.OrderID
			&& this.ProductID == obj.ProductID
			&& this.UnitPrice == obj.UnitPrice
			&& this.Quantity == obj.Quantity
			&& this.Discount == obj.Discount;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_ORDERID, typeof(int)));
			dt.Columns.Add(new DataColumn(_PRODUCTID, typeof(int)));
			dt.Columns.Add(new DataColumn(_UNITPRICE, typeof(decimal)));
			dt.Columns.Add(new DataColumn(_QUANTITY, typeof(short)));
			dt.Columns.Add(new DataColumn(_DISCOUNT, typeof(float)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_ORDERID] = this.OrderID,
				[_PRODUCTID] = this.ProductID,
				[_UNITPRICE] = this.UnitPrice,
				[_QUANTITY] = this.Quantity,
				[_DISCOUNT] = this.Discount
			};
		}
		
		public Order_Details(IDictionary<string, object> dict)
		{
			this.OrderID = (int)dict[_ORDERID];
			this.ProductID = (int)dict[_PRODUCTID];
			this.UnitPrice = (decimal)dict[_UNITPRICE];
			this.Quantity = (short)dict[_QUANTITY];
			this.Discount = (float)dict[_DISCOUNT];
		}
		
		public override string ToString()
		{
			return string.Format("{{OrderID:{0}, ProductID:{1}, UnitPrice:{2}, Quantity:{3}, Discount:{4}}}", 
			OrderID, 
			ProductID, 
			UnitPrice, 
			Quantity, 
			Discount);
		}
		
		public const string TableName = "Order Details";
		public static readonly string[] Keys = new string[] { _ORDERID, _PRODUCTID };
		
		public const string _ORDERID = "OrderID";
		public const string _PRODUCTID = "ProductID";
		public const string _UNITPRICE = "UnitPrice";
		public const string _QUANTITY = "Quantity";
		public const string _DISCOUNT = "Discount";
	}
}