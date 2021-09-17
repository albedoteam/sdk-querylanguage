namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using System;
    using System.Collections.Generic;

    public readonly struct StringSegment
    {
        public StringSegment(string sourceString, int start, int length)
        {
            SourceString = sourceString ?? throw new ArgumentNullException(nameof(sourceString));
            Start = start;
            Length = length;
            End = start + length;
        }

        public string SourceString { get; }
        public int Start { get; }
        public int End { get; }
        public int Length { get; }

        private string Value => SourceString.Substring(Start, Length);

        public bool IsRightOf(StringSegment segment)
        {
            if (SourceString != segment.SourceString)
                throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));

            return segment.End <= Start;
        }

        public bool IsRightOf(int index)
        {
            return Start >= index;
        }

        public bool IsLeftOf(StringSegment segment)
        {
            if (SourceString != segment.SourceString)
                throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));

            return segment.Start >= End;
        }

        public bool IsLeftOf(int index)
        {
            return End <= index;
        }

        public static StringSegment Encompass(IEnumerable<StringSegment> segments)
        {
            using var enumerator = segments.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException($"{nameof(segments)} must have at least one item", nameof(segments));

            var sourceString = enumerator.Current.SourceString;
            var minStart = enumerator.Current.Start;
            var maxEnd = enumerator.Current.End;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current!.SourceString != sourceString)
                    throw new ArgumentException($"{nameof(segments)} must all have the same source string",
                        nameof(segments));
                minStart = Math.Min(enumerator.Current.Start, minStart);
                maxEnd = Math.Max(enumerator.Current.End, maxEnd);
            }

            return new StringSegment(sourceString, minStart, maxEnd - minStart);
        }

        public static StringSegment Encompass(params StringSegment[] segments)
        {
            return Encompass((IEnumerable<StringSegment>)segments);
        }

        public bool IsBetween(StringSegment segment1, StringSegment segment2)
        {
            if (SourceString != segment1.SourceString)
                throw new ArgumentException($"{nameof(segment1)} must have the same source string", nameof(segment1));

            if (SourceString != segment2.SourceString)
                throw new ArgumentException($"{nameof(segment2)} must have the same source string", nameof(segment2));

            return segment1.End <= Start && segment2.Start >= End;
        }

        public static StringSegment Between(StringSegment segment1, StringSegment segment2)
        {
            if (segment1.SourceString != segment2.SourceString)
                throw new ArgumentException($"{nameof(segment1)} and {nameof(segment2)} must the same source string");

            return new StringSegment(
                segment1.SourceString,
                segment1.End,
                segment2.Start - segment1.End);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}