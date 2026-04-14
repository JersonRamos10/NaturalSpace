using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base()
        {
        }

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
