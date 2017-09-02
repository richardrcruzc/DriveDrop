using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";


            var tetString = "";

            var client = new HttpClient();
            // Create a task and supply a user delegate by using a lambda expression. 
            Task taskA = new Task(async () =>
            {
            var address = "5215 90th ste e 98446 tacoma wa";

            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = await client.GetStringAsync(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address)));

                var msg =  stringTask;
                Console.Write(msg);
                tetString = msg;
                //dynamic jsonResponse = JsonConvert.DeserializeObject(msg);

                Console.WriteLine("Hello from taskA.");

            });

            // Start the task.
            taskA.Start();

            // Output a message from the calling thread.
            Console.WriteLine("Hello from thread '{0}'.",
                              Thread.CurrentThread.Name);
            taskA.Wait();
            // create service collection
            //var serviceCollection = new ServiceCollection();

            //IHttpClient _apiClient = typeof(IHttpClient);


            //var address = "5215 90th ste e 98446 tacoma wa";
            //var requestUri = string.Format("https://maps.googleapis.com/maps/api/js?key={0}&address={1}", Uri.EscapeDataString(address));

            //var currentDataString =   _apiClient.GetStringAsync(requestUri);
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    // Do something
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
