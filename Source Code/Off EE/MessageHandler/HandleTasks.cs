using System;
using System.Collections.Generic;
using Mod;
using System.Threading;
using System.IO;

namespace Off_EE
{
	//The class to handle tasks sent by mods
	public static class HandleTasks
	{
		/// <summary>
		/// Handles all the current tasks
		/// </summary>
		/// <param name="i">Handle a list of tasks</param>
		public static void Handle(List<Task> i)
		{
			if (i == null) return;
			if (i.Count == 0) return;

			Mod.ServiceHandler.SetBusy(true, Program.game);
			redo:
			try
			{
				foreach (Task n in i)
					Handle(n);
			} catch(InvalidOperationException e)
			{
				goto redo; //We have to retry, all the mods will eventually know we are not going to being accepting input.
			}
			Mod.ServiceHandler.ClearTasks(Program.game);
			Mod.ServiceHandler.SetBusy(false, Program.game);
		}

		/// <summary>
		/// Handle a single task
		/// </summary>
		/// <param name="i">The task</param>
		private static void Handle(Task i)
		{
			switch(i.TaskId)
			{
					//Placing a block?
				case "PlaceBlock":
					//Argument Checking
					if (i.Arguments.Length == 5)
					{
						CheckArgument(0, "System.Int32", i.Arguments);
						CheckArgument(1, "System.Int32", i.Arguments);
						CheckArgument(2, "System.Int32", i.Arguments);
						if (i.Arguments[3].GetType().ToString() != "System.Int32" && i.Arguments[3].GetType().ToString() != "System.UInt32") throw new ArgumentException("Argument 3 should be type System.Int32 or System.UInt32. It is " + i.Arguments[3].GetType().ToString());
						CheckArgument(4, "System.String", i.Arguments);

						//Casting
						int layer = (int)i.Arguments[0];
						int x = (int)i.Arguments[1];
						int y = (int)i.Arguments[2];
						int id = Convert.ToInt32(i.Arguments[3]);

						//Checking positions
						if (x > -1 && y > -1 &&
							x < Program.game.World.Width &&
							y < Program.game.World.Height)
							Program.game.World.PlaceBlock(layer, x, y, id, (string)i.Arguments[4]);

						Program.game.LogLine("Mod " + (string)i.Arguments[4] + " placed a block.");
					}
					break;

					//Change your hotbar?
				case "SetHotbar":
					//Checking Argyments
					if(i.Arguments.Length == 2)
					{
						CheckArgument(0, "System.Int32", i.Arguments);
						CheckArgument(1, "System.Int32", i.Arguments);
						CheckArgument(2, "System.String", i.Arguments);

						//Casting & Setting it
						//Hotbar does the checking for us
						Program.game.Hotbar.SetSlot((int)i.Arguments[0], (int)i.Arguments[1]);

						Program.game.LogLine("Mod " + (string)i.Arguments[2] + " set your hotbar, slot " + (int)i.Arguments[0] + " to " + (int)i.Arguments[1]);
					}
					break;
					//Chat
				case "Chat":
					//Arguments
					if (i.Arguments.Length == 1)
					{
						if (i.Arguments[0].GetType().ToString() != "System.String") throw new ArgumentException("Argument 0 should be type System.String. It is " + i.Arguments[0].GetType().ToString());
						
						//Chat
						Program.game.Chat.Add((string)i.Arguments[0]);

						Program.game.LogLine("Mod chat message: " + (string)i.Arguments[0]);
					}
					break;
					//Select a slot in a hotbar
				case "SelectHotbar":
					if (i.Arguments.Length == 1)
					{
						CheckArgument(0, "System.Int32", i.Arguments);

						Program.game.Hotbar.SelectSlot((int)i.Arguments[0]);

						Program.game.LogLine("A mod forced you to select an item in the hotbar.");
					}
					break;
				case "DrawObject":
					if(i.Arguments.Length == 4)
					{
						CheckArgument(0, "System.Int32", i.Arguments);
						CheckArgument(1, "System.Int32", i.Arguments);
						CheckArgument(2, "System.String", i.Arguments);
						CheckArgument(3, "System.String", i.Arguments);

						if (!File.Exists(@"data\" + (string)i.Arguments[3]))
							throw new FileNotFoundException((string)i.Arguments[3] + " was not found in the data folder.");

						if(Program.game.CustomModDrawnObjects.ContainsKey((string)i.Arguments[2]))
							throw new ArgumentException("The key \"" + i.Arguments[2] + "\" already exists within the custom game object array.");

						System.Drawing.Bitmap CustomDraw;

						try
						{
							CustomDraw = new System.Drawing.Bitmap(@"data\" + (string)i.Arguments[3]);
						}
						catch(ArgumentException e)
						{
							Program.game.Chat.Add("* SYSTEM > There was a problem drawing \"" + (string)i.Arguments[3] + "\" to the screen.");
							return;
						}

						while (Program.game.IteratingThroughCustomModObjects) { /**/ }

						Program.game.CustomModDrawnObjects.Store((string)i.Arguments[2], OffEE.GetTexture(Program.game.GraphicsDevice, CustomDraw));
						Program.game.CustomModDrawinObjectsLocation.Store((string)i.Arguments[2], new Microsoft.Xna.Framework.Point((int)i.Arguments[0], (int)i.Arguments[1]));

						Program.game.LogLine("A mod drew a raw texture");
					}
					break;
				case "RemoveObject":
					if (i.Arguments.Length == 1)
					{
						CheckArgument(0, "System.String", i.Arguments);

						string key = (string)i.Arguments[0];

						if (!Program.game.CustomModDrawnObjects.ContainsKey(key))
							throw new ArgumentException("No customly mod object drawn has the key of \"" + key + "\"");

						while (Program.game.IteratingThroughCustomModObjects) { /**/ }

						Program.game.CustomModDrawnObjects.Remove(key);

						Program.game.LogLine("Mod " + (string)i.Arguments[4] + " removed a customly drawn texture");
					}
					break;
			}
		}

		public static void CheckArgument(int ArgId, string Type, object[] Arguments)
		{
			if (Arguments[ArgId].GetType().ToString() != Type) throw new ArgumentException("Argument " + ArgId.ToString() + " should be type " + Type + ". It is " + Arguments[ArgId].GetType().ToString());
		}
	}
}
