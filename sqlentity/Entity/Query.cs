using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data;

namespace Sys.Data.Entity
{
    public static class Query
    {
        public static Func<string, IDbCmd> DefaultSqlCommand { get; set; } 

        private static T Invoke<T>(this Func<DataContext, T> func)
        {
            using (var db = new DataContext(DefaultSqlCommand))
            {
                return func(db);
            }
        }

        public static int Submit<TEntity>(this Action<Table<TEntity>> action) where TEntity : class
        {
            return Invoke(db =>
            {
                var table = db.GetTable<TEntity>();
                action(table);
                return db.SubmitChanges();
            });
        }
        
        /// <summary>
        /// Use SelectOnSumbit(...) in action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IQueryResultReader Select(this Action<DataContext> action)
        {
            return Invoke(db =>
            {
                action(db);
                return db.SumbitQueries();
            });
        }


        /// <summary>
        /// SELECT * FROM entity-table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>() where TEntity : class
        {
            return Invoke(db => db.GetTable<TEntity>().Select(where: string.Empty));
        }

        /// <summary>
        /// SELECT * FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>(string where = null) where TEntity : class
        {
            return Invoke(db => db.GetTable<TEntity>().Select(where));
        }

        /// <summary>
        /// SELECT * FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>(this Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            return Invoke(db => db.GetTable<TEntity>().Select(where));
        }

        /// <summary>
        /// SELECT col1,col2,... FROM entity-table WHERE ...
        /// e.g.
        ///   Query.Select<Categories>(row => new { row.CategoryID, row.CategoryName }, row => row.CategoryName == "Beverages");
        ///   SELECT CategoryID,CategoryName,... FROM Categories WHERE CategoryName = 'Beverages'
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="selectedColumns"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>(this Expression<Func<TEntity, object>> selectedColumns, Expression<Func<TEntity, bool>> where = null) where TEntity : class, new()
        {
            TEntity CreateInstance(System.Reflection.PropertyInfo[] properties, DataRow row, IEnumerable<string> columns)
            {
                TEntity entity = new TEntity();
                foreach (var property in properties)
                {
                    if (columns.Contains(property.Name))
                        property.SetValue(entity, row.GetField<object>(property.Name));
                }

                return entity;
            }

            return Invoke(db =>
            {
                var table = db.GetTable<TEntity>();

                List<string> _columns = new PropertyTranslator().Translate(selectedColumns);
                string _where = new QueryTranslator().Translate(where);
                string SQL = table.SelectFromWhere(_where, _columns);

                var dt = db.FillDataTable(SQL);
                if (dt == null || dt.Rows.Count == 0)
                    return new List<TEntity>();

                var properties = typeof(TEntity).GetProperties();
                return dt.ToList(row => CreateInstance(properties, row, _columns));
            });
        }

        /// <summary>
        /// SELECT * FROM entity-table WHERE key-selector IN (SELECT result-selector FROM result-table WHERE ...)
        /// e.g.
        ///   Query.Select<Categories, int, Products>(row => row.CategoryName == "Beverages", row => row.CategoryID, row => row.CategoryID);
        ///   SELECT * FROM [Products] WHERE CategoryID IN (SELECT CategoryID FROM Categories WHERE CategoryName == 'Beverages')
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="where"></param>
        /// <param name="keySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TEntity, TKey, TResult>(this Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TResult, TKey>> resultSelector)
            where TEntity : class
            where TResult : class
        {
            var translator = new QueryTranslator();
            string _where = translator.Translate(where);
            string _keySelector = translator.Translate(keySelector);
            string _resultSelector = translator.Translate(resultSelector);

            return Invoke(db =>
            {
                var dt = db.GetTable<TEntity>();
                string SQL = $"{_resultSelector} IN (SELECT {_keySelector} FROM {dt} WHERE {_where})";
                return db.GetTable<TResult>().Select(SQL);
            });
        }

      

        /// <summary>
        /// INSERT INTO entity-table (...) VALUE (....)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Insert<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.InsertOnSubmit(entities));

        /// <summary>
        /// UPDATE entity-table SET ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.UpdateOnSubmit(entities));


        /// <summary>
        /// Update partial columns of entity, values of primary key requried
        /// example of partial entity
        ///   1.object: new { Id=7, Name="XXXX"} 
        ///   2.Dictionary: new Dictionary&lt;string, object&gt;{["Id"]=7, ["Name"]="XXXX"}</string>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static int PatialUpdate<TEntity>(this IEnumerable<object> entities, bool throwException = false) where TEntity : class
            => Submit<TEntity>(table => table.PartialUpdateOnSubmit(entities, throwException));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="modifiedProperties">The properties are modified</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int PatialUpdate<TEntity>(this TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where) where TEntity : class
            => Submit<TEntity>(table => table.PartialUpdateOnSubmit(entity, modifiedProperties, where));

        public static int InsertOrUpdate<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.InsertOrUpdateOnSubmit(entities));

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.DeleteOnSubmit(entities));


        /// <summary>
        /// DELETE FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(string where = null) where TEntity : class
        {
            return Submit<TEntity>(table => table.DeleteOnSubmit(where));
        }

        /// <summary>
        /// DELETE FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            return Submit<TEntity>(table => table.DeleteOnSubmit(where));
        }

        /// <summary>
        /// Expand detail table from master table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSubEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(this IEnumerable<TEntity> entities) where TEntity : class where TSubEntity : class
            => Invoke(db => db.Expand<TEntity, TSubEntity>(entities));

        /// <summary>
        /// Expand assocation tables with foreign keys
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static IQueryResultReader Expand<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Invoke(db => db.Expand(entities));

        public static IEnumerable<T> AsEnumerable<T>(this T item)
            => new T[] { item };

        public static IEnumerable<T> Enumerable<T>(params T[] items)
            => items;

    }
}
