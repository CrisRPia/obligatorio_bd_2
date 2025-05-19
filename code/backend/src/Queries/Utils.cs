using System.Data.Common;
using System.Reflection;
using MySqlConnector;

namespace backend.src.Queries;

public static class Utils
{
    private static MySqlParameter CreateMySqlParameter(
        string name,
        object value
    )
    {
        var parameter = new MySqlParameter($"@{name}", value ?? DBNull.Value);

        return parameter;
    }

    public static void AddFromObject(
        this MySqlParameterCollection parameters,
        object paramObject
    )
    {
        PropertyInfo[] properties = paramObject
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in properties)
        {
            string paramName = prop.Name;
            var paramValue =
                prop.GetValue(paramObject)
                ?? throw new NotImplementedException();
            var parameter = CreateMySqlParameter(paramName, paramValue);
            parameters.Add(parameter);
        }
    }
}
