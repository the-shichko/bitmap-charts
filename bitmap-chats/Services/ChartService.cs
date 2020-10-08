using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;

namespace bitmap_chats.Services
{
    public class ChartService<T>
    {
        public int DivisionAxisX { get; set; } = 1;
        public int DivisionAxisY { get; set; } = 1;
        public Font ChartFont { get; set; } = new Font("Bahnschrift", 20);
        public Pen AxisPen { get; set; } = new Pen(Brushes.Black, 1);
        public Pen ChartPen { get; set; } = new Pen(Brushes.Black, 5);
        public int RaduisPoint { get; set; } = 10;
        public Brush EllipseColor { get; set; } = Brushes.RoyalBlue;
        public Brush DivisionColor { get; set; } = Brushes.Black;
        public Brush TextColor { get; set; } = Brushes.Black;
        public Brush BackgroundColor { get; set; } = Brushes.White;
        private int _paddingLeft = 50;
        public string ChartTitle { get; set; } = "Chart1";
        public IEnumerable<T> Items { get; set; }

        private readonly int _width;
        private readonly int _height;
        private const int Margin = 50;
        private Point _start;
        private Point _end;

        public ChartService(int width, int height)
        {
            _width = width;
            _height = height;

            _start = new Point(Margin + _paddingLeft, height - Margin);
            _end = new Point(width - Margin, Margin + 100);
        }

        public Bitmap GetChart(ChartMode mode, Func<T, double> valueAxisY, Func<T, object> valueAxisX)
        {
            return mode switch
            {
                ChartMode.LineMode => GetLineChart(valueAxisY, valueAxisX),
                ChartMode.RectangleMode => GetRectangleChart(valueAxisY, valueAxisX),
                _ => null
            };
        }

        private Bitmap GetLineChart(Func<T, double> valueAxisY, Func<T, object> valueAxisX)
        {
            var bitmap = new Bitmap(_width, _height);

            using var graph = Graphics.FromImage(bitmap);
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graph.InterpolationMode = InterpolationMode.High;

            var imageSize = new Rectangle(0, 0, _width, _height);
            graph.FillRectangle(BackgroundColor, imageSize);

            //title
            graph.DrawString(ChartTitle, new Font(ChartFont.FontFamily, (int) (ChartFont.Size * 1.5)), TextColor,
                _width / 2 - (ChartTitle.Length / 2 * 18), 30);

            graph.DrawLine(AxisPen, Margin, Margin + 100, Margin, _height - Margin);
            graph.DrawLine(AxisPen, Margin, _height - Margin, _width - Margin, _height - Margin);

            var maxValue = Math.Ceiling(Items.Max(valueAxisY));
            var minValue = (int) Items.Min(valueAxisY);
            var countValue = Items.Count();

            var chartWidth = Math.Abs(_end.X - _start.X - 50);
            var chartHeight = Math.Abs(_end.Y - (_start.Y - 50));

            var divisionHeight = chartHeight / (maxValue - minValue);
            var divisionWidth = chartWidth / countValue;

            #region Draw divisions

            var startDivX = _start.X + divisionWidth;
            foreach (var item in Items.Select(valueAxisX))
            {
                graph.FillEllipse(DivisionColor, startDivX - RaduisPoint / 2, _start.Y - RaduisPoint / 2,
                    RaduisPoint,
                    RaduisPoint);
                graph.DrawString(item.ToString(), ChartFont, TextColor,
                    new PointF(startDivX - 7, _start.Y + Margin / 2 - 7));
                startDivX += divisionWidth * DivisionAxisX;
            }

            var startDivY = _start.Y - 25;
            for (var i = minValue; i <= maxValue; i += DivisionAxisY)
            {
                graph.FillEllipse(DivisionColor, _start.X - _paddingLeft - RaduisPoint / 2, startDivY - RaduisPoint / 2,
                    RaduisPoint,
                    RaduisPoint);
                graph.DrawString(i.ToString(CultureInfo.InvariantCulture), ChartFont, TextColor,
                    new PointF(_start.X - _paddingLeft + Margin / 2, startDivY - 10));
                startDivY -= (int) (divisionHeight * DivisionAxisY);
            }

            #endregion

            #region Draw points

            var prevPoint = new Point();

            var ellipsePoints = new List<PointModel>();
            for (var i = 0; i < Items.Count(); i++)
            {
                var pixelYValue = divisionHeight * Items.Select(valueAxisY).ToList()[i] -
                    divisionHeight * Items.Min(valueAxisY) + 25;
                var pixelXValue = divisionWidth * (i + 1);

                if (i > 0)
                {
                    var currentPoint = new Point(_start.X + pixelXValue, (int) (_start.Y - pixelYValue));
                    graph.DrawLine(ChartPen, prevPoint, currentPoint);
                }

                ellipsePoints.Add(new PointModel()
                {
                    Point = new Point(_start.X + pixelXValue, (int) (_start.Y - pixelYValue)),
                    Value = Items.Select(valueAxisY).ToList()[i]
                });

                prevPoint = new Point(_start.X + pixelXValue, (int) (_start.Y - pixelYValue));
            }

            foreach (var pointModel in ellipsePoints)
            {
                graph.FillEllipse(EllipseColor, pointModel.Point.X - RaduisPoint / 2,
                    pointModel.Point.Y - RaduisPoint / 2, RaduisPoint, RaduisPoint);
                graph.DrawString($"({pointModel.Value})", ChartFont, TextColor,
                    pointModel.Point.X -
                    ChartFont.Size * pointModel.Value.ToString(CultureInfo.InvariantCulture).Length,
                    pointModel.Point.Y - ChartFont.Size - 15);
            }

            #endregion

            return bitmap;
        }

