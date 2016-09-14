using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class Server
{
	public static void ServerConnect()
	{
		if (Global._connection == false)
		{
			try
			{
				Global.sClient = new Client_Server(11000, IPAddress.Parse("127.0.0.1"));
				Global.sClient.Start();
				Global._connection = true;
			}
			catch
			{
				ServerConnect();
			}
		}
	}
}