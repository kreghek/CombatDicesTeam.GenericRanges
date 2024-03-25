using JetBrains.Annotations;

namespace CombatDicesTeam.GenericRanges;

public sealed record GenericRange<T>(T Min, T Max): IComparable<GenericRange<T>>, IComparable<T>, IComparable where T : IComparable<T>
{
    [PublicAPI]
    public static GenericRange<T> CreateMono(T value)
    {
        return new GenericRange<T>(value, value);
    }
    
    #region IComparable<GenericRange<T>> Members
    
    /// <summary>
    /// See <see cref="System.IComparable{T}.CompareTo"/>.
    /// </summary>
    public int CompareTo(GenericRange<T>? other)
    {
        T? otherMin = other is null ? default : other.Min;
        return Min.CompareTo(otherMin);
    }
    
    #endregion

    #region IComparable<T> Members
    
    /// <summary>
    /// See <see cref="System.IComparable{T}.CompareTo"/>.
    /// </summary>
    public int CompareTo(T? other)
    {
        return Min.CompareTo(other);
    }
    
    #endregion

    #region IComparable Members
    
    /// <summary>
    /// See <see cref="System.IComparable.CompareTo"/>.
    /// </summary>
    public int CompareTo(object? obj)
    {
        return obj switch
        {
            GenericRange<T> range => CompareTo(range),
            T other => CompareTo(other),
            _ => throw new InvalidOperationException($"Cannot compare to {obj}")
        };
    }
    
    #endregion
}