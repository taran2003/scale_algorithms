using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace Lab1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            img = new BitmapImage();
            time = new Stopwatch();
        }

        double factorX,factorY;
        BitmapImage img;
        double wight, height, dpx, dpy;
        Stopwatch time;
        string forTime = @"Время работы: ";

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            withPixelsPair((old, @new) =>
            {
                for (int y = 0; y < old.Height; y++)
                {
                    for (int x = 0; x < old.Width; x++)
                    {
                        @new[(int)(y * factorY), (int)(x * factorX)] = old[y, x];
                    }
                }

                if (factorX > 1.0f)
                {
                    for (int i = 0; i < old.Height; i++)
                    {
                        for (int j = 0; j < old.Width - 1; j++)
                        {
                            int gap = (int)((j + 1) * factorX) - (int)(j * factorX) - 1;
                            if (gap > 0)
                            {
                                var delta = GetColorFactor(old[i, j], old[i, j + 1],old.Bpp,gap);
                                for (int l = 1; l <= gap; l++)
                                {
                                    @new[(int)(i * factorY), (int)(j * factorX) + l] = NewColorWithFactor(old[i,j], delta, old.Bpp, l);
                                }
                            }
                        }
                    }
                }

                if (factorY > 1.0f)
                {
                    for (int i = 0; i < (int)(old.Width * factorX); i++)
                    {
                        for (int j = 0; j < old.Height - 1; j++)
                        {
                            int gap = (int)((j + 1) * factorY) - (int)(j * factorY) - 1;
                            if (gap > 0)
                            {
                                var delta = GetColorFactor(@new[(int)(j * factorY), i], @new[(int)((j + 1) * factorY), i], old.Bpp, gap);
                                for (int l = 1; l <= gap; l++)
                                {
                                    @new[(int)(j * factorY) + l, i] = NewColorWithFactor(@new[(int)(j * factorY), i], delta, old.Bpp, l);
                                }
                            }
                        }
                    }
                }

                if (factorX <= 1.0f || factorY <= 1.0f) { return; }
                
                    for (int i = (int)(@new.Width - factorX); i < @new.Width; i++)
                    {
                        for (int j = 0; j < (int)(@new.Height - factorX) + 1; j++)
                        {
                            @new[j, i] = @new[j, i - 1];
                        }
                    }

                    for (int i = (int)(@new.Height - factorY); i < @new.Height; i++)
                    {
                        for (int j = 0; j < @new.Width; j++)
                        {
                            @new[i, j] = @new[i - 1, j];
                        }
                    }
                
            });
        }

        private byte[] NewColorWithFactor(byte[] left, double[] delta, int bpp, int factor)
        {
            byte[] colorFactor = new byte[bpp];
            for (int i = 0; i < bpp; i++)
            {
                colorFactor[i] = (byte)(left[i] + delta[i] * factor);
            }
            return colorFactor;            
        }

        private double[] GetColorFactor(byte[] pixel1, byte[] pixel2, int bpp, int factor)
        {
            double[] colorFactor = new double[bpp];
            for (int i = 0; i < bpp; i++)
            {
                colorFactor[i] = (pixel2[i] - pixel1[i]) / (factor + 1);
            }
            return colorFactor;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                String filePath = "SavedImage.png";
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                encoder.Save(stream);
                Save.Text = "Успешно сохранено";
            }
            catch
            {
                Error.Text = "Нельзя сохранить";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            withPixelsPair((old, @new) => {
                double FactorX = (double)(old.Width - 1) / (double)(@new.Width - 1);
                double FactorY = (double)(old.Height - 1) / (double)(@new.Height - 1);
                for(int i = 0; i < @new.Width; ++i)
                {
                    double scaleX = i * FactorX;
                    double scaleX1 = Math.Floor(scaleX);
                    double scaleX2 = scaleX1 + 1;
                    for(int j = 0; j < @new.Height; ++j)
                    {
                        double scaleY = j * FactorY;
                        double scaleY1 = Math.Floor(scaleY);
                        double scaleY2 = scaleY1 + 1;
                        if (scaleY2 == old.Height)
                        {
                            if (scaleX2 == old.Width)
                            {
                                @new[j, i] = old[(int)scaleY1, (int)scaleX1];
                            }
                            else
                            {
                                double w = (scaleX - scaleX1) / (scaleX2 - scaleX1);
                                byte[] color1 = old[(int)scaleY1, (int)scaleX1];
                                byte[] color2 = old[(int)scaleY1, (int)scaleX2];
                                @new[j, i] = color1.Zip(color2).Select((cs) => (byte)(cs.First + (cs.Second - cs.First) * w)).ToArray();
                            }
                            continue;
                        }
                        else if (scaleX2 == old.Width)
                        {
                            double w = (scaleY - scaleY1) / (scaleY2 - scaleY1);
                            byte[] color1 = old[(int)scaleY1, (int)scaleX1];
                            byte[] color2 = old[(int)scaleY2, (int)scaleX1];
                            @new[j, i] = color1.Zip(color2).Select((cs) => (byte)(cs.First + (cs.Second - cs.First) * w)).ToArray();
                            continue;
                        }
                        double factor11 = (scaleX2 - scaleX) * (scaleY2 - scaleY);
                        double factor12 = (scaleX2 - scaleX) * (scaleY - scaleY1);
                        double factor21 = (scaleX - scaleX1) * (scaleY2 - scaleY);
                        double factor22 = (scaleX - scaleX1) * (scaleY - scaleY1);
                        byte[] color11 = old[(int)scaleY1, (int)scaleX1];
                        byte[] color12 = old[(int)scaleY2, (int)scaleX1];
                        byte[] color21 = old[(int)scaleY1, (int)scaleX2];
                        byte[] color22 = old[(int)scaleY2, (int)scaleX2];
                        byte[] result = new byte[old.Bpp];

                        for(int c = 0; c < old.Bpp; ++c)
                        {
                            result[c] = (byte)(factor11 * color11[c] + factor12 * color12[c] + factor21 * color21[c] + factor22 * color22[c]);
                        }
                        @new[j, i] = result;
                    }
                }
            });
        }

        private void withPixelsPair(Action<Pixels, Pixels> algo)
        {
            if (!Initializing(FactorX, FactorY, path, Error, Save)) { return; }

            WriteableBitmap newImg = new WriteableBitmap((int)wight, (int)height, dpx, dpy, img.Format, null);
            Pixels old = new Pixels(img.Format, img.PixelWidth, img.PixelHeight);
            Pixels @new = new Pixels(newImg.Format, newImg.PixelWidth, newImg.PixelHeight);

            ResolutionNew.Text = String.Format("new: {0}x{1}",@new.Width,@new.Height);
            ResolutionOld.Text = String.Format("old: {0}x{1}",old.Width,old.Height);
            Size.Text = String.Format("размер файла: {0}", new FileInfo(path.Text).Length.ToString());
            format.Text = String.Format("формат файла: {0}", new FileInfo(path.Text).Extension);

            time.Restart();
            img.CopyPixels(old.Data, old.Stride, 0);
            algo(old, @new);
            newImg.WritePixels(new Int32Rect(0, 0, (int)wight, (int)height), @new.Data, @new.Stride, 0);
            time.Stop();

            Time.Text = forTime + time.Elapsed.ToString();

            Image.Source = newImg;
        }


        private bool Initializing(TextBox FactorX, TextBox FactorY, TextBox path, TextBlock Error, TextBlock Save)
        {
            Error.Text = ""; 
            Save.Text = "";
            try
            {
                factorX = double.Parse(FactorX.Text);
                if (FactorY.Text.Length == 0) { factorY = factorX; }
                else { factorY = double.Parse(FactorY.Text); }
            }
            catch
            {
                Error.Text = "Некоректное значение множителя";
                return false;
            }
            img.BeginInit();
            img.UriSource = new Uri(path.Text, UriKind.RelativeOrAbsolute);
            img.EndInit();
            try
            {
                dpx = img.DpiX;
                dpy = img.DpiY;
            }
            catch
            { 
                Error.Text = "Некоректный путь к файлу";
                return false; 
            }
            wight = Math.Ceiling(img.PixelWidth * Math.Abs(factorX));
            height = Math.Ceiling(img.PixelHeight * Math.Abs(factorY));
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            withPixelsPair((old, @new) =>
            {
                double x, y, bufX, bufY;
                bufX = (img.PixelWidth / wight);
                bufY = (img.PixelHeight / height);
                for (int i = 0; i < (int)wight; i++)
                {
                    x = i * bufX;
                    for (int j = 0; j < (int)height; j++)
                    {
                        y = j * bufY;
                        @new[j, i] = old[(int)y, (int)x];
                    }
                }
            });
        }
    }    
}
