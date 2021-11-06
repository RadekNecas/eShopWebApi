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
    }
}
