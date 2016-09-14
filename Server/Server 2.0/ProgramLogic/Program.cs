using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Net;

namespace Server_2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            Global._server = new Server(11000, IPAddress.Parse("127.0.0.1"));
            Global._server.StartServer();
        }
    }
}
