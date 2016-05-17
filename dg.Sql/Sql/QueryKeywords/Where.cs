﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class Where
    {
        private WhereComparison _Comparison = WhereComparison.None;
        private WhereCondition _Condition = WhereCondition.AND;
        private object _First = null;
        private ValueObjectType _FirstType = ValueObjectType.Literal;
        private object _Second = null;
        private ValueObjectType _SecondType = ValueObjectType.Literal;
        private object _Third = null;
        private ValueObjectType _ThirdType = ValueObjectType.Literal;
        private string _FirstTableName = null;
        private string _SecondTableName = null;
        private string _ThirdTableName = null;

        public WhereComparison Comparison
        {
            get { return _Comparison; }
            set { _Comparison = value; }
        }

        public WhereCondition Condition
        {
            get { return _Condition; }
            set { _Condition = value; }
        }

        public object First
        {
            get { return _First; }
            set { _First = value; }
        }

        public ValueObjectType FirstType
        {
            get { return _FirstType; }
            set { _FirstType = value; }
        }

        public object Second
        {
            get { return _Second; }
            set { _Second = value; }
        }

        public ValueObjectType SecondType
        {
            get { return _SecondType; }
            set { _SecondType = value; }
        }

        public object Third
        {
            get { return _Third; }
            set { _Third = value; }
        }

        public ValueObjectType ThirdType
        {
            get { return _ThirdType; }
            set { _ThirdType = value; }
        }

        public string FirstTableName
        {
            get { return _FirstTableName; }
            set { _FirstTableName = value; }
        }

        public string SecondTableName
        {
            get { return _SecondTableName; }
            set { _SecondTableName = value; }
        }

        public string ThirdTableName
        {
            get { return _ThirdTableName; }
            set { _ThirdTableName = value; }
        }

        public Where()
        {
        }

        public Where(WhereList whereList)
        {
            First = whereList;
        }

        public Where(object thisLiteral, WhereComparison comparedBy, object thatLiteral)
        {
            Comparison = comparedBy;
            First = thisLiteral;
            Second = thatLiteral;
        }

        public Where(object thisObject, ValueObjectType thisObjectType, WhereComparison comparedBy, object thatObject, ValueObjectType thatObjectType)
        {
            Comparison = comparedBy;
            First = thisObject;
            FirstType = thisObjectType;
            Second = thatObject;
            SecondType = thatObjectType;
        }

        public Where(object thisLiteral, object betweenThisLiteral, object andThatLiteral)
        {
            Comparison = WhereComparison.Between;
            First = thisLiteral;
            Second = betweenThisLiteral;
            Third = andThatLiteral;
        }

        public Where(object thisObject, ValueObjectType thisObjectType,
            object betweenThisObject, ValueObjectType betweenThisObjectType,
            object andThatObject, ValueObjectType andThatObjectType)
        {
            Comparison = WhereComparison.Between;
            First = thisObject;
            FirstType = thisObjectType;
            Second = betweenThisObject;
            SecondType = betweenThisObjectType;
            Third = andThatObject;
            ThirdType = andThatObjectType;
        }

        public Where(string tableName, string columnName,
            WhereComparison comparedBy, object value)
        {
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(string tableName, string columnName,
            WhereComparison comparedBy, string thatTableName, string thatColumnName)
        {
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            SecondTableName = thatTableName;
            Second = thatColumnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public Where(WhereCondition condition, WhereList whereList)
        {
            Condition = condition;
            First = whereList;
        }

        public Where(WhereCondition condition, object thisLiteral, WhereComparison comparedBy, object thatLiteral)
        {
            Condition = condition;
            Comparison = comparedBy;
            First = thisLiteral;
            Second = thatLiteral;
        }

        public Where(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparison comparedBy, object thatObject, ValueObjectType thatObjectType)
        {
            Condition = condition;
            Comparison = comparedBy;
            First = thisObject;
            FirstType = thisObjectType;
            Second = thatObject;
            SecondType = thatObjectType;
        }

        public Where(WhereCondition condition, object thisLiteral, object betweenThisLiteral, object andThatLiteral)
        {
            Condition = condition;
            Comparison = WhereComparison.Between;
            First = thisLiteral;
            Second = betweenThisLiteral;
            Third = andThatLiteral;
        }

        public Where(WhereCondition condition,
            object thisObject, ValueObjectType thisObjectType,
            object betweenThisObject, ValueObjectType betweenThisObjectType,
            object andThatObject, ValueObjectType andThatObjectType)
        {
            Condition = condition;
            Comparison = WhereComparison.Between;
            First = thisObject;
            FirstType = thisObjectType;
            Second = betweenThisObject;
            SecondType = betweenThisObjectType;
            Third = andThatObject;
            ThirdType = andThatObjectType;
        }

        public Where(WhereCondition condition,
            string tableName, string columnName,
            WhereComparison comparedBy, object value)
        {
            Condition = condition;
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(WhereCondition condition,
            string tableName, string columnName,
            WhereComparison comparedBy, string thatTableName, string thatColumnName)
        {
            Condition = condition;
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            SecondTableName = thatTableName;
            Second = thatColumnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public void BuildCommand(StringBuilder outputBuilder, bool isFirst, ConnectorBase conn, Query relatedQuery)
        {
            BuildCommand(outputBuilder, isFirst, conn, relatedQuery, null, null);
        }

        public void BuildCommand(StringBuilder outputBuilder, bool isFirst, ConnectorBase conn, Query relatedQuery, TableSchema rightTableSchema, string rightTableName)
        {
            if (!isFirst)
            {
                switch (Condition)
                {
                    case WhereCondition.AND:
                        outputBuilder.Append(@" AND ");
                        break;
                    case WhereCondition.OR:
                        outputBuilder.Append(@" OR ");
                        break;
                }
            }

            if (Comparison == WhereComparison.None &&  // Its not a comparison
                // And there's no list or the list is empty
                (!(First is WhereList) || ((WhereList)First).Count == 0) &&
                // And it's not a literal expression
                FirstType != ValueObjectType.Literal &&
                FirstType != ValueObjectType.Value
                )
            {
                outputBuilder.Append(@"1"); // dump a dummy TRUE condition to fill the blank
                return;
            }

            if (First is WhereList)
            {
                outputBuilder.Append('(');
                ((WhereList)First).BuildCommand(outputBuilder, conn, relatedQuery, rightTableSchema, rightTableName);
                outputBuilder.Append(')');
            }
            else
            {
                if (FirstType == ValueObjectType.Value)
                {
                    if (SecondType == ValueObjectType.ColumnName)
                    {
                        if (object.ReferenceEquals(SecondTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                        {
                            outputBuilder.Append(Query.PrepareColumnValue(rightTableSchema.Columns.Find((string)Second), First, conn, relatedQuery));
                        }
                        else
                        {
                            TableSchema schema = null;
                            if (relatedQuery != null)
                            {
                                if (SecondTableName == null || !relatedQuery.TableAliasMap.TryGetValue(SecondTableName, out schema))
                                {
                                    schema = relatedQuery.Schema;
                                }
                            }

                            if (schema != null)
                            {
                                outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)Second), First, conn, relatedQuery));
                            }
                            else
                            {
                                outputBuilder.Append(conn.PrepareValue(First, relatedQuery));
                            }
                        }
                    }
                    else
                    {
                        outputBuilder.Append(conn.PrepareValue(First, relatedQuery));
                    }
                }
                else if (FirstType == ValueObjectType.ColumnName)
                {
                    if (FirstTableName != null)
                    {
                        if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                        {
                            outputBuilder.Append(conn.EncloseFieldName(rightTableName));
                        }
                        else
                        {
                            outputBuilder.Append(conn.EncloseFieldName(FirstTableName));
                        }
                        outputBuilder.Append('.');
                    }
                    outputBuilder.Append(conn.EncloseFieldName((string)First));
                }
                else
                {
                    outputBuilder.Append(First == null ? @"NULL" : First);
                }

                if (Comparison != WhereComparison.None)
                {
                    switch (Comparison)
                    {
                        case WhereComparison.EqualsTo:
                            if (First == null || Second == null) outputBuilder.Append(@" IS ");
                            else outputBuilder.Append(@" = ");
                            break;
                        case WhereComparison.NotEqualsTo:
                            if (First == null || Second == null) outputBuilder.Append(@" IS NOT ");
                            else outputBuilder.Append(@" <> ");
                            break;
                        case WhereComparison.GreaterThan:
                            outputBuilder.Append(@" > ");
                            break;
                        case WhereComparison.GreaterThanOrEqual:
                            outputBuilder.Append(@" >= ");
                            break;
                        case WhereComparison.LessThan:
                            outputBuilder.Append(@" < ");
                            break;
                        case WhereComparison.LessThanOrEqual:
                            outputBuilder.Append(@" <= ");
                            break;
                        case WhereComparison.Is:
                            outputBuilder.Append(@" IS ");
                            break;
                        case WhereComparison.IsNot:
                            outputBuilder.Append(@" IS NOT ");
                            break;
                        case WhereComparison.Like:
                            outputBuilder.Append(@" LIKE ");
                            break;
                        case WhereComparison.Between:
                            outputBuilder.Append(@" BETWEEN ");
                            break;
                        case WhereComparison.In:
                            outputBuilder.Append(@" IN ");
                            break;
                        case WhereComparison.NotIn:
                            outputBuilder.Append(@" NOT IN ");
                            break;
                    }

                    if (Comparison != WhereComparison.In && Comparison != WhereComparison.NotIn)
                    {
                        if (SecondType == ValueObjectType.Value)
                        {
                            if (Second is Query)
                            {
                                outputBuilder.Append('(');
                                outputBuilder.Append(((Query)Second).BuildCommand(conn));
                                outputBuilder.Append(')');
                            }
                            else
                            {
                                if (FirstType == ValueObjectType.ColumnName)
                                {
                                    // Match SECOND value to FIRST's column type
                                    if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                    {
                                        outputBuilder.Append(Query.PrepareColumnValue(rightTableSchema.Columns.Find((string)First), Second, conn, relatedQuery));
                                    }
                                    else
                                    {
                                        TableSchema schema = null;
                                        if (relatedQuery != null)
                                        {
                                            if (FirstTableName == null || !relatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                            {
                                                schema = relatedQuery.Schema;
                                            }
                                        }

                                        if (schema != null)
                                        {
                                            outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Second, conn, relatedQuery));
                                        }
                                        else
                                        {
                                            outputBuilder.Append(conn.PrepareValue(Second, relatedQuery));
                                        }
                                    }
                                }
                                else
                                {
                                    outputBuilder.Append(conn.PrepareValue(Second, relatedQuery));
                                }
                            }
                        }
                        else if (SecondType == ValueObjectType.ColumnName)
                        {
                            if (SecondTableName != null)
                            {
                                if (object.ReferenceEquals(SecondTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(rightTableName));
                                }
                                else
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(SecondTableName));
                                }
                                outputBuilder.Append('.');
                            }
                            outputBuilder.Append(conn.EncloseFieldName((string)Second));
                        }
                        else
                        {
                            if (Second == null) outputBuilder.Append(@"NULL");
                            else outputBuilder.Append(Second);
                        }
                    }
                    else
                    {
                        if (Second is Query) outputBuilder.AppendFormat(@"({0})", Second.ToString());
                        else
                        {
                            ICollection collIn = Second as ICollection;
                            if (collIn != null)
                            {
                                StringBuilder sbIn = new StringBuilder();
                                sbIn.Append('(');
                                bool first = true;

                                TableSchema schema = null;
                                if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    schema = rightTableSchema;
                                }
                                else
                                {
                                    if (relatedQuery != null)
                                    {
                                        if (FirstTableName == null || !relatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                        {
                                            schema = relatedQuery.Schema;
                                        }
                                    }
                                }

                                foreach (object objIn in collIn)
                                {
                                    if (first) first = false;
                                    else sbIn.Append(',');

                                    if (schema != null)
                                    {
                                        sbIn.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), objIn, conn, relatedQuery));
                                    }
                                    else
                                    {
                                        sbIn.Append(conn.PrepareValue(objIn, relatedQuery));
                                    }
                                }

                                sbIn.Append(')');
                                outputBuilder.Append(sbIn.ToString());
                            }
                            else outputBuilder.Append(Second);
                        }
                    }

                    if (Comparison == WhereComparison.Between)
                    {
                        outputBuilder.Append(@" AND ");
                        if (ThirdType == ValueObjectType.Value)
                        {
                            if (FirstType == ValueObjectType.ColumnName)
                            {
                                TableSchema schema = null;
                                if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    schema = rightTableSchema;
                                }
                                else
                                {
                                    if (relatedQuery != null)
                                    {
                                        if (FirstTableName == null || !relatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                        {
                                            schema = relatedQuery.Schema;
                                        }
                                    }
                                }

                                if (schema != null)
                                {
                                    outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Third, conn, relatedQuery));
                                }
                                else
                                {
                                    outputBuilder.Append(conn.PrepareValue(Third, relatedQuery));
                                }
                            }
                            else
                            {
                                outputBuilder.Append(conn.PrepareValue(Third, relatedQuery));
                            }
                        }
                        else if (ThirdType == ValueObjectType.ColumnName)
                        {
                            if (ThirdTableName != null)
                            {
                                if (object.ReferenceEquals(ThirdTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(rightTableName));
                                }
                                else
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(ThirdTableName));
                                }
                                outputBuilder.Append('.');
                            }
                            outputBuilder.Append(conn.EncloseFieldName((string)Third));
                        }
                        else outputBuilder.Append(Third == null ? @"NULL" : Third);
                    }

                    if (Comparison == WhereComparison.Like)
                    {
                        outputBuilder.Append(' ');
                        outputBuilder.Append(conn.LikeEscapingStatement);
                        outputBuilder.Append(' ');
                    }
                }
            }
        }
    }
}
