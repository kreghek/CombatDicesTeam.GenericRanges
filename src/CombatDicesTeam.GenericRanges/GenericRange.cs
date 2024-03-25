using JetBrains.Annotations;

namespace CombatDicesTeam.GenericRanges;

public sealed record GenericRange<T>(T Min, T Max)
{
    [PublicAPI]
    public static GenericRange<T> CreateMono(T value)
    {
        return new GenericRange<T>(value, value);
    }
}