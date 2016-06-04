using System;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace HK_internship
{
public class Proxy
{
        public static string swagger;
        public static void SimpleListenerExample(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            string proxyAddress = "http://localhost:5000";
            if (prefixes[1] != null)
            {
                proxyAddress = prefixes[1];
                HttpWebRequest t = (HttpWebRequest)WebRequest.Create(proxyAddress + "/swagger/v1/swagger.json");
                HttpWebResponse r = (HttpWebResponse)t.GetResponse();
                swagger = new StreamReader(r.GetResponseStream()).ReadToEnd();
            }
            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            listener.Prefixes.Add(prefixes[0]);
            Generate.Initializer();
            listener.Start();
            while (true)
            {
                Console.WriteLine("\nListening...\n");
                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                
                if(request.RawUrl.ToLower().Equals("/generate/csharp"))
                {
                           
                    HttpListenerResponse response1 = context.Response;
                    // Construct a response.
                    string responseString1 = Generate.generate();
                    Console.WriteLine("Code Generated\n");
                    byte[] buffer1 = Encoding.UTF8.GetBytes(responseString1);
                    // Get a response stream and write the response to it.
                    response1.ContentLength64 = buffer1.Length;
                    Stream output1 = response1.OutputStream;
                    output1.Write(buffer1, 0, buffer1.Length);
                    continue;
                }
                // Obtain a response object.
                string method = request.HttpMethod;
                HttpWebRequest pRequest;
                string k;
                HttpWebResponse res = null;
                Record record = new Record();
                record.req.path = request.RawUrl;
                pRequest = (HttpWebRequest)WebRequest.Create(proxyAddress + request.RawUrl);
              
                foreach (var x in request.Headers.Keys)
                {
                    string key = x.ToString();
                    string value = request.Headers.Get(x.ToString());
                    if (!WebHeaderCollection.IsRestricted(x.ToString()))
                        pRequest.Headers.Add(key,value);
                    record.req.Headers.Add(key, value);
                }

                pRequest.Method = method;
                record.req.method = method;
                pRequest.ContentType = "application/json";
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new System.IO.StreamReader(body, encoding);
                string Body = reader.ReadToEnd();
                record.req.body = Body;
                reader.Close();
                if(!method.Equals("GET"))
                    pRequest.GetRequestStream().Write(Encoding.ASCII.GetBytes(Body), 0, Body.Length);
                try
                {
                    res = (HttpWebResponse)pRequest.GetResponse();
                    k = new StreamReader(res.GetResponseStream()).ReadToEnd();
                    record.res.statusCode = (int)res.StatusCode;
                    record.res.body = k;
                    //to record request and response
                    MemoryStream ms = new MemoryStream();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Record));
                    ser.WriteObject(ms, record);
                    byte[] json = ms.ToArray();
                    ms.Close();
                    string rec = Encoding.UTF8.GetString(json, 0, json.Length).Replace("\\/", "/");
                    rec = rec.Replace("\u000d", "\r");
                    rec = rec.Replace("\u000a", ",");
                    StreamWriter file = File.AppendText(@"Generates.json");
                    file.WriteLine(rec);
                    file.Close();
                    Console.WriteLine("Recorded...\n");
                }
                catch(Exception e)
                {
                    k = "ERROR 404";
                }

                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString = k;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
                
            }
            
        }
        
        public static Record decode(string input)
        {
            Record deserializedUser = new Record();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as Record;
            ms.Close();
            return deserializedUser;
        }  
}
    [DataContract]
    public class Record
    {
        public Record()
        {
            req = new Request();
            res = new Response();
        }
        [DataMember(Name = "request")]
        public Request req;
        [DataMember(Name = "response")]
        public Response res; 
    }

    [DataContract]
    public class Response
    {
        public Response() { }
        [DataMember (Name = "statusCode")]
        public int statusCode;
        [DataMember(Name = "body")]
        public string body;
    }

    [DataContract]
    public class Request
    {
        public Request() { }
        [DataMember(Name = "method")]
        public string method;
        [DataMember(Name = "path")]
        public string path;
        [DataMember(Name = "body")]
        public string body;
        [DataMember(Name = "Headers")]
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
    }
}

