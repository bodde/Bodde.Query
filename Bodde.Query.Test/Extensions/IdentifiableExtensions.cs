using Bodde.Query.Test.Models;

namespace Bodde.Query.Test.Extensions;

public static class IdentifiableExtensions
{
    public static T[] GetIds<T>(this IEnumerable<IIdentifiable<T>> identifiables)
    {
        return [.. identifiables.Select(i => i.Id)];
    }

    public static string GetIdsCsv<T>(this IEnumerable<IIdentifiable<T>> identifiables)
    {
        return string.Join(", ", identifiables.GetIds());
    }
}
