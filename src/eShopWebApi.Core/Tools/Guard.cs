using System;

namespace eShopWebApi.Core.Tools
{
    public static class Guard
    {
        public static void IsNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static T ReturnIfChecked<T>(T argumentValue, string argumentName)
        {
            IsNotNull(argumentValue, argumentName);
            return argumentValue;
        }

        public static string ReturnIfNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if(string.IsNullOrEmpty(argumentValue))
            {
                throw new ArgumentException($"{argumentName} cannot be empty.", argumentName);
            }

            return argumentValue;
        }

        public static decimal ReturnIfPositiveNumber(decimal argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentException($"{argumentName} cannot be negative.", argumentName);
            }

            return argumentValue;
        }
    }
}
