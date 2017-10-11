using System;

namespace DynORM.Exceptions
{
    public class ExpressionNotSupportedException: Exception
    {
        public ExpressionNotSupportedException() : base()
        {
            
        }

        public ExpressionNotSupportedException(string message) : base(message)
        {

        }

        public ExpressionNotSupportedException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
