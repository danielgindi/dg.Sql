﻿using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Least : IPhrase
    {
        public ValueWrapper Value1;
        public ValueWrapper Value2;

        #region Constructors
        
        public Least(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2
            )
        {
            this.Value1 = new ValueWrapper(value1, valueType1);
            this.Value2 = new ValueWrapper(value2, valueType2);
        }

        public Least(
            string tableName1, string column1,
            string tableName2, string column2
            )
        {
            this.Value1 = new ValueWrapper(tableName1, column1);
            this.Value2 = new ValueWrapper(tableName2, column2);
        }

        public Least(
            string tableName1, string column1,
            object value2, ValueObjectType valueType2
            )
        {
            this.Value1 = new ValueWrapper(tableName1, column1);
            this.Value2 = new ValueWrapper(value2, valueType2);
        }

        public Least(
            string tableName1, string column1,
            object value2
            )
        {
            this.Value1 = new ValueWrapper(tableName1, column1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Least(
            object value1, ValueObjectType valueType1,
            string tableName2, string column2
            )
        {
            this.Value1 = new ValueWrapper(value1, valueType1);
            this.Value2 = new ValueWrapper(tableName2, column2);
        }

        public Least(
            object value1,
            string tableName2, string column2
            )
        {
            this.Value1 = new ValueWrapper(value1, ValueObjectType.Value);
            this.Value2 = new ValueWrapper(tableName2, column2);
        }

        public Least(
            object value1,
            object value2
            )
        {
            this.Value1 = new ValueWrapper(value1, ValueObjectType.Value);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "LEAST(";
            
            ret += Value1.Build(conn, relatedQuery);

            ret += ", ";
            
            ret += Value2.Build(conn, relatedQuery);

            ret += ')';

            return ret;
        }
    }
}