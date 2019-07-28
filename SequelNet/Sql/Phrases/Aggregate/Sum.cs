﻿using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Sum : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Sum()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Sum(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Sum(string columnName)
            : this(null, columnName)
        {
        }

        public Sum(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Sum(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Sum(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"SUM(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}