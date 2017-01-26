using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Off_EE
{
	/// <summary>
	/// Physics for your player.
	/// </summary>
	public class Physics
	{
		/// <summary>
		/// The location of the player
		/// </summary>
		public Point PlayerLocation = new Point(16, 16);
		//public Point BlockLocation = new Point(1, 1);

		private bool JustGodSwitch = false;

		/// <summary>
		/// If the player is in god mode.
		/// </summary>
		public bool InGodMode = true;

		public double x = 16;
		public double y = 16;

		public double speedX = 0;
		public double speedY = 0;

		public static int speed = 4;

		public void Tick()
		{
			//Our physics are not yet ready for player physics.
			/*
			if (!Keyboard.GetState().IsKeyDown(Keys.G))
			{ JustGodSwitch = false; }

			if (Keyboard.GetState().IsKeyDown(Keys.G) && !JustGodSwitch)
			{ InGodMode = !InGodMode; JustGodSwitch = true; }
			*/

			FixPlayerBounds();

			//Define the vertical & horizontal directions of the player
			double Vertical = 0;
			double Horizontal = 0;

			var state = Keyboard.GetState();

			if (state.IsKeyDown(Keys.Up) ||
				state.IsKeyDown(Keys.W))
				Vertical--;

			if (state.IsKeyDown(Keys.Down) ||
				state.IsKeyDown(Keys.S))
				Vertical++;

			if (state.IsKeyDown(Keys.Left) ||
				state.IsKeyDown(Keys.A))
				Horizontal--;

			if (state.IsKeyDown(Keys.Right) ||
				state.IsKeyDown(Keys.D))
				Horizontal++;
			
			//Invert horizontal/vertical so we move in correct direction.

			Horizontal *= -1;
			Vertical *= -1;

			//Check if we're in god mode, and make sure we can go up

			/*
			if (!InGodMode && Vertical != 0)
			{
				if (!(GetBlockAtLocation() == 4))
					Vertical = 0;
			}
			 */

			//If the speeds are 0, set the correctly.

			if (speedX == 0 && Horizontal != 0)
				speedX = Horizontal;

			if (speedY == 0 && Vertical != 0)
				speedY = Vertical;

			//We are dealing with negitives, keep that in mind for our math

			if (Horizontal > 0)
				speedX = (speedX < 0 ? speedX += (speedX * -1) / 4 + 0.1 : speedX *= Horizontal + 0.4);

			if (Vertical > 0)
				speedY = (speedY < 0 ? speedY += (speedY * -1) / 4 + 0.1 : speedY *= Vertical + 0.4);

			if (Horizontal < 0)
				speedX = (speedX < 0 ? speedX *= (Horizontal - 0.4) * -1 : speedX += (speedX * -1) / 4 - 0.1);

			if (Vertical < 0)
				speedY = (speedY < 0 ? speedY *= (Vertical - 0.4) * -1 : speedY += (speedY * -1) / 4 - 0.1);

			//Add drag

			if (!InGodMode)
			{
				speedX *= 0.899;
				speedY *= 0.899;

				//And gravity
				speedY -= 1.5;

				y -= speedY;
			}

			//Cap the values

			speedX = (speedX > speed ? speed : speedX);
			speedY = (speedY > speed ? speed : speedY);

			speedX = (speedX < -speed ? -speed : speedX);
			speedY = (speedY < -speed ? -speed : speedY);

			//Cap it off if it's .1

			if (speedX > 0)
				if (speedX < 0.11)
					speedX = 0;

			if (speedY > 0)
				if (speedY < 0.11)
					speedY = 0;

			if (speedX < 0)
				if (speedX > -0.11)
					speedX = 0;

			if (speedY < 0)
				if (speedY > -0.11)
					speedY = 0;

			//Add drag

			if (!InGodMode)
			{
				speedX *= 0.899;
				speedY *= 0.899;

				//Gravity

				y += speedY;
			}

			//Add to the X and Y

			if (speedX != 0)
				if (speedX > 0)
					x += speedX;
				else
					x -= -speedX;

			if (speedY != 0)
				if (speedY > 0)
					y += speedY;
				else
					y -= -speedY;

			//Depending on the direction we're going, bounce back if we're touching a block.

			/*
			if (!InGodMode)
			{
				UpdateLocations();

				if (TouchingSolid() && speedX > 0)
					x -= speedX;

				if (PlayerLocation.X > 32)
					if (TouchingSolid(0, 0, 0) && speedX < 0)
						x += -speedX;

				if (TouchingSolid() && speedY > 0)
					y -= speedY;

				if (PlayerLocation.Y > 32)
					if (TouchingSolid(0, -1, 0) && speedY < 0)
						y += speedY;

				//Gravity

				if(!TouchingSolid(0, 1, 1))
				{
					FixPlayerBounds();

					if (!TouchingSolid(0, 1, 1))
						y = Math.Round(y);

					if(!TouchingSolid(0, 1, 1))
					{
						speedY += 1;
					}

					y -= speedY * 2;

					if(!TouchingSolid(0, 1, 0))
					{
						y -= 8;
						speedY = 0;
					}
				} 
			}
			 */

			UpdateLocations();

			//Decrease the speeds

			if (Horizontal == 0)
				speedX += (speedX > 0 ? -0.1 : (speedX < 0 ? 0.1 : 0)); //We want to make sure SpeedX isn't 0 already

			if (Vertical == 0)
				speedY += (speedY > 0 ? -0.1 : (speedY < 0 ? 0.1 : 0));
		}

		public void UpdateLocations()
		{
			//Update Values

			PlayerLocation.X = Convert.ToInt32(Math.Round(x));
			PlayerLocation.Y = Convert.ToInt32(Math.Round(y));
		}
		/*
		public int GetBlockAtLocation()
		{
			UpdateLocations();

			int block = Program.game.World.GetBlock(0, BlockLocation.X, BlockLocation.Y);
			return block;
		}

		public int GetBlockAtLocation(int addX = 0, int addY = 0)
		{
			UpdateLocations();

			if (!(BlockLocation.X + addX > -1 &&
				BlockLocation.Y + addY > -1))
				return 0;

			int block = Program.game.World.GetBlock(0, BlockLocation.X + addX, BlockLocation.Y + addY);
			return block;
		}
		public bool TouchingSolid()
		{
			bool touching = (GetBlockAtLocation() == 164 ? false : (9 <= GetBlockAtLocation() && GetBlockAtLocation() <= 97 || 122 <= GetBlockAtLocation() && GetBlockAtLocation() <= 217 || GetBlockAtLocation() >= 1001 && GetBlockAtLocation() <= 2000) && GetBlockAtLocation() != 83 && GetBlockAtLocation() != 77);

			if (!touching && !(PlayerLocation.X % 16 == 0))
				touching = (GetBlockAtLocation(1) == 164 ? false : (9 <= GetBlockAtLocation(1) && GetBlockAtLocation(1) <= 97 || 122 <= GetBlockAtLocation(1) && GetBlockAtLocation(1) <= 217 || GetBlockAtLocation(1) >= 1001 && GetBlockAtLocation(1) <= 2000) && GetBlockAtLocation(1) != 83 && GetBlockAtLocation(1) != 77);

			return touching;
		}

		public bool TouchingSolid(int negX_, int negY_, int modifier_)
		{
			
			//Gravity checks
			int negX = negX_;
			int negY = negY_;

			if (negY_ < 0)
				negY_ = 0;

			if (modifier_ + negY < 0)
				modifier_ = 0;

			int modifier = 0;
			

			bool touching = (GetBlockAtLocation(negX) == 164 ? false : (9 <= GetBlockAtLocation(negX + modifier, negY + modifier) && GetBlockAtLocation(negX + modifier, negY + modifier) <= 97 || 122 <= GetBlockAtLocation(negX + modifier, negY + modifier) && GetBlockAtLocation(negX + modifier, negY + modifier) <= 217 || GetBlockAtLocation(negX + modifier, negY + modifier) >= 1001 && GetBlockAtLocation(negX + modifier, negY + modifier) <= 2000) && GetBlockAtLocation(negX + modifier, negY + modifier) != 83 && GetBlockAtLocation(negX + modifier, negY + modifier) != 77);

			if (!touching && !(PlayerLocation.X % 16 == 0))
				touching = (GetBlockAtLocation(negX, negY) == 164 ? false : (9 <= GetBlockAtLocation(negX, negY) && GetBlockAtLocation(negX, negY) <= 97 || 122 <= GetBlockAtLocation(negX, negY) && GetBlockAtLocation(negX, negY) <= 217 || GetBlockAtLocation(negX, negY) >= 1001 && GetBlockAtLocation(negX, negY) <= 2000) && GetBlockAtLocation(negX, negY) != 83 && GetBlockAtLocation(negX, negY) != 77);

			return touching;
		}
		*/

		public void FixPlayerBounds()
		{
			UpdateLocations();

			if (x < -((Program.game.World.Width * 16) - (320 + 16)))
			{ x = -((Program.game.World.Width * 16) - (320 + 16)) + 1; speedX = 0; }

			if (y < -((Program.game.World.Height * 16) - (250 + 16)))
			{ y = -((Program.game.World.Height * 16) - (250 + 16)) + 1; speedY = 0; }

			if (x > 320)
			{ x = 319; speedX = 0; }

			if (y > 250)
			{ y = 249; speedY = 0; }

			UpdateLocations();

		}
	}
}

