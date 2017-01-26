using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mod;
using Mod.CustomPoint;

namespace super_test_mod
{
	public class test : IMod
	{
		public string Version() { return "1.0.0"; }
		public string Author() { return "mod author"; }
		public string ModName() { return "mod"; }

		public bool HasSystemPrivledges = false;
		public Service s;
		
		//We don't need system privledges
		public bool RequestSystemPrivledges() { return true; }
		public void SystemPrivledgesGiven(Service systemPrivledgesGiven) { s = systemPrivledgesGiven; HasSystemPrivledges = true; }
		public void SystemPrivledgesDenied() {

			s = new Service(this);
			s.Chat( "y u no sys privledge);" );
		}

		public void LoadCustomItems() {  } //Not programmed yet

		public void Stop() {  }

		public void Init()
		{
			Events.OnBlockPlaced += Event_BlockPlaced;
		}

		private void Event_BlockPlaced(BlockPlacedArgs e)
		{
			s.PlaceBlock(e.layer, e.location.X, e.location.Y, e.id + 1);
			if(HasSystemPrivledges)
			{
				s.Chat("ha im system");
			}
		}

	}
}
