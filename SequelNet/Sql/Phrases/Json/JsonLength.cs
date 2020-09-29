﻿using System;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// Returns the length of a json object/array
    /// </summary>
    public class JsonLength : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors
        
        public JsonLength(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public JsonLength(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public JsonLength(string columnName)
            : this(null, columnName)
        {
        }

        public JsonLength(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public JsonLength(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        ret += "JSON_LENGTH(";
                        ret += Value.Build(conn, relatedQuery);
                        ret += ")";
                    }
                    break;

                case ConnectorBase.SqlServiceType.POSTGRESQL:
                    {
                        ret += "json_array_length(";
                        ret += Value.Build(conn, relatedQuery);
                        ret += ")";
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonLength is not supported by current DB type");
            }

            return ret;
        }
    }
}