/*
FLASH PHYSICS FROM OLD EE
		/// <summary>
		/// Do a physics tick. The more ticks, the faster the physics will go.
		/// </summary>
		public void DoPhysicsTick()
		{

		}
      
      protected double _x = 0;
      
      protected double _y = 0;
      
      protected double _speedX = 0;
      
      protected double _speedY = 0;
      
      protected double _modifierX = 0;
      
      protected double _modifierY = 0;
      
      protected double _dragX = 0;
      
      protected double _dragY = 0;
      
      public int width = 1;
      
      public int height = 1;
      
      public bool moving = false;

	  public double mx;

	  public double my;
      
      //public double hitmap:BlTilemap;
      
      public double last;

		public string[] queue;
      

		// -- == -- CODE FROM OLD CLIENT -- == --

		/// <summary>
		/// Start moving in a certain direction
		/// </summary>
		/// <param name="dir">The direction</param>
		public void StartMoving(Direction dir)
		{
         //double p:Player = null;
         int xo = 0;
         int yo = 0;

		 if (queue == null)
			 queue = new string[2];

         int length = this.queue.Length;

			//Shift array
			while(length != 0){
			
			 string[] newAr = new string[queue.Length];
for (int i = 1; i < queue.Length; i++)
   newAr[i - 1] = queue[i];

				length--;
			}
         double nx = 0;
         double ny = 0;
         bool changed = false;

         if(_modifierY == 0)
         {
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && speedY() == 0)
            {
               speedY(speedY() - modifierY() * 280);
               changed = true;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Left))
            {
               nx = -1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Right))
            {
               nx = 1;
            }
         }
         else if(modifierX() == 0)
         {
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && speedX() == 0)
            {
               speedX(speedX() - modifierX() * 280);
               changed = true;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
               ny = -1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Down))
            {
               ny = 1;
            }
         }
         else
         {
            if(Keyboard.GetState().IsKeyDown(Keys.Left))
            {
               nx = -1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Right))
            {
               nx = 1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
               ny = -1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Down))
            {
               ny = 1;
            }
         }
         if(mx != nx || my != ny || changed)
         {
            mx = nx;
            my = ny;
         }
		 update();
		}

		/// <summary>
		/// Stop moving in a certain direction
		/// </summary>
		/// <param name="dir">The direction</param>
		public void StopMoving(Direction dir)
		{

		}


      public void update()
      {
         double ox = 0;
         double oy = 0;
         if(this._speedX != 0 || this._modifierX != 0)
         {
            this._speedX = this._speedX + this._modifierX;
            this._speedX = this._speedX * this._dragX;
               ox = this._x;
               this._x = this._x + this._speedX;
               if(false) //Hitmap Overlap
               {
                  this._x = Math.Round(ox);
                  this._speedX = 0;
               }
         }
         if(this._speedY != 0 || this._modifierY != 0)
         {
            this._speedY = this._speedY + this._modifierY;
            this._speedY = this._speedY * this._dragY;
               oy = this._y;
               this._y = this._y + this._speedY;
               if(false) //Hitmap Overlap
               {
                  this._y = Math.Round(oy);
                  this._speedY = 0;
               }
         }
         this.moving = Convert.ToInt32(this._speedX) * 512 > 0 || Convert.ToInt32(this._speedY) * 512 > 0 ? true : false;
      }
      
      public void draw(System.Drawing.Imaging.BitmapData target, int ox, int oy)
      {
         //target.setPixel(this._x + ox >> 0,this._y + oy >> 0,16777215);
      }
      
      public void enterFrame()
      {
      }
      
      public void tick()
      {
         for(int a = 0; a < Bl.elapsed; a++)
         {
            this.update();
         }
      }
      
      public void exitFrame()
      {
      }
      
      public int speedX()
      {
         return Convert.ToInt32(_speedX) * 1000;
      }
      
      public void speedX(double value)
      {
         this._speedX = value / 1000;
      }
      
      public double speedY()
      {
         return this._speedY * 1000;
      }
      
      public void speedY(double value)
      {
         this._speedY = value / 1000;
      }
      
      public double modifierX()
      {
         return this._modifierX * 1000;
      }
      
      public void modifierX(double value)
      {
         this._modifierX = value / 1000;
      }
      
      public double modifierY()
      {
         return this._modifierY * 1000;
      }
      
      public void modifierY(double value)
      {
         this._modifierY = value / 1000;
      }
      
      public double dragX()
      {
         return this._dragX;
      }
      
      public void dragX(double value)
      {
         this._dragX = value;
      }
      
      public double dragY()
      {
         return this._dragY;
      }
      
      public void dragY(double value)
      {
         this._dragY = value;
      }
      
      public void x(double nx)
      {
         this._x = nx;
      }
      
      public double x()
      {
         return this._x;
      }
      
      public void y(double ny)
      {
         this._y = ny;
      }
      
      public double y()
      {
         return this._y;
      }

*/