namespace NorthSouthSystems.Linq;

public static class BetweenExtensions
{
#pragma warning disable CA1034 // False positive.
    extension(Enumerable)
    {
        public static IEnumerable<int> Between(int startInclusive, int endInclusive)
        {
            if (startInclusive > endInclusive)
                (endInclusive, startInclusive) = (startInclusive, endInclusive);

            return Enumerable.Range(startInclusive, endInclusive - startInclusive + 1);
        }
    }
#pragma warning restore
}