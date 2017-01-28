using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Mod
{
	class IModSystem : IMod //Custom IMod to send * SYSTEM > <mod loaded> messages.
	{
        public string Name() { return "SYSTEM"; }
        public string Author() { return "SYSTEM"; }
        public string Version() { return "SYSTEM";  }

		Service service = null;

		public bool RequestSystemPriviledges() { return false; }
        public bool RequestsChatDisguise() { return false; }

		public void SystemPriviledgesGiven() { }
		public void SystemPriviledgesDenied() { }
        public void PermissionGranted(string Permission) { }
        public void PermissionDenied(string Permission) { }
		public void LoadCustomItems() { }

        public void Init(Service serv) { this.service = serv; }

        public void ReInit(Service serv) { this.service = serv; }

		public void Stop() { }
	}

	public static class ModLoader
	{
		//Define a single master, as to who used the Mod assembly first, to have certain methods only availible to the master.
		internal static object Master = null;

		#region Load Mods
		private static List<IMod> ModsLoaded = null;
		private static ICollection<IMod> LoadMods(string path)
		{
			string[] dllFileNames = null;
			if (Directory.Exists(path))
			{
				dllFileNames = Directory.GetFiles(path, "*.dll");
				ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
				foreach (string dllFile in dllFileNames)
				{
					AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
					Assembly assembly = Assembly.Load(an);
					assemblies.Add(assembly);
				}
				Type ModType = typeof(IMod);
				ICollection<Type> ModTypes = new List<Type>();
				foreach (Assembly assembly in assemblies)
				{
					if (assembly != null)
					{
						try
						{
							Type[] types = assembly.GetTypes();
							foreach (Type type in types)
							{
								if (type.IsInterface || type.IsAbstract)
								{
									continue;
								}
								else
								{
									if (type.GetInterface(ModType.FullName) != null)
									{
										ModTypes.Add(type);
									}
								}
							}
						}catch(Exception e)
						{
							Console.WriteLine(e.Message);
						}
					}
				}

				ICollection<IMod> Mods = new List<IMod>(ModTypes.Count);
				foreach (Type type in ModTypes)
				{
					try
					{
						IMod Mod = (IMod)Activator.CreateInstance(type);
						if (Mod == null) break;
						Mods.Add(Mod);
					}
					catch (Exception e) { Console.WriteLine(e.ToString()); }
				}
				return Mods;
			}
			return null;
		}
		public static void LoadMods(object sender)
		{
			SysReq.Clear();
			if (Directory.Exists(@"mods") && Master == null)
			{
				Master = sender;
				ModsLoaded = new List<IMod>();
				ICollection<IMod> ts = LoadMods(@"mods");
				if (ts != null)
				{
					IModSystem l = new IModSystem();
					Service s = new Service(l);
					foreach (var x in ts)
					{
                        try
                        {
                            IMod i = (IMod)x;
                            ModsLoaded.Add(i);

                            if (i.RequestSystemPriviledges())
                                SysReq.Add(i);
                            if (i.RequestsChatDisguise())
                                PermReq.Add(new ModPermissionRequest(i, "Chat Disguise"));

                            if (!(i.Name() == "SYSTEM" &&
                                i.Author() == "SYSTEM" &&
                                i.Version() == "SYSTEM"))
                            {
                                if (i.Name() == "User" || i.Name() == "SYSTEM")
                                {
                                    ServiceHandler.Chat("WARNING: Mod '" + i.Name() + "' by '" + i.Author() + "' will not be loaded because they're using an invalid ModName ( \"User\", \"SYSTEM\" )", s);
                                }
                                else
                                {
                                    i.Init(s);
                                    ServiceHandler.Chat("Loaded: '" + i.Name() + "' by '" + i.Author() + "'", s);
                                }
                            }
                        }
                        catch { }
					}
				}
			}
		}
		#endregion

		public static bool ModsHaveLoaded = false;

		public static void StopMods(object master)
		{
			if (master != Master)
				throw new MasterException();

			foreach (IMod i in ModsLoaded)
				i.Stop();

			ModsStopped = true;

			ModsHaveLoaded = true;
		}

		public static bool ModsStopped { get; set; }

		private static List<IMod> SysReq = new List<IMod>();
        private static List<ModPermissionRequest> PermReq = new List<ModPermissionRequest>();

		public static List<IMod> SystemRequests()
		{
			return SysReq;
		}

        public static List<ModPermissionRequest> PermissionRequests()
        {
            return PermReq;
        }

        public static bool HasSystemRequests
        {
            get { return SysReq.Count > 0; }
        }

        public static bool HasPermissionRequests
        {
            get { return PermReq.Count > 0; }
        }

		public static IMod GetSystem(object master)
		{
			if (master != Master)
				throw new MasterException();

			return new IModSystem();
		}

		public static Service GetSystemService(object master, IMod giveSystem)
		{
			if (master != Master)
				throw new MasterException();

			return new Service(true, giveSystem);
		}
	}

    public class ModPermissionRequest
    {
        private string request = "";
        private IMod mod;

        public string Request { get { return request; } }
        public IMod Mod { get { return mod; } }

        public ModPermissionRequest(IMod mod, string request)
        {
            this.mod = mod;
            this.request = request;
        }
    }
}