using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
using System.Diagnostics;

namespace NorthSouthSystems.Entities;

public static class SqlSprocExtensions
{
    public static IQueryable<TEntity> FromSqlSproc<TEntity>(this DbSet<TEntity> set,
        string schemaName, string sprocName, params SqlParameter[] parameters)
        where TEntity : class =>
        Sproc(Throw.IfNull(set).FromSqlRaw, schemaName, sprocName, parameters);

    public static IQueryable<TResult> SqlSproc<TResult>(this DatabaseFacade database,
        string schemaName, string sprocName, params SqlParameter[] parameters) =>
        Sproc(Throw.IfNull(database).SqlQueryRaw<TResult>, schemaName, sprocName, parameters);

    private static IQueryable<T> Sproc<T>(Func<string, SqlParameter[], IQueryable<T>> sqlExecutor,
        string schemaName, string sprocName, params SqlParameter[] parameters)
    {
        Throw.IfNullOrWhiteSpace(schemaName);
        Throw.IfNullOrWhiteSpace(sprocName);
        parameters ??= [];

        var returnParameter = parameters.SingleOrDefault(p => p.Direction == ParameterDirection.ReturnValue);

        string? returnPrefix = returnParameter is not null
            ? string.Create(InvariantCulture, $"{returnParameter.ParameterName} =")
            : null;

        var inOutParameters = parameters.Where(p => p.Direction != ParameterDirection.ReturnValue);

        // BAD! This syntax results in positional sproc argument to parameter matching which can lead to nasty bugs:
        // BAD! EXEC [schema].[proc] @Foo, @Bar OUTPUT
        //
        // GOOD! This syntax results in named sproc argument to parameter matching:
        // GOOD! EXEC [schema].[proc] @Foo = @Foo, @Bar = @Bar OUTPUT
        string inOutParameterArgCsv =
            string.Join(", ", inOutParameters
                .Select(p => string.Create(InvariantCulture, $"{p.ParameterName} = {ToSprocArg(p)}")));

        return sqlExecutor(
            string.Create(InvariantCulture, $"EXEC {returnPrefix} {Bracket(schemaName)}.{Bracket(sprocName)} {inOutParameterArgCsv}"),
            parameters);
    }

    private static string Bracket(string name) => name[0] == '[' ? name : string.Create(InvariantCulture, $"[{name}]");

    private static string ToSprocArg(SqlParameter parameter) => parameter.Direction switch
    {
        ParameterDirection.Input => parameter.ParameterName,

        ParameterDirection.InputOutput
            or ParameterDirection.Output => string.Create(InvariantCulture, $"{parameter.ParameterName} OUTPUT"),

        ParameterDirection.ReturnValue => throw new UnreachableException(parameter.Direction.ToString()),

        _ => throw new UnreachableException(parameter.Direction.ToString())
    };
}