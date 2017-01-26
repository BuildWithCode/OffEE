using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Off_EE
{
	public class HotBar
	{
		#region Constructor

		/// <summary>
		/// Hotbar
		/// </summary>
		public HotBar()
		{
			Bar = new int[Length];
			int Counter = 0;
			int TryInt = 0;

			foreach (string i in Program.game.Resources.Hotbar.GetData)
			{
				if (Int32.TryParse(i, out TryInt) && Counter < 11)
				{
					if (TryInt > -1)
					{
						Bar[Counter] = TryInt;
						Counter++;
					}
				}
			}
		}

		#endregion

		#region Variables

		private int[] Bar;

		/// <summary>
		/// The length of the hotbar
		/// </summary>
		public int Length { get { return 11; } set { } }

		/// <summary>
		/// The slot that is selected
		/// </summary>
		public int SelectedSlot = 1;

		#endregion

		/// <summary>
		/// Get a slot
		/// </summary>
		/// <param name="SlotId">The Id of the slot to get</param>
		/// <returns></returns>
		public int GetSlot(int SlotId)
		{
			if (WithinSlotRange(SlotId))
			{ return Bar[SlotId]; }
			else
			{ throw new ArgumentException(SlotId + " is not within the slot range."); }
		}

		/// <summary>
		/// Set a slot
		/// </summary>
		/// <param name="SlotId">The Id of the slot</param>
		/// <param name="Block">The BlocK Id</param>
		public void SetSlot(int SlotId, int Block)
		{
			if (WithinSlotRange(SlotId))
			{
				if (Bar[SlotId] != Block)
				{
					Bar[SlotId] = Block;

					var e = new Mod.HotbarSlotSelectedArgs();
					e.SelectedBlock = Bar[SlotId];
					e.SelectedSlot = SlotId;
					Mod.Events.FireHotbarSlotSet(Program.game, e);
				}
			}
			else
			{ throw new ArgumentException(SlotId + " is not within the slot range."); }
		}

		/// <summary>
		/// Select one of the slots
		/// </summary>
		/// <param name="SlotId">The slot Id</param>
		public void SelectSlot(int SlotId)
		{
			if (WithinSlotRange(SlotId))
			{
				if (SelectedSlot != SlotId)
				{
					SelectedSlot = SlotId;
					Program.game.BlockUsing = this.GetSlot(SlotId);

					var e = new Mod.HotbarSlotSelectedArgs();
					e.SelectedBlock = Bar[SlotId];
					e.SelectedSlot = SlotId;
					Mod.Events.FireHotbarSlotSelected(Program.game, e);
				}
			}
			else
			{ throw new ArgumentException(SlotId + " is not within the slot range."); }
		}

		/// <summary>
		/// Check if a slot is within a certain range
		/// </summary>
		/// <param name="Slot">The slot to check</param>
		/// <returns></returns>
		public bool WithinSlotRange(int Slot)
		{ return (Slot < 0 || Slot > Length - 1 ? false : true); }

		internal int GetSelectedSlot()
		{
			return SelectedSlot;
		}
	}
}
