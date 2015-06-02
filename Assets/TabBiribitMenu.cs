using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class TabBiribitMenu : Silver.UI.TabImmediate 
{
	private string clientName = "ClientName";
	private string appId = "app-client-test";
	private string chat = "";
	private int serverInfoSelected = 0;
	private int serverConnectionSelected = 0;
	private int roomSelected = 0;
	private List<string> serverInfoStrings = new List<string>();
	private List<string> serverConnectionStrings = new List<string>();
	private List<string> roomStrings = new List<string>();
	private List<string> chats = new List<string>();

	private int slots = 4;
	private int jointSlot = 0;

	public override string TabName()
	{
		return "Biribit";
	}

	public override void DrawUI()
	{
		BiribitClient client = Biribit.Instance;
		BiribitClient.Received recv;
		while ((recv = client.PullReceived()) != null)
		{
			StringBuilder b = new StringBuilder();
			var secs = recv.when / 1000;
			var mins = secs / 60;
			b.Append(mins % 60); b.Append(":");b.Append(secs % 60);
			b.Append(" ["); b.Append(recv.room_id); b.Append(",");
			b.Append(recv.slot_id); b.Append( "]: ");
			b.Append(Encoding.ASCII.GetString(recv.data));

			chats.Reverse();
			chats.Add(b.ToString());
			chats.Reverse();
		}

		ui.VerticalLayout(() =>
		{
			ui.TitleField("Menu");
			ui.LineSeparator();

			if(ui.Button("Reload scene"))
				Application.LoadLevel(Application.loadedLevel);

			ui.Separator(1);
			ui.LineSeparator();
			if (ui.Button("Discover in LAN"))
				client.DiscoverOnLan();

			if (ui.Button("Clear list of servers"))
				client.ClearDiscoverInfo();

			if (ui.Button("Refresh servers"))
				client.RefreshDiscoverInfo();

			BiribitClient.ServerInfo[] serverInfoArray = client.GetDiscoverInfo();
			if (serverInfoArray.Length > 0)
			{
				serverInfoStrings.Clear();
				foreach (BiribitClient.ServerInfo serverInfo in serverInfoArray)
				{
					StringBuilder builder = new StringBuilder();
					builder.Append(serverInfo.name); builder.Append(", ping "); builder.Append(serverInfo.ping);
					builder.Append(serverInfo.passwordProtected != 0 ? ". Password protected." : ". No password.");
					serverInfoStrings.Add(builder.ToString());
				}
				ui.Separator(1);
				ui.LabelField("Server");
				ui.LineSeparator();
				serverInfoSelected = ui.Popup("Server", serverInfoSelected, serverInfoStrings.ToArray(), Silver.UI.Immediate.FlagMask.NoFieldLabel);
				BiribitClient.ServerInfo info = serverInfoArray[serverInfoSelected];

				if (ui.Button("Connect selected"))
					client.Connect(info.addr, info.port);
			}

			BiribitClient.ServerConnection[] serverConnectionArray = client.GetConnections();
			if (serverConnectionArray.Length > 0)
			{
				serverConnectionStrings.Clear();
				foreach (BiribitClient.ServerConnection serverConnection in serverConnectionArray)
				{
					StringBuilder builder = new StringBuilder();
					builder.Append(serverConnection.id); builder.Append(": ");
					builder.Append(serverConnection.name); builder.Append(". Ping: "); builder.Append(serverConnection.ping);
					serverConnectionStrings.Add(builder.ToString());
				}

				ui.Separator(1);
				ui.LabelField("Connection");
				ui.LineSeparator();
				serverConnectionSelected = ui.Popup("Connection", serverConnectionSelected, serverConnectionStrings.ToArray(), Silver.UI.Immediate.FlagMask.NoFieldLabel);
				BiribitClient.ServerConnection connection = serverConnectionArray[serverConnectionSelected];
				if (ui.Button("Disconnect"))
					client.Disconnect(connection.id);

				if (ui.Button("Disconnect all"))
					client.Disconnect();

				ui.Separator(1);
				ui.LineSeparator();
				clientName = ui.StringField("Client name", clientName);
				appId = ui.StringField("Application Id", appId);
				if (ui.Button("Set name and appid"))
					client.SetLocalClientParameters(connection.id, clientName, appId);

				BiribitClient.RemoteClient[] remoteClientsArray = client.GetRemoteClients(connection.id);
				uint localClientId = client.GetLocalClientId(connection.id);
				if (remoteClientsArray.Length > 0)
				{
					ui.Separator(1);
					ui.LabelField("Client list");
					ui.LineSeparator();
					foreach (BiribitClient.RemoteClient remoteClient in remoteClientsArray)
					{
						StringBuilder builder = new StringBuilder();
						builder.Append(remoteClient.id); builder.Append(": ");
						builder.Append(remoteClient.name);
						if (remoteClient.id == localClientId)
							builder.Append(" << YOU");
						ui.LabelField(builder.ToString(), 14);
					}
				}


				ui.Separator(1);
				ui.LabelField("Create room");
				ui.LineSeparator();
				ui.HorizontalLayout(() =>
				{
					slots = ui.IntField("Num slots", slots);
					if (ui.Button("Create"))
						client.CreateRoom(connection.id, (uint) slots);
				});
				
				ui.HorizontalLayout(() =>
				{
					jointSlot = ui.IntField("Joining slot", jointSlot);
				if (ui.Button("& Join"))
					client.CreateRoom(connection.id, (uint)slots, (uint) jointSlot);
				});

				if (ui.Button("Random or create")) {
					client.JoinRandomOrCreateRoom(connection.id, (uint)slots);
				}
				

				BiribitClient.Room[] roomArray = client.GetRooms(connection.id);
				uint joinedRoomId = client.GetJoinedRoomId(connection.id);
				uint joinedRoomSlot = client.GetJoinedRoomSlot(connection.id);
				if (roomArray.Length > 0)
				{
					roomStrings.Clear();
					foreach (BiribitClient.Room room in roomArray)
					{
						StringBuilder builder = new StringBuilder();
						builder.Append("Room "); builder.Append(room.id);

						if (room.id == joinedRoomId)
						{
							builder.Append(" | Joined: ");
							builder.Append(joinedRoomSlot);
						}

						roomStrings.Add(builder.ToString());
					}

					ui.Separator(1);
					ui.LabelField("Rooms");
					ui.LineSeparator();
					roomSelected = ui.Popup("Room", roomSelected, roomStrings.ToArray(), Silver.UI.Immediate.FlagMask.NoFieldLabel);
					BiribitClient.Room rm = roomArray[roomSelected];

					if (ui.Button("Join"))
						client.JoinRoom(connection.id, rm.id);

					if (ui.Button("Leave"))
						client.JoinRoom(connection.id, 0);

					if (ui.Button("Refresh rooms"))
						client.RefreshRooms(connection.id);
					
					ui.Separator(1);
					ui.LabelField("Room");
					ui.LineSeparator();

					for (int i = 0; i < rm.slots.Length; i++)
					{
						if (rm.slots[i] == BiribitClient.UnassignedId)
							ui.LabelField("Slot " + i.ToString() + ": Free", 14);
						else
						{
							int pos = client.GetRemoteClientArrayPos(rm.slots[i]);
							if (pos < 0)
								ui.LabelField("Slot " + i.ToString() + ": " + rm.slots[i], 14);
							else
								ui.LabelField("Slot " + i.ToString() + ": " + remoteClientsArray[i].name, 14);
						}
					}
				}
				else
				{
					if (ui.Button("Refresh rooms"))
						client.RefreshRooms(connection.id);
				}

				if (joinedRoomId != BiribitClient.UnassignedId)
				{
					ui.Separator(1);
					ui.LabelField("Chat");
					ui.LineSeparator();
					chat = ui.StringField("Chat", chat, Silver.UI.Immediate.FlagMask.NoFieldLabel);
					if(ui.Button("Send") && !string.IsNullOrEmpty(chat.Trim()))
					{
						byte[] data = Encoding.ASCII.GetBytes(chat.Trim());
						client.SendBroadcast(connection.id, data);
						chat = "";
					}

					foreach(string linechat in chats) {
						ui.LabelField(linechat, 14);
					}
				}
			}
		});
	}
}
