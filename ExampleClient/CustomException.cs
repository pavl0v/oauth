using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleClient
{
    public class CustomException : Exception
    {
        private int _errorCode = 0;
        private string _errorReasonPhrase = string.Empty;
        private string _errorContent = string.Empty;

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public string ErrorContent
        {
            get { return _errorContent; }
        }

        public string ErrorReasonPhrase
        {
            get { return _errorReasonPhrase; }
        }

        public CustomException(int errorCode, string errorReasonPhrase, string errorContent)
        {
            _errorCode = errorCode;
            _errorReasonPhrase = errorReasonPhrase;
            _errorContent = errorContent;
        }
    }
}
