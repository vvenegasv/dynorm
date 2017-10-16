using System;

namespace DynORM.Exceptions
{
    public class InvalidConverter: Exception
    {
        public InvalidConverter() : base()
        {
            
        }

        public InvalidConverter(string message) : base(message)
        {

        }

        public InvalidConverter(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
