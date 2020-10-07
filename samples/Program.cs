using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using bitmap_chats.Services;

namespace samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var chartService = new ChartService<TestModel>(1300, 1000)
            {
                DivisionAxisX = 5,
                DivisionAxisY = 15,
                ChartFont = new Font("Bahnschrift", 20),
                AxisPen = new Pen(Brushes.Black, 3),
                ChartPen = new Pen(Brushes.DodgerBlue, 5),
                RaduisPoint = 15,
                EllipseColor = Brushes.RoyalBlue,
                DivisionColor = Brushes.RoyalBlue,
                TextColor = Brushes.Black,
                BackgroundColor = Brushes.WhiteSmoke,
                ChartTitle = "Test nick @the_shichko.exe"
            };

            chartService.Items = new List<TestModel>()
            {
                new TestModel() {Value = 30.4, Name = "Name"},
                new TestModel() {Value = 50.7, Name = "Name"},
                new TestModel() {Value = 90.3, Name = "Name"},
                new TestModel() {Value = 70.4, Name = "Name"},
                new TestModel() {Value = 34.1, Name = "Name"},
                new TestModel() {Value = 47.6, Name = "Name"},
                new TestModel() {Value = 89.4, Name = "Name"},
                new TestModel() {Value = 54.9, Name = "Name"},
                new TestModel() {Value = 52.2, Name = "Name"},
                new TestModel() {Value = 78.3, Name = "Name"},
                new TestModel() {Value = 98.45, Name = "Name"},
                new TestModel() {Value = 12.67, Name = "Name"},
                new TestModel() {Value = 16.4, Name = "Name"},
                new TestModel() {Value = 90.8, Name = "Name"},
            };
            chartService.GetChart(x => x.Value)
                .Save(Directory.GetCurrentDirectory() + "\\chart.png");

            Console.WriteLine("Completed");
            await Task.Delay(-1);
        }
    }
}