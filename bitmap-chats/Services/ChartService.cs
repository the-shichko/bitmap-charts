using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

namespace bitmap_chats.Services
{
    public class ChartService<T>
    {
        public int DivisionAxisX { get; set; } = 1;
        public int DivisionAxisY { get; set; } = 1;
        private readonly int _width;
        private readonly int _height;
        private const int Margin = 50;
        private readonly Font _chartFont = new Font("Bahnschrift", 20);

        public IEnumerable<T> Items { get; set; }
        private readonly Pen _axisPen = new Pen(Brushes.Black, 3);
        private readonly Pen _chartPen = new Pen(Brushes.DodgerBlue, 5);
        private readonly int _raduisPoint = 15;
        private readonly Brush _ellipseColor = Brushes.RoyalBlue;
        private readonly Brush _divisionColor = Brushes.Black;
        private Point _start;
        private Point _end;

        public ChartService(int width, int height)
        {
            _width = width;
            _height = height;

            _start = new Point(Margin, height - Margin);
            _end = new Point(width - Margin, Margin);
        }

        public Bitmap Draw(List<int> items)
        {
            var bitmap = new Bitmap(_width, _height);

            using var graph = Graphics.FromImage(bitmap);
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graph.InterpolationMode = InterpolationMode.High;

            var imageSize = new Rectangle(0, 0, _width, _height);
            graph.FillRectangle(Brushes.White, imageSize);

            graph.DrawLine(_axisPen, Margin, Margin, Margin, _height - Margin);
            graph.DrawLine(_axisPen, Margin, _height - Margin, _width - Margin, _height - Margin);

            var maxValue = items.Max();
            var minValue = items.Min();
            var countValue = items.Count();
            graph.DrawString(countValue.ToString(), _chartFont, Brushes.Black,
                new PointF(_end.X - 7, _start.Y + Margin / 2 - 9));
            graph.DrawString(maxValue.ToString(), _chartFont, Brushes.Black,
                new PointF(_start.X + Margin / 2, _end.Y - 7));

            var chartWidth = Math.Abs(_end.X - _start.X);
            var chartHeight = Math.Abs(_end.Y - _start.Y);

            var divisionHeight = chartHeight / (maxValue - minValue);
            var divisionWidth = chartWidth / countValue;

            #region Draw divisions

            var startDivX = _start.X + divisionWidth;
            for (var i = 1; i < countValue; i += DivisionAxisX)
            {
                graph.FillEllipse(_divisionColor, startDivX - _raduisPoint / 2, _start.Y - _raduisPoint / 2, _raduisPoint,
                    _raduisPoint);
                graph.DrawString(i.ToString(), _chartFont, Brushes.Black,
                    new PointF(startDivX - 7, _start.Y + Margin / 2 - 7));
                startDivX += divisionWidth * DivisionAxisX;
            }

            var startDivY = _start.Y - divisionHeight * DivisionAxisY;
            for (var i = minValue; i <= maxValue; i += DivisionAxisY)
            {
                graph.FillEllipse(_divisionColor, _start.X - _raduisPoint / 2, startDivY - _raduisPoint / 2, _raduisPoint,
                    _raduisPoint);
                graph.DrawString(i.ToString(), _chartFont, Brushes.Black,
                    new PointF(_start.X + Margin / 2, startDivY - 10));
                startDivY -= divisionHeight * DivisionAxisY;
            }

            #endregion

            #region Draw points

            var prevPoint = new Point();

            var ellipsePoints = new List<Point>();
            for (var i = 0; i < items.Count(); i++)
            {
                var pixelYValue = divisionHeight * items[i] - (divisionHeight * items.Min());
                var pixelXValue = divisionWidth * (i + 1);

                if (i > 0)
                {
                    var currentPoint = new Point(_start.X + pixelXValue, _start.Y - pixelYValue);
                    graph.DrawLine(_chartPen, prevPoint, currentPoint);
                }
                ellipsePoints.Add(new Point(_start.X + pixelXValue, _start.Y - pixelYValue));
                
                prevPoint = new Point(_start.X + pixelXValue, _start.Y - pixelYValue);
            }

            foreach (var point in ellipsePoints)
            {
                graph.FillEllipse(_ellipseColor, point.X - _raduisPoint / 2,
                    point.Y - _raduisPoint / 2, _raduisPoint, _raduisPoint);
            }

            #endregion

            return bitmap;
        }
    }
}