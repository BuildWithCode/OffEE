using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Off_EE
{
	public class World
	{
		/// <summary>
		/// Create a new world
		/// </summary>
		/// <param name="width">The width of the world</param>
		/// <param name="height">The height of the world</param>
		public World(int width, int height)
		{
			_width = width;
			_height = height;
			Blocks = new uint[2, width + 1, height + 1];
		}

		private int _width, _height;

		private uint[, ,] Blocks = new uint[2, 40, 32];

		/// <summary>
		/// The width of the world
		/// </summary>
		public int Width { get { return _width; } private set { _width = value; } }

		/// <summary>
		/// The height of the world
		/// </summary>
		public int Height { get { return _height; } private set { _height = value; } }

		/// <summary>
		/// Load the world from a file
		/// </summary>
		/// <param name="file"></param>
		public void Load(string file)
		{
			var w = new ResourceText(@"data\world.txt");

			string[] datal = w.GetData;
			System.Text.StringBuilder datab = new System.Text.StringBuilder();

			string[] rep = {
								   "\r",
								   "\n",
								   "\n\n"
								};

			foreach (string i in datal)
				datab.Append(i.Replace(rep[0], rep[1]).Replace(rep[2], rep[1]) + "\n");

			string data = datab.ToString();

			bool[] chkmarks = {
								  false //Got WW
								 ,false //Got WH
								 ,false //Found block id
								 ,false //Got pos x
								 ,false // y
							  };

			uint
				BlockId = 0
			;int
				 x = 0
				 ,y = 0

			;
			foreach (string i in data.Split('\n'))
			{
				if (i.Length != 0) {
					if (!chkmarks[0])
					{
						//Get world width
						if (Int32.TryParse(i, out _width))
							chkmarks[0] = true;
					}
					else
					{
						if (!chkmarks[1])
						{
							//Get world height
							if (Int32.TryParse(i, out _height))
								chkmarks[1] = true;

							Blocks = new uint[2, _width, _height];
						}
						else
						{
							if (i.Substring(0, 1) == "b" &&
								chkmarks[3] == false)
								chkmarks[2] = false;
							if (!chkmarks[2])
							{
								//Store block id
								if (i.Substring(0, 1) == "b")
								{
									if (UInt32.TryParse(i.Substring(1), out BlockId))
										chkmarks[2] = true;
								}
							}
							else
							{
								if (!chkmarks[3])
								{
									//Store x
									if (Int32.TryParse(i, out x))
									chkmarks[3] = true;
								}
								else
								{
									if (chkmarks[4])
									{
										int bid = 0;

										if (Int32.TryParse(i, out bid))
										{
											if (bid == 0 || bid == 1)
											{
												if (x < Width && y < Height)
												{
													Blocks[bid, x, y] = BlockId;
												}
												chkmarks[4] = false;
												chkmarks[3] = false;
											}
										}
									}
									else
									{
										//Store y
										if (Int32.TryParse(i, out y))
											chkmarks[4] = true;
									}
								}
							}
						}
					}
				}
			}
		}

		public void Save(string file)
		{
			string save = string.Concat(
				Width
				, "\n"
				, Height
				, "\n");

			bool wrote = false;

			//For each valid block
			foreach(var b in Program.game.ValidBlocks)
			{
				if(b != 0)
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						//Check if on the location is that block

						for (int l = 0; l < 2; l++)
						{
							if (Blocks[l, x, y] == b)
							{
								if (!wrote)
								{
									save = string.Concat(save, "b", b, "\n");
									wrote = true;
								}

								save = string.Concat(save
									, x
									, "\n"
									, y
									, "\n"
									, l
									, "\n");
							}
						}
					}
				}
				wrote = false;
			}

			System.IO.File.WriteAllText(@"data\world.txt", save);
		}

		public void PlaceBlock(int layer, int x, int y, int id, string AssemblyFrom, bool FireEvent = true)
		{
			//The system is the most powerful block placer
			if (!Program.game.ValidBlocks.Contains(id))
			{
				if (AssemblyFrom != "SYSTEM")
				{
					throw new Exception("The block Id isn't a valid block as determined in game files.");
				}
			}

			if (GetBlock(layer, x, y) != id)
			{
				if (id < 0)
					throw new ArgumentException("Blocks less than 0 are not allowed.");

				Blocks[layer, x, y] = Convert.ToUInt32(id);

				if (FireEvent)
				{
					var e = new Mod.BlockPlacedArgs();

					e.layer = layer;
					e.location = new Mod.CustomPoint.Point(x, y);
					e.id = id;
					e.From = AssemblyFrom;

					Mod.Events.FireBlockPlaced(Program.game, e);
				}
			}
		}

		public int GetBlock(int layer, int x, int y)
		{
			if (x < 0 || y < 0 || x > Width || y > Height)
				throw new ArgumentException("Location out of bounds.");

			if (layer < 0 || layer > 1)
				throw new ArgumentException("Layer has to be either 0 (foreground) or 1 (background)");

			return Convert.ToInt32(Blocks[layer, x, y]);
		}
	}
}
