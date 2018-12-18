using System;
using System.Collections.Generic;
using System.Text;

namespace Fotron.Common
{
    public class CustomException : Exception
    {

        public CustomException(string message) : base(message)
        {

        }
    }
}
