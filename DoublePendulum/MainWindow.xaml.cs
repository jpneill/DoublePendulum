using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DoublePendulum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        Color black = Color.FromRgb(0, 0, 0);
        Color blue = Color.FromRgb(0, 0, 255);
        Color red = Color.FromRgb(255, 0, 0);
        double mass1X, mass1Y, mass2X, mass2Y; //x and y coords of masses 1 and 2

        //constants
        const double g = -9.81;
        const double wallPegX = 295;
        const double wallPegY = 205;

        //drawn objects
        Ellipse mass1 = null;
        Ellipse mass2 = null;
        Line line1 = null;
        Line line2 = null;
        HashSet<Point> pointsSet = null;
        Polyline trace = null;
        PointCollection tracePoints = null;

        //initial conditions
        double m1; //mass1
        double m2; //mass2
        double L1; //length of line1
        double L2; //length of line2
        double theta1;
        double theta2;
        double w1;
        double w2;
        double h; //timestep

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(20);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            double theta12, theta22, w12, w22;

            //solve equations to get new values for the pendulum
            w22 = DoublePendulumEquations.SolveEquation4(h, m1, m2, theta1, theta2, w1, w2, L1, L2, w2);
            w12 = DoublePendulumEquations.SolveEquation3(h, m1, m2, theta1, theta2, w1, w2, L1, L2, w1);
            theta22 = DoublePendulumEquations.SolveEquation2(h, w22, theta2);
            theta12 = DoublePendulumEquations.SolveEquation1(h, w12, theta1);

            //erase the pendulum from the last step
            PaintCanvas.Children.Remove(line1);
            PaintCanvas.Children.Remove(line2);
            PaintCanvas.Children.Remove(mass1);
            PaintCanvas.Children.Remove(mass2);
            PaintCanvas.Children.Remove(trace);

            //update new values for next time tick
            w2 = w22;
            w1 = w12;
            theta1 = theta12;
            theta2 = theta22;

            //draw the pendulum again
            mass1X = L1 * Math.Sin(theta1) + wallPegX;
            mass1Y = L1 * Math.Cos(theta1) + wallPegY;
            line1 = CreateLine(wallPegX, wallPegY, mass1X, mass1Y, black);
            mass1 = CreateEllipse(15, 15, blue);
            PaintCanvas.Children.Add(line1);
            PaintCanvas.Children.Add(mass1);
            Canvas.SetTop(mass1, mass1Y - 7.5);
            Canvas.SetLeft(mass1, mass1X - 7.5);
            Canvas.SetZIndex(mass1, 1);

            mass2X = L2 * Math.Sin(theta2) + mass1X;
            mass2Y = L2 * Math.Cos(theta2) + mass1Y;
            line2 = CreateLine(mass1X, mass1Y, mass2X, mass2Y, black);
            mass2 = CreateEllipse(15 * m2 / m1, 15 * m2 / m1, blue);
            PaintCanvas.Children.Add(line2);
            PaintCanvas.Children.Add(mass2);
            Canvas.SetLeft(mass2, mass2X - ((15 * m2 / m1) / 2));
            Canvas.SetTop(mass2, mass2Y - ((15 * m2 / m1) / 2));
            Canvas.SetZIndex(mass2, 1);

            //draw the trace line
            Point p = new Point(mass2X, mass2Y);
            if (pointsSet.Add(p))
                tracePoints.Add(p);

            SolidColorBrush redBrush = new SolidColorBrush(red);
            trace.Stroke = redBrush;
            trace.StrokeThickness = 1;
            trace.Points = tracePoints;

            PaintCanvas.Children.Add(trace);
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            //set initial conditions
            m1 = 5;
            m2 = 3;
            L1 = 75;
            L2 = 75;
            theta1 = Math.PI / 4;
            theta2 = Math.PI / 6;
            w1 = 0;
            w2 = 0;
            h = 0.05;

            //remove the pendulums if they already are drawn on screen
            if (line1 != null)
                PaintCanvas.Children.Remove(line1);
            if (line2 != null)
                PaintCanvas.Children.Remove(line2);
            if (mass1 != null)
                PaintCanvas.Children.Remove(mass1);
            if (mass2 != null)
                PaintCanvas.Children.Remove(mass2);
            if (pointsSet != null)
            {
                PaintCanvas.Children.Remove(trace);
                pointsSet.Clear();
                tracePoints.Clear();
            }

            //draw the initial positions
            mass1X = L1 * Math.Sin(theta1) + wallPegX;
            mass1Y = L1 * Math.Cos(theta1) + wallPegY;
            line1 = CreateLine(wallPegX, wallPegY, mass1X, mass1Y, black);
            mass1 = CreateEllipse(15, 15, blue);
            PaintCanvas.Children.Add(line1);
            PaintCanvas.Children.Add(mass1);
            Canvas.SetTop(mass1, mass1Y - 7.5);
            Canvas.SetLeft(mass1, mass1X - 7.5);
            Canvas.SetZIndex(mass1, 1);

            mass2X = L2 * Math.Sin(theta2) + mass1X;
            mass2Y = L2 * Math.Cos(theta2) + mass1Y;
            line2 = CreateLine(mass1X, mass1Y, mass2X, mass2Y, black);
            mass2 = CreateEllipse(15 * m2 / m1, 15 * m2 / m1, blue);
            PaintCanvas.Children.Add(line2);
            PaintCanvas.Children.Add(mass2);
            Canvas.SetLeft(mass2, mass2X - ((15 * m2 / m1) / 2));
            Canvas.SetTop(mass2, mass2Y - ((15 * m2 / m1) / 2));
            Canvas.SetZIndex(mass2, 1);

            //begin filling the set to draw the trace line
            trace = new Polyline();
            pointsSet = new HashSet<Point>();
            tracePoints = new PointCollection();
            Point p = new Point(mass2X, mass2Y);
            pointsSet.Add(p);
            tracePoints.Add(p);

            timer.Start();
        }

        //create an ellipse
        public Ellipse CreateEllipse(double height, double width, Color colour)
        {
            SolidColorBrush fillBrush = new SolidColorBrush(colour);

            return new Ellipse()
            {
                Height = height,
                Width = width,
                Fill = fillBrush
            };
        }

        //create a line
        public Line CreateLine(double x1, double y1, double x2, double y2, Color colour)
        {
            SolidColorBrush brush = new SolidColorBrush(colour);

            return new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                Stroke = brush,
                StrokeThickness = 2
            };
        }

        #region Pendulum Diagram
        /*
         *               * fixed peg
         *               |\
         *               | \
         *               |  \ L1     L1 = length of firm rod connecting mass m1 to fixed peg
         *               |t1 \       t1 = theta1 = angle offset from vertical for mass m1
         *               |    \
         *                     O m1
         *                    /|
         *                   / |
         *               L2 /  |     L2 = length of firm rod connecting mass m2 to mass m1
         *                 / t2|     t2 = theta2 = angle offset from vertical for mass m2
         *                /    |
         *               O m2
         * 
         * Equations of motion:
         * 
         * 1)   theta1' = w1
         * 
         * 2)   theta2' = w2
         * 
         * 3)   w1' = -g(2m1+m2)sin(theta1)-m2gsin(theta1-2theta2)-2sin(theta1-theta2)m2(w2^2 L2-w1^2 L1cos(theta1-theta2))
         *            -----------------------------------------------------------------------------------------------------
         *                                                L1(2m1+m2-m2cos(2theta1-2theta2))
         * 
         * 4)   w2' = 2sin(theta1-theta2)(w2^2 L1(m1+m2)+g(m1+m2)cos(theta1)+w2^2 L2m2Cos(theta1-theta2))
         *            -----------------------------------------------------------------------------------
         *                                       L2(2m1+m2-m2cos(2theta1-2theta2))
         */
        #endregion


    }
}
