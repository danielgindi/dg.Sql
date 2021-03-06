﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SequelNet.Connector;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SequelNet
{
    [Serializable]
    public abstract class AbstractRecord<T> : IRecord
        where T : AbstractRecord<T>, new()
    {
        #region Static private variables, caches

        private static bool __LOOKED_FOR_PRIMARY_KEY_NAME = false;
        private static bool __PRIMARY_KEY_MULTI = false;
        private static object __PRIMARY_KEY_NAME = null;
        private static TableSchema __TABLE_SCHEMA = null;
        private static Type __CLASS_TYPE = null;
        private static PropertyInfo __PRIMARY_KEY_PROP_INFO = null;

        private static bool __FLAGS_RETRIEVED = false;
        private static bool __HAS_DELETED = false;
        private static bool __HAS_IS_DELETED = false;
        private static bool __HAS_CREATED_ON= false;
        private static bool __HAS_MODIFIED_ON = false;

        #endregion

        #region Private variables

        private static bool _AtomicUpdates = false;
        private HashSet<string> _MutatedColumns = null;

        #endregion

        #region Constructors

        public AbstractRecord() { }

        public AbstractRecord(object keyValue)
        {
            LoadByKey(keyValue);
        }

        public AbstractRecord(string columnName, object columnValue)
        {
            LoadByParam(columnName, columnValue);
        }

        #endregion

        #region Table schema stuff

        /// <summary>
        /// Caches information about the primary key for this record. 
        /// This is automatically called the first time that a primary-key-related action is called.
        /// </summary>
        private static void CachePrimaryKeyName()
        {
            var keyNames = new List<string>();

            foreach (var col in Schema.Columns)
            {
                if (col.IsPrimaryKey)
                {
                    keyNames.Add(col.Name);
                }
            }

            if (keyNames.Count == 0)
            {
                foreach (var idx in Schema.Indexes)
                {
                    if (idx.Mode == TableSchema.IndexMode.PrimaryKey)
                    {
                        keyNames.AddRange(idx.ColumnNames);
                        break;
                    }
                }
            }

            if (keyNames.Count == 1)
            {
                __PRIMARY_KEY_NAME = keyNames[0];
            }
            else if (keyNames.Count > 1)
            {
                __PRIMARY_KEY_NAME = keyNames.ToArray();
            }

            __PRIMARY_KEY_MULTI = keyNames.Count > 1;

            if (!__PRIMARY_KEY_MULTI && keyNames.Count > 0)
            {
                var classType = typeof(AbstractRecord<T>);
                var propInfo = classType.GetProperty(keyNames[0]);
                if (propInfo == null) propInfo = classType.GetProperty(keyNames[0] + @"X");
                __PRIMARY_KEY_PROP_INFO = propInfo;
            }

            __LOOKED_FOR_PRIMARY_KEY_NAME = true;
        }

        /// <summary>
        /// Resets the cache of information about the primary key.
        /// </summary>
        public static void ResetPrimaryKeyCache()
        {
            __LOOKED_FOR_PRIMARY_KEY_NAME = false;
        }

        public abstract object GetPrimaryKeyValue();

        public abstract TableSchema GenerateTableSchema();

        private static bool IsCompoundPrimaryKey()
        {
            if (!__LOOKED_FOR_PRIMARY_KEY_NAME)
                CachePrimaryKeyName();

            return __PRIMARY_KEY_MULTI;
        }

        [XmlIgnore]
        public static TableSchema Schema
        {
            get
            {
                if (__TABLE_SCHEMA == null) __TABLE_SCHEMA = (new T()).GenerateTableSchema();
                return __TABLE_SCHEMA;
            }
            set
            {
                __TABLE_SCHEMA = value;
            }
        }

        [XmlIgnore]
        public static string SchemaName => Schema.Name;

        /// <summary>
        /// The primary key name for this record's schema.
        /// It is found automatically and cached.
        /// Could be either a String, an <typeparamref name="Array&lt;String&gt;"/> or <value>null</value>.
        /// </summary>
        [XmlIgnore]
        public static object SchemaPrimaryKeyName
        {
            get
            {
                if (!__LOOKED_FOR_PRIMARY_KEY_NAME)
                {
                    CachePrimaryKeyName();
                }
                return __PRIMARY_KEY_NAME;
            }
            set
            {
                if (value is ICollection)
                { // Make sure this is an Array!
                    List<string> keys = new List<string>();
                    foreach (string key in (IEnumerable)__PRIMARY_KEY_NAME)
                    {
                        keys.Add(key);
                    }
                    value = keys.ToArray();
                }

                __PRIMARY_KEY_NAME = value;
                __PRIMARY_KEY_MULTI = __PRIMARY_KEY_NAME is ICollection;

                if (!__PRIMARY_KEY_MULTI)
                {
                    var classType = typeof(AbstractRecord<T>);
                    var propInfo = classType.GetProperty(__PRIMARY_KEY_NAME as string);
                    if (propInfo == null) propInfo = classType.GetProperty(__PRIMARY_KEY_NAME as string + @"X");
                    __PRIMARY_KEY_PROP_INFO = propInfo;
                }

                __LOOKED_FOR_PRIMARY_KEY_NAME = true;
            }
        }

        private static void RetrieveFlags()
        {
            __HAS_DELETED = Schema.Columns.Find(@"Deleted") != null;
            __HAS_IS_DELETED = Schema.Columns.Find(@"IsDeleted") != null;
            __HAS_CREATED_ON = Schema.Columns.Find(@"CreatedOn") != null;
            __HAS_MODIFIED_ON = Schema.Columns.Find(@"ModifiedOn") != null;
            __FLAGS_RETRIEVED = true;
        }

        #endregion

        #region Mutated

        public bool IsNewRecord { get; set; } = true;

        public static bool AtomicUpdates
        {
            get { return _AtomicUpdates; }
            set { _AtomicUpdates = value; }
        }

        public virtual void MarkColumnMutated(string column)
        {
            if (_MutatedColumns == null)
            {
                _MutatedColumns = new HashSet<string>();
            }

            _MutatedColumns.Add(column);
        }

        public virtual void MarkColumnNotMutated(string column)
        {
            if (_MutatedColumns == null) return;

            _MutatedColumns.Remove(column);
        }

        public virtual void MarkAllColumnsNotMutated()
        {
            if (_MutatedColumns == null) return;

            _MutatedColumns.Clear();
        }

        public virtual bool IsColumnMutated(string column)
        {
            return _MutatedColumns != null && _MutatedColumns.Contains(column);
        }

        public virtual bool HasMutatedColumns()
        {
            return _MutatedColumns != null && _MutatedColumns.Count > 0;
        }

        public void MarkOld()
        {
            IsNewRecord = false;
        }

        public void MarkNew()
        {
            IsNewRecord = true;
        }

        #endregion

        #region Virtual Read/Write Actions

        public virtual void SetPrimaryKeyValue(object value)
        {
            if (!__LOOKED_FOR_PRIMARY_KEY_NAME)
            {
                CachePrimaryKeyName();
            }

            var propInfo = __PRIMARY_KEY_PROP_INFO;
            propInfo?.SetValue(
                this,
                Convert.ChangeType(value, Schema.Columns.Find(SchemaPrimaryKeyName as string)?.Type), null);
        }

        public virtual Query GetInsertQuery()
        {
            if (!__FLAGS_RETRIEVED)
                RetrieveFlags();

            if (__CLASS_TYPE == null)
                __CLASS_TYPE = this.GetType();

            var qry = new Query(Schema);

            foreach (var column in Schema.Columns)
            {
                var propInfo = __CLASS_TYPE.GetProperty(column.Name);

                if (propInfo == null)
                    propInfo = __CLASS_TYPE.GetProperty(column.Name + @"X");

                if (propInfo != null)
                    qry.Insert(column.Name, propInfo.GetValue(this, null));
            }

            if (__HAS_CREATED_ON)
            {
                qry.Insert(@"CreatedOn", DateTime.UtcNow);
            }

            return qry;
        }

        public virtual Query GetUpdateQuery()
        {
            if (!__FLAGS_RETRIEVED)
                RetrieveFlags();

            if (__CLASS_TYPE == null)
                __CLASS_TYPE = this.GetType();

            object primaryKey = SchemaPrimaryKeyName;
            bool isPrimaryKeyNullOrString = primaryKey == null || primaryKey is string;

            var qry = new Query(Schema);

            foreach (var column in Schema.Columns)
            {
                if ((isPrimaryKeyNullOrString && column.Name == (string)primaryKey) ||
                    (!isPrimaryKeyNullOrString && StringArrayContains((string[])primaryKey, column.Name))) continue;

                var propInfo = __CLASS_TYPE.GetProperty(column.Name);
                if (propInfo == null) propInfo = __CLASS_TYPE.GetProperty(column.Name + @"X");
                if (propInfo != null)
                {
                    if (_AtomicUpdates && !IsColumnMutated(column.Name)) continue;

                    qry.Update(column.Name, propInfo.GetValue(this, null));
                }
            }

            if (!_AtomicUpdates || qry.HasInsertsOrUpdates)
            {
                if (__HAS_MODIFIED_ON)
                {
                    qry.Update(@"ModifiedOn", DateTime.UtcNow);
                }
            }

            return qry;
        }

        public virtual void Insert(ConnectorBase connection = null)
        {
            var qry = GetInsertQuery();

            if (IsCompoundPrimaryKey())
            {
                qry.Execute(connection);
            }
            else
            {
                if (qry.Execute(out var lastInsertId, connection) > 0)
                {
                    SetPrimaryKeyValue(lastInsertId);
                }
            }

            MarkOld();
            MarkAllColumnsNotMutated();
        }

        public virtual async Task InsertAsync(ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            var qry = GetInsertQuery();

            if (IsCompoundPrimaryKey())
            {
                await qry.ExecuteAsync(conn, cancellationToken).ConfigureAwait(false);

                MarkOld();
                MarkAllColumnsNotMutated();
            }
            else
            {
                var results = await qry.ExecuteWithLastInsertIdAsync(conn, cancellationToken).ConfigureAwait(false);

                if (results.updates > 0)
                {
                    SetPrimaryKeyValue(results.lastInsertId);

                    MarkOld();
                    MarkAllColumnsNotMutated();
                }
            }
        }

        public Task InsertAsync(CancellationToken? cancellationToken)
        {
            return InsertAsync(null, cancellationToken);
        }

        public virtual void Update(ConnectorBase connection = null)
        {
            var qry = GetUpdateQuery();

            if (qry.HasInsertsOrUpdates)
            {
                qry.Execute(connection);
            }

            MarkAllColumnsNotMutated();
        }

        public virtual async Task UpdateAsync(ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            var qry = GetUpdateQuery();

            if (qry.HasInsertsOrUpdates)
            {
                await qry.ExecuteAsync(conn, cancellationToken).ConfigureAwait(false);
            }

            MarkAllColumnsNotMutated();
        }

        public Task UpdateAsync(CancellationToken? cancellationToken)
        {
            return UpdateAsync(null, cancellationToken);
        }

        public virtual void Read(DataReader reader)
        {
            if (__CLASS_TYPE == null)
                __CLASS_TYPE = this.GetType();

            PropertyInfo propInfo;
            foreach (var column in Schema.Columns)
            {
                propInfo = __CLASS_TYPE.GetProperty(column.Name);
                if (propInfo == null) propInfo = __CLASS_TYPE.GetProperty(column.Name + @"X");
                if (propInfo != null)
                {
                    propInfo.SetValue(this, Convert.ChangeType(reader[column.Name], column.Type), null);
                }
            }

            MarkOld();
            MarkAllColumnsNotMutated();
        }

        public virtual void Save(ConnectorBase connection = null)
        {
            if (IsNewRecord)
            {
                Insert(connection);
            }
            else
            {
                Update(connection);
            }
        }

        public virtual Task SaveAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            if (IsNewRecord)
            {
                return InsertAsync(connection, cancellationToken);
            }
            else
            {
                return UpdateAsync(connection, cancellationToken);
            }
        }

        public Task SaveAsync(CancellationToken? cancellationToken)
        {
            if (IsNewRecord)
            {
                return InsertAsync(null, cancellationToken);
            }
            else
            {
                return UpdateAsync(null, cancellationToken);
            }
        }

        #endregion

        #region Deleting a record

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(object primaryKeyValue, ConnectorBase connection = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DeleteByParameter(columnName, primaryKeyValue, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DeleteAsync(object primaryKeyValue, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return Task.FromResult(0);
            return DeleteByParameterAsync(columnName, primaryKeyValue, connection, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DeleteAsync(object primaryKeyValue, CancellationToken? cancellationToken)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return Task.FromResult(0);
            return DeleteByParameterAsync(columnName, primaryKeyValue, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(string columnName, object value, ConnectorBase connection = null)
        {
            return DeleteByParameter(columnName, value, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DeleteAsync(string columnName, object value, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            return DeleteByParameterAsync(columnName, value, connection, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DeleteAsync(string columnName, object value, CancellationToken? cancellationToken)
        {
            return DeleteByParameterAsync(columnName, value, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        private static int DeleteByParameter(object columnName, object value, ConnectorBase connection = null)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }

            if (__HAS_DELETED || __HAS_IS_DELETED)
            {
                Query qry = new Query(Schema);

                if (__HAS_DELETED) qry.Update(@"Deleted", true);
                if (__HAS_IS_DELETED) qry.Update(@"IsDeleted", true);
                if (__HAS_MODIFIED_ON) qry.Update(@"ModifiedOn", DateTime.UtcNow);

                if (columnName is ICollection)
                {
                    if (!(value is ICollection)) return 0;

                    IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                    IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                    while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                    {
                        qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                    }
                }
                else
                {
                    qry.Where((string)columnName, value);
                }
                return qry.ExecuteNonQuery(connection);
            }
            return DestroyByParameter(columnName, value, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        private static Task<int> DeleteByParameterAsync(object columnName, object value, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }

            if (__HAS_DELETED || __HAS_IS_DELETED)
            {
                Query qry = new Query(Schema);

                if (__HAS_DELETED) qry.Update(@"Deleted", true);
                if (__HAS_IS_DELETED) qry.Update(@"IsDeleted", true);
                if (__HAS_MODIFIED_ON) qry.Update(@"ModifiedOn", DateTime.UtcNow);

                if (columnName is ICollection)
                {
                    if (!(value is ICollection)) return Task.FromResult(0);

                    IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                    IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                    while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                    {
                        qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                    }
                }
                else
                {
                    qry.Where((string)columnName, value);
                }
                return qry.ExecuteNonQueryAsync(connection, cancellationToken);
            }
            return DestroyByParameterAsync(columnName, value, connection, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        private static Task<int> DeleteByParameterAsync(object columnName, object value, CancellationToken? cancellationToken)
        {
            return DeleteByParameterAsync(columnName, value, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Destroy(object primaryKeyValue, ConnectorBase connection = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DestroyByParameter(columnName, primaryKeyValue, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DestroyAsync(object primaryKeyValue, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return Task.FromResult(0);
            return DestroyByParameterAsync(columnName, primaryKeyValue, connection, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DestroyAsync(object primaryKeyValue, CancellationToken? cancellationToken)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return Task.FromResult(0);
            return DestroyByParameterAsync(columnName, primaryKeyValue, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Destroy(string columnName, object value, ConnectorBase connection = null)
        {
            return DestroyByParameter(columnName, value, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DestroyAsync(string columnName, object value, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            return DestroyByParameterAsync(columnName, value, connection, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public static Task<int> DestroyAsync(string columnName, object value, CancellationToken? cancellationToken)
        {
            return DestroyByParameterAsync(columnName, value, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        private static int DestroyByParameter(object columnName, object value, ConnectorBase connection)
        {
            Query qry = new Query(Schema).Delete();

            if (columnName is ICollection)
            {
                if (!(value is ICollection)) return 0;

                IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)columnName, value);
            }

            return qry.ExecuteNonQuery(connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        private static Task<int> DestroyByParameterAsync(object columnName, object value, ConnectorBase connection, CancellationToken? cancellationToken = null)
        {
            Query qry = new Query(Schema).Delete();

            if (columnName is ICollection)
            {
                if (!(value is ICollection)) return Task.FromResult(0);

                IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)columnName, value);
            }

            return qry.ExecuteNonQueryAsync(connection, cancellationToken);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        private static Task<int> DestroyByParameterAsync(object columnName, object value, CancellationToken? cancellationToken)
        {
            return DestroyByParameterAsync(columnName, value, null, cancellationToken);
        }

        #endregion

        #region Column utilities
        
        #endregion

        #region Loading utilities

        /// <summary>
        /// Fetches a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static T FetchByID(object primaryKeyValue, ConnectorBase connection = null)
        {
            Query qry = new Query(Schema).LimitRows(1);

            object primaryKey = SchemaPrimaryKeyName;
            if (IsCompoundPrimaryKey())
            {
                if (!(primaryKeyValue is ICollection)) return null;

                IEnumerator keyEnumerator = ((IEnumerable)primaryKey).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)primaryKeyValue).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)primaryKey, primaryKeyValue);
            }

            return FetchByQuery(qry);
        }

        /// <summary>
        /// Fetches a record from the db, by passing a cooked query.
        /// </summary>
        /// <param name="qry">A query to execute. You should probably .</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static T FetchByQuery(Query qry, ConnectorBase connection = null)
        {
            using (var reader = qry.ExecuteReader(connection))
            {
                if (reader.Read())
                    return FromReader(reader);
            }
            return null;
        }

        /// <summary>
        /// Fetches a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static Task<T> FetchByIdAsync(object primaryKeyValue, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            Query qry = new Query(Schema).LimitRows(1);

            object primaryKey = SchemaPrimaryKeyName;
            if (IsCompoundPrimaryKey())
            {
                if (!(primaryKeyValue is ICollection)) return null;

                IEnumerator keyEnumerator = ((IEnumerable)primaryKey).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)primaryKeyValue).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)primaryKey, primaryKeyValue);
            }

            return FetchByQueryAsync(qry, connection, cancellationToken);
        }

        /// <summary>
        /// Fetches a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static Task<T> FetchByIdAsync(object primaryKeyValue, CancellationToken? cancellationToken)
        {
            return FetchByIdAsync(primaryKeyValue, null, cancellationToken);
        }

        /// <summary>
        /// Fetches a record from the db, by passing a cooked query.
        /// </summary>
        /// <param name="qry">A query to execute. You should probably .</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static async Task<T> FetchByQueryAsync(Query qry, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await qry.ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    return FromReader(reader);
            }
            return null;
        }

        /// <summary>
        /// Fetches a record from the db, by passing a cooked query.
        /// </summary>
        /// <param name="qry">A query to execute. You should probably .</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static Task<T> FetchByQueryAsync(Query qry, CancellationToken? cancellationToken)
        {
            return FetchByQueryAsync(qry, null, cancellationToken);
        }

        /// <summary>
        /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        public void LoadByKey(object primaryKeyValue, ConnectorBase connection = null)
        {
            LoadByParam(SchemaPrimaryKeyName, primaryKeyValue, connection);
        }

        /// <summary>
        /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task LoadByKeyAsync(object primaryKeyValue, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            return LoadByParamAsync(SchemaPrimaryKeyName, primaryKeyValue, connection, cancellationToken);
        }

        /// <summary>
        /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task LoadByKeyAsync(object primaryKeyValue, CancellationToken? cancellationToken)
        {
            return LoadByParamAsync(SchemaPrimaryKeyName, primaryKeyValue, null, cancellationToken);
        }

        /// <summary>
        /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        public void LoadByParam(object columnName, object value, ConnectorBase connection = null)
        {
            Query qry = new Query(Schema).LimitRows(1);

            if (columnName is ICollection)
            {
                if (!(value is ICollection)) return;

                IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)columnName, value);
            }

            using (var reader = qry.ExecuteReader(connection))
            {
                if (reader.Read())
                {
                    Read(reader);
                }
            }
        }

        /// <summary>
        /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LoadByParamAsync(object columnName, object value, ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            Query qry = new Query(Schema).LimitRows(1);

            if (columnName is ICollection)
            {
                if (!(value is ICollection)) 
                    return;

                IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)columnName, value);
            }

            using (var reader = await qry.ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    Read(reader);
                }
            }
        }

        /// <summary>
        /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task LoadByParamAsync(object columnName, object value, CancellationToken? cancellationToken = null)
        {
            return LoadByParamAsync(columnName, value, null, cancellationToken);
        }

        /// <summary>
        /// Creates a new instance of this record, and loads it from the <paramref name="reader"/>.
        /// Will me marked as "old".
        /// </summary>
        /// <param name="reader">The reader to use for loading the new record</param>
        /// <returns>The new <typeparamref name="T"/>.</returns>
        public static T FromReader(DataReader reader)
        {
            T item = new T();
            item.Read(reader);
            return item;
        }

        #endregion

        #region Helpers

        private static bool StringArrayContains(string[] array, string containsItem)
        {
            foreach (string item in array)
            {
                if (item == containsItem) return true;
            }
            return false;
        }

        #endregion
    }
}
