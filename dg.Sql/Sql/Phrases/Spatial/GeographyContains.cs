﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class GeographyContains : IPhrase
    {
        public string OuterTableName;
        public object OuterValue;
        public ValueObjectType OuterValueType;
        public string InnerTableName;
        public object InnerValue;
        public ValueObjectType InnerValueType;

        public GeographyContains(
            string outerTableName, object outerValue, ValueObjectType outerValueType,
            string innerTableName, object innerValue, ValueObjectType innerValueType)
        {
            this.OuterTableName = outerTableName;
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerValue;
            this.InnerValueType = innerValueType;
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            object innerValue, ValueObjectType innerValueType)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerValue = innerValue;
            this.InnerValueType = innerValueType;
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string CctainedColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerValue = CctainedColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerTableName, string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(
            Geometry outerValue,
            string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(
            Geometry outerValue,
            string innerTableName, string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(Geometry outerValue, Geometry innerValue)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerValue = innerValue;
            this.InnerValueType = ValueObjectType.Value;
        }

        public GeographyContains(
            string outerColumnName,
            Geometry InnerObject)
        {
            this.OuterValue = outerColumnName;
            this.OuterValueType = ValueObjectType.ColumnName;
            this.InnerValue = InnerObject;
            this.InnerValueType = ValueObjectType.Value;
        }

        public GeographyContains(
            string outerTableName, string outerColumnName, 
            Geometry InnerObject)
        {
            this.OuterTableName = outerTableName;
            this.OuterValue = outerColumnName;
            this.OuterValueType = ValueObjectType.ColumnName;
            this.InnerValue = InnerObject;
            this.InnerValueType = ValueObjectType.Value;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            StringBuilder sb = new StringBuilder();

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
            }
            else 
            {
                if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                {
                    sb.Append(@"ST_Contains(");
                }
                else // MYSQL
                {
                    sb.Append(@"MBRContains(");
                }
            }

            if (OuterValueType == ValueObjectType.ColumnName)
            {
                if (OuterTableName != null && OuterTableName.Length > 0)
                {
                    sb.Append(conn.EncloseFieldName(OuterTableName));
                    sb.Append(".");
                }
                sb.Append(conn.EncloseFieldName(OuterValue.ToString()));
            }
            else if (OuterValueType == ValueObjectType.Value)
            {
                if (OuterValue is Geometry)
                {
                    ((Geometry)OuterValue).BuildValue(sb, conn);
                }
                else
                {
                    sb.Append(conn.PrepareValue(OuterValue));
                }
            }
            else sb.Append(OuterValue);

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
                sb.Append(@".STContains(");
            }
            else // MYSQL, PostgreSQL
            {
                sb.Append(@",");
            }

            if (InnerValueType == ValueObjectType.ColumnName)
            {
                if (InnerTableName != null && InnerTableName.Length > 0)
                {
                    sb.Append(conn.EncloseFieldName(InnerTableName));
                    sb.Append(".");
                }
                sb.Append(conn.EncloseFieldName(InnerValue.ToString()));
            }
            else if (InnerValueType == ValueObjectType.Value)
            {
                if (InnerValue is Geometry)
                {
                    ((Geometry)InnerValue).BuildValue(sb, conn);
                }
                else
                {
                    sb.Append(conn.PrepareValue(InnerValue));
                }
            }
            else sb.Append(InnerValue);

            sb.Append(@")");

            return sb.ToString();
        }
    }
}
