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
        public string Author() { return "modauthor"; }
        public string Name() { return "mod"; }

        public bool HasSystemPriviledges = false;

        public Service s;

        //We don't need system priviledges; but let's do them anyways
        public bool RequestSystemPriviledges() { return true; }
        public bool RequestsChatDisguise() { return false; }

        public void SystemPriviledgesGiven() { HasSystemPriviledges = true; }
        public void SystemPriviledgesDenied()
        {
            s = new Service(this);
            s.Chat("y u no sys privledge");
        }

        public void PermissionGranted(string Perm) { }
        public void PermissionDenied(string Perm) { }

        public void LoadCustomItems() { } //Not programmed yet

        public void Stop() { }

        public void Init(Service serv)
        {
            this.s = serv; // Initialize service
            Events.OnBlockPlaced += Event_BlockPlaced;
        }

        public void ReInit(Service serv)
        {

        }

        private void Event_BlockPlaced(BlockPlacedArgs e)
        {
            s.PlaceBlock(e.layer, e.location.X, e.location.Y, e.id + 1);
            if (HasSystemPriviledges)
            {
                s.SystemChat("hoi system priviledge arooay!");
            }
        }
    }
}
