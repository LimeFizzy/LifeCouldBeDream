using System;

namespace API.Exceptions
{
    public class WeakPasswordException : Exception
    {
        public WeakPasswordException() : base("The provided password is too weak.")
        {
        }

        public WeakPasswordException(string message) : base(message)
        {
        }
    }
}
