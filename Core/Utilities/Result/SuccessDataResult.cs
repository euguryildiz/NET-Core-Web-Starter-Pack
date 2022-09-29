using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Result
{
    public class SuccessDataResult<T> : DataResult<T>
    {

        public SuccessDataResult(T data, bool succes) : base(data, true)
        {
        }

        public SuccessDataResult(T data, bool succes, string message) : base(data, true, message)
        {
        }
        public SuccessDataResult(T data) : base(data, true)
        {
        }
        public SuccessDataResult(string message) : base(default, true, message)
        {
        }

       
    }
}
