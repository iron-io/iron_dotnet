using System;

namespace IronIO.Core
{
    public struct IronTimespan
    {
        public static implicit operator IronTimespan(int? value)
        {
            return new IronTimespan(value.GetValueOrDefault());
        }

        public static implicit operator IronTimespan(int value)
        {
            return new IronTimespan(value);
        }

        public static implicit operator IronTimespan(TimeSpan value)
        {
            return new IronTimespan(value);
        }

        public static IronTimespan None = new IronTimespan(0);

        public IronTimespan(int seconds)
        {
            Seconds = seconds;
        }

        public IronTimespan(TimeSpan timeSpan)
        {
            Seconds = timeSpan.Seconds;
        }

        public int Seconds { get; }

        public bool Equals(IronTimespan other)
        {
            return Seconds == other.Seconds;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is IronTimespan && Equals((IronTimespan)obj);
        }

        public override int GetHashCode()
        {
            return Seconds;
        }

        public int? GetSeconds(int? defaultValue = default(int?), int? min = null, int? max = null)
        {
            return Seconds > 0 ? GetRange(Seconds, min, max) : defaultValue;
        }

        private static int? GetRange(int seconds, int? min, int? max)
        {
            if (min.HasValue && seconds < min.Value)
            {
                return min.Value;
            }

            if (max.HasValue && seconds > max.Value)
            {
                return max.Value;
            }

            return seconds;
        }

        public override string ToString() => Convert.ToString(GetSeconds());
    }
}