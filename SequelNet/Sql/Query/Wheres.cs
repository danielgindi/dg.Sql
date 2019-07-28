﻿namespace SequelNet
{
    public partial class Query
    {
        public Query ClearWhere()
        {
            _ListWhere = null;
            return this;
        }

        /// <summary>
        /// Adds a where condition. Does not remove any existing conditions.
        /// </summary>
        /// <param name="where"></param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Where(Where where)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Add(where);
            return this;
        }

        public Query Where(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        }

        public Query Where(string columnName, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, value, ValueObjectType.Value));
        }

        public Query Where(string columnName, WhereComparison comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value));
        }

        public Query Where(string literalExpression)
        {
            return Where(new Where(WhereCondition.AND, literalExpression, ValueObjectType.Literal, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query Where(IPhrase phrase)
        {
            return Where(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query Where(WhereList whereList)
        {
            return Where(new Where(WhereCondition.AND, whereList));
        }

        public Query Where(string tableName, string columnName, WhereComparison comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, value));
        }

        public Query Where(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
        }

        public Query Where(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Where(where);
        }

        public Query Where(string columnName, object betweenValue, object andValue)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value));
        }

        public Query Where(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            return Where(new Where(WhereCondition.AND, phrase, comparison, tableName, columnName));
        }

        public Query Where(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Where(new Where(WhereCondition.AND, phrase, comparison, value, valueType));
        }

        public Query Where(Query query, WhereComparison comparison, string tableName, string columnName)
        {
            return Where(new Where(WhereCondition.AND, query, comparison, tableName, columnName));
        }

        public Query Where(Query query, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Where(new Where(WhereCondition.AND, query, comparison, value, valueType));
        }

        public Query AND(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        }

        public Query AND(string columnName, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, value, ValueObjectType.Value));
        }

        public Query AND(string columnName, WhereComparison comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value));
        }

        public Query AND(string literalExpression)
        {
            return Where(new Where(WhereCondition.AND, literalExpression, ValueObjectType.Literal, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query AND(IPhrase phrase)
        {
            return Where(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query AND(WhereList whereList)
        {
            return Where(new Where(WhereCondition.AND, whereList));
        }

        public Query AND(string tableName, string columnName, WhereComparison comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, value));
        }

        public Query AND(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
        }

        public Query AND(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Where(where);
        }

        public Query AND(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            return Where(where);
        }

        public Query AND(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            return Where(new Where(WhereCondition.AND, phrase, comparison, tableName, columnName));
        }

        public Query AND(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Where(new Where(WhereCondition.AND, phrase, comparison, value, valueType));
        }

        public Query AND(Query query, WhereComparison comparison, string tableName, string columnName)
        {
            return Where(new Where(WhereCondition.AND, query, comparison, tableName, columnName));
        }

        public Query AND(Query query, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Where(new Where(WhereCondition.AND, query, comparison, value, valueType));
        }

        public Query OR(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(WhereCondition.OR, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        }

        public Query OR(string columnName, object value)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, value, ValueObjectType.Value));
        }

        public Query OR(string columnName, WhereComparison comparison, object value)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value));
        }

        public Query OR(string literalExpression)
        {
            return Where(new Where(WhereCondition.OR, literalExpression, ValueObjectType.Literal, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query OR(IPhrase phrase)
        {
            return Where(new Where(WhereCondition.OR, phrase, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query OR(WhereList whereList)
        {
            return Where(new Where(WhereCondition.OR, whereList));
        }

        public Query OR(string tableName, string columnName, WhereComparison comparison, object value)
        {
            return Where(new Where(WhereCondition.OR, tableName, columnName, comparison, value));
        }

        public Query OR(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            return Where(new Where(WhereCondition.OR, tableName, columnName, comparison, otherTableName, otherColumnName));
        }

        public Query OR(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Where(where);
        }

        public Query OR(string columnName, object betweenValue, object andValue)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value));
        }

        public Query OR(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            return Where(new Where(WhereCondition.OR, phrase, comparison, tableName, columnName));
        }

        public Query OR(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Where(new Where(WhereCondition.OR, phrase, comparison, value, valueType));
        }

        public Query OR(Query query, WhereComparison comparison, string tableName, string columnName)
        {
            return Where(new Where(WhereCondition.OR, query, comparison, tableName, columnName));
        }

        public Query OR(Query query, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Where(new Where(WhereCondition.OR, query, comparison, value, valueType));
        }

        public Query AddFromList(WhereList whereList)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            foreach (Where where in whereList)
            {
                _ListWhere.Add(where);
            }
            return this;
        }
    }
}