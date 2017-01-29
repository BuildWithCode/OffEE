using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Mod;
using System.Threading;
using System.IO;

namespace Off_EE
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class OffEE : Game
	{
		#region Variables
		public GraphicsDeviceManager
			graphics;

		public SpriteBatch
			spriteBatch;

		public Cache
			EEBlocks = (new Cache(20000)),
			RectangleCache = (new Cache(2000000)),
			EEBlockBitmaps = (new Cache(20000)),
			CustomModDrawnObjects = (new Cache(20000)),
			CacheGetTexture = (new Cache(20000)),
			CustomModDrawinObjectsLocation = (new Cache(20000));

		public ResourceManager
			Resources;

		public Color
			AuraColor;

		public List<string>
			Chat = new List<string>();

		public List<int>
			ValidBlocks = new List<int>();

		public SpriteFont
			ChatFont,
			VisitorFont;

		public Point
			OutlinePosition = new Point(0, 0),
			SmileyPosition = new Point(20 * 16, 16 * 16);

		public int
			Yvel = 0,
			Xvel = 0,
			BlockUsing = 9,
			MapRenderX = 0,
			MapRenderY = 0,
			WorldWidth = 100, //40
			WorldHeight = 100, //32
			xPlace2 = 0,
			yPlace2 = 0;

		public HotBar
			Hotbar;

		public World
			World;

		public Physics
			player = new Physics();

		public StreamWriter
			LogFile;

        public Mod.IMod
            LastSysReq,
            LastPermReq;

		public string
			LogName = "";

		public bool
			CanRun = true,
			IteratingThroughCustomModObjects = false,
			rightJustPressed = false;
		#endregion

		#region Cacheing
		/// <summary>
		/// Get a block, and maybe store it in the cache
		/// </summary>
		/// <param name="Id">The Id of the block</param>
		/// <param name="x">The X</param>
		/// <param name="y">The Y</param>
		/// <param name="temp"></param>
		/// <returns></returns>
		public Texture2D GetBlock(int Id, int x, int y, bool temp = false)
		{
			if (temp)
				return GetTexture(GraphicsDevice, (System.Drawing.Bitmap)Resources.AllBlocks.GetImage.BitPic.Clone(new System.Drawing.Rectangle(Id * 16, 0, 16, 16), Resources.AllBlocks.GetImage.BitPic.PixelFormat));

			//If it doesn't contain the Id
			if (!EEBlocks.ContainsKey(Id))
			{
				try //EEBlocks might be too full.
				{
					EEBlocks.Store(Id, GetTexture(GraphicsDevice, (System.Drawing.Bitmap)Resources.AllBlocks.GetImage.BitPic.Clone(new System.Drawing.Rectangle(Id * 16, 0, 16, 16), Resources.AllBlocks.GetImage.BitPic.PixelFormat)));
				}
				catch (OutOfMemoryException e)
				{
					Chat.Add("* SYSTEM > ERROR: OUT OF MEMORY. DISABLING ALL MODS, AND RESTARTING. ( " + e.Message + " )");
					Mod.ModLoader.StopMods(this);
					EEBlocks.Clean();

					EEBlocks.Store(0, GetTexture(GraphicsDevice, (System.Drawing.Bitmap)Resources.AllBlocks.GetImage.BitPic.Clone(new System.Drawing.Rectangle(0 * 16, 0, 16, 16), Resources.AllBlocks.GetImage.BitPic.PixelFormat)));

					return ((Texture2D)(EEBlocks.Get(0)));
				}
			}

			//Return it
			var ret = ((Texture2D)(EEBlocks.Get(Id)));
			return ret;
		}

		/// <summary>
		/// Get the bitmap of a block
		/// </summary>
		/// <param name="Id">The Id of the block</param>
		/// <param name="x">The X</param>
		/// <param name="y">The Y</param>
		/// <param name="temp"></param>
		/// <returns></returns>
		public System.Drawing.Bitmap GetBlockBitmap(int Id, int x, int y, bool temp = false)
		{
			if (temp)
			{
				return (System.Drawing.Bitmap)Resources.AllBlocks.GetImage.BitPic.Clone(new System.Drawing.Rectangle(Id * 16, 0, 16, 16), Resources.AllBlocks.GetImage.BitPic.PixelFormat);
			}
			else
			{
				if (!EEBlockBitmaps.ContainsKey(Id))
				{
					try
					{
						EEBlockBitmaps.Store(Id, (System.Drawing.Bitmap)Resources.AllBlocks.GetImage.BitPic.Clone(new System.Drawing.Rectangle(Id * 16, 0, 16, 16), Resources.AllBlocks.GetImage.BitPic.PixelFormat));
					}
					catch (OutOfMemoryException e)
					{
						Chat.Add("* SYSTEM > ERROR: OUT OF MEMORY. DISABLING ALL MODS, AND RESTARTING. ( " + e.Message + " )");
						Mod.ModLoader.StopMods(this);
						EEBlockBitmaps.Clean();

						EEBlockBitmaps.Store(0, (System.Drawing.Bitmap)Resources.AllBlocks.GetImage.BitPic.Clone(new System.Drawing.Rectangle(0 * 16, 0, 16, 16), Resources.AllBlocks.GetImage.BitPic.PixelFormat));

						return ((System.Drawing.Bitmap)(EEBlockBitmaps.Get(0)));
					}
				}
				return ((System.Drawing.Bitmap)(EEBlockBitmaps.Get(Id)));
			}
		}

		/// <summary>
		/// Get the rectangle of a block
		/// </summary>
		/// <param name="X">The X</param>
		/// <param name="Y">The Y</param>
		/// <param name="UsingSmiley"></param>
		/// <param name="customWidth"></param>
		/// <param name="customHeight"></param>
		/// <returns></returns>
		public Rectangle GetRectangle(int X, int Y, bool UsingSmiley = false, int customWidth = 16, int customHeight = 16)
		{

			int Width = customWidth,
				Height = customHeight;

			if (UsingSmiley) { Width = 26; Height = 26; }

			string key = X.ToString() + "::" + Y.ToString() + "::" + Width.ToString() + "::" + Height.ToString();

			if (!RectangleCache.ContainsKey(key))
				RectangleCache.Store(key, new Rectangle(X, Y, Width, Height));

			return ((Rectangle)(RectangleCache.Get(key)));
		}

		/// <summary>
		/// Clean the cache
		/// </summary>
		public void CleanCache()
		{
			CacheGetTexture.Clean();
			EEBlocks.Clean();
			EEBlockBitmaps.Clean();
			RectangleCache.Clean();
			Chat.Clear();
		}
		#endregion

		#region Bitmap Testing
		/// <summary>
		/// Make sure two bitmaps are the same
		/// </summary>
		/// <param name="bmp1"></param>
		/// <param name="bmp2"></param>
		/// <returns></returns>
		public bool BitmapsSame(System.Drawing.Bitmap bmp1, System.Drawing.Bitmap bmp2)
		{
			// http://codereview.stackexchange.com/questions/39980/determining-if-2-images-are-the-same
			bool equals = true;
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp1.Width, bmp1.Height);
			System.Drawing.Imaging.BitmapData bmpData1 = bmp1.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp1.PixelFormat);
			System.Drawing.Imaging.BitmapData bmpData2 = bmp2.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp2.PixelFormat);
			unsafe
			{
				byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
				byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
				int width = rect.Width * 3; // for 24bpp pixel data
				for (int y = 0; equals && y < rect.Height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						if (*ptr1 != *ptr2)
						{
							equals = false;
							break;
						}
						ptr1++;
						ptr2++;
					}
					ptr1 += bmpData1.Stride - width;
					ptr2 += bmpData2.Stride - width;
				}
			}
			bmp1.UnlockBits(bmpData1);
			bmp2.UnlockBits(bmpData2);

			return equals;
		}

		/// <summary>
		/// Get the texture2d of a bitmap
		/// </summary>
		/// <param name="graphicsDevice"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Texture2D GetTexture(GraphicsDevice graphicsDevice, System.Drawing.Bitmap b)
		{
			Texture2D tx = null;
			using (System.IO.MemoryStream s = new System.IO.MemoryStream())
			{
				b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
				s.Seek(0, System.IO.SeekOrigin.Begin);
				tx = Texture2D.FromStream(graphicsDevice, s);
			}
			return tx;
		}
		#endregion

		#region Console Logging
		/// <summary>
		/// Log something
		/// </summary>
		/// <param name="p"></param>
		public void Log(string p)
		{
			LogFile.Write(p);
			Console.Write(p);
		}

		public void LogLine(string p)
		{
			Log(p + "\r\n");
		}
		#endregion

		#region Game Initialization
		/// <summary>
		/// Game Initialization
		/// </summary>
		public OffEE()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			BitmapToTexture2d.graphicsDevice = GraphicsDevice;

			LogName = DateTime.Now.ToString().Replace('\\', '-').Replace('/', '-').Replace(':', '-');
			LogName += ".txt";

			bool Complete = false;
			while (!Complete)
			{
				try
				{
					LogFile = new StreamWriter(@"logs\" + LogName);
					Console.Write("XX");
					Complete = true;
				}
				catch { }
			}

			//Set the screen size

			LogLine("Setting screen size...");

			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = 850;
			graphics.PreferredBackBufferHeight = 500;
			graphics.ApplyChanges();

			if (!File.Exists(@"data\world.txt"))
			{
				LogLine("Creating world ( " + WorldWidth + ", " + WorldHeight + " )");

				World = new World(WorldWidth, WorldHeight); //Create the world
			}
			else
			{
				LogLine("Loading world...");

				World = new World(1, 1);

				World.Load(@"data\world.txt");
			}
			Mod.ModLoader.LoadMods(this); //Let's load all the mods. This will be the first instruction Mod has heard, and will take me as the owner.

			Mod.ServiceHandler.ChangeSize(this, WorldWidth, WorldHeight);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			//Initiate Variables
			BitmapToTexture2d.graphicsDevice = GraphicsDevice;
			Resources = new ResourceManager();
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//Load Spritefonts
			LogLine("Loading spritefonts...");
			ChatFont = this.Content.Load<SpriteFont>("chat");
			VisitorFont = this.Content.Load<SpriteFont>("visitor");

			//Load Hotbar
			LogLine("Loading Hotbar...");
			Hotbar = new HotBar();

			//Load Aura
			#region Aura Loading
			LogLine("Loading Aura...");
			AuraColor = Color.White;
			#region Color Checking
			switch (Resources.AuraFile.GetData[1])
			{
				case "white":
					AuraColor = Color.White;
					break;
				case "black":
					AuraColor = Color.Black;
					break;
				case "brown":
					AuraColor = Color.Brown;
					break;
				case "orange":
					AuraColor = Color.Orange;
					break;
				case "red":
					AuraColor = Color.Red;
					break;
				case "yellow":
					AuraColor = Color.Yellow;
					break;
				case "green":
					AuraColor = Color.Green;
					break;
				case "cyan":
					AuraColor = Color.Cyan;
					break;
				case "blue":
					AuraColor = Color.Blue;
					break;
			}
			#endregion
			#endregion

			LogLine("Checking valid/invalid blocks...");

			//If the resources are valid
			#region V/I Blocks
			#region Read
			if (Resources.Valid != null)
			{
				LogLine("Reading valid/invalid blocks...");
				foreach (string i in Resources.Valid.GetData)
				{
					int x;
					if (Int32.TryParse(i, out x))
					{
						ValidBlocks.Add(x);
						Log("V: " + i);
					}
				}
			}
			#endregion
			else
			#region Create Resources
			{
				//We need to create it
				LogLine("No default file found for valid/invalid blocks.");
				LogLine("Initializing valid blocks... ( V = Valid, I = Invalid )");

				//0 is always a valid block
				ValidBlocks.Add(0);

				//Variables
				int CursorPos = Console.CursorTop,
					WidthOfFile = Resources.AllBlocks.GetImage.BitPic.Width / 16;

				//Temporary Blocks
				System.Drawing.Bitmap Block0 = GetBlockBitmap(0, 0, 0);
				System.Drawing.Bitmap Tmp = null;

				for (int i = 1; i < WidthOfFile; i++)
				{
					Console.SetCursorPosition(0, CursorPos);

					Log("[ " + i + " / " + WidthOfFile + " ] ");

					Tmp = GetBlockBitmap(i, 0, 0, true);
					if (BitmapsSame(Tmp, Block0))
						Log("I: " + i + " ");
					else
					{
						Log("V: " + i + " ");
						ValidBlocks.Add(i);
					}
				}
				//Dispose these
				Tmp.Dispose();
				Block0.Dispose();

				LogLine("Saving valid blocks...");

				List<string> ToFile = new List<string>();

				foreach (int i in ValidBlocks)
				{
					ToFile.Add(i.ToString());
				}

				File.WriteAllLines(@"data\valid.txt", ToFile.ToArray());

				ToFile.Clear();

				LogLine("Blocks saved.");
			}
			#endregion
			#endregion

			CleanCache(); //We want a fresh start for our game.

			LogLine("");
			LogLine("Creating world...");

			#region Create World
			if (!File.Exists(@"data\world.txt"))
			{
				for (int x = 0; x < World.Width; x++) //Place all the blocks in the world
					for (int y = 0; y < World.Height; y++)
					{
						int id = 0;
						if (x == 0 || y == 0 ||
							x == World.Width - 1 || y == World.Height - 1)
							id = 9;
						World.PlaceBlock(0, x, y, id, "SYSTEM", false);
						if(id != 9)
						World.PlaceBlock(1, x, y, id, "SYSTEM", false);
					}
			}
			#endregion

			IsMouseVisible = true;
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			CleanCache();
		}
		#endregion

		#region Game Updates
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			var state = Keyboard.GetState();
			var cstate = Mouse.GetState();

			int
				xOffset = player.PlayerLocation.X,
				yOffset = player.PlayerLocation.Y;

			Mod.Events.FireOnTick(this);

			player.Tick();

            #region Enable/Disable System Priviledged Mods

            if (Mod.ModLoader.HasSystemRequests)
            {
                IMod i = Mod.ModLoader.SystemRequests()[0];

                if (LastSysReq != i)
                {
                    LastSysReq = i;
                    ServiceHandler.Chat("CAUTION: The mod '" + i.Name() + "' by '" + i.Author() + @"' requests system priviledges. System Priviledges allows mods to: Record Key strokes ( if you're in focus of the application ) Edit raw world data Draw any picture at any location of the game screen And not only that, but malicious mods can spread their system rights across to any other mods, so if you enable system rights for just that mod, think again. Press 'Y' to enable, 'N' to disable.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                }

                if ((state.IsKeyDown(Keys.Y) ||
                    state.IsKeyDown(Keys.N)) &&
                    CanRun)
                {
                    if (state.IsKeyDown(Keys.Y))
                    {
                        i.SystemPriviledgesGiven();
                        Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)) = new Service(true, i);

                        ServiceHandler.Chat("System Priviledges Granted.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                    }
                    else if (state.IsKeyDown(Keys.N))
                    {
                        i.SystemPriviledgesDenied();

                        ServiceHandler.Chat("System Priviledges Denied.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                    }

                    i.ReInit(Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));

                    CanRun = false;

                    System.Threading.Tasks.Task.Factory.StartNew((() =>
                    {
                        while (state.IsKeyDown(Keys.Y) || state.IsKeyDown(Keys.N)) { } //Wait for the user to quit pressing y/n
                        CanRun = true;
                    }));

                    Mod.ModLoader.SystemRequests().RemoveAt(0);
                }
            }

            #endregion

            #region Enable/Disable Custom Permissions Mods

            if (Mod.ModLoader.HasPermissionRequests)
            {
                ModPermissionRequest p = Mod.ModLoader.PermissionRequests()[0];
                IMod i = p.Mod;
                string req = p.Request,
                    answermsg = "";

                bool LegalRun = true;

                if (LastPermReq != i)
                {
                    LastPermReq = i;
                    if (req == "Chat Disguise")
                    {
                        answermsg = "Chat Disguise Access";
                        ServiceHandler.Chat("CAUTION: The mod '" + i.Name() + "' by '" + i.Author() + "' is requesting chat disguising permissions. This allows the mod to talk in chat with a name other than the mod's name. They could even chat as SYSTEM (with no power tho). Please think if you will accept or not. Press 'Y' to accept, 'N' to deny.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                    }
                    else
                    {
                        LegalRun = false;
                        ServiceHandler.Chat("The mod '" + i.Name() + "' by '" + i.Author() + "' has requested an invalid permission. (" + req + "). This request will be skipped.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                    }
                }

                if ((state.IsKeyDown(Keys.Y) ||
                    state.IsKeyDown(Keys.N)) &&
                    CanRun && LegalRun)
                {
                    if (state.IsKeyDown(Keys.Y))
                    {
                        i.PermissionGranted(req);
                        Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)).Permissions.Add(req);

                        ServiceHandler.Chat(answermsg + " Granted.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                    }
                    else if (state.IsKeyDown(Keys.N))
                    {
                        i.PermissionDenied(req);

                        ServiceHandler.Chat(answermsg + " Denied.", Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));
                    }

                    i.ReInit(Mod.ModLoader.GetSystemService(this, Mod.ModLoader.GetSystem(this)));

                    CanRun = false;

                    System.Threading.Tasks.Task.Factory.StartNew((() =>
                    {
                        while (state.IsKeyDown(Keys.Y) || state.IsKeyDown(Keys.N)) { } //Wait for the user to quit pressing y/n
                        CanRun = true;
                    }));

                    Mod.ModLoader.PermissionRequests().RemoveAt(0);
                }
            }

            #endregion

			foreach (var i in state.GetPressedKeys())
			{
				var k = i.ToString().ToLower();

				Mod.Events.FireKeyStroke(this, new KeyStrokeArgs() { Key = Convert.ToChar((i.ToString().Length > 1 ? (i.ToString().ToLower() == "space" ? " " : i.ToString().ToCharArray()[0].ToString()) : i.ToString())) });
			}

			if (state.IsKeyDown(Keys.Escape))
				Exit();

			#region Hotbar Selector

			int HotbarSet = -1;

			if (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
				HotbarSet = 0;
			if (state.IsKeyDown(Keys.D0))
				HotbarSet = 10;
			if (state.IsKeyDown(Keys.D1))
				HotbarSet = 1;
			if (state.IsKeyDown(Keys.D2))
				HotbarSet = 2;
			if (state.IsKeyDown(Keys.D3))
				HotbarSet = 3;
			if (state.IsKeyDown(Keys.D4))
				HotbarSet = 4;
			if (state.IsKeyDown(Keys.D5))
				HotbarSet = 5;
			if (state.IsKeyDown(Keys.D6))
				HotbarSet = 6;
			if (state.IsKeyDown(Keys.D7))
				HotbarSet = 7;
			if (state.IsKeyDown(Keys.D8))
				HotbarSet = 8;
			if (state.IsKeyDown(Keys.D9))
				HotbarSet = 9;

			if (HotbarSet != -1)
				Hotbar.SelectSlot(HotbarSet);

			#endregion

			int x = cstate.Position.X;
			int y = cstate.Position.Y;

			//Mouse inside of building frame
			if (x < 0)
				x = 0;

			if (x >= 850)
				x = 850;

			if (y < 0)
				y = 0;

			if (y >= 500)
				y = 500;

			OutlinePosition.X = (x - xOffset) / 16;
			OutlinePosition.Y = (y - yOffset) / 16;

			//Mouse inside of world

			if (OutlinePosition.X < 0)
				OutlinePosition.X = 0;

			if (OutlinePosition.Y < 0)
				OutlinePosition.Y = 0;

			if (OutlinePosition.X > WorldWidth - 1)
				OutlinePosition.X = WorldWidth - 1;

			if (OutlinePosition.Y > WorldHeight - 1)
				OutlinePosition.Y = WorldHeight - 1;

			int
				x2 = OutlinePosition.X,
				y2 = OutlinePosition.Y;

			xPlace2 = x2;
			yPlace2 = y2;

			if (x2 > World.Width - 1)
				x2 = World.Width - 1;

			if (y2 > World.Height - 1)
				y2 = World.Height - 1;

			if (cstate.RightButton != ButtonState.Pressed && rightJustPressed)
			{
				rightJustPressed = false;
			}

			if (cstate.LeftButton == ButtonState.Pressed || cstate.RightButton == ButtonState.Pressed)
			{
				if (x > 178 && x < 180 + (Hotbar.Length * 16) &&
					y > 480 && y < 498)
				{
					x = x - 179;
					x = x - (x % 16);
					x = x / 16;

					if (x == 11)
						x--;

					if (cstate.RightButton == ButtonState.Pressed && !rightJustPressed)
					{
						Hotbar.SetSlot(x, Hotbar.GetSlot(x) + 1);
						rightJustPressed = true;
						Hotbar.SelectSlot(x);
					}
					else if (cstate.RightButton != ButtonState.Pressed && rightJustPressed)
					{
						rightJustPressed = false;
						Hotbar.SelectSlot(x);
					}
					else if (cstate.RightButton != ButtonState.Pressed && cstate.LeftButton == ButtonState.Pressed)
					{
						rightJustPressed = false;
						Hotbar.SelectSlot(x);
					}

					BlockUsing = Hotbar.GetSlot(x);
				}
				else if (cstate.RightButton != ButtonState.Pressed)
				{
					World.PlaceBlock(0, x2, y2, BlockUsing, "User");
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			var cstate = Mouse.GetState();

			HandleTasks.Handle(Mod.ServiceHandler.Tasks(this));

			Texture2D fg = null, bg = null;
			Rectangle BarRect = new Rectangle(0, 470, Resources.Bar.GetImage.MonoPic.Width, Resources.Bar.GetImage.MonoPic.Height);
			spriteBatch.Begin();

			int
				xOffset = player.PlayerLocation.X,
				yOffset = player.PlayerLocation.Y;

			Blocks.Animation.AdvanceAnimationTicks();

			for (int y = 0; y < World.Height; y++)
				for (int x = 0; x < World.Width; x++)
				{
					//Only render what's on screen
					if (!(x * 16 + xOffset > (850 - 211) || y * 16 + yOffset > (850 - 211)) &&
						!(x * 16 + xOffset < -16 || y * 16 + yOffset < -16))
					{
						bg = GetBlock(Convert.ToInt32(World.GetBlock(1, x, y)), x, y);
						fg = GetBlock(Convert.ToInt32(World.GetBlock(0, x, y)), x, y);
						if (bg != null)
						{
							if (Blocks.Animation.IsAnimationBlock(Convert.ToInt32(World.GetBlock(1, x, y))))
							{
								bg = Blocks.Animation.GetCurrentAnimation(Convert.ToInt32(World.GetBlock(1, x, y)));
							}
							spriteBatch.Draw(
								bg,
								GetRectangle((x * 16) + xOffset, (y * 16) + yOffset),
								Color.White
								);
						}
						if (fg != null)
						{
							if (Blocks.Animation.IsAnimationBlock(Convert.ToInt32(World.GetBlock(0, x, y))))
							{
								fg = Blocks.Animation.GetCurrentAnimation(Convert.ToInt32(World.GetBlock(0, x, y)));
							}
							spriteBatch.Draw(
								fg,
								GetRectangle((x * 16) + xOffset, (y * 16) + yOffset),
								Color.White
								);
						}
					}
				}

			//Draw chat
			spriteBatch.Draw(Resources.Chat.GetImage.MonoPic, GetRectangle(850 - 211, 0, false, 211, 500), Color.Black);

			//write in sidebar
			int ChatYIncrease = 0;

			foreach (string inn in Chat)
			{
				string i = inn;
				if (i.Length > 34)
					while (i.Length > 34)
					{
						spriteBatch.DrawString(ChatFont, i.Substring(0, 34), new Vector2(645, ChatYIncrease), Color.White);
						ChatYIncrease += 9;
						i = i.Substring(34);
					}
				spriteBatch.DrawString(ChatFont, i, new Vector2(645, ChatYIncrease), Color.White);

				ChatYIncrease += 9;
			}

			while (ChatYIncrease > 490)
			{
				if (Chat.Count > 0)
				{
					Chat.RemoveAt(0);
					ChatYIncrease -= 9;
				}
			}


			spriteBatch.DrawString(ChatFont, "x: " + player.PlayerLocation.X + ", y: " + player.PlayerLocation.Y, new Vector2(10, 10), Color.White);

			spriteBatch.Draw(Resources.Outline.GetImage.MonoPic, GetRectangle(
				(xPlace2 * 16) + Convert.ToInt32(player.x)
				,
				(yPlace2 * 16) + Convert.ToInt32(player.y)
				), new Color(255, 255, 255, 0)); //Block Outline

			spriteBatch.Draw(Resources.Aura.GetImage.MonoPic, GetRectangle(
				320 - 24 + Convert.ToInt32(player.speedX * 1.998),
				250 - 24 + Convert.ToInt32(player.speedY * 1.998), false, 64, 64), AuraColor); //Aura

			spriteBatch.Draw(Resources.Smiley.GetImage.MonoPic, GetRectangle(
				320 - 5 + Convert.ToInt32(player.speedX * 1.998),
				250 - 5 + Convert.ToInt32(player.speedY * 1.998), true), Color.White); //Smiley
			
			//Hotbar

			spriteBatch.Draw( //Draw the fake hotbar
				Resources.Bar.GetImage.MonoPic,
				BarRect,
				Color.White);

			for (int i = 0; i < Hotbar.Length; i++) //Draw every block in the hotbar
			{
				//-2147483648
				spriteBatch.Draw(
					GetBlock(Hotbar.GetSlot(i), 0, 0),
					GetRectangle(179 + (i * 16), 481),
					Color.White);
			}

			spriteBatch.Draw( //Draw the outline on the hotbar for the selected block
				Resources.Outline.GetImage.MonoPic,
				GetRectangle(179 + (Hotbar.SelectedSlot * 16), 481),
				Color.White);

			//amount
			if (CustomModDrawnObjects.Count > 0) //For ever customly drawn mod object
			{
				IteratingThroughCustomModObjects = true;
				foreach (string i in CustomModDrawnObjects.GetKeys())
				{
					Texture2D TmpDraw = ((Texture2D)(CustomModDrawnObjects.Get(i)));
					Point drawAt = ((Point)(CustomModDrawinObjectsLocation.Get(i)));

					spriteBatch.Draw(TmpDraw,
						GetRectangle(drawAt.X, drawAt.Y, false, TmpDraw.Width, TmpDraw.Height),
						Color.White
						);
				}
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
		#endregion
	}
}
