using Shouldly;

namespace Test.Shared;

public static class EnumExtensionTest
{
    public static void VerifyCanProcessAllKnown<TEnum>(Action<TEnum> action)
        where TEnum : struct, Enum
    {
        foreach (var value in Enum.GetValues<TEnum>())
        {
            Should.NotThrow(() => action(value));
        }
    }

    public static void VerifyInvalidValueThrows<TEnum>(Action<TEnum> extension)
        where TEnum : struct, Enum
        => Should.Throw<ArgumentOutOfRangeException>(() => extension((TEnum)(object)999));
}