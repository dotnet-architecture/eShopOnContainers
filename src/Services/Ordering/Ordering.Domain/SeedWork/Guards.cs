using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork
{
    public class Guards
    {

        private static int ArgumentCharCount(string argument) => string.IsNullOrEmpty(argument) ? 0 : argument.Trim().Length;

        public static void AssertNotNull(Object arg1)
        {
            if (arg1 == null)
            {
                string message = "Argument Invalid, is null";
                throw new ArgumentException(message);
            }

        }

        public static void AssertGuidNotEmpty(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                string message = $"Invalid GUID value. Set an ID not empty";
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertGuidsAreDifferent(Guid Id1, Guid Id2)
        {
            AssertGuidNotEmpty(Id1);
            AssertGuidNotEmpty(Id2);
            if (Id1 == Id2)
            {
                string message = $"Invalid request, the GUIDs are equal. Guid 1 {Id1} and Guid 2 {Id2}";
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertIntIsGreaterThan0(int number)
        {
            if (number < 0)
            {
                string message = $"Invalid int value. Set an interger greater than 0";
                throw new InvalidOperationException(message);
            }

        }

        public static void AssertStringRange(string arg1, int upperBound, int lowerBound)
        {
            AssertNotNull(arg1);
            if (ArgumentCharCount(arg1) > upperBound || ArgumentCharCount(arg1) < lowerBound)
            {
                string message = $"Invalid string range. Lower bound rule:{lowerBound} and Upper bound rule:{upperBound}. Whitespace borders are not allowed";
                throw new InvalidOperationException(message);
            }

        }

        public static void AssertStringIsNotEmptyOrNull(string arg)
        {
            if (String.IsNullOrEmpty(arg))
            {
                string message = $"Invalid string value";
                throw new InvalidOperationException(message);
            }

        }

        public static void AssertStringUpperBound(string arg1, int upperBound)
        {
            AssertNotNull(arg1);
            if (ArgumentCharCount(arg1) > upperBound)
            {
                string message = $"Invalid string upper bound. Upper bound rule:{upperBound}. Whitespace borders are not allowed";
                throw new InvalidOperationException(message);
            }

        }

        public static void AssertStringLowerBound(string arg1, int lowerBound)
        {
            AssertNotNull(arg1);
            if (ArgumentCharCount(arg1) < lowerBound)
            {
                string message = $"Invalid string lower bound. Lower bound rule:{lowerBound}. Whitespace borders are not allowed";
                throw new InvalidOperationException(message);
            }

        }

        public static void AssertIsFalse(bool flag, string message)
        {
            if (flag)
            {
                throw new InvalidOperationException(message);
            }

        }

        public static void AssertIsTrue(bool flag,string message)
        {
            if (!flag)
            {
                throw new InvalidOperationException(message);
            }

        }
    }
}
