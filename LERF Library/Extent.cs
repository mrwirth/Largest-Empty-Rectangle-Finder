using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LERF_Library
{
    public class Extent
    {
        public Rectangle Rectangle;
        public bool Connected;
        public Extent LeftExtent;
        public Extent RightExtent;
        public Extent TopExtent;
        public Extent BottomExtent;
        private int left;
        private int top;
        private int v;
        private int height;

        // These are here because I can't inherite from Rectangle
        public int X => Rectangle.X;
        public int Y => Rectangle.Y;
        public int Width => Rectangle.Width;
        public int Height => Rectangle.Height;
        public int Left => Rectangle.Left;
        public int Right => Rectangle.Right;
        public int Top => Rectangle.Top;
        public int Bottom => Rectangle.Bottom;
        public long Area => Rectangle.Width * Rectangle.Height;

        public Extent(Rectangle rectangle)
        {
            Rectangle = rectangle;
            Connected = false;
        }

        public Extent(int x, int y, int width, int height)
        {
            Rectangle = new Rectangle(x, y, width, height);
        }

        public Extent(Extent origin, int width, int height)
        {
            Rectangle = new Rectangle(origin.X, origin.Y, width, height);
        }

        public Extent(Extent topLeft, Extent bottomRight)
        {
            var width = bottomRight.Right - topLeft.Left;
            var height = bottomRight.Bottom - topLeft.Top;
            Rectangle = new Rectangle(topLeft.X, topLeft.Y, width, height);
        }
    }
}
