﻿using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class UTC_TIMESTAMP : IPhrase
    {
        public UTC_TIMESTAMP()
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.UtcNow();
        }
    }
}