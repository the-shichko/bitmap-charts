using System;
using System.Collections.Generic;
using System.Drawing;
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
                DivisionAxisY = 30,
                ChartFont = new Font("Bahnschrift", 20),
                AxisPen = new Pen(Brushes.Black, 3),
                ChartPen = new Pen(Brushes.DodgerBlue, 5),
                RaduisPoint = 15,
                EllipseColor = Brushes.RoyalBlue,
                DivisionColor = Brushes.Gray,
                TextColor = Brushes.Black,
                BackgroundColor = Brushes.WhiteSmoke,
            };

            chartService.GetChart(new List<int>{30, 150, 190, 68, 69, 100, 78, 40, 54, 23, 98, 66, 140}).Save(Directory.GetCurrentDirectory() + "\\chart.png");

            Console.WriteLine("Completed");
            await Task.Delay(-1);
        }
    }
}