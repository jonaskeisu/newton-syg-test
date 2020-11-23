using System;

namespace PokerLib
{
    [System.Serializable]
    public class HandException : System.Exception
    {
        public HandException() { }
        public HandException(string message) : base(message) { }
        public HandException(string message, System.Exception inner) : base(message, inner) { }
        protected HandException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}

