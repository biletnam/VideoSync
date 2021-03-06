﻿using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using System.Collections.Generic;

namespace VideoSync
{
    class Program
    {
        public static List<string> roomCodes;

        public static void Main(string[] args)
        {

#if DEBUG
            int port = 8080;    // set debug mode port to 8080 so we don't need to run with elevated permissions
#elif RELEASE           
            int port = 80;      // default http port
#endif
            
            roomCodes = new List<string>();

            var httpsv = new HttpServer(port);


			var timer = new System.Threading.Timer((e) =>
			{
				CleanSocketServices(httpsv);
			}, null, 0, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);


			// To resolve to wait for socket in TIME_WAIT state.
			//httpsv.ReuseAddress = true;

			// Set the document root path.
            httpsv.RootPath = "../../Public";

            // Set the HTTP GET request event.
            httpsv.OnGet += (sender, e) => {
                var req = e.Request;
                var res = e.Response;

                var path = req.RawUrl;

                if (path.Length == 6 && !path.Contains("."))
                {
                    if (roomCodes.Contains(path.Remove(0, 1)))
                    {
                        path = "/index.min.html";
                    }
                    else
                    {
                        path = "/index.min.html";
                        var newCode = RandomString(5);
                        roomCodes.Add(newCode);
                        httpsv.AddWebSocketService<Server>("/" + newCode, () => new Server () {
                                                                    IgnoreExtensions = true
                                                                });
						Console.WriteLine("Created socket service on path: " + newCode);
                        res.Redirect((req.Url.GetLeftPart(UriPartial.Authority) +"/"+ newCode));
                    }
                }
                

                if (path == "/")
                {
                    path = "/index.min.html";
                    var newCode = RandomString(5);
                    roomCodes.Add(newCode);
                    httpsv.AddWebSocketService<Server>("/" + newCode, () => new Server () {
                                                                IgnoreExtensions = true
                                                            });
					Console.WriteLine("Created socket service on path: " + newCode);
                    res.Redirect(req.Url+ newCode);
                }
				else if (path=="/faq")
				{
					path = "/faq.html";
				}

					var content = httpsv.GetFile(path);

                if (content == null)
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                if (path.EndsWith(".html"))
                {
                    res.ContentType = "text/html";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith(".js"))
                {
                    res.ContentType = "application/javascript";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith(".css"))
                {
                    res.ContentType = "text/css";
                    res.ContentEncoding = Encoding.UTF8;
                }
                
                res.WriteContent(content);
            };
            
            httpsv.Start();
            if (httpsv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", httpsv.Port);
                foreach (var path in httpsv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }


#if DEBUG
            // in debug mode we want to be able to easily stop the server,
            // but in release we want to keep the server running no matter what.
            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();

            httpsv.Stop();
#endif
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

		public static void CleanSocketServices(HttpServer http)
		{
			Console.WriteLine("Cleaning empty socket services...");
			List<string> removecodes = new List<string>();
			foreach(string s in roomCodes)
			{
				WebSocketServiceHost wsh;
				if(http.WebSocketServices.TryGetServiceHost("/"+s, out wsh))
				{
					if(wsh.Sessions.Count==0)
					{
						http.RemoveWebSocketService("/"+s);
						removecodes.Add(s);
						Console.WriteLine("Removed service code: " + s);
					}
				}
			}
			foreach(string s in removecodes)
			{
				roomCodes.Remove(s);
			}
		}
    }
}
