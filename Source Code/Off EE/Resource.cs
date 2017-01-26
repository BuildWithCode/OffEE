using System.IO;
using System;

namespace Off_EE
{
	public class Resource
	{
		private string _loc;
		private void Throw(string Message)
		{
			Console.WriteLine(Message);
			throw new Exception(Message);
		}

		public Resource(string ResourceLocation)
		{
			if (ResourceLocation != "")
			{
				_loc = ResourceLocation;
				if (!Exists)
				{
					Throw(string.Concat("The resource located in ",
						ResourceLocation,
						"doesn't exist."));
				}
			}
		}

		public bool Exists
		{
			get
			{
				return File.Exists(_loc);
			}
			set { }
		}
	}

	public class ResourceImg : Resource
	{
		private Img _this;

		public ResourceImg(string ResourceLocation, int ImgWidth, int ImgHeight) : base(ResourceLocation)
		{
			_this = new Img(ResourceLocation, ImgWidth, ImgHeight);
		}

		public ResourceImg(System.Drawing.Bitmap img) : base("")
		{
			_this = new Img(img);
		}

		public Img GetImage
		{
			get
			{
				return _this;
			}
			set { }
		}
	}

	public class ResourceText : Resource
	{
		private string _loc;
		
		public ResourceText(string ResourceLocation) : base(ResourceLocation)
		{
			_loc = ResourceLocation;
		}

		public string[] GetData
		{
			get
			{
				if (Exists)
					return File.ReadAllLines(_loc);
				else throw new Exception("The ResourceText specified doesn't exist.");
			}
			set { }
		}
	}
}
