using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod
{
	public interface IMod
	{
		string ModName();
		string Author();
		string Version();

		bool RequestSystemPrivledges();

		void SystemPrivledgesGiven(Service service);
		void SystemPrivledgesDenied();

		void LoadCustomItems();
		void Init();
		void Stop();
	}
}
