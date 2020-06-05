using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//using MathNet.Numerics.RootFinding;

namespace Fabric
{
    class Dot
    {
        public Point StartingCoordinates { get; set; }
        public Point Coordinates { get; set; }
        public float ChangedX { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public List<Dot> Neighbors { get; private set; }
        public bool Stationary { get; set; }
        public bool Taken { get; set; }
        public float Mass { get; private set; }      // kg
        public Vector2 Gravity { get; set; }         // Pix/frame^2
        public float TentionCof { get; set; }        // N/Pix
        public float Resistance { get; set; }        // %
        public Vector2 Velocity { get; set; }        // Pix/frame
        int Distance;

        public Dot(Point coordinates, float mass, bool stationary, Vector2 gravity, float tentionCof, float resistance, int distance, int H, int which)
        {
            Distance = distance;
            StartingCoordinates = coordinates;
            Coordinates = coordinates;
            X = coordinates.X;
            Y = coordinates.Y;
            Mass = mass;
            Stationary = stationary;
            Gravity = gravity;
            TentionCof = tentionCof;
            Resistance = resistance;
            Taken = false;
        }

        public Tuple<float, float> GetVector()
        {
            double[] yn = new double[4];
            double[] xn = new double[4];
            double upperY = 0;
            double upperX = 0;
            double gama = 0;
            double k = 0;
            int dir = 0;
            int i = 0; //Y
            int j = 0; //X
            int Dir;
            foreach(Dot n in Neighbors)
            {
                if (StartingCoordinates.Y == n.StartingCoordinates.Y)
                {
                    double m;
                    int direction;
                    if (X > n.X)
                        direction = 1;
                    else
                        direction = -1;

                    if ((n.X < X && n.ChangedX > ChangedX))
                        Dir = -1;
                    else if (n.X > X && n.ChangedX < ChangedX)
                        Dir = -1;
                    else
                        Dir = 1;
                        m = (Math.Abs(n.X - X) * Dir - Math.Abs(n.ChangedX - ChangedX)) * direction * Dir;
                    xn[j] = m;

                    m = Math.Abs(n.Y - Y) * Math.Sign(n.Y - Y) * -1;
                    yn[j + 2] = m;
                    j++;
                }
                if (StartingCoordinates.X == n.StartingCoordinates.X)
                {
                    double m;
                    if (StartingCoordinates.Y > n.StartingCoordinates.Y)
                    {
                        upperY = n.Y;
                        upperX = n.X;
                    }
                        
                    int direction;
                    if (Y > n.Y)
                        direction = -1;
                    else
                        direction = 1;
                    if (Math.Abs(n.Y - Y) <= Distance)
                        m = 0;
                    else
                    {
                        m = (Math.Abs(n.Y - Y) - Distance) * direction * -1;
                    }
                    
                    yn[i] = m;
                    i++;
                    if(StartingCoordinates.Y > n.StartingCoordinates.Y)
                    {
                        double lenght = Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(n.Y - Y, 2));
                        if (Double.IsNaN(lenght))
                            lenght = 0;
                        double s = (Math.Abs(n.X - X) - lenght) * Math.Sign(X - n.X);
                        xn[2] = s;
                        gama = 0.5 * Math.Cos(4 * Math.Atan((n.X - X) / Math.Abs(n.Y - Y)) + Math.PI) + 0.5;
                        k = (Math.Abs(n.Y - Y) * gama) / Math.Abs(n.X - X);
                        if (Double.IsNaN(k))
                            k = 0;
                        if (X > n.X)
                            dir = 1;
                        else
                            dir= -1;
                    }
                    else
                    {
                        double s = (Math.Abs(n.X - X)) * Math.Sign(X - n.X);
                        xn[3] = s;
                    }
                }        
            }
            double dx;
            double D = Math.Sqrt(Math.Pow((X - ChangedX), 2) + Math.Pow((Y - upperY), 2));
            
            if (D != 0)
                dx = -1 * (TentionCof * (xn[0] + xn[1] + xn[2] * 0.4 + xn[3] * 0.4) + dir * Mass * Gravity.Y * gama * k) / (0.5 * Mass + 2 * TentionCof);
            else
                dx = 0;

            double root = 1 * (TentionCof * (yn[0] + yn[1] + yn[2] * 0.4 + yn[3] * 0.4) + Mass * Gravity.Y * -1) / (0.5 * Mass + 2 * TentionCof);

            Velocity = Vector2.Add(Velocity, new Vector2(Convert.ToSingle(dx), Convert.ToSingle(root)));

            Velocity = Velocity * Resistance;

            return new Tuple<float, float>(X + Velocity.X, Y - Velocity.Y);
        }

        

        public void FindNeighbors(Dot[,] dots, int distance)
        {
            Neighbors = new List<Dot>();
            for (int i = 0; i < dots.GetLength(0); i++)
            {
                for (int j = 0; j < dots.GetLength(1); j++)
                {
                    if ((Math.Abs(StartingCoordinates.X - dots[i, j].StartingCoordinates.X) == distance && Math.Abs(StartingCoordinates.Y - dots[i, j].StartingCoordinates.Y) == 0) ||
                        (Math.Abs(StartingCoordinates.X - dots[i, j].StartingCoordinates.X) == 0 && Math.Abs(StartingCoordinates.Y - dots[i, j].StartingCoordinates.Y) == distance))
                        Neighbors.Add(dots[i, j]);
                }
            }
        }

    }
}
