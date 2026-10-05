namespace NorthSouthSystems;

public static partial class MathX
{
    public static double CeilingToFactor(this double value, double factor)
    {
        ThrowIfFactorOutOfRange(factor);

        return Math.Ceiling(value / factor) * factor;
    }

    public static double FloorToFactor(this double value, double factor)
    {
        ThrowIfFactorOutOfRange(factor);

        return Math.Floor(value / factor) * factor;
    }

    public static double RoundToFactor(
        this double value,
        double factor,
        MidpointRounding mode = MidpointRounding.AwayFromZero)
    {
        ThrowIfFactorOutOfRange(factor);

        if (mode == MidpointRounding.ToEven)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mode),
                $"{nameof(MidpointRounding)}.{MidpointRounding.ToEven} is ambiguous when calling {nameof(RoundToFactor)}.");
        }

        return Math.Round(value / factor, mode) * factor;
    }

    private static void ThrowIfFactorOutOfRange(double factor)
    {
        if (factor <= 0)
            throw new ArgumentOutOfRangeException(nameof(factor), "Must be > 0.");
    }
}
