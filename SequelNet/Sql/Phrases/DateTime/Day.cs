﻿using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Day : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Day(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Day(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Day(string columnName)
            : this(null, columnName)
        {
        }

        public Day(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Day(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.DayPartOfDate(ret);
        }
    }
}
