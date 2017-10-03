using System;
using System.Collections.Generic;
using System.Text;

namespace DynORM.Filters
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
