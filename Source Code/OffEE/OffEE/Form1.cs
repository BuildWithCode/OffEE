using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace OffEE
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		private void InitGame() //Initialize, Load, and start a physics thread.
		{
			Bitmap empty = new Bitmap(640, 509);
			empty = Overlap(empty, new Bitmap("b0.png"), 16, 16);
			pictureBox1.BackgroundImage = empty;
			pictureBox1.Image = empty;
			pictureBox1.Image = Overlap(empty, new Bitmap("bl.png"), 0, 480);
			pictureBox1.Image = Overlap(new Bitmap(pictureBox1.Image), new Bitmap("godselect_OFF.png"), 0, 480);
			int width = 39;
			int height = 29;
			for (int i = 0; i <= width; i++)
				for (int n = 0; n >= height; n++)
					map[n, i] = 0;
			pictureBox1.BackgroundImage = new Bitmap(@"Worlds\blank\world.png");
			string[] blankworld = System.IO.File.ReadAllLines(@"Worlds\blank\blank.wrld");
			foreach (string i in blankworld)
			{
				try
				{
					int x = Convert.ToInt32(i.Split(':')[0].Split(',')[0]);
					int y = Convert.ToInt32(i.Split(':')[0].Split(',')[1]);
					int id = Convert.ToInt32(i.Split(':')[1]);
					map[x, y] = id;
				}
				catch
				{

				}
			}
			firess = new System.Threading.Thread(delegate() { PlayerRenderer(); });
			firess.Start();
		}
		#region Tools & Variables
		private System.Threading.Thread firess; //Used to fire off the rendering thread
		public static int playerx = 32; //Current player X
		public static int playery = 32; //Current player Y
		public static int oldplayerx = 0; //Old player X - Used for rendering, helpful for increasing speed
		public static int oldplayery = 0; //Old player Y - Used for rendering, helpful for increasing speed
		public static int mx = 0; //The mouse X ( ON THE FORM )
		public static int my = 0; //The mouse Y ( ON THE FORM )
		public static bool down = false; //If the mouse is down
		public static int placing = -1; //The Block ID Being Placed
		public static bool God = true; //In God Mode?
		private int[,] map = new int[40, 30]; //The map in an int[,] list
		public Bitmap Overlap(Bitmap current, Bitmap overlap, int x, int y) //Overlap 2 bitmaps, useful for placing stuff
		{
			using (Graphics g = Graphics.FromImage(current))
			{
				g.DrawImageUnscaled(overlap, x, y);
			}
			return current;
		}
		private void setmap(Bitmap gmap, int x, int y, int id) //Set a block on the map, updating the map[x,y] position and placing image onto game
		{
			map[x, y] = id;
			pictureBox1.BackgroundImage = Overlap(gmap, new Bitmap("b" + id.ToString() + ".png"), x * 16, y * 16);
			if (System.IO.File.Exists("pop.wav") && System.IO.File.Exists("erase.wav") && placing != -1)
				playsound();
		}
		private void playsound() //Play the pop and erase sounds
		{
			SoundPlayer pop = new SoundPlayer("pop.wav");
			SoundPlayer erase = new SoundPlayer("erase.wav");
			if (placing == 0)
				erase.Play();
			if (placing != 0 && placing != -1)
				pop.Play();
		}
		private void PlaceABlock() //Place A Block
		{
			if (down)
			{
				int x = (mx / 16);
				int y = (my / 16);
				try
				{
					if (map[x, y] == 0)
					{
						if (placing == -1)
							placing = 9;
						if (placing == 9)
							setmap(new Bitmap(pictureBox1.BackgroundImage), x, y, 9);
					}
					else if (map[x, y] == 9)
					{
						if (placing == -1)
							placing = 0;
						if (placing == 0)
							setmap(new Bitmap(pictureBox1.BackgroundImage), x, y, 0);
					}
				}
				catch { }
			}
			else
			{
				placing = -1;
			}
		}
		private void ToggleGod()
		{
			God = !God;
			RenderGod();
		}
		private void RenderGod()
		{
			Bitmap pim = new Bitmap(pictureBox1.Image);
			pictureBox1.Invoke((MethodInvoker)(() => pictureBox1.Image = Overlap(pim, new Bitmap("bl.png"), 0, 480)));
			if (God)
			{
				pictureBox1.Invoke((MethodInvoker)(() => pictureBox1.Image = Overlap(pim, new Bitmap("godselect_ON.png"), 0, 480)));
			}
			else
			{
				pictureBox1.Invoke((MethodInvoker)(() => pictureBox1.Image = Overlap(pim, new Bitmap("godselect_OFF.png"), 0, 480)));
			}
		}
		#endregion
		#region Render the Player with another thread at a constant rate so less crashing chance
		public void PlayerRenderer() //Thread
		{
			while (true)
			{
				RenderPlayer();
				if (playery >= 29 * 16)
					playery = 0;
				if (God == false)
					if (map[(playerx / 16), (playery / 16) + 1] == 0) //Gravity
						playery += 16;
				System.Threading.Thread.Sleep(100);
			}
		}
		private void RenderPlayer() //Void
		{
			Image backup = pictureBox1.Image;
			if (!(oldplayery == playery && oldplayerx == playerx))
				try
				{
					Bitmap empty = new Bitmap(640, 509);
					if (God)
						empty = Overlap(empty, new Bitmap("aura.png"), playerx - 24, playery - 24);/**/
					empty = Overlap(empty, new Bitmap("cm0.png"), playerx - 5, playery - 5);
					#region renderGod
					empty = Overlap(empty, new Bitmap("bl.png"), 0, 480);
					if (God)
					{
						empty = Overlap(empty, new Bitmap("godselect_ON.png"), 0, 480);
					}
					else
					{
						empty = Overlap(empty, new Bitmap("godselect_OFF.png"), 0, 480);
					}
					#endregion
					pictureBox1.Invoke((MethodInvoker)(() => pictureBox1.Image = empty));
					oldplayerx = playerx;
					oldplayery = playery;
				}
				catch
				{
					pictureBox1.Invoke((MethodInvoker)(() => pictureBox1.Image = backup));
				}
		}
		#endregion
		private void Form1_Load(object sender, EventArgs e) //When the form loads
		{
			InitGame();
		}
		private void MouseDown(object sender, MouseEventArgs e) //When the left mouse button is down
		{
			down = true;
			if (my >= 480 && down)
			{
				if (mx <= 28)
					ToggleGod();
			}
			else
				PlaceABlock();
		}
		private void MouseMoved(object sender, MouseEventArgs e) //When the mouse moves
		{
			mx = e.X; //Set global variables of the mouse X and Y
			my = e.Y;
			PlaceABlock();
		}
		private void MouseUp(object sender, MouseEventArgs e) //When the left mouse button is released
		{
			down = false;
		}
		private void WhenAKeyIsPressed(object sender, KeyPressEventArgs e) //When a key is pressed
		{
			//Make the player move a full block
			try
			{
				if (God == true || false) // || false is for future gravity dots
					if (e.KeyChar.ToString().ToLower() == "w" && (map[playerx / 16, (playery - 16) / 16] == 0 || God))
						playery -= 16;
				if (e.KeyChar.ToString().ToLower() == "a" && (map[(playerx - 16) / 16, playery / 16] == 0 || God))
					playerx -= 16;
				if (e.KeyChar.ToString().ToLower() == "s" && (map[playerx / 16, (playery + 16) / 16] == 0 || God))
					playery += 16;
				if (e.KeyChar.ToString().ToLower() == "d" && (map[(playerx + 16) / 16, playery / 16] == 0 || God))
					playerx += 16;
				if (e.KeyChar.ToString().ToLower() == "g")
					ToggleGod();
				if (God == false)
					if (e.KeyChar == ' ') //Jumping Physics
						if (map[(playerx / 16), (playery / 16)] == 0 && (map[(playerx / 16), (playery / 16) + 1] != 0 || (map[(playerx / 16) + 1, (playery / 16) + 1] != 0 || map[(playerx / 16) - 1, (playery / 16) + 1] != 0)))
							if (map[(playerx / 16), (playery / 16) - 1] == 0)
								if (map[(playerx / 16), (playery / 16) - 2] == 0)
									if (map[(playerx / 16), (playery / 16) - 3] == 0)
										playery -= 48;
									else
										playery -= 32;
								else
									playery -= 16;
							else
								playery -= 0;
			}
			catch (IndexOutOfRangeException index)
			{

			}
		}
		private void FormIsClosing(object sender, FormClosedEventArgs e) //When the form is closed
		{
			firess.Abort();
			Application.Exit();
		}
	}
}
