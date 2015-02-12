using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinqDefer.Sample.EfModel
{
    public class CommandOnlyLogger : DatabaseLogFormatter
    {
        private readonly string[] _ignoreStrings = { "__MigrationHistory", "INFORMATION_SCHEMA" };
        private readonly string _delimiter = new string('-', 40);

        public CommandOnlyLogger(DbContext context, Action<string> writeAction)
            : base(context, writeAction)
        {
        }

        public override void LogCommand<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            // simple filtering of queries to database
            if (_ignoreStrings.Any(i => command.CommandText.Contains(i)))
            {
                return;
            }

            var formattedSql = FormatSql(command.CommandText);
            Write(
                _delimiter + Environment.NewLine
                + formattedSql + Environment.NewLine
                + _delimiter + Environment.NewLine);
        }

        private string FormatSql(string sql)
        {
            return Regex.Replace(sql, "\\s{2,}", " ")
                .Replace(" WHERE ", Environment.NewLine + "WHERE" + Environment.NewLine + "\t")
                .Replace(" ORDER BY ", Environment.NewLine + "ORDER BY" + Environment.NewLine + "\t")
                .Replace(" FROM ", Environment.NewLine + "FROM" + Environment.NewLine + "\t")
                .Replace(",", "," + Environment.NewLine + "\t");
        }

        public override void LogResult<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
        }

        public override void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        public override void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        public override void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        public override void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }
    }
}