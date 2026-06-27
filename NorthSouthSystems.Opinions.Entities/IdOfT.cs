using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace NorthSouthSystems.Entities;

internal static class IdOfT
{
    internal static SqlParameter NewSqlParameter<T>(string parameterName, IEnumerable<T> ids) =>
        new(parameterName, ToIdOfTRecords(ids).ToImmutableArray())
        {
            SqlDbType = SqlDbType.Structured,
            TypeName = GetTypeName<T>()
        };

    private static IEnumerable<SqlDataRecord> ToIdOfTRecords<T>(IEnumerable<T> ids)
    {
        var meta = new SqlMetaData("Id", GetSqlDbType<T>());
        var setter = GetSetter<T>();

        foreach (var id in ids)
        {
            var record = new SqlDataRecord(meta);
            setter(record, 0, id);

            yield return record;
        }
    }

    private static string GetTypeName<T>()
    {
        if (typeof(T) == typeof(Guid)) return Schemas.Shared + ".IdOfGuid";
        else if (typeof(T) == typeof(int)) return Schemas.Shared + ".IdOfInt";
        else throw new NotSupportedException(typeof(T).FullName);
    }

    private static SqlDbType GetSqlDbType<T>()
    {
        if (typeof(T) == typeof(Guid)) return SqlDbType.UniqueIdentifier;
        else if (typeof(T) == typeof(int)) return SqlDbType.Int;
        else throw new NotSupportedException(typeof(T).FullName);
    }

    private static Action<SqlDataRecord, int, T> GetSetter<T>()
    {
        Delegate? setter;

        if (typeof(T) == typeof(Guid)) setter = GuidSetter;
        else if (typeof(T) == typeof(int)) setter = IntSetter;
        else throw new NotSupportedException(typeof(T).FullName);

        return (Action<SqlDataRecord, int, T>)setter;
    }

    private static void GuidSetter(SqlDataRecord record, int index, Guid id) =>
        record.SetGuid(index, id);

    private static void IntSetter(SqlDataRecord record, int index, int id) =>
        record.SetInt32(index, id);
}