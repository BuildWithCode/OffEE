using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Mod
{
	public static class ServiceHandler
	{
		static ServiceHandler()
		{
			Tasks_ = new List<Task>();
		}

		private static bool Busy = false;
		private static List<Task> Tasks_;
		internal static uint[, ,] WorldData = new uint[2, 1, 1];

		public static void ChangeSize(object sender, int width, int height)
		{
			if (sender != ModLoader.Master)
				throw new MasterException();

			WorldData = new uint[2, width, height];
		}

		public static List<Task> Tasks(object sender)
		{
			if (sender != ModLoader.Master)
				throw new MasterException();

			return Tasks_;
		}

		public static void SetWorldData(uint[,,] worldData, object master)
		{
			if(master != ModLoader.Master)
				throw new MasterException();

			WorldData = worldData;
		}

		public static void SetBusy(bool value, object sender)
		{
			if (sender != ModLoader.Master)
				throw new MasterException();
			Busy = value;
		}
		
		public static void SetHotbarBlock(int slot, int block, Service User)
		{
			if (!ModLoader.ModsStopped)
				System.Threading.Tasks.Task.Factory.StartNew(() =>
				{
					while (Busy) { /**/ }

					Tasks_.Add(new Task("SetHotbar", slot, block));
				});
		}

        public static void Chat(string msg, Service User, string disguise = "")
        {
            disguise = User.ServiceUser.Name();

            if (User.SystemRights)
            {
                if (!ModLoader.ModsStopped)
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        while (Busy) { /**/ }

                        Tasks_.Add(new Task("Chat", "* " + disguise + " > " + msg));
                    });
            }
            else
                Chat("ERROR: Cannot chat as SYSTEM: No priviledges.", new Service(true, new IModSystem()));
        }

        public static void SystemChat(string msg, Service User)
        {
            if (!ModLoader.ModsStopped)
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    while (Busy) { /**/ }

                    Tasks_.Add(new Task("Chat", "* SYSTEM > " + msg));
                });
        }

		public static void PlaceBlock(int layer, int x, int y, int id, Service User)
		{
			if (!ModLoader.ModsStopped)
				System.Threading.Tasks.Task.Factory.StartNew(() =>
				{
					while (Busy) { /**/ }

					Tasks_.Add(new Task("PlaceBlock", layer, x, y, id, (User.SystemRights ? "SYSTEM" : User.ServiceUser.Name())));
				});
		}

		public static void SelectHotbar(int slot, Service User)
		{
			if (!ModLoader.ModsStopped)
				System.Threading.Tasks.Task.Factory.StartNew(() =>
				{
					while (Busy) { /**/ }

					Tasks_.Add(new Task("SelectHotbar", slot));
				});
		}

		public static void DrawObject(int X, int Y, string Key, string FileName, Service User)
		{
			if (!ModLoader.ModsStopped)
				if (User.SystemRights)
					System.Threading.Tasks.Task.Factory.StartNew(() =>
					{
						while (Busy) { /**/ }

						Tasks_.Add(new Task("DrawObject", X, Y, Key, FileName));
					});
				else
					Chat("ERROR: Cannot draw an object without system rights.", new Service(true, new IModSystem()));
		}

		public static void RemoveObject(string Key, Service User)
		{
			if (!ModLoader.ModsStopped)
				if (User.SystemRights)
					System.Threading.Tasks.Task.Factory.StartNew(() =>
					{
						while (Busy) { /**/ }

						Tasks_.Add(new Task("RemoveObject", Key));
					});
				else
					Chat("ERROR: Cannot remove an object without system rights.", new Service(true, new IModSystem()));
		}

		public static void ClearTasks(object sender)
		{
			if (sender != ModLoader.Master)
				throw new MasterException();

			Tasks_.Clear();
		}

		public static object GetInformation(string p)
		{
			switch (p)
			{
				case "WorldData":
					return WorldData;
			}

			return null;
		}
	}

	public class Service
	{
		internal Service(bool SystemRights_, IMod serviceUser)
		{
			ServiceUser = serviceUser;
			SystemRights = SystemRights_;
		}

		public Service(IMod serviceUser)
		{
			ServiceUser = serviceUser;
		}

		internal bool SystemRights = false;
		internal IMod ServiceUser;
		
		public void SetHotbarBlock(int slot, int block)
		{
			if (!ModLoader.ModsStopped)
				ServiceHandler.SetHotbarBlock(slot, block, this);
		}

		public void Chat(string msg, string disguise = "")
		{
			if (!ModLoader.ModsStopped)
				ServiceHandler.Chat(msg, this, disguise);
		}

        public void SystemChat(string msg)
        {
            if (!ModLoader.ModsStopped)
                ServiceHandler.SystemChat(msg, this);
        }

		public void PlaceBlock(int layer, int x, int y, int id)
		{
			if (!ModLoader.ModsStopped)
				ServiceHandler.PlaceBlock(layer, x, y, id, this);
		}

		public void SelectHotbar(int slot)
		{
			if (!ModLoader.ModsStopped)
				ServiceHandler.SelectHotbar(slot, this);
		}

		public void DrawObject(int X, int Y, string Key, string FileName)
		{
			if(!File.Exists(@"data\" + FileName))
				throw new FileNotFoundException(FileName + " was not found in the data folder.");

			ServiceHandler.DrawObject(X, Y, Key, FileName, this);
		}

		public uint[,,] GetWorldData()
		{
			return ((uint[,,])ServiceHandler.GetInformation("WorldData"));
		}

		public void RemoveObject(string Key)
		{
			ServiceHandler.RemoveObject(Key, this);
		}
	}
}
