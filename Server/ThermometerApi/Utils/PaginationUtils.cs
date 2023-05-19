using Models.Pagination;

namespace Utils
{
    public static class PaginationUtils
    {
        public static (string, IEnumerable<object>) GetWhereStatementFromCriterias(IEnumerable<Criteria?> criterias, IEnumerable<string> columns, int parameterCounterStartIndex = 0)
        {
            var whereStatement = "";
            List<object> parameters = new();

            if (criterias.Any(x => x != null)) whereStatement = "WHERE ";

            for (int i = 0; i < criterias.Count(); i++)
            {
                if (criterias.ElementAt(i) == null) continue;

                var criteria = criterias.ElementAt(i)!;
                var column = columns.ElementAt(i)!;

                if (criteria.Min != null && criteria.Max != null)
                {
                    parameters.Add(criteria.Min);
                    parameters.Add(criteria.Max);

                    whereStatement += $"{(criteria.Negate ? "NOT " : "")}{column} BETWEEN @{parameterCounterStartIndex + parameters.Count - 2} AND @{parameterCounterStartIndex + parameters.Count - 1}";
                }

                if (criteria.Min != null && criteria.Max == null)
                {
                    parameters.Add(criteria.Min);

                    whereStatement += $"{(criteria.Negate ? "NOT " : "")}{column} >= @{parameterCounterStartIndex + parameters.Count - 1}";
                }

                if (criteria.Min == null && criteria.Max != null)
                {
                    parameters.Add(criteria.Max);

                    whereStatement += $"{(criteria.Negate ? "NOT " : "")}{column} <= @{parameterCounterStartIndex + parameters.Count - 1}";
                }

                if (criteria.Min == null && criteria.Max == null) continue;
                if (i == criterias.Count() - 1) continue;

                whereStatement += $"{(criteria.Or ? " OR " : " AND ")}";
            }

            whereStatement = whereStatement.Trim();

            if (whereStatement.EndsWith("AND"))
            {
                whereStatement = whereStatement.Substring(0, whereStatement.Length - "AND".Length);
            }

            if (whereStatement.EndsWith("OR"))
            {
                whereStatement = whereStatement.Substring(0, whereStatement.Length - "OR".Length);
            }

            return (whereStatement, parameters);
        }
    }
}
