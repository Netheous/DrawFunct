using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NCalc;
using System.Diagnostics;

namespace ElegantForms
{
    class ElegantForm : Form
    {
        public PointF[] points;
        public int start;
        public float staging;
        public int end;
        public float scale = 1f;
        public string PreParsedFormula;
        public int extrax, extray;
        public bool Unreals = false;
        public bool Asymptotis = false;

        public ElegantForm()
        {
            DoubleBuffered = true;
            extrax = 0;
            extray = 0;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if( e.KeyChar == ' ' )
            {
                scale = 1f;
                extrax = 0;
                extray = 0;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            scale += (e.Delta / 120);
            Invalidate();
        }

        private Point MouseDownLocation;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if( e.Button == MouseButtons.Left )
            {
                MouseDownLocation = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                extrax += e.X - MouseDownLocation.X;
                extray += e.Y - MouseDownLocation.Y;
                MouseDownLocation = e.Location;

                Invalidate();
            }

            Invalidate();
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        static Font infiFont = new Font("Arial", 8);
        static Font pointFont = new Font("Arial", 12);
        static Font formulaFont = new Font("Arial", 16);

        static Color backgroundColor = Color.FromArgb(30, 30, 30);
        static Color lineColor = Color.FromArgb(153, 255, 205);
        static Color angColor = Color.FromArgb(65, 65, 65);
        static Color gridColor = Color.FromArgb(45, 45, 45);

        static Rectangle rect;
        static Pen pen = new Pen(lineColor, 1);
        static Pen valueLine = new Pen(Color.FromArgb(255, 89, 89), 1);
        static Pen infipen = new Pen(Color.FromArgb(90, 90, 255), 1);
        static Pen angpen = new Pen(angColor, 1);
        static Pen gridpen = new Pen(gridColor, 1);
        static SolidBrush brush = new SolidBrush(backgroundColor);
        protected override void OnPaint(PaintEventArgs e)
        {

            rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
            e.Graphics.FillRectangle(brush, rect);

            for ( float stage = start; stage <= end; stage+=staging )
            {
                e.Graphics.DrawLine(gridpen, new PointF(extrax + Size.Width / 2 + stage * scale, extray + Size.Height / 2 + (start * scale) ), new PointF(extrax + Size.Width / 2 + stage * scale, extray + Size.Height / 2 + (end * scale)));
                e.Graphics.DrawLine(gridpen, new PointF(extrax + Size.Width / 2 + (start * scale), extray + Size.Height / 2 + stage * scale), new PointF(extrax + Size.Width / 2 + (end * scale), extray + Size.Height / 2 + stage * scale));
            }

            e.Graphics.DrawLine(angpen, new PointF(extrax + Size.Width / 2, extray + Size.Height / 2 + (start * scale)), new PointF(extrax + Size.Width / 2, extray + Size.Height / 2 + (end * scale)));
            e.Graphics.DrawLine(angpen, new PointF(extrax + Size.Width / 2 + (start * scale), extray + Size.Height / 2), new PointF(extrax + Size.Width / 2 + (end * scale), extray + Size.Height / 2 ));

            if (points.Length > 0)
            {
                for (int n = 1; n < points.Length; n++)
                {
                    if ( Double.IsInfinity(points[n].Y) || Double.IsInfinity( points[n - 1].Y ) )
                    {
                        PointF blockf, blockt;
                        if (Double.IsInfinity(points[n].Y))
                        {
                            Asymptotis = true;
                            blockf = new PointF(extrax + Size.Width / 2 - points[n - 1].X * scale - 5, extray + Size.Height / 2 - points[n - 1].Y * scale);
                            blockt = new PointF(extrax + Size.Width / 2 - points[n - 1].X * scale + 5, extray + Size.Height / 2 - points[n - 1].Y * scale);
                        }
                        else
                        {
                            Asymptotis = true;
                            blockf = new PointF(extrax + Size.Width / 2 - points[n].X * scale - 5, extray + Size.Height / 2 - points[n].Y * scale);
                            blockt = new PointF(extrax + Size.Width / 2 - points[n].X * scale + 5, extray + Size.Height / 2 - points[n].Y * scale);
                        }
                        e.Graphics.DrawLine(infipen, blockf, blockt);
                        continue;
                    }
                    PointF p = new PointF(extrax + Size.Width / 2 - Clamp( points[n - 1].X, -10000000, 10000000) * scale, extray + Size.Height / 2 + Clamp(points[n - 1].Y, -10000000, 10000000) * scale);
                    PointF pe = new PointF(extrax + Size.Width / 2 - Clamp(points[n].X, -10000000, 10000000) * scale, extray + Size.Height / 2 + Clamp(points[n].Y, -10000000, 10000000) * scale);

                    e.Graphics.DrawLine(pen, p, pe);
                }
            }

            //info
            e.Graphics.DrawString($"Zakres(x/y): { start }/{ end }", infiFont, Brushes.Gold, new PointF(5, 5));
            e.Graphics.DrawString($"Dokladnosc(x/y): { staging }", infiFont, Brushes.Gold, new PointF(5, 15));
            e.Graphics.DrawString($"Skala: { scale }", infiFont, Brushes.Gold, new PointF(5, 25));

            //controls
            e.Graphics.DrawString("Zoom [koło myszy]", infiFont, Brushes.Peru, new PointF(5, Size.Height / 2 + 5));
            e.Graphics.DrawString("Przesuwanie [LMB + Przeciąganie]", infiFont, Brushes.Peru, new PointF(5, Size.Height / 2 + 15));
            e.Graphics.DrawString("Zeruj [Spacja]", infiFont, Brushes.Peru, new PointF(5, Size.Height / 2 + 25));

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //formula
            e.Graphics.DrawString($"F(x) = {PreParsedFormula}", formulaFont, Brushes.Peru, new PointF(Size.Width / 2, 15), stringFormat);

            if( Unreals )
            {
                e.Graphics.DrawString("*liczby nierealne (zawęź zakres)", pointFont, Brushes.IndianRed, new PointF(Size.Width / 2, 35), stringFormat);
            }

            if(Asymptotis)
            {
                e.Graphics.DrawString("*asymptoty (w przypadku łączenia asymptot - zwiększ dokładność)", pointFont, Brushes.IndianRed, new PointF(Size.Width / 2, 50), stringFormat);
            }

            //draw values
            float originx = extrax + Size.Width / 2 + (start * scale);
            float dist = Cursor.Position.X - originx;
            dist = (float)Math.Floor( dist / (staging * scale) ) * staging;
            dist = originx + dist * scale;

            int selectedx = (int)((dist - originx) / (staging * scale));
            selectedx = selectedx > points.Length - 1 ? points.Length - 1 : selectedx;
            selectedx = selectedx < 0 ? 0 : selectedx;

            PointF from = new PointF( dist, extray + Size.Height / 2);
            PointF to = new PointF( dist, extray + Size.Height / 2 + Clamp(points[selectedx].Y * scale, -10000000, 10000000));
            e.Graphics.DrawLine(valueLine, from, to);
            e.Graphics.DrawString($"({selectedx}, {-points[selectedx].Y})", pointFont, Brushes.Green, to, stringFormat);
        }
    }
}
