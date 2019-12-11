﻿using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Count : BaseAggregatePhrase
    {
        public bool Distinct = false;

        #region Constructors

        public Count(bool distinct = false) : base()
        {
            this.Distinct = distinct;
        }

        public Count(string tableName, string columnName, bool distinct = false) : base(tableName, columnName)
        {
            this.Distinct = distinct;
        }

        public Count(string columnName, bool distinct = false) : base(columnName)
        {
            this.Distinct = distinct;
        }

        public Count(object value, ValueObjectType valueType, bool distinct = false) : base(value, valueType)
        {
            this.Distinct = distinct;
        }

        public Count(IPhrase phrase, bool distinct = false) : base(phrase)
        {
            this.Distinct = distinct;
        }

        public Count(Where where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        public Count(WhereList where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = Distinct ? "COUNT(DISTINCT " : "COUNT(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
