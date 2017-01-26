using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Off_EE.Mod;

namespace test_mod
{
    public class TestMod : IMod
    {
		public void Init()
		{
			Console.WriteLine("hello world from testmod");
			while(true)
			{
				Console.WriteLine("hello world from testmod");
			}
		}
    }
}
