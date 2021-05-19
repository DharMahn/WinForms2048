using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        const int mapSize = 4;
        int[,] map = new int[4, 4];
        List<int> powers = new List<int>();
        int borderSize = 1;
        int cellSize = 50;
        public Form1()
        {
            InitializeComponent();
        }
        Random r = new Random();
        SolidBrush myBrush;
        private void Form1_Load(object sender, EventArgs e)
        {
            int num = 2;
            /*for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        map[x, y] = 0;
                        continue;
                    }
                    map[x, y] = num;
                    num <<= 1;
                }
            }*/
            /*map[0, 2] = 4;
            map[1, 2] = 4;
            map[2, 2] = 4;
            map[3, 2] = 4;
            map[0, 1] = 4;
            map[0, 2] = 4;
            map[0, 3] = 4;*/
            powers.Add(0);
            for (int i = 2; i < int.MaxValue && i != int.MinValue;)
            {
                powers.Add(i);
                i <<= 1;
            }
            myBrush = new SolidBrush(Color.Black);
            GenerateRandomCells();
            GenerateRandomCells();
        }

        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            StringBuilder oldMapString = new StringBuilder();
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    oldMapString.Append(map[x, y]);
                }
            }
            if (e.KeyCode == Keys.Left)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    for (int x = 0; x < mapSize; x++)
                    {
                        MoveLeft(x, y);
                    }
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    for (int x = mapSize - 1; x >= 0; x--)
                    {
                        MoveRight(x, y);
                    }
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    for (int y = 0; y < mapSize; y++)
                    {
                        MoveUp(x, y);
                    }
                }
            }
            if (e.KeyCode == Keys.Down)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    for (int y = mapSize - 1; y >= 0; y--)
                    {
                        MoveDown(x, y);
                    }
                }
            }
            StringBuilder newMapString = new StringBuilder();
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    newMapString.Append(map[x, y]);
                }
            }
            if (!newMapString.ToString().Equals(oldMapString.ToString()))
            {
                GenerateRandomCells();
            }
            Invalidate();
        }

        private void MoveLeft(int x, int y)
        {
            while (x > 0)
            {
                if (!InBounds(x - 1, y))
                {
                    return;
                }
                if (map[x, y] == 0)
                {
                    return;
                }
                if (map[x - 1, y] == 0)
                {
                    map[x - 1, y] = map[x, y];
                    map[x, y] = 0;
                    x--;
                    continue;
                }
                if (map[x - 1, y] == map[x, y])
                {
                    map[x - 1, y] += map[x, y];
                    map[x, y] = 0;
                    return;
                }
                x--;
            }
        }
        private void MoveRight(int x, int y)
        {
            
            while (x < mapSize)
            {
                if (!InBounds(x + 1, y))
                {
                    return;
                }
                if (map[x, y] == 0)
                {
                    return;
                }
                if (map[x + 1, y] == 0)
                {
                    map[x + 1, y] = map[x, y];
                    map[x, y] = 0;
                    x++;
                    continue;
                }
                if (map[x + 1, y] == map[x, y])
                {
                    map[x + 1, y] += map[x, y];
                    map[x, y] = 0;
                    return;
                }
                x++;
            }
        }
        private void MoveUp(int x, int y)
        {
            while (y > 0)
            {
                if (!InBounds(x, y - 1))
                {
                    return;
                }
                if (map[x, y] == 0)
                {
                    return;
                }
                if (map[x, y - 1] == 0)
                {
                    map[x, y - 1] = map[x, y];
                    map[x, y] = 0;
                    y--;
                    continue;
                }
                if (map[x, y - 1] == map[x, y])
                {

                    map[x, y - 1] += map[x, y];
                    map[x, y] = 0;
                    return;
                }
                x--;
            }
        }
        private void MoveDown(int x, int y)
        {
            while (y < mapSize)
            {
                if (!InBounds(x, y + 1))
                {
                    return;
                }
                if (map[x, y] == 0)
                {
                    return;
                }
                if (map[x, y + 1] == 0)
                {
                    map[x, y + 1] = map[x, y];
                    map[x, y] = 0;
                    y++;
                    continue;
                }
                if (map[x, y + 1] == map[x, y])
                {
                    map[x, y + 1] += map[x, y];
                    map[x, y] = 0;
                    return;
                }
                y++;
            }
        }
        bool InBounds(int x, int y)
        {
            return x >= 0 && x < mapSize && y >= 0 && y < mapSize;
        }

        private void GenerateRandomCells()
        {
            int emptyCount = 0;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 0)
                    {
                        emptyCount++;
                    }
                }
            }
            if (emptyCount == 0)
            {
                return;
            }
            int x = r.Next(mapSize);
            int y = r.Next(mapSize);

            while (map[x, y] != 0)
            {
                x = r.Next(mapSize);
                y = r.Next(mapSize);
            }
            if (r.NextDouble() < 0.10)
            {
                map[x, y] = 4;
            }
            else
            {
                map[x, y] = 2;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int r, g, b;
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    int index = powers.IndexOf(map[x, y]);
                    double h = index*25;
                    HsvToRgb(h, 1, 1, out r, out g, out b);
                    myBrush.Color = Color.FromArgb(r, g, b);
                    e.Graphics.FillRectangle(myBrush, (x * cellSize) + borderSize, (y * cellSize) + borderSize, cellSize - borderSize - borderSize, cellSize - borderSize - borderSize);
                    e.Graphics.FillRectangle(Brushes.Black, (x * 50) + borderSize, (y * 50) + 15, 48, 18);
                    e.Graphics.DrawString(map[x, y].ToString(), Font, Brushes.White, x * 50, (y * 50) + 15);
                }
            }
        }
    }
}
