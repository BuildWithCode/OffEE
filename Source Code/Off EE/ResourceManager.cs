using System;
namespace Off_EE
{
	//We use a public class so we can run this code when desired.

	/// <summary>
	/// Resource Manager to manage our resources
	/// </summary>
	public class ResourceManager
	{
		public ResourceImg
			AllBlocks,
			Bar,
			Outline,
			Smiley,
			Aura,
			Chat;

		public ResourceText
			AuraFile,
			Hotbar,
			Valid;

		public ResourceManager()
		{
			string DataLocation = @"data\";
			
			AllBlocks = new ResourceImg(System.IO.Path.Combine(DataLocation, "full.png"), 21728, 16);
			Bar       = new ResourceImg(System.IO.Path.Combine(DataLocation, "bar.png"), 641, 30);
			Outline   = new ResourceImg(System.IO.Path.Combine(DataLocation, "outline.png"), 16, 16);
			Smiley    = new ResourceImg(System.IO.Path.Combine(DataLocation, "cm0.png"), 26, 26);
			Aura      = new ResourceImg(System.IO.Path.Combine(DataLocation, "aura.png"), 64, 64);
			this.Chat = new ResourceImg(System.IO.Path.Combine(DataLocation, "chat.png"), 211, 500);

			AuraFile  = new ResourceText(System.IO.Path.Combine(DataLocation, "aura.txt"));
			Hotbar    = new ResourceText(System.IO.Path.Combine(DataLocation, "hotbar.txt"));

			if (System.IO.File.Exists(System.IO.Path.Combine(DataLocation, "valid.txt")))
				Valid = new ResourceText(System.IO.Path.Combine(DataLocation, "valid.txt"));
			else
				Valid = null;
		}
	}
}
