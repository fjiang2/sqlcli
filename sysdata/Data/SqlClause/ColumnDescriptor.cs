using System.Linq.Expressions;

namespace Sys.Data
{
    public class ColumnDescriptor
    {
        public string ColumnName { get; set; }
        public string ColumnCaption { get; set; }

        public System.Linq.Expressions.Expression Expression { get; set; }
    }

}
