﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Year : BasePhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;

        public Year(string TableName, object Object, ValueObjectType ObjectType)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
        }
        public Year(object Object, ValueObjectType ObjectType)
            : this(null, Object, ObjectType)
        {
        }
        public Year(string ColumnName)
            : this(null, ColumnName, ValueObjectType.ColumnName)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = "";

            if (ObjectType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.encloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.encloseFieldName(Object.ToString());
            }
            else if (ObjectType == ValueObjectType.Value)
            {
                ret += conn.prepareValue(Object);
            }
            else ret += Object;

            return conn.func_YEAR(ret);
        }
    }
}
