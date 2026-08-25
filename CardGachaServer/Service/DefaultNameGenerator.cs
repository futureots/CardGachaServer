namespace CardGachaServer.Service;

public static class DefaultNameGenerator
{
    public static string Generate(int digitCount = 6)
    {
        if (digitCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(digitCount));

        var maxExclusive = (int)Math.Pow(10, digitCount);
        var number = Random.Shared.Next(0, maxExclusive);
        return $"User{number.ToString().PadLeft(digitCount, '0')}";
    }
}