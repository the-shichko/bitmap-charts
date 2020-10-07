using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using bitmap_chats.Services;

namespace samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var chartService = new ChartService<int>(1300,1000)
            {
                DivisionAxisX = 1,
                DivisionAxisY = 25
            };

            chartService.Draw(new List<int>{30, 15, 45, 60, 122, 150, 190}).Save(Directory.GetCurrentDirectory() + "\\chart.png");

            Console.WriteLine("Completed");
            await Task.Delay(-1);
        }
    }
}