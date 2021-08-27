using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class Territories
		: IEntityRow, IEquatable<Territories>
	{
		public string TerritoryID { get; set; }
		public string TerritoryDescription { get; set; }
		public int RegionID { get; set; }
		
		public Territories()
		{
		}
		
		public Territories(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.TerritoryID = row.GetField<string>(_TERRITORYID);
			this.TerritoryDescription = row.GetField<string>(_TERRITORYDESCRIPTION);
			this.RegionID = row.GetField<int>(_REGIONID);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_TERRITORYID, this.TerritoryID);
			row.SetField(_TERRITORYDESCRIPTION, this.TerritoryDescription);
			row.SetField(_REGIONID, this.RegionID);
		}
		
		public void CopyTo(Territories obj)
		{
			obj.TerritoryID = this.TerritoryID;
			obj.TerritoryDescription = this.TerritoryDescription;
			obj.RegionID = this.RegionID;
		}
		
		public bool Equals(Territories obj)
		{
			return this.TerritoryID == obj.TerritoryID
			&& this.TerritoryDescription == obj.TerritoryDescription
			&& this.RegionID == obj.RegionID;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_TERRITORYID, typeof(string)));
			dt.Columns.Add(new DataColumn(_TERRITORYDESCRIPTION, typeof(string)));
			dt.Columns.Add(new DataColumn(_REGIONID, typeof(int)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_TERRITORYID] = this.TerritoryID,
				[_TERRITORYDESCRIPTION] = this.TerritoryDescription,
				[_REGIONID] = this.RegionID
			};
		}
		
		public Territories(IDictionary<string, object> dict)
		{
			this.TerritoryID = (string)dict[_TERRITORYID];
			this.TerritoryDescription = (string)dict[_TERRITORYDESCRIPTION];
			this.RegionID = (int)dict[_REGIONID];
		}
		
		public TerritoriesAssociation GetAssociation()
		{
			return GetAssociation(new Territories[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<TerritoriesAssociation> GetAssociation(IEnumerable<Territories> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<TerritoriesAssociation>();
			
			var _EmployeeTerritories = reader.Read<EmployeeTerritories>();
			var _Region = reader.Read<Region>();
			
			foreach (var entity in entities)
			{
				var association = new TerritoriesAssociation
				{
					EmployeeTerritories = new EntitySet<EmployeeTerritories>(_EmployeeTerritories.Where(row => row.TerritoryID == entity.TerritoryID)),
					Region = new EntityRef<Region>(_Region.FirstOrDefault(row => row.RegionID == entity.RegionID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{TerritoryID:{0}, TerritoryDescription:{1}, RegionID:{2}}}", 
			TerritoryID, 
			TerritoryDescription, 
			RegionID);
		}
		
		public const string TableName = "Territories";
		public static readonly string[] Keys = new string[] { _TERRITORYID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<EmployeeTerritories>
			{
				ThisKey = _TERRITORYID,
				OtherKey = EmployeeTerritories._TERRITORYID,
				OneToMany = true
			},
			new Constraint<Region>
			{
				Name = "FK_Territories_Region",
				ThisKey = _REGIONID,
				OtherKey = Region._REGIONID,
				IsForeignKey = true
			}
		};
		
		public const string _TERRITORYID = "TerritoryID";
		public const string _TERRITORYDESCRIPTION = "TerritoryDescription";
		public const string _REGIONID = "RegionID";
	}
	
	public class TerritoriesAssociation
	{
		public EntitySet<EmployeeTerritories> EmployeeTerritories { get; set; }
		public EntityRef<Region> Region { get; set; }
	}
}