        private Bitmap GetRectangleChart(Func<T, double> valueAxisY, Func<T, object> valueAxisX)
        {
            var bitmap = new Bitmap(_width, _height);

            using var graph = Graphics.FromImage(bitmap);
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graph.InterpolationMode = InterpolationMode.High;

            var imageSize = new Rectangle(0, 0, _width, _height);
            graph.FillRectangle(BackgroundColor, imageSize);

            //title
            graph.DrawString(ChartTitle, new Font(ChartFont.FontFamily, (int) (ChartFont.Size * 1.5)), TextColor,
                _width / 2 - (ChartTitle.Length / 2 * 18), 30);

            graph.DrawLine(AxisPen, Margin, Margin + 100, Margin, _height - Margin);
            graph.DrawLine(AxisPen, Margin, _height - Margin, _width - Margin, _height - Margin);

            var maxValue = Math.Ceiling(Items.Max(valueAxisY));
            var minValue = (int) Items.Min(valueAxisY);
            var countValue = Items.Count();

            var chartWidth = Math.Abs(_end.X - _start.X - 50);
            var chartHeight = Math.Abs(_end.Y - (_start.Y - 50));

            var divisionHeight = chartHeight / (maxValue - minValue);
            var divisionWidth = chartWidth / countValue;

            #region Draw divisions

            var startDivX = _start.X + divisionWidth;
            foreach (var item in Items.Select(valueAxisX))
            {
                graph.FillEllipse(DivisionColor, startDivX - RaduisPoint / 2, _start.Y - RaduisPoint / 2,
                    RaduisPoint,
                    RaduisPoint);
                graph.DrawString(item.ToString(), ChartFont, TextColor,
                    new PointF(startDivX - 7, _start.Y + Margin / 2 - 7));
                startDivX += divisionWidth * DivisionAxisX;
            }

            var startDivY = _start.Y - 25;
            for (var i = minValue; i <= maxValue; i += DivisionAxisY)
            {
                graph.FillEllipse(DivisionColor, _start.X - _paddingLeft - RaduisPoint / 2, startDivY - RaduisPoint / 2,
                    RaduisPoint,
                    RaduisPoint);
                graph.DrawString(i.ToString(CultureInfo.InvariantCulture), ChartFont, TextColor,
                    new PointF(_start.X - _paddingLeft + Margin / 2, startDivY - 10));
                startDivY -= (int) (divisionHeight * DivisionAxisY);
            }

            #endregion

            #region Draw rectangles

            for (var i = 0; i < Items.Count(); i++)
            {
                var itemValue = Items.Select(valueAxisY).ToList()[i];
                var pixelYValue = divisionHeight * itemValue -
                    divisionHeight * Items.Min(valueAxisY) + 25;
                var pixelXValue = divisionWidth * (i + 1);
                const int borderWidth = 2;

                graph.FillRectangle(EllipseColor, _start.X + pixelXValue - divisionWidth / 2,
                    _start.Y - (int) pixelYValue,
                    divisionWidth, (int) pixelYValue - borderWidth);

                var commonX = _start.X + pixelXValue - divisionWidth / 2;
                graph.DrawLine(new Pen(BackgroundColor, borderWidth), commonX,
                    _start.Y - borderWidth, commonX, _start.Y - (int) pixelYValue);
                graph.DrawLine(new Pen(BackgroundColor, borderWidth), commonX,
                    _start.Y - (int) pixelYValue, commonX + divisionWidth,
                    _start.Y - (int) pixelYValue);
                graph.DrawLine(new Pen(BackgroundColor, borderWidth),
                    commonX + divisionWidth,
                    _start.Y - (int) pixelYValue, commonX + divisionWidth,
                    _start.Y - borderWidth);

                graph.DrawString(itemValue.ToString(CultureInfo.InvariantCulture), ChartFont, TextColor, commonX,
                    _start.Y - (int) pixelYValue - ChartFont.Size - borderWidth);
            }
            #endregion

            return bitmap;
        }
    }
}