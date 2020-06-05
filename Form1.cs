using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fabric
{
    public partial class Form1 : Form
    {
        Dot[,] dots;
        const int fabricLenght = 60;
        const int fabricHeight = 30;
        const int distanceBetween = 10;
        const float dotMass = 2f;
        const float tentionCof = 10f;
        const float resistance = 0.97f;

        Timer t = new Timer();
        Vector2 gravity = new Vector2(0f, 5f);
        double time = 0;
        Dot selected;

        public Form1()
        {
            InitializeComponent();
            SetToPrimayPositions(fabricLenght, fabricHeight, distanceBetween);
            ChangePrimaryPositions(fabricLenght, fabricHeight);
            t.Interval = 40;
            t.Tick += T_Tick;
        }

        private void T2_Tick(object sender, EventArgs e)
        {
            time += 0.1;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            ChangePositions();
            Refresh();
        }

        private void ChangePositions()
        {
            int i = 0;
            List<Tuple<float, float>> points = new List<Tuple<float, float>>();
            foreach(Dot d in dots)
            {
                if(!d.Stationary && !d.Taken)
                    points.Add(d.GetVector());
            }
            foreach (Dot d in dots)
            {
                if (!d.Stationary && !d.Taken)
                {
                    d.X = points[i].Item1;
                    d.Y = points[i].Item2;
                    d.Coordinates = new Point(Convert.ToInt32(d.X), Convert.ToInt32(d.Y));
                    i++;
                }
                if (d.Taken)
                {
                    d.Coordinates = PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
                    d.X = d.Coordinates.X;
                    d.Y = d.Coordinates.Y;
                }
                    
            }
        }

        private void ChangePrimaryPositions(int L, int H)
        {
            Random rnd = new Random();
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < L; j++)
                {
                    //dots[j, i].Coordinates = new Point(dots[j, i].Coordinates.X + i * distanceBetween / 2 + rnd.Next(0, distanceBetween), dots[j, i].Coordinates.Y);
                    //dots[j, i].Coordinates = new Point(dots[j, i].Coordinates.X + i * distanceBetween, dots[j, i].Coordinates.Y - j * distanceBetween / 2);
                    dots[j, i].Coordinates = new Point(dots[j, i].Coordinates.X + i * distanceBetween / 2 + rnd.Next(0, distanceBetween), dots[j, i].Coordinates.Y - j * 3 * distanceBetween / 4 + rnd.Next(0, distanceBetween / 2));
                    if (dots[j, i].Stationary)
                    {
                        for (int k = 0; k < H; k++)
                        {
                            dots[j, k].ChangedX = dots[j, i].Coordinates.X;
                        }
                    }
                    dots[j, i].X = dots[j, i].Coordinates.X;
                    dots[j, i].Y = dots[j, i].Coordinates.Y;
                }
            }
        }

        private void SetToPrimayPositions(int L, int H, int distance)
        {
            Dot d;
            dots = new Dot[L, H];
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < L; j++)
                {
                    if (i == 0)
                        d = new Dot(new Point((j * distance) + 500, (i * distance) + 500), dotMass, true, gravity, tentionCof, resistance, distance, H, i);
                    else
                        d = new Dot(new Point((j * distance) + 500, (i * distance) + 500), dotMass, false, gravity, tentionCof, resistance, distance, H, i);
                    dots[j, i] = d;
                }
            }
            foreach(Dot dot in dots)
            {
                dot.FindNeighbors(dots, distance);
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            t.Start();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.Black);
            foreach(Dot d in dots)
            {
                foreach (Dot n in d.Neighbors)
                {
                    if (n.StartingCoordinates.X - d.StartingCoordinates.X > 0 || n.StartingCoordinates.Y - d.StartingCoordinates.Y > 0)
                        e.Graphics.DrawLine(p, n.Coordinates, d.Coordinates);
                }     
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            Point l = e.Location;
            Dot nearest = dots[0,0];
            double dist = 100;
            foreach(Dot d in dots)
            {
                double n = Math.Sqrt(Math.Pow(d.X - l.X, 2) + Math.Pow(d.Y - l.Y, 2));
                if (dist > n)
                {
                    dist = n;
                    nearest = d;
                }
            }
            selected = nearest;
            selected.Taken = true;
            //Console.WriteLine(selected.StartingCoordinates);
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if(selected != null)
                selected.Taken = false;
            selected = null;
        }
    }

}
