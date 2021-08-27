using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Sys.Data;

namespace UnitTestProject
{
    [TestClass]
    public class SqlBuilderUnitTest
    {
        readonly Expression ProductId = "ProductId".AsColumn();
        readonly string Products = "Products";
        readonly string Categories = "Categories";

        public SqlBuilderUnitTest()
        {

        }

        [TestMethod]
        public void TOP_TestMethod()
        {
            string sql = "SELECT TOP 20 * FROM Products WHERE [ProductId] < 10";
            string query = new SqlBuilder().SELECT().TOP(20).COLUMNS().FROM(Products).WHERE(ProductId < 10).ToString();

            Debug.Assert(sql == query.Substring(0, sql.Length));
        }

        [TestMethod]
        public void IS_NULL_TestMethod()
        {
            string sql = "SELECT COUNT(*) FROM Products WHERE [ProductId] IS NULL";
            string query = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR).FROM(Products).WHERE(ProductId.IS_NULL()).ToString();

            Debug.Assert(sql == query.Substring(0, sql.Length));
        }

        [TestMethod]
        public void IS_NOT_NULL_TestMethod()
        {
            string sql = "SELECT COUNT(*) FROM Products WHERE [ProductId] IS NOT NULL";
            string query = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR).FROM(Products).WHERE(ProductId != null).ToString();

            Debug.Assert(sql == query.Substring(0, sql.Length));
        }


        [TestMethod]
        public void JOIN_TestMethod()
        {
            string sql = @"SELECT Categories.[CategoryName], Products.[ProductName], Products.[QuantityPerUnit], Products.[UnitsInStock], Products.[Discontinued] 
FROM Categories INNER JOIN Products ON Categories.[CategoryID] = Products.[CategoryID] 
WHERE Products.[Discontinued] <> 1 ";

            string query = new SqlBuilder()
                .SELECT()
                .COLUMNS(
                    "CategoryName".AsColumn(Categories),
                    "ProductName".AsColumn(Products),
                    "QuantityPerUnit".AsColumn(Products),
                    "UnitsInStock".AsColumn(Products),
                    "Discontinued".AsColumn(Products)
                    )
                .AppendLine()
                .FROM(Categories)
                .INNER().JOIN(Products).ON("CategoryID".AsColumn(Categories) == "CategoryID".AsColumn(Products))
                .AppendLine()
                .WHERE("Discontinued".AsColumn(Products) != 1)
                .ToString();

            Debug.Assert(sql == query.Substring(0, sql.Length));
        }

        [TestMethod]
        public void UPDATE_TestMethod()
        {
            string sql = "UPDATE Products SET [ProductName] = N'Apple', [UnitPrice] = 20 WHERE [ProductId] BETWEEN 10 AND 30";
            string query = new SqlBuilder()
                .UPDATE(Products)
                .SET("ProductName".AsColumn() == "Apple", "UnitPrice".AsColumn() == 20)
                .WHERE(ProductId.BETWEEN(10, 30))
                .ToString();

            Debug.Assert(sql == query.Substring(0, sql.Length));
        }
    }
}
