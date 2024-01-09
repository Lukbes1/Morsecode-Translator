using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class MorseCharNotFoundException : Exception
	{
		public MorseCharNotFoundException()
		{
		}

		public MorseCharNotFoundException(string message) : base(message)
		{
		}

		public MorseCharNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MorseCharNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
