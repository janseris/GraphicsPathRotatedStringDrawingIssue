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
        PointF Position = PointF.Empty;

        public Form1()
        {
            InitializeComponent();
            Timer.Tick += (s, e) => { Rotation++; Rotation %= 360; Redraw(); };
            Timer.Start();
            this.Paint += Form1_Paint;
            this.MouseMove += (s, e) => { Position = e.Location; Redraw(); };
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


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawText(e.Graphics, pinSymbol); //draws OK
            DrawText(e.Graphics, arrowTopLeftSymbol); //draws OK
            
            DrawText(e.Graphics, arrowTopRightSymbol);
            DrawText(e.Graphics, arrowBottomRightSymbol);

            e.Graphics.DrawEllipse(new Pen(Color.Black, width: 5), new RectangleF(Position, new Size(1, 1)));
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

                var m1 = new Matrix();
                m1.Translate(Position.X - bp.X, Position.Y - bp.Y); //set draw to position with correction by unwanted string drawing offset ("bp")
                SizeF relativeTranslate = SizeF.Empty; //default
                //OK
                if(text == arrowTopLeftSymbol)
                {
                    relativeTranslate = topLeftTranslate; //in fact, same as default
                } 
                //OK
                if(text == pinSymbol)
                {
                    relativeTranslate = bottomLeftTranslate;
                }
                //???
                if(text == arrowTopRightSymbol)
                {
                    relativeTranslate = topRightTranslate;
                }
                if(text == arrowBottomRightSymbol)
                {
                    relativeTranslate = bottomRightTranslate;
                }
                m1.Translate(relativeTranslate.Width, relativeTranslate.Height); //switch to different drawing position
                path.Transform(m1);

                // To rotate arround the center of the path:
                //PointF o = new PointF( br.Width / 2, br.Height / 2 );
                //var m2 = new Matrix( );
                //m2.Translate( -o.X, -o.Y );
                //path.Transform( m2 );

                var m = new Matrix();
                m.RotateAt(Rotation, Position);
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
            PointF alteredPosition = Position;
            var boundingRectangle = originalPath.GetBounds(); //at rotation 0
            if (verticalCenter)
            {
                //move up by 1/2 of height = center vertically
                alteredPosition = MovePoint(Position, new SizeF(0, -boundingRectangle.Height / 2f)); 
            }
            var rect = new RectangleF(alteredPosition, boundingRectangle.Size);

            using (var rectPath = new GraphicsPath())
            {
                rectPath.AddRectangle(rect);
                //rotate
                Matrix rotationMatrix = new Matrix();
                rotationMatrix.RotateAt(Rotation, Position);
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
            path.AddString("sample text", new FontFamily("Arial"), (int)FontStyle.Regular, 120, Position, stringFormat);

            DrawRectangle(g, path); //this is the rectangle/position I need the text to be
            DrawRectangle(g, path, verticalCenter: true); //this is the rectangle/position I need the text to be

            //rotate
            Matrix rotationMatrix = new Matrix();
            rotationMatrix.RotateAt(Rotation, Position);
            path.Transform(rotationMatrix);

            var boundingRectangle = path.GetBounds();

            //offset is present for an unknown reason in GraphicsPath containing text
            var boundingRectanglePosition = boundingRectangle.Location;
            if (Rotation == 0)
            {
                var offset = new PointF(boundingRectanglePosition.X - Position.X, boundingRectanglePosition.Y - Position.Y);
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
