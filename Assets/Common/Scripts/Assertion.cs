using System;

namespace APlusOrFail
{
    public static class Assertion
    {
        public static void AssertNotNull(string name, object value)
        {
            if (value == null)
            {
                throw new NullReferenceException($"{name} cannot be null");
            }
        }

        public static void AssertAssignableTo(Type type, Type super)
        {
            if (!super.IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type} is not assignable to {super}");
            }
        }
    }
}
