using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod
{
	public interface IMod
	{
        string Name();
        string Author();
        string Version();

        bool RequestSystemPriviledges();
        bool RequestsChatDisguise();

		void SystemPriviledgesGiven();
		void SystemPriviledgesDenied();
        void PermissionGranted(string perm);
        void PermissionDenied(string perm);

		void LoadCustomItems();
		void Init(Service serv);
        void ReInit(Service serv);
		void Stop();
	}
}
