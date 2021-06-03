using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Linq;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Categories
		: IEntityRow, IEquatable<Categories>
	{
		public int CategoryID { get; set; }
		public string CategoryName { get; set; }
		public string Description { get; set; }
		public byte[] Picture { get; set; }
		
		public Categories()
		{
		}
		
		public Categories(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.CategoryID = row.GetField<int>(_CATEGORYID);
			this.CategoryName = row.GetField<string>(_CATEGORYNAME);
			this.Description = row.GetField<string>(_DESCRIPTION);
			this.Picture = row.GetField<byte[]>(_PICTURE);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_CATEGORYID, this.CategoryID);
			row.SetField(_CATEGORYNAME, this.CategoryName);
			row.SetField(_DESCRIPTION, this.Description);
			row.SetField(_PICTURE, this.Picture);
		}
		
		public void CopyTo(Categories obj)
		{
			obj.CategoryID = this.CategoryID;
			obj.CategoryName = this.CategoryName;
			obj.Description = this.Description;
			obj.Picture = this.Picture;
		}
		
		public bool Equals(Categories obj)
		{
			return this.CategoryID == obj.CategoryID
			&& this.CategoryName == obj.CategoryName
			&& this.Description == obj.Description
			&& this.Picture == obj.Picture;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_CATEGORYID, typeof(int)));
			dt.Columns.Add(new DataColumn(_CATEGORYNAME, typeof(string)));
			dt.Columns.Add(new DataColumn(_DESCRIPTION, typeof(string)));
			dt.Columns.Add(new DataColumn(_PICTURE, typeof(byte[])));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_CATEGORYID] = this.CategoryID,
				[_CATEGORYNAME] = this.CategoryName,
				[_DESCRIPTION] = this.Description,
				[_PICTURE] = this.Picture
			};
		}
		
		public Categories(IDictionary<string, object> dict)
		{
			this.CategoryID = (int)dict[_CATEGORYID];
			this.CategoryName = (string)dict[_CATEGORYNAME];
			this.Description = (string)dict[_DESCRIPTION];
			this.Picture = (byte[])dict[_PICTURE];
		}
		
		public override string ToString()
		{
			return string.Format("{{CategoryID:{0}, CategoryName:{1}, Description:{2}, Picture:{3}}}", 
			CategoryID, 
			CategoryName, 
			Description, 
			Picture);
		}
		
		public const string TableName = "Categories";
		public static readonly string[] Keys = new string[] { _CATEGORYID };
		public static readonly string[] Identity = new string[] { _CATEGORYID };
		
		public const string _CATEGORYID = "CategoryID";
		public const string _CATEGORYNAME = "CategoryName";
		public const string _DESCRIPTION = "Description";
		public const string _PICTURE = "Picture";
	}
}