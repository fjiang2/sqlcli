namespace Sys.Data.Entity
{
    interface ITableSchema
    {
        string SchemaName { get; }
        string TableName { get; }
        string[] PrimaryKeys { get; }
        string[] IdentityKeys { get; }
        IConstraint[] Constraints { get; }
    }
}