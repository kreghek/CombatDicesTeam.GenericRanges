using JetBrains.Annotations;

namespace CombatDicesTeam.GenericRanges;

[PublicAPI]
public static class GenericRangeExtensions
{
    /// <summary>
    /// Indicates if the range contains <code>value</code>.
    /// </summary>
    /// <param name="range"> Generic range object to operate. </param>
    /// <param name="value">The value to look for.</param>
    /// <returns>true if the range contains <code>value</code>, false otherwise.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    public static bool Contains<T>(this GenericRange<T> range, T value) where T : IComparable<T>
    {
        return ((range.Min.CompareTo(value) <= 0) && (range.Max.CompareTo(value) >= 0));
    }

    /// <summary>
    /// Indicates if the range contains <code>value</code>.
    /// </summary>
    /// <param name="range"> Generic range object to operate. </param>
    /// <param name="value">A range to test.</param>
    /// <returns>true if the entire range in <code>value</code> is within this range.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    public static bool Contains<T>(this GenericRange<T> range, GenericRange<T> value) where T : IComparable<T>
    {
        int i1 = range.Min.CompareTo(value.Min);
        int i2 = range.Max.CompareTo(value.Max);
        return (i1 <= 0) && (i2 >= 0);
    }

    /// <summary>
    /// Indicates if the range is contained by <code>value</code>.
    /// </summary>
    /// <param name="range"> Generic range object to operate. </param>
    /// <param name="value">A range to test.</param>
    /// <returns>true if the entire range is within <code>value</code>.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    public static bool IsContainedBy<T>(this GenericRange<T> range, GenericRange<T> value) where T : IComparable<T>
    {
        return value.Contains(range);
    }

    /// <summary>
    /// Indicates if the range overlaps <code>value</code>.
    /// </summary>
    /// <param name="range"> Generic range object to operate. </param>
    /// <param name="value">A range to test.</param>
    /// <returns>true if any of the range in <code>value</code> is within this range.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    public static bool Overlaps<T>(this GenericRange<T> range, GenericRange<T> value) where T : IComparable<T>
    {
        return (range.Contains(value.Min) || range.Contains(value.Max) || value.Contains(range.Min) ||
                value.Contains(range.Max));
    }

    /// <summary>
    /// Returns the range that represents the intersection of this range and <code>value</code>.
    /// </summary>
    /// <param name="range"> Generic range object to operate. </param>
    /// <param name="value">The range to intersect with.</param>
    /// <returns>A range that contains the values that are common in both ranges, or null if there is no intersection.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    /// <exception cref="System.ArgumentException"><code>value</code> does not overlap the range.</exception>
    public static GenericRange<T> Intersect<T>(this GenericRange<T> range, GenericRange<T> value) where T : IComparable<T>
    {
        //TODO Looks like Math.Max
        var start = range.Min.CompareTo(value.Min) > 0 ? range.Min : value.Min;

        // TODO Looks like Math.Max(range.Max, value.Max)
        if (range.Max.CompareTo(value.Max) < 0)
        {
            return range with
            {
                Min = start
            };
        }

        return range with
        {
            Min = start
        };
    }

    /// <summary>
    /// Returns the range that represents the union of this range and <code>value</code>.
    /// </summary>
    /// <param name="range"> Generic range object to operate. </param>
    /// <param name="value">The range to union with.</param>
    /// <returns>A range that contains both ranges, or null if there is no union.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    /// <exception cref="System.ArgumentException"><code>value</code> is not contiguous with the range.</exception>
    public static GenericRange<T> Union<T>(this GenericRange<T> range, GenericRange<T> value)where T : IComparable<T>
    {
        // Assert.NotNull("value", value);
        // Assert.IsTrue("value", IsContiguousWith(value)); // Union makes no sense unless there is a contiguous border

        // If either one is a subset of the other, then is it the union
        if (range.Contains(value))
        {
            return range;
        }

        if (value.Contains(range))
        {
            return value;
        }
        
        // TODO Try new range with Math.Min(range.Min, value.Min) - Math.Max(range.Max, value.Max) 
            
        T start;
        start = range.Min.CompareTo(value.Min) < 0 ? range.Min : value.Min;

        if (range.Max.CompareTo(value.Max) > 0)
        {
            return range with
            {
                Min = start
            };
        }

        return value with
        {
            Min = start
        };
    }

