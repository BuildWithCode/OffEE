using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mod
{
	public class MasterException : Exception
	{
		public MasterException() : base("You are not the master.")
		{

		}
	}
}
