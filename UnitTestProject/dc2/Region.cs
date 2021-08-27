using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Region
		: IEntityRow, IEquatable<Region>
	{
		public int RegionID { get; set; }
		public string RegionDescription { get; set; }
		
		public Region()
		{
		}
		
		public Region(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.RegionID = row.GetField<int>(_REGIONID);
			this.RegionDescription = row.GetField<string>(_REGIONDESCRIPTION);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_REGIONID, this.RegionID);
			row.SetField(_REGIONDESCRIPTION, this.RegionDescription);
		}
		
		public void CopyTo(Region obj)
		{
			obj.RegionID = this.RegionID;
			obj.RegionDescription = this.RegionDescription;
		}
		
		public bool Equals(Region obj)
		{
			return this.RegionID == obj.RegionID
			&& this.RegionDescription == obj.RegionDescription;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_REGIONID, typeof(int)));
			dt.Columns.Add(new DataColumn(_REGIONDESCRIPTION, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_REGIONID] = this.RegionID,
				[_REGIONDESCRIPTION] = this.RegionDescription
			};
		}
		
		public Region(IDictionary<string, object> dict)
		{
			this.RegionID = (int)dict[_REGIONID];
			this.RegionDescription = (string)dict[_REGIONDESCRIPTION];
		}
		
		public RegionAssociation GetAssociation()
		{
			return GetAssociation(new Region[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<RegionAssociation> GetAssociation(IEnumerable<Region> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<RegionAssociation>();
			
			var _Territories = reader.Read<Territories>();
			
			foreach (var entity in entities)
			{
				var association = new RegionAssociation
				{
					Territories = new EntitySet<Territories>(_Territories.Where(row => row.RegionID == entity.RegionID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{RegionID:{0}, RegionDescription:{1}}}", 
			RegionID, 
			RegionDescription);
		}
		
		public const string TableName = "Region";
		public static readonly string[] Keys = new string[] { _REGIONID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Territories>
			{
				ThisKey = _REGIONID,
				OtherKey = Territories._REGIONID,
				OneToMany = true
			}
		};
		
		public const string _REGIONID = "RegionID";
		public const string _REGIONDESCRIPTION = "RegionDescription";
	}
	
	public class RegionAssociation
	{
		public EntitySet<Territories> Territories { get; set; }
	}
}