    /// <summary>
    /// Returns a range which contains the current range, minus <code>value</code>.
    /// </summary>
    /// <param name="value">The value to complement the range by.</param>
    /// <returns>The complemented range.</returns>
    /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
    /// <exception cref="System.ArgumentException">
    /// <code>value</code> is contained by this range, complementing would lead to a split range.
    /// </exception>
    public Range<T> Complement(Range<T> value)
    {
        Assert.NotNull("value", value);
        Assert.IsFalse("value", Contains(value));

        if (Overlaps(value))
        {
            T start;

            // If value's start and end straddle our start, move our start up to be values end.
            if ((LowerBound.CompareTo(value.LowerBound) > 0) && (LowerBound.CompareTo(value.UpperBound) < 0))
            {
                start = value.UpperBound;
            }
            else
            {
                start = LowerBound;
            }

            // If value's start and end straddle our end, move our end back down to be values start.
            if ((UpperBound.CompareTo(value.LowerBound) > 0) && (UpperBound.CompareTo(value.UpperBound) < 0))
            {
                return new Range<T>(start, value.LowerBound);
            }
            else
            {
                return new Range<T>(start, UpperBound);
            }
        }
        else
        {
            return this;
        }
    }

    /// <summary>
    /// Splits the range into two.
    /// </summary>
    /// <param name="position">The position to split the range at.</param>
    /// <returns>The split ranges.</returns>
    /// <exception cref="System.ArgumentNullException"><code>position</code> is null.</exception>
    /// <exception cref="System.ArgumentException"><code>position</code> is not contained within the range.</exception>
    public IEnumerable<Range<T>> Split(T position)
    {
        Assert.NotNull("position", (object)position);
        Assert.IsTrue("position", Contains(position));

        if ((LowerBound.CompareTo(position) == 0) || (UpperBound.CompareTo(position) == 0))
        {
            // The position is at a boundary, so a split does not happen
            yield return this;
        }
        else
        {
            yield return Range.Create(LowerBound, position);
            yield return Range.Create(position, UpperBound);
        }
    }

    /// <summary>
    /// Iterates the range.
    /// </summary>
    /// <param name="incrementor">A function which takes a value, and returns the next value.</param>
    /// <returns>The items in the range.</returns>
    public IEnumerable<T> Iterate(Func<T, T> incrementor)
    {
        yield return LowerBound;
        T item = incrementor(LowerBound);
        while (UpperBound.CompareTo(item) >= 0)
        {
            yield return item;
            item = incrementor(item);
        }
    }

    /// <summary>
    /// Iterates the range in reverse.
    /// </summary>
    /// <param name="decrementor">A function which takes a value, and returns the previous value.</param>
    /// <returns>The items in the range.</returns>
    public IEnumerable<T> ReverseIterate(Func<T, T> decrementor)
    {
        yield return UpperBound;
        T item = decrementor(UpperBound);
        while (CompareTo(item) <= 0)
        {
            yield return item;
            item = decrementor(item);
        }
    }

    /// <summary>
    /// Indicates if this range is contiguous with <code>range</code>.
    /// </summary>
    /// <param name="range">The range to check.</param>
    /// <returns>true if the two ranges are contiguous, false otherwise.</returns>
    /// <remarks>Contiguous can mean containing, overlapping, or being next to.</remarks>
    public bool IsContiguousWith(Range<T> range)
    {
        if (Overlaps(range) || range.Overlaps(this) || range.Contains(this) || Contains(range))
        {
            return true;
        }

        // Once we remove overlapping and containing, only touching if available
        return ((UpperBound.Equals(range.LowerBound)) || (LowerBound.Equals(range.UpperBound)));
    }
}