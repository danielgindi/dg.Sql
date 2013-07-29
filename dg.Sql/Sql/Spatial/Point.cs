﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using System.Globalization;

namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class Point : Geometry
        {
            public double X;
            public double Y;
            public double? Z;
            public double? M;

            public Point()
            {

            }
            public Point(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }
            public Point(double X, double Y, double Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
            public Point(double X, double Y, double? Z, double? M)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
                this.M = M;
            }

            public override bool IsEmpty
            {
                get
                {
                    return false;
                }
            }
            public override bool IsValid
            {
                get
                {
                    if (Double.IsNaN(X)) return false;
                    if (Double.IsInfinity(X)) return false;
                    if (Double.IsNaN(Y)) return false;
                    if (Double.IsInfinity(Y)) return false;
                    return true;
                }
            }

            static IFormatProvider formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            public override void BuildValue(StringBuilder sb, ConnectorBase conn)
            {
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    if (this.IsGeographyType)
                    {
                        sb.Append(@"geography::STGeomFromText('");
                    }
                    else
                    {
                        sb.Append(@"geometry::STGeomFromText('");
                    }
                }
                else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                {
                    if (this.IsGeographyType)
                    {
                        sb.Append(@"ST_GeogFromText('");
                    }
                    else
                    {
                        sb.Append(@"ST_GeomFromText('");
                    }
                }
                else
                {
                    sb.Append(@"GeomFromText('");
                }

                sb.Append(@"POINT(");
                sb.Append(X.ToString(formatProvider));
                sb.Append(' ');
                sb.Append(Y.ToString(formatProvider));

                if (SRID != null)
                {
                    sb.Append(@")',");
                    sb.Append(SRID.Value);
                    sb.Append(')');
                }
                else
                {
                    sb.Append(@")')");
                }
            }
            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
                sb.Append(@"POINT(");
                sb.Append(X.ToString(formatProvider));
                sb.Append(' ');
                sb.Append(Y.ToString(formatProvider));
                sb.Append(@")");
            }
        }
    }
}
