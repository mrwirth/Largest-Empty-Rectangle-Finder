using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Demo.ViewModels
{
    public class BaseRectangleVM : BaseVM
    {
        public Rectangle Rectangle;
        public int X => Rectangle.X;
        public int Y => Rectangle.Y;
        public int Width => Rectangle.Width;
        public int Height => Rectangle.Height;

        public BaseRectangleVM(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }
    }
}
