using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod
{
	public class Task
	{
		public Task(string TaskType, params object[] TaskArguments)
		{
			TaskId = TaskType;
			Arguments = TaskArguments;
		}

		public string TaskId { get; private set; }
		public object[] Arguments { get; private set; }
	}
}