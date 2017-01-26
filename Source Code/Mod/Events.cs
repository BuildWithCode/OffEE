using Mod.CustomPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod
{
	public delegate void Tick();
	public delegate void HotbarInit(HotbarInitArgs e);
	public delegate void HotbarSlotSelected(HotbarSlotSelectedArgs e);
	public delegate void HotbarSlotSet(HotbarSlotSelectedArgs e);
	public delegate void BlockPlaced(BlockPlacedArgs e);
	public delegate void ChatMessage(ChatMessageArgs e);
	public delegate void KeyStroke(KeyStrokeArgs e);

	public static class Events
	{
		public static event Tick OnTick;
		public static event HotbarInit OnHotbarInit;
		public static event HotbarSlotSelected OnHotbarSlotSelected;
		public static event HotbarSlotSet OnHotbarSlotSet;
		public static event BlockPlaced OnBlockPlaced;
		public static event ChatMessage OnChatMesage;
		public static event KeyStroke OnKeyStroke;

		public static void FireOnTick(object sender)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			if (OnTick != null)
				OnTick();
		}

		public static void FireHotbarInit(object sender, HotbarInitArgs e)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			if (e.Bar == null)
				throw new ArgumentException("Bar is not assigned.");

			if (OnHotbarInit != null)
				OnHotbarInit(e);
		}

		public static void FireHotbarSlotSelected(object sender, HotbarSlotSelectedArgs e)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			if (e.SelectedSlot == -1)
				throw new ArgumentException("Selected Slot is not assigned.");

			if (e.SelectedBlock == -1)
				throw new ArgumentException("Selected Block is not assigned.");

			if (OnHotbarSlotSelected != null)
				OnHotbarSlotSelected(e);
		}

		public static void FireHotbarSlotSet(object sender, HotbarSlotSelectedArgs e)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			if (e.SelectedSlot == -1)
				throw new ArgumentException("Selected Slot is not assigned.");

			if (e.SelectedBlock == -1)
				throw new ArgumentException("Selected Block is not assigned.");

			if (OnHotbarSlotSet != null)
				OnHotbarSlotSet(e);
		}

		public static void FireBlockPlaced(object sender, BlockPlacedArgs e)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			//Check for null values
			if (e.id == -1)
				throw new ArgumentException("Block ID is not assigned.");

			if (e.layer == -1)
				throw new ArgumentException("Layer is not assigned.");

			if (e.location == null)
				throw new ArgumentException("Location is not assigned.");

			if (OnBlockPlaced != null)
				OnBlockPlaced(e);

			ServiceHandler.WorldData[e.layer, e.location.X, e.location.Y] = Convert.ToUInt32(e.id);
		}

		public static void FireChatMessage(object sender, ChatMessageArgs e)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			if (e.From == "")
				throw new ArgumentException("e.From cannot be blank.");

			if (e.Message == "")
				throw new ArgumentException("e.Message cannot be blank.");

			if (e.RawMessage == "")
				e.RawMessage = "* " + e.From + " > " + e.Message;

			if (OnChatMesage != null)
				OnChatMesage(e);
		}

		public static void FireKeyStroke(object sender, KeyStrokeArgs e)
		{
			if (ModLoader.Master != sender)
				throw new MasterException();

			if (OnKeyStroke != null)
				OnKeyStroke(e);
		}
	}

	public class KeyStrokeArgs : EventArgs
	{
		public char Key = '_';

		public KeyStrokeArgs()
			: base()
		{

		}
	}

	public class ChatMessageArgs : EventArgs
	{
		public string From = "";
		public string Message = "";
		public string RawMessage = "";

		public ChatMessageArgs()
			: base()
		{

		}
	}

	public class HotbarSlotSelectedArgs : EventArgs
	{
		public int SelectedSlot = -1;
		public int SelectedBlock = -1;

		public HotbarSlotSelectedArgs()
			: base()
		{

		}
	}

	public class BlockPlacedArgs : EventArgs
	{
		public int layer = -1;
		public Point location = null;
		public int id = -1;
		public int[] data = null;
		public string From = "User";

		public BlockPlacedArgs()
			: base()
		{

		}
	}

	public class HotbarInitArgs : EventArgs
	{
		public int[] Bar = null;

		public HotbarInitArgs()
			: base()
		{

		}
	}
}
