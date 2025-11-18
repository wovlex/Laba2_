using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba2_
{
    public class CIconList
    {
        private List<CIcon> icons;
        private int iconWidth;
        private int iconHeight;
        private int columns;
        private int rows;

        public CIconList(int width, int height, int cols, int rows)
        {
            icons = new List<CIcon>();
            iconWidth = width;
            iconHeight = height;
            columns = cols;
            this.rows = rows;
        }

        public void Load(string folderPath)
        {
            icons.Clear();

            if (!Directory.Exists(folderPath))
            {
                return;
            }

            string filter = "*.png";
            string[] files = Directory.GetFiles(folderPath, filter);

            if (files.Length == 0)
            {
                return;
            }

            double x = 10;
            double y = 10;
            int count = 0;

            foreach (string file in files)
            {
                try
                {
                    System.Windows.Point position = new System.Windows.Point(x, y);
                    CIcon icon = new CIcon(iconWidth, iconHeight, file, position);
                    icons.Add(icon);

                    x += iconWidth + 10;
                    count++;

                    if (count >= columns)
                    {
                        x = 10;
                        y += iconHeight + 10;
                        count = 0;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public CIcon findByName(string name)
        {
            return icons.Find(icon => icon.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public CIcon IsMouseOver(System.Windows.Point mousePos)
        {
            foreach (var icon in icons)
            {
                if (icon.IsPointInside(mousePos))
                {
                    return icon;
                }
            }
            return null;
        }

        public List<CIcon> GetIcons()
        {
            return icons;
        }
    }
}
