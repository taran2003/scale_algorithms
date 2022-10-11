using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lab1
{
    internal class Pixels
    {

        byte[] array;
        PixelFormat format;
        int width, height;
        int bpp;

        public int Width => width;
        public int Height => height;

        public int Stride => width * bpp;

        public Array Data => array;

        public int Bpp => bpp;

        public Pixels(PixelFormat format, int wigth, int height)
        {
            this.format = format;
            this.width = wigth;
            this.height = height;
            bpp = (format.BitsPerPixel + 7) / 8;
            this.array = new byte[wigth * height * bpp];
        }

        public byte[] this[int y, int x]
        {
            get
            {
                if(x < 0 || x >= width || y < 0 || y >= height)
                {
                    throw new IndexOutOfRangeException();
                }
                byte[] pixel = new byte[bpp];
                int i = (y * width + x) * bpp;
                Array.Copy(array, i, pixel, 0, bpp);
                return pixel;
            }
            set
            {
                if (x < 0 || x > width || y < 0 || y >= height)
                {
                    throw new IndexOutOfRangeException();
                }
                Array.Copy(value, 0, array, (y * width + x) * bpp, bpp);
            }
        }
    }
        
}
