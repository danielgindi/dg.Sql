﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardVarianceOfSample : IPhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;

        public StandardVarianceOfSample(string TableName, object Object, ValueObjectType ObjectType)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
        }
        public StandardVarianceOfSample(object Object, ValueObjectType ObjectType)
            : this(null, Object, ObjectType)
        {
        }
        public StandardVarianceOfSample(string ColumnName)
            : this(null, ColumnName, ValueObjectType.ColumnName)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"VAR_SAMP(";

            if (ObjectType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Object.ToString());
            }
            else if (ObjectType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Object);
            }
            else ret += Object;

            ret += ")";

            return ret;
        }
    }
}
