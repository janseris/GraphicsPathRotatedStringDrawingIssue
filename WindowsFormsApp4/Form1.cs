using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        Timer Timer = new Timer()
        {
            Interval = 20
        };

        int Rotation = 0;
        PointF DrawLocation = PointF.Empty;

        public Form1()
        {
            InitializeComponent();
            Timer.Tick += (s, e) => { Rotation++; Rotation %= 360; Redraw(); };
            Timer.Start();
            this.Paint += Form1_Paint;
            this.MouseMove += (s, e) => { DrawLocation = e.Location; Redraw(); };
            this.DoubleBuffered = true; //prevent flickering

            this.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) { if (Timer.Enabled) { Timer.Stop(); } else { Timer.Start(); } } if (e.Button == MouseButtons.Right) { Rotation = 0; Redraw(); } };
        }

        //private void Form1_Paint(object sender, PaintEventArgs e)
        //{
        //    var textDefault = GetTextPath(e.Graphics, StringFormat.GenericDefault);
        //    var textVerticalCenter = GetTextPath(e.Graphics, new StringFormat() { LineAlignment = StringAlignment.Center}); //as provided by reza-aghaei

        //    DrawTextPath(e.Graphics, textDefault); textDefault.Dispose();
        //    DrawTextPath(e.Graphics, textVerticalCenter); textVerticalCenter.Dispose();

        //    e.Graphics.DrawEllipse(new Pen(Color.Black, width: 5), new RectangleF(Position, new Size(1, 1)));
        //}


        //https://docs.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font
        string arrowTopLeftSymbol => ((char)0xE742).ToString(); //points to top left corner
        string arrowTopRightSymbol => ((char)0xE8AD).ToString(); //points to top right corner
        string pinSymbol => ((char)0xE840).ToString(); //points to bottom left corner
        string arrowBottomRightSymbol => ((char)0xE741).ToString(); //points to bottom right corner

        string arrowBottomSymbol => ((char)0xE74B).ToString();
        string arrowTopSymbol => ((char)0xE74A).ToString();
        string arrowLeftSymbol => ((char)0xE72B).ToString();
        string arrowRightSymbol => ((char)0xE72A).ToString();
       
        string centerSymbol => ((char)0xE713).ToString();

        List<string> Symbols => new List<string>
        {
            arrowTopSymbol,
            arrowBottomSymbol,
            arrowTopLeftSymbol,
            arrowTopRightSymbol,
            pinSymbol,
            arrowBottomRightSymbol,
            centerSymbol,
            arrowLeftSymbol,
            arrowRightSymbol,
        };

        public Position GetPosition(string symbol)
        {
            if(symbol == arrowTopLeftSymbol)
            {
                return Position.TopLeft;
            }
            if(symbol == arrowTopRightSymbol)
            {
                return Position.TopRight;
            }
            if (symbol == pinSymbol)
            {
                return Position.BottomLeft;
            }
            if (symbol == arrowBottomRightSymbol)
            {
                return Position.BottomRight;
            }

            if (symbol == arrowBottomSymbol)
            {
                return Position.Bottom;
            }
            if (symbol == arrowTopSymbol)
            {
                return Position.Top;
            }
            if (symbol == arrowLeftSymbol)
            {
                return Position.Left;
            }
            if (symbol == arrowRightSymbol)
            {
                return Position.Right;
            }
            if (symbol == centerSymbol)
            {
                return Position.Center;
            }
            throw new ArgumentException();
        }



        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach(var symbol in Symbols)
            {
                DrawText(e.Graphics, symbol);
            }

            //DrawText(e.Graphics, pinSymbol); //draws OK
            //DrawText(e.Graphics, arrowTopLeftSymbol); //draws OK
            
            //DrawText(e.Graphics, arrowTopRightSymbol);
            //DrawText(e.Graphics, arrowBottomRightSymbol);

            e.Graphics.DrawEllipse(new Pen(Color.Black, width: 5), new RectangleF(DrawLocation, new Size(1, 1)));
        }

        private void ApplyTranslationRelativeToTopLeft(Matrix matrix, SizeF boundingRectangleSize, Position position)
        {
            SizeF topLeftTranslate = new SizeF(0, 0);
            SizeF bottomLeftTranslate = new SizeF(0, -boundingRectangleSize.Height);
            SizeF topRightTranslate = new SizeF(-boundingRectangleSize.Width, 0);
            SizeF bottomRightTranslate = new SizeF(-boundingRectangleSize.Width, -boundingRectangleSize.Height);

            SizeF topTranslate = new SizeF(-boundingRectangleSize.Width / 2f, 0);
            SizeF bottomTranslate = new SizeF(-boundingRectangleSize.Width / 2f, -boundingRectangleSize.Height);

            SizeF leftTranslate = new SizeF(0, -boundingRectangleSize.Height / 2f);
            SizeF rightTranslate = new SizeF(-boundingRectangleSize.Width, -boundingRectangleSize.Height / 2f);

            SizeF centerTranslate = new SizeF(-boundingRectangleSize.Width / 2f, -boundingRectangleSize.Height / 2f);

            SizeF translation;
            switch (position)
            {
                case Position.Top:
                    translation = topTranslate;
                    break;
                case Position.Bottom:
                    translation = bottomTranslate;
                    break;
                case Position.Left:
                    translation = leftTranslate;
                    break;
                case Position.Right:
                    translation = rightTranslate;
                    break;
                case Position.TopLeft:
                    translation = topLeftTranslate;
                    break;
                case Position.TopRight:
                    translation = topRightTranslate;
                    break;
                case Position.BottomLeft:
                    translation = bottomLeftTranslate;
                    break;
                case Position.BottomRight:
                    translation = bottomRightTranslate;
                    break;
                case Position.Center:
                    translation = centerTranslate;
                    break;
                default:
                    throw new ArgumentException();
            }

            matrix.Translate(translation.Width, translation.Height);
        }

        private void DrawText(Graphics g, string text, bool drawBoundingRectangle = false)
        {
            using (var path = new GraphicsPath())
            {
                path.AddString(text, new FontFamily("Segoe MDL2 Assets"), (int)FontStyle.Regular, 120, new PointF(0, 0), StringFormat.GenericDefault);

                var br = path.GetBounds(); //"bounding rectangle"
                var bp = br.Location; //"base point"?

                SizeF topLeftTranslate = new SizeF(0, 0); //OK
                SizeF bottomLeftTranslate = new SizeF(0, -br.Height); //OK
                SizeF topRightTranslate = new SizeF(-br.Width, 0); 
                SizeF bottomRightTranslate = new SizeF(-br.Width, -br.Height); 

                var matrix = new Matrix();
                matrix.Translate(DrawLocation.X - bp.X, DrawLocation.Y - bp.Y); //set draw to position with correction by unwanted string drawing offset ("bp")

                var relativePositionToOrigin = GetPosition(text);
                ApplyTranslationRelativeToTopLeft(matrix, br.Size, relativePositionToOrigin);
                path.Transform(matrix);

                // To rotate arround the center of the path:
                //PointF o = new PointF( br.Width / 2, br.Height / 2 );
                //var m2 = new Matrix( );
                //m2.Translate( -o.X, -o.Y );
                //path.Transform( m2 );

                var m = new Matrix();
                m.RotateAt(Rotation, DrawLocation);
                path.Transform(m);

                g.DrawPath(new Pen(Brushes.Black), path);
                if (drawBoundingRectangle)
                {
                    g.DrawRectangles(new Pen(Brushes.Black), new RectangleF[] { path.GetBounds() });
                }
            }
        }

        private void DrawRectangle(Graphics g, GraphicsPath originalPath, bool verticalCenter = false)
        {
            PointF alteredPosition = DrawLocation;
            var boundingRectangle = originalPath.GetBounds(); //at rotation 0
            if (verticalCenter)
            {
                //move up by 1/2 of height = center vertically
                alteredPosition = MovePoint(DrawLocation, new SizeF(0, -boundingRectangle.Height / 2f)); 
            }
            var rect = new RectangleF(alteredPosition, boundingRectangle.Size);

            using (var rectPath = new GraphicsPath())
            {
                rectPath.AddRectangle(rect);
                //rotate
                Matrix rotationMatrix = new Matrix();
                rotationMatrix.RotateAt(Rotation, DrawLocation);
                rectPath.Transform(rotationMatrix);
                g.DrawPath(new Pen(Brushes.Black), rectPath);
            }
        }

        private static PointF MovePoint(PointF original, SizeF size)
        {
            return new PointF(original.X + size.Width, original.Y + size.Height);
        }

        private GraphicsPath GetTextPath(Graphics g, StringFormat stringFormat)
        {
            var path = new GraphicsPath();
            path.AddString("sample text", new FontFamily("Arial"), (int)FontStyle.Regular, 120, DrawLocation, stringFormat);

            DrawRectangle(g, path); //this is the rectangle/position I need the text to be
            DrawRectangle(g, path, verticalCenter: true); //this is the rectangle/position I need the text to be

            //rotate
            Matrix rotationMatrix = new Matrix();
            rotationMatrix.RotateAt(Rotation, DrawLocation);
            path.Transform(rotationMatrix);

            var boundingRectangle = path.GetBounds();

            //offset is present for an unknown reason in GraphicsPath containing text
            var boundingRectanglePosition = boundingRectangle.Location;
            if (Rotation == 0)
            {
                var offset = new PointF(boundingRectanglePosition.X - DrawLocation.X, boundingRectanglePosition.Y - DrawLocation.Y);
                Trace.WriteLine($"Offset against mouse position: {offset}");
            }
            return path;
        }

        private void DrawTextPath(Graphics g, GraphicsPath path)
        {
            g.DrawPath(new Pen(Brushes.Black), path);
            g.DrawRectangles(new Pen(Brushes.Red), new RectangleF[] { path.GetBounds() });
        }

        private void Redraw()
        {
            this.Invalidate(); //request paint event
        }
    }
}
