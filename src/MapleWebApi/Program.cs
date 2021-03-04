using System;
using System.IO;
using System.Net;

class ProgramMapleWebApi
{
    static HttpListener listener;

    static int responseSet = 200;

    static void Main()
    {
        Console.Title = "MapleWebApi";
        Console.WriteLine("Press enter to toggle the server and return an error or success");
        Console.WriteLine("Press any other key to exit");

        using (listener = new HttpListener())
        {
            listener.Prefixes.Add("http://localhost:57811/");
            listener.Start();
            listener.BeginGetContext(ListenerCallback, listener);

            while (true)
            {
                ReportStatus();

                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key == ConsoleKey.Q)
                {
                    listener.Close();
                    return;
                }

                if (key.Key == ConsoleKey.D2)
                    responseSet = 200;

                if (key.Key == ConsoleKey.D3)
                    responseSet = 300;

                if (key.Key == ConsoleKey.D4)
                    responseSet = 400;

                if (key.Key == ConsoleKey.D5)
                    responseSet = 500;
            }
        }
    }

    static void ReportStatus()
    {
        Console.WriteLine("\r\nCurrently returning {0}", responseSet.ToString());
    }

    static void ListenerCallback(IAsyncResult result)
    {
        if (!listener.IsListening)
        {
            return;
        }

        var context = listener.EndGetContext(result);
        var response = context.Response;

        ShowRequestProperties1(context.Request);

        if (responseSet == 200)
            WriteResponse(response, HttpStatusCode.OK);
        if (responseSet == 300)
            WriteResponse(response, HttpStatusCode.Moved);
        if (responseSet == 400)
            WriteResponse(response, HttpStatusCode.BadRequest);
        if (responseSet == 500)
            WriteResponse(response, HttpStatusCode.GatewayTimeout);
        response.Close();

        listener.BeginGetContext(ListenerCallback, listener);
    }

    static void WriteResponse(HttpListenerResponse response, HttpStatusCode statusCode)
    {
        response.StatusCode = (int)statusCode;
        response.StatusDescription = "OK";
        var text = Guid.NewGuid().ToString();
        using (var streamWriter = new StreamWriter(response.OutputStream))
        {
            streamWriter.Write(text);
        }
    }

    public static void ShowRequestProperties1(HttpListenerRequest request)
    {
        // Display the MIME types that can be used in the response.
        string[] types = request.AcceptTypes;
        if (types != null)
        {
            Console.WriteLine("Acceptable MIME types:");
            foreach (string s in types)
            {
                Console.WriteLine(s);
            }
        }
        // Display the language preferences for the response.
        types = request.UserLanguages;
        if (types != null)
        {
            Console.WriteLine("Acceptable natural languages:");
            foreach (string l in types)
            {
                Console.WriteLine(l);
            }
        }

        // Display the URL used by the client.
        Console.WriteLine("URL: {0}", request.Url.OriginalString);
        Console.WriteLine("Raw URL: {0}", request.RawUrl);
        Console.WriteLine("Query: {0}", request.QueryString);

        // Display the referring URI.
        Console.WriteLine("Referred by: {0}", request.UrlReferrer);

        //Display the HTTP method.
        Console.WriteLine("HTTP Method: {0}", request.HttpMethod);
        //Display the host information specified by the client;
        Console.WriteLine("Host name: {0}", request.UserHostName);
        Console.WriteLine("Host address: {0}", request.UserHostAddress);
        Console.WriteLine("User agent: {0}", request.UserAgent);
    }
}
