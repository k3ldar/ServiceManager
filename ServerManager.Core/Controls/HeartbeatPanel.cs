/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  Service Manager is distributed under the GNU General Public License version 3 and  
 *  is also available under alternative licenses negotiated directly with Simon Carter.  
 *  If you obtained Service Manager under the GPL, then the GPL applies to all loadable 
 *  Service Manager modules used on your system as well. The GPL (version 3) is 
 *  available at https://opensource.org/licenses/GPL-3.0
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *  See the GNU General Public License for more details.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2010 - 2018 Simon Carter.  All Rights Reserved.
 *
 *  Product:  Service Manager
 *  
 *  File: HeartbeatPanel.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  07/07/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

#pragma warning disable IDE0044

namespace ServiceManager.Core.Controls
{
    public class HeartbeatPanel : Panel
    {
        #region Private Members

        private Pen _primaryPen;
        private Pen _secondaryPen;
        private Brush _backGroundBrush;
        private Brush _primaryBrush;
        private Brush _secondaryBrush;
        private Queue<int> _points;
        private float _highestPoint;

        #endregion Private Members

        #region Constructors

        public HeartbeatPanel()
        {
            BackGround = Color.LightCyan;
            PrimaryColor = Color.SlateGray;
            SecondaryColor = Color.BlueViolet;
            MaximumPoints = 60;
            _points = new Queue<int>(MaximumPoints);

            _primaryBrush = new SolidBrush(PrimaryColor);
            _secondaryBrush = new SolidBrush(SecondaryColor);
            _backGroundBrush = new SolidBrush(BackGround);
            _primaryPen = new Pen(_primaryBrush);
            _secondaryPen = new Pen(_secondaryBrush);
            DoubleBuffered = true;
        }

        public HeartbeatPanel(Color backGround, Color primary, Color secondary)
        {
            BackGround = backGround;
            PrimaryColor = primary;
            SecondaryColor = secondary;
            MaximumPoints = 60;
            _points = new Queue<int>(MaximumPoints);

            _primaryBrush = new SolidBrush(PrimaryColor);
            _secondaryBrush = new SolidBrush(SecondaryColor);
            _backGroundBrush = new SolidBrush(BackGround);
            _primaryPen = new Pen(_primaryBrush);
            _secondaryPen = new Pen(_secondaryBrush);
            DoubleBuffered = true;
        }

        public HeartbeatPanel(Color backGround, Color primary, Color secondary, int maximumPoints)
        {
            BackGround = backGround;
            PrimaryColor = primary;
            SecondaryColor = secondary;
            MaximumPoints = maximumPoints;
            _points = new Queue<int>(MaximumPoints);

            _primaryBrush = new SolidBrush(PrimaryColor);
            _secondaryBrush = new SolidBrush(SecondaryColor);
            _backGroundBrush = new SolidBrush(BackGround);
            _primaryPen = new Pen(_primaryBrush);
            _secondaryPen = new Pen(_secondaryBrush);
            DoubleBuffered = true;
            AutoPoints = false;
        }

        #endregion Constructors

        #region Properties

        public Color BackGround { get; set; }

        public Color PrimaryColor { get; set; }

        public Color SecondaryColor { get; set; }

        public int MaximumPoints { get; set; }

        public bool AutoPoints { get; set; }

        #endregion Properties

        #region Public Methods

        public void AddPoint(int point)
        {
            if (!AutoPoints && (point < 0 || point > 100))
                throw new ArgumentOutOfRangeException(nameof(point));

            if (_points.Count >= MaximumPoints)
                _points.Dequeue();

            _points.Enqueue(point);

            if (AutoPoints)
            {
                _highestPoint = 0;

                foreach (int pt in _points)
                {
                    if (pt > _highestPoint)
                        _highestPoint = pt;
                }
            }

            Invalidate();
        }

        #endregion Public Methods

        #region Overridden Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawHeartBeat(e.Graphics, e.ClipRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_primaryPen != null)
                    _primaryPen.Dispose();

                if (_primaryBrush != null)
                    _primaryBrush.Dispose();

                if (_secondaryBrush != null)
                    _secondaryBrush.Dispose();

                if (_backGroundBrush != null)
                    _backGroundBrush.Dispose();

