﻿using dg.Sql.Phrases;
using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public static class PhraseHelper
    {
        #region Strings

        public static Length Length(string tableName, object value, ValueObjectType valueType)
        {
            return new Length(tableName, value, valueType);
        }

        public static Length Length(object value, ValueObjectType valueType)
        {
            return new Length(value, valueType);
        }

        public static Length Length(string columnName)
        {
            return new Length(columnName);
        }

        public static Length Length(IPhrase phrase)
        {
            return new Length(phrase);
        }

        public static Lower Lower(string tableName, object value, ValueObjectType valueType)
        {
            return new Lower(tableName, value, valueType);
        }

        public static Lower Lower(object value, ValueObjectType valueType)
        {
            return new Lower(value, valueType);
        }

        public static Lower Lower(string columnName)
        {
            return new Lower(columnName);
        }

        public static Lower Lower(IPhrase phrase)
        {
            return new Lower(phrase);
        }

        public static Upper Upper(string tableName, object value, ValueObjectType valueType)
        {
            return new Upper(tableName, value, valueType);
        }

        public static Upper Upper(object value, ValueObjectType valueType)
        {
            return new Upper(value, valueType);
        }

        public static Upper Upper(string columnName)
        {
            return new Upper(columnName);
        }

        public static Upper Upper(IPhrase phrase)
        {
            return new Upper(phrase);
        }

        #endregion

        #region Encoding

        public static MD5 MD5(string tableName, object value, ValueObjectType valueType)
        {
            return new MD5(tableName, value, valueType);
        }

        public static MD5 MD5(object value, ValueObjectType valueType)
        {
            return new MD5(value, valueType);
        }

        public static MD5 MD5(string columnName)
        {
            return new MD5(columnName);
        }

        public static MD5 MD5(IPhrase phrase)
        {
            return new MD5(phrase);
        }

        #endregion

        #region DateTime

        public static DateTimeAdd DateTimeAdd(string tableName, object value, ValueObjectType valueType, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(tableName, value, valueType, unit, interval);
        }

        public static DateTimeAdd DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(value, valueType, unit, interval);
        }

        public static DateTimeAdd DateTimeAdd(string columnName, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(columnName, unit, interval);
        }

        public static UTC_TIMESTAMP UtcTimestamp()
        {
            return new UTC_TIMESTAMP();
        }

        public static Year Year(string tableName, object value, ValueObjectType valueType)
        {
            return new Year(tableName, value, valueType);
        }

        public static Year Year(object value, ValueObjectType valueType)
        {
            return new Year(value, valueType);
        }

        public static Year Year(string columnName)
        {
            return new Year(columnName);
        }

        public static Year Year(IPhrase phrase)
        {
            return new Year(phrase);
        }

        public static Month Month(string tableName, object value, ValueObjectType valueType)
        {
            return new Month(tableName, value, valueType);
        }

        public static Month Month(object value, ValueObjectType valueType)
        {
            return new Month(value, valueType);
        }

        public static Month Month(string columnName)
        {
            return new Month(columnName);
        }

        public static Month Month(IPhrase phrase)
        {
            return new Month(phrase);
        }

        public static Day Day(string tableName, object value, ValueObjectType valueType)
        {
            return new Day(tableName, value, valueType);
        }

        public static Day Day(object value, ValueObjectType valueType)
        {
            return new Day(value, valueType);
        }

        public static Day Day(string columnName)
        {
            return new Day(columnName);
        }

        public static Day Day(IPhrase phrase)
        {
            return new Day(phrase);
        }

        public static Hour Hour(string tableName, object value, ValueObjectType valueType)
        {
            return new Hour(tableName, value, valueType);
        }

        public static Hour Hour(object value, ValueObjectType valueType)
        {
            return new Hour(value, valueType);
        }

        public static Hour Hour(string columnName)
        {
            return new Hour(columnName);
        }

        public static Hour Hour(IPhrase phrase)
        {
            return new Hour(phrase);
        }

        public static Minute Minute(string tableName, object value, ValueObjectType valueType)
        {
            return new Minute(tableName, value, valueType);
        }

        public static Minute Minute(object value, ValueObjectType valueType)
        {
            return new Minute(value, valueType);
        }

        public static Minute Minute(string columnName)
        {
            return new Minute(columnName);
        }

        public static Minute Minute(IPhrase phrase)
        {
            return new Minute(phrase);
        }

        public static Second Second(string tableName, object value, ValueObjectType valueType)
        {
            return new Second(tableName, value, valueType);
        }

        public static Second Second(object value, ValueObjectType valueType)
        {
            return new Second(value, valueType);
        }

        public static Second Second(string columnName)
        {
            return new Second(columnName);
        }

        public static Second Second(IPhrase phrase)
        {
            return new Second(phrase);
        }

        #endregion

        #region Quantitative

        public static Min Min(object value, ValueObjectType valueType)
        {
            return new Min(value, valueType);
        }

        public static Min Min(string tableName, string columnName)
        {
            return new Min(tableName, columnName);
        }

        public static Min Min(string columnName)
        {
            return new Min(columnName);
        }

        public static Min Min(IPhrase phrase)
        {
            return new Min(phrase);
        }

        public static Max Max(object value, ValueObjectType valueType)
        {
            return new Max(value, valueType);
        }

        public static Max Max(string tableName, string columnName)
        {
            return new Max(tableName, columnName);
        }

        public static Max Max(string columnName)
        {
            return new Max(columnName);
        }

        public static Max Max(IPhrase phrase)
        {
            return new Max(phrase);
        }

        public static Count Count(object value, ValueObjectType valueType, bool distinct = false)
        {
            return new Count(value, valueType, distinct);
        }

        public static Count Count(string tableName, string columnName, bool distinct = false)
        {
            return new Count(tableName, columnName, distinct);
        }

        public static Count Count(string columnName, bool distinct = false)
        {
            return new Count(columnName, distinct);
        }

        public static Count Count(IPhrase phrase, bool distinct = false)
        {
            return new Count(phrase, distinct);
        }

        public static CountDistinct CountDistinct(object value, ValueObjectType valueType)
        {
            return new CountDistinct(value, valueType);
        }

        public static CountDistinct CountDistinct(string tableName, string columnName)
        {
            return new CountDistinct(tableName, columnName);
        }

        public static CountDistinct CountDistinct(string columnName)
        {
            return new CountDistinct(columnName);
        }

        public static CountDistinct CountDistinct(IPhrase phrase)
        {
            return new CountDistinct(phrase);
        }

        public static Sum Sum(object value, ValueObjectType valueType)
        {
            return new Sum(value, valueType);
        }

        public static Sum Sum(string tableName, string columnName)
        {
            return new Sum(tableName, columnName);
        }

        public static Sum Sum(string columnName)
        {
            return new Sum(columnName);
        }

        public static Sum Sum(IPhrase phrase)
        {
            return new Sum(phrase);
        }

        public static Avg Avg(object value, ValueObjectType valueType)
        {
            return new Avg(value, valueType);
        }

        public static Avg Avg(string tableName, string columnName)
        {
            return new Avg(tableName, columnName);
        }

        public static Avg Avg(string columnName)
        {
            return new Avg(columnName);
        }

        public static Avg Avg(IPhrase phrase)
        {
            return new Avg(phrase);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, object value, ValueObjectType valueType)
        {
            return new PassThroughAggregate(aggregateType, value, valueType);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, string tableName, string columnName)
        {
            return new PassThroughAggregate(aggregateType, tableName, columnName);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, string columnName)
        {
            return new PassThroughAggregate(aggregateType, columnName);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, IPhrase phrase)
        {
            return new PassThroughAggregate(aggregateType, phrase);
        }
        
        #endregion
    }
}
