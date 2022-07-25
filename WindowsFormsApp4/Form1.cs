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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var textDefault = GetTextPath(e.Graphics, StringFormat.GenericDefault);
            var textVerticalCenter = GetTextPath(e.Graphics, new StringFormat() { LineAlignment = StringAlignment.Center}); //as provided by reza-aghaei
             
            DrawTextPath(e.Graphics, textDefault); textDefault.Dispose();
            DrawTextPath(e.Graphics, textVerticalCenter); textVerticalCenter.Dispose();

            e.Graphics.DrawEllipse(new Pen(Color.Black, width: 5), new RectangleF(Position, new Size(1, 1)));

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

            DrawRectangle(g, path); //this is the position I need the text to be
            DrawRectangle(g, path, verticalCenter: true); //this is the position I need the text to be

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