                if (_secondaryPen != null)
                    _secondaryPen.Dispose();
            }
        }

        #endregion Overridden Methods

        #region Private Methods


        private void DrawHeartBeat(in Graphics graphics, in Rectangle rectangle)
        {
            int[] points = _points.ToArray();
            Rectangle fullArea = new Rectangle(0, 0, rectangle.Width - 1, rectangle.Height - 1);
            SmoothingMode previousSmoothingMode = graphics.SmoothingMode;

            if (MaximumPoints < 1)
                MaximumPoints = points.Length - 1;

            float pointWidth = (float)fullArea.Width / MaximumPoints;
            float pointHeight = (float)fullArea.Height / (AutoPoints ? _highestPoint == 0 ? 100 : _highestPoint : 100);

            graphics.FillRectangle(_backGroundBrush, rectangle);
            graphics.DrawRectangle(_primaryPen, fullArea);

            if (points.Length == 0)
                return;

            float leftPos = (fullArea.Width - (points.Length * pointWidth)) + pointWidth;
            PointF lastPosition = new PointF(leftPos, fullArea.Height - (pointHeight * points[0]));

            if (graphics.SmoothingMode != SmoothingMode.AntiAlias)
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                for (int i = 0; i < points.Length; i++)
                {
                    PointF currentPosition = new PointF(leftPos, fullArea.Height - (pointHeight * points[i]));

                    using (GraphicsPath gp = new GraphicsPath(FillMode.Winding))
                    {
                        gp.AddLine(lastPosition, currentPosition);
                        gp.AddLine(lastPosition.X, lastPosition.Y, lastPosition.X, fullArea.Height);
                        gp.AddLine(lastPosition.X, fullArea.Height, currentPosition.X, fullArea.Height);
                        gp.AddLine(currentPosition.X, fullArea.Height, currentPosition.X, currentPosition.Y);
                        graphics.DrawPath(_secondaryPen, gp);
                        graphics.FillPath(_secondaryBrush, gp);
                    }

                    graphics.DrawLine(_primaryPen, lastPosition, currentPosition);
                    lastPosition = currentPosition;
                    leftPos += pointWidth;
                }
            }
            finally
            {
                if (graphics.SmoothingMode != previousSmoothingMode)
                    graphics.SmoothingMode = previousSmoothingMode;
            }
        }
    

        // from https://stackoverflow.com/questions/21001202/how-do-i-fill-everything-over-a-straight-line-and-under-a-curve
        //private List<GraphicsPath> getPaths(ChartArea ca, Series ser, double limit)
        //{
        //    List<GraphicsPath> paths = new List<GraphicsPath>();
        //    List<PointF> points = new List<PointF>();
        //    int first = 0;
        //    float limitPix = (float)ca.AxisY.ValueToPixelPosition(limit);

        //    for (int i = 0; i < ser.Points.Count; i++)
        //    {
        //        if ((ser.Points[i].YValues[0] > limit) && (i < ser.Points.Count - 1))
        //        {
        //            if (points.Count == 0) first = i;  // remember group start
        //                                               // insert very first point:
        //            if (i == 0) points.Insert(0, new PointF(
        //                 (float)ca.AxisX.ValueToPixelPosition(ser.Points[0].XValue), limitPix));

        //            points.Add(pointfFromDataPoint(ser.Points[i], ca)); // the regular points
        //        }
        //        else
        //        {
        //            if (points.Count > 0)
        //            {
        //                if (first > 0) points.Insert(0, median(
        //                                 pointfFromDataPoint(ser.Points[first - 1], ca),
        //                                 pointfFromDataPoint(ser.Points[first], ca), limitPix));
        //                if (i == ser.Points.Count - 1)
        //                {
        //                    if ((ser.Points[i].YValues[0] > limit))
        //                        points.Add(pointfFromDataPoint(ser.Points[i], ca));
        //                    points.Add(new PointF(
        //                  (float)ca.AxisX.ValueToPixelPosition(ser.Points[i].XValue), limitPix));
        //                }
        //                else
        //                    points.Add(median(pointfFromDataPoint(ser.Points[i - 1], ca),
        //                                 pointfFromDataPoint(ser.Points[i], ca), limitPix));

        //                GraphicsPath gp = new GraphicsPath();
        //                gp.FillMode = FillMode.Winding;
        //                gp.AddLines(points.ToArray());
        //                gp.CloseFigure();
        //                paths.Add(gp);
        //                points.Clear();
        //            }
        //        }
        //    }
        //    return paths;
        //}

        //PointF pointfFromDataPoint(DataPoint dp, ChartArea ca)
        //{
        //    return new PointF((float)ca.AxisX.ValueToPixelPosition(dp.XValue),
        //                       (float)ca.AxisY.ValueToPixelPosition(dp.YValues[0]));
        //}

        //PointF median(PointF p1, PointF p2, float y0)
        //{
        //    float x0 = p2.X - (p2.X - p1.X) * (p2.Y - y0) / (p2.Y - p1.Y);
        //    return new PointF(x0, y0);
        //}

        #endregion Private Methods
    }
}
