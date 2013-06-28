using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatFilter
{
	public class LibraryManager : ICommListener
	{
		public List<Card> Cards { get; private set; }
		private Communicator comm;
		private Action callback;

		public LibraryManager()
		{
			this.comm = App.Communicator;
		}

		public void LoadLibrary(Action callback)
		{	
			this.callback = callback;
			
			this.comm.addListener(this);
			this.comm.sendRequest(new LibraryViewMessage());
		}

		#region ICommListener implementation

		public void handleMessage(Message msg)
		{
			Console.WriteLine("Messages: " + msg);

			LibraryViewMessage libraryMessage = msg as LibraryViewMessage;
			if (libraryMessage == null) {
				return;
			}

			Cards = new List<Card>(libraryMessage.cards);

			if (callback != null) {
				callback();
			}

			this.comm.removeListener(this);
		}

		public void onReconnect()
		{
		}

		#endregion
	}
}

