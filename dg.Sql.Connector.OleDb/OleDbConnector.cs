﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class OleDbConnector : ConnectorBase
    {
        OleDbTransaction _transaction = null;
        Stack<OleDbTransaction> _transactions = null;

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MSACCESS; }
        }

        public static OleDbConnection CreateSqlConnection(string connectionStringKey)
        {
            return new OleDbConnection(GetWebsiteConnectionString(connectionStringKey));
        }

        OleDbConnection _conn = null;

        public OleDbConnector()
        {
            _conn = CreateSqlConnection(null);
        }
        public OleDbConnector(string connectionStringKey)
        {
            _conn = CreateSqlConnection(connectionStringKey);
        }
        ~OleDbConnector()
        {
            Dispose(false);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            // Now clean up Native Resources (Pointers)
        }

        public override void Close()
        {
            try
            {
                if (_conn != null && _conn.State != ConnectionState.Closed)
                {
                    _conn.Close();
                }
            }
            catch (Exception) { }
            if (_conn != null) _conn.Dispose();
            _conn = null;
        }
        public OleDbConnection GetConn()
        {
            return _conn;
        }
        public override int ExecuteNonQuery(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new OleDbCommand(strSQL, _conn, _transaction).ExecuteNonQuery();
        }
        public override int ExecuteNonQuery(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return command.ExecuteNonQuery();
        }
        public override object ExecuteScalar(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new OleDbCommand(strSQL, _conn, _transaction).ExecuteScalar();
        }
        public override object ExecuteScalar(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return command.ExecuteScalar();
        }
        public override DataReaderBase ExecuteReader(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new OleDbDataReader(
                new OleDbCommand(strSQL, _conn, _transaction).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(String strSQL, bool attachConnectionToReader)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new OleDbDataReader(
                new OleDbCommand(strSQL, _conn, _transaction).ExecuteReader(), attachConnectionToReader ? this : null);
        }
        public override DataReaderBase ExecuteReader(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return new OleDbDataReader(((OleDbCommand)command).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return new OleDbDataReader(((OleDbCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }
        public override DataSet ExecuteDataSet(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            using (OleDbCommand cmd = new OleDbCommand(strSQL, _conn, _transaction))
            {
                DataSet dataSet = new DataSet();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                {
                    adapter.Fill(dataSet);
                }
                return dataSet;
            }
        }
        public override DataSet ExecuteDataSet(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            DataSet dataSet = new DataSet();
            using (OleDbDataAdapter adapter = new OleDbDataAdapter((OleDbCommand)command))
            {
                adapter.Fill(dataSet);
            }
            return dataSet;
        }
        public override int ExecuteScript(String strSQL)
        {
            throw new NotImplementedException(@"ExecuteScript");
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT @@identity AS id");
        }

        public override bool checkIfTableExists(string tableName)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return ExecuteScalar(@"SELECT name FROM MSysObjects WHERE name like '" + fullEscape(tableName) + "'") != null;
        }

        public override bool beginTransaction()
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                _transaction = _conn.BeginTransaction();
                if (_transactions == null) _transactions = new Stack<OleDbTransaction>(1);
                _transactions.Push(_transaction);
                return (_transaction != null);
            }
            catch (OleDbException) { }
            return false;
        }
        public override bool beginTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                _transaction = _conn.BeginTransaction(isolationLevel);
                if (_transactions == null) _transactions = new Stack<OleDbTransaction>(1);
                _transactions.Push(_transaction);
            }
            catch (OleDbException) { return false; }
            return (_transaction != null);
        }
        public override bool commitTransaction()
        {
            if (_transaction == null) return false;
            else
            {
                try
                {
                    _transaction.Commit();
                }
                catch (OleDbException) { return false; }
                _transactions.Pop();
                if (_transactions.Count > 0) _transaction = _transactions.Peek();
                else _transaction = null;
                return true;
            }
        }
        public override bool rollbackTransaction()
        {
            if (_transaction == null) return false;
            else
            {
                try
                {
                    _transaction.Rollback();
                }
                catch (OleDbException) { return false; }
                _transactions.Pop();
                if (_transactions.Count > 0) _transaction = _transactions.Peek();
                else _transaction = null;
                return true;
            }
        }
        public override bool hasTransaction
        {
            get { return _transactions != null && _transactions.Count > 0; }
        }
        public override int currentTransactions
        {
            get { return _transactions == null ? 0 : _transactions.Count; }
        }
        public override DbTransaction Transaction
        {
            get { return _transaction; }
        }
        public override DbConnection Connection
        {
            get { return _conn; }
        }

        public override string fullEscape(string strToEscape)
        {
            return strToEscape.Replace(@"'", @"''");
        }
        public override string prepareGuid(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }
        public override string prepareBoolean(bool value)
        {
            return value ? @"true" : @"false";
        }
        public override string encloseFieldName(string fieldName)
        {
            return '[' + fieldName + ']';
        }
        public override string formatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }
        public override string sqlAddPaginationAndOrdering(
            string selectFieldsList,
            string primaryKeysList,
            string tablesList,
            string where,
            int limit /* = 0 */, int offset /* = 0 */,
            string orderBy /* = NULL */)
        {
            if (limit <= 0 && offset <= 0 && (orderBy == null || orderBy.Length == 0)) return @"SELECT " + selectFieldsList + @" FROM " + tablesList + @" WHERE " + where;
            else
            {
                string sql = string.Empty;
                tablesList = tablesList.TrimStart(new char[] { ' ' });
                if (tablesList.Length > 0) tablesList = @" FROM " + tablesList;
                where = where.TrimStart(new char[] { ' ' });
                if (where.Length > 0) where = @" WHERE " + where;
                if (orderBy == null) orderBy = string.Empty;
                orderBy = orderBy.TrimStart(new char[] { ' ' });
                if (orderBy.Length > 0) orderBy = @" ORDER BY " + orderBy;
                if (limit <= 0 && offset <= 0)
                {
                    sql = @"SELECT " + selectFieldsList + tablesList + where + orderBy;
                }
                else if (limit > 0 && offset <= 0)
                {
                    sql = @"SELECT TOP " + limit + @" " + selectFieldsList + tablesList + where + orderBy;
                }
                else if (limit <= 0 && offset > 0)
                {
                    if (orderBy.Length == 0) orderBy = primaryKeysList;
                    string inverseOrderBy = orderBy;
                    string[] bys = inverseOrderBy.Split(new char[] { ',' });
                    inverseOrderBy = string.Empty;
                    foreach (string by in bys)
                    {
                        if (inverseOrderBy.Length > 0) inverseOrderBy += @", ";
                        string tmp = by.Trim();
                        if (tmp.EndsWith(@" ASC"))
                        {
                            tmp.Remove(tmp.Length - 4);
                            tmp += @" DESC";
                        }
                        else if (tmp.EndsWith(@" DESC"))
                        {
                            tmp.Remove(tmp.Length - 5);
                            tmp += @" ASC";
                        }
                        else tmp += @" DESC";
                        inverseOrderBy += tmp;
                    }
                    sql =
                       @" SELECT * " +
                       @" FROM " +
                       @"    (SELECT TOP (SELECT COUNT(*) " + tablesList + where + ") - " + offset.ToString() +
                                    selectFieldsList + tablesList + where +
                                inverseOrderBy +
                       @"  ) p " + orderBy;
                }
                else if (limit > 0 && offset > 0)
                {
                    if (orderBy.Length == 0) orderBy = primaryKeysList;
                    string inverseOrderBy = orderBy;
                    if (inverseOrderBy.Length == 0) inverseOrderBy = primaryKeysList;
                    string[] bys = inverseOrderBy.Split(new char[] { ',' });
                    inverseOrderBy = string.Empty;
                    foreach (string by in bys)
                    {
                        if (inverseOrderBy.Length > 0) inverseOrderBy += @", ";
                        string tmp = by.Trim();
                        if (tmp.EndsWith(@" ASC"))
                        {
                            tmp.Remove(tmp.Length - 4);
                            tmp += @" DESC";
                        }
                        else if (tmp.EndsWith(@" DESC"))
                        {
                            tmp.Remove(tmp.Length - 5);
                            tmp += @" ASC";
                        }
                        else tmp += @" DESC";
                        inverseOrderBy += tmp;
                    }
                    sql =
                       @" SELECT * " +
                       @" FROM " +
                       @"  (SELECT TOP " + limit + " * FROM" +
                       @"    (SELECT TOP " + (limit + offset) + " " + selectFieldsList + tablesList + where +
                                orderBy +
                       @"    ) pp " + inverseOrderBy +
                       @"  ) p " + orderBy;
                }
                return sql;
            }
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_");
        }

        public override Geometry ReadGeometry(object value)
        {
            byte[] geometryData = value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }

        public override string func_UTC_NOW
        {
            get { return @"now()"; } // NOT UTC
        }
        public override string func_LOWER
        {
            get { return @"LCASE"; }
        }
        public override string func_UPPER
        {
            get { return @"UCASE"; }
        }

        public override string func_HOUR(string date)
        {
            return @"DATEPART(hour, " + date + ")";
        }
        public override string func_MINUTE(string date)
        {
            return @"DATEPART(minute, " + date + ")";
        }
        public override string func_SECOND(string date)
        {
            return @"DATEPART(second, " + date + ")";
        }

        public override string type_TINYINT { get { return @"BYTE"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT"; } }
        public override string type_SMALLINT { get { return @"SHORT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SHORT"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT"; } }
        public override string type_BIGINT { get { return @"INT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"INT"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_VARCHAR { get { return @"VARCHAR"; } }
        public override string type_CHAR { get { return @"CHAR"; } }
        public override string type_TEXT { get { return @"TEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"TEXT"; } }
        public override string type_LONGTEXT { get { return @"TEXT"; } }
        public override string type_BOOLEAN { get { return @"BIT"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_GUID { get { return @"UNIQUEIDENTIFIER"; } }
        public override string type_BLOB { get { return @"IMAGE"; } }
        public override string type_AUTOINCREMENT { get { return @"AUTOINCREMENT"; } }
    }
}
