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
                DivisionAxisX = 4,
                DivisionAxisY = 30
            };

            chartService.Draw(new List<int>{30, 150, 190, 68, 69, 100, 78, 40, 54, 23, 98, 66, 140}).Save(Directory.GetCurrentDirectory() + "\\chart.png");

            Console.WriteLine("Completed");
            await Task.Delay(-1);
        }
    }
}