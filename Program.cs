﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace searchfight2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                List<Data> resultsList = new List<Data>();
                for (int i = 0; i < args.Length; i++)
                {
                    Int64 googleValue = 0;
                    Int64 azureValue = 0;
                    SearchFromEngine.GetInfoFromEngine(args[i], ref googleValue, ref azureValue);
                    Data data = new Data();
                    data.TextSearched = args[i];
                    data.GoogleResult = googleValue;
                    data.AzureResult = azureValue;
                    resultsList.Add(data);
                }

                var googleResult = resultsList.OrderByDescending(t => t.GoogleResult).First();
                var azureResult = resultsList.OrderByDescending(t => t.AzureResult).First();
                var totalResult = resultsList.OrderByDescending(t => t.AzureResult + t.GoogleResult).First();

                Console.WriteLine($"Google winner: {googleResult.TextSearched}");
                Console.WriteLine($"MSN Search winner: {azureResult.TextSearched}");
                Console.WriteLine($"Total winner: {totalResult.TextSearched}");

            }
        }
    }
}
