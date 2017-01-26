using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Off_EE
{
	public class Chat
	{
		public List<string> ChatMessages = new List<string>();

		/// <summary>
		/// Add a message to the chat
		/// </summary>
		/// <param name="From"></param>
		/// <param name="Message"></param>
		public void AddMessage(string From, string Message)
		{
			string FormattedMessage = "* " + From + " > " + Message;

			if(Mod.ModLoader.ModsHaveLoaded)
			{
				Mod.Events.FireChatMessage(Program.game, new Mod.ChatMessageArgs()
				{
					From = From,
					Message = Message,
					RawMessage = FormattedMessage
				});
			}

			ChatMessages.Add(FormattedMessage);
		}
	}
}
