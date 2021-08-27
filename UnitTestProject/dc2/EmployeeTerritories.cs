using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc2
{
	public partial class EmployeeTerritories
		: IEntityRow, IEquatable<EmployeeTerritories>
	{
		public int EmployeeID { get; set; }
		public string TerritoryID { get; set; }
		
		public EmployeeTerritories()
		{
		}
		
		public EmployeeTerritories(DataRow row)
		{
			FillObject(row);
		}
		
		public void FillObject(DataRow row)
		{
			this.EmployeeID = row.GetField<int>(_EMPLOYEEID);
			this.TerritoryID = row.GetField<string>(_TERRITORYID);
		}
		
		public void UpdateRow(DataRow row)
		{
			row.SetField(_EMPLOYEEID, this.EmployeeID);
			row.SetField(_TERRITORYID, this.TerritoryID);
		}
		
		public void CopyTo(EmployeeTerritories obj)
		{
			obj.EmployeeID = this.EmployeeID;
			obj.TerritoryID = this.TerritoryID;
		}
		
		public bool Equals(EmployeeTerritories obj)
		{
			return this.EmployeeID == obj.EmployeeID
			&& this.TerritoryID == obj.TerritoryID;
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn(_EMPLOYEEID, typeof(int)));
			dt.Columns.Add(new DataColumn(_TERRITORYID, typeof(string)));
			
			return dt;
		}
		
		public IDictionary<string, object> ToDictionary()
		{
			return new Dictionary<string,object>() 
			{
				[_EMPLOYEEID] = this.EmployeeID,
				[_TERRITORYID] = this.TerritoryID
			};
		}
		
		public EmployeeTerritories(IDictionary<string, object> dict)
		{
			this.EmployeeID = (int)dict[_EMPLOYEEID];
			this.TerritoryID = (string)dict[_TERRITORYID];
		}
		
		public EmployeeTerritoriesAssociation GetAssociation()
		{
			return GetAssociation(new EmployeeTerritories[] { this }).FirstOrDefault();
		}
		
		public static IEnumerable<EmployeeTerritoriesAssociation> GetAssociation(IEnumerable<EmployeeTerritories> entities)
		{
			var reader = entities.Expand();
			
			var associations = new List<EmployeeTerritoriesAssociation>();
			
			var _Employee = reader.Read<Employees>();
			var _Territory = reader.Read<Territories>();
			
			foreach (var entity in entities)
			{
				var association = new EmployeeTerritoriesAssociation
				{
					Employee = new EntityRef<Employees>(_Employee.FirstOrDefault(row => row.EmployeeID == entity.EmployeeID)),
					Territory = new EntityRef<Territories>(_Territory.FirstOrDefault(row => row.TerritoryID == entity.TerritoryID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public override string ToString()
		{
			return string.Format("{{EmployeeID:{0}, TerritoryID:{1}}}", 
			EmployeeID, 
			TerritoryID);
		}
		
		public const string TableName = "EmployeeTerritories";
		public static readonly string[] Keys = new string[] { _EMPLOYEEID, _TERRITORYID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Employees>
			{
				Name = "FK_EmployeeTerritories_Employees",
				ThisKey = _EMPLOYEEID,
				OtherKey = Employees._EMPLOYEEID,
				IsForeignKey = true
			},
			new Constraint<Territories>
			{
				Name = "FK_EmployeeTerritories_Territories",
				ThisKey = _TERRITORYID,
				OtherKey = Territories._TERRITORYID,
				IsForeignKey = true
			}
		};
		
		public const string _EMPLOYEEID = "EmployeeID";
		public const string _TERRITORYID = "TerritoryID";
	}
	
	public class EmployeeTerritoriesAssociation
	{
		public EntityRef<Employees> Employee { get; set; }
		public EntityRef<Territories> Territory { get; set; }
	}
}