using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = "http://localhost:8000/";

            var fileInfo = new FileInfo("EE.txt");
            var directoryInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "indexes"));
            if(fileInfo.Exists == false)
            {
                Console.WriteLine("Unable to locate sample input file. Cannot start CodingChallenge Web API...");
            }
            if(directoryInfo.Exists == false)
            {
                directoryInfo.Delete(true);
                directoryInfo.Create();
            }
            else
            {
                using (WebApp.Start(baseAddress, builder => new Startup(fileInfo, directoryInfo).ConfigureApp(builder)))
                {
                    Console.WriteLine("Started CodingChallenge Web API...");
                    Console.WriteLine("***********************Starting Test***************************");
                    Console.WriteLine();
                    Console.WriteLine();

                    var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(baseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var zipCodeSearchResult = httpClient.PostAsJsonAsync("api/zipcode/search/4", 
                        new ZipCodeSearchOptions { Take = 10 }).GetAwaiter().GetResult();
                    if(zipCodeSearchResult.IsSuccessStatusCode == false)
                    {
                        Console.WriteLine("Zipcode search API :: [{0}] failed with status code :: [{1}]", 
                            "api/zipcode/search/{searchTerm = 4}", zipCodeSearchResult.StatusCode);
                    } 
                    else
                    {
                        var result = zipCodeSearchResult.Content.ReadAsAsync<ZipCodeSearchResult>().GetAwaiter().GetResult();
                        Console.WriteLine("Zipcode search API :: [{0}] succeeded and returned [{1}] records and continuation token [{2}]. The records are as below::",
                            "api/zipcode/search/{searchTerm = 4}", result.ZipCodes.Count(), result.ContinuationToken);
                        foreach(var zipCode in result.ZipCodes)
                        {
                            Console.WriteLine(zipCode);
                        }
                    }

                    var locationSearchResult = httpClient.PostAsJsonAsync("api/location/search/v",
                        new LocationSearchOptions { Take = 10 }).GetAwaiter().GetResult();
                    if (locationSearchResult.IsSuccessStatusCode == false)
                    {
                        Console.WriteLine("Location search API :: [{0}] failed with status code :: [{1}]",
                            "api/location/search/{searchTerm = v}", locationSearchResult.StatusCode);
                    }
                    else
                    {
                        var result = locationSearchResult.Content.ReadAsAsync<LocationSearchResult>().GetAwaiter().GetResult();
                        Console.WriteLine("Location search API :: [{0}] succeeded and returned [{1}] records and continuation token [{2}]. The records are as below::",
                            "api/location/search/{searchTerm = v}", result.Locations.Count(), result.ContinuationToken);
                        foreach (var location in result.Locations)
                        {
                            Console.WriteLine(location);
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("***********************Completed Test***************************");
                    Console.WriteLine("Press Enter to quit.");

                    Console.ReadLine();
                }
            }
        }
    }
}
