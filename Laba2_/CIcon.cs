using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Laba2_
{
    public class CIcon
    {
        private Point position;
        private string name;
        private Rectangle icon;
        private int width;
        private int height;
        string path;
        private string imagePath;


        public Point Position => position;
        public string Name => name;
        public Rectangle GetIcon() => icon;

        public string Path => path;
        public CIcon(int iconWidth, int iconHeight, string imagePath, Point pos)
        {
            width = iconWidth;
            height = iconHeight;
            position = pos;
            path = imagePath;
            CreateIcon(iconWidth, iconHeight, imagePath);
        }



        public void CreateIcon(int iconWidth, int iconHeight, string imagePath)
        {
            name = System.IO.Path.GetFileNameWithoutExtension(imagePath);
            icon = new Rectangle();
            this.imagePath = imagePath;
            // Установка цвета линии обводки
            icon.Stroke = Brushes.Black;
            icon.StrokeThickness = 1;

            // Загрузка изображения
            try
            {
                ImageBrush ib = new ImageBrush();
                ib.AlignmentX = AlignmentX.Left;
                ib.AlignmentY = AlignmentY.Top;
                ib.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                icon.Fill = ib;
            }
            catch (Exception ex)
            {
                // Если не удалось загрузить изображение, используем цветную заливку
                icon.Fill = Brushes.Gray;
            }

            icon.HorizontalAlignment = HorizontalAlignment.Left;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.Height = iconHeight;
            icon.Width = iconWidth;

            // Устанавливаем позицию
            Canvas.SetLeft(icon, position.X);
            Canvas.SetTop(icon, position.Y);
        }

        public bool IsPointInside(Point point) // System.Windows.Point
        {
            return point.X >= position.X && point.X <= position.X + width &&
                   point.Y >= position.Y && point.Y <= position.Y + height;
        }
    }
}
