using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoublePendulum
{
    class DoublePendulumEquations
    {
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
         * 4)   w2' = 2sin(theta1-theta2)(w1^2 L1(m1+m2)+g(m1+m2)cos(theta1)+w2^2 L2m2Cos(theta1-theta2))
         *            -----------------------------------------------------------------------------------
         *                                       L2(2m1+m2-m2cos(2theta1-2theta2))
         *                                       
         * 4th order Runge-Kutta:
         * 
         * 4th order runge-kutta method:
         * y2 = y1 + (1 / 6) * (k1 + 2*k2 + 2*k3 + k4) * h
         * 
         * k1 = f(x, y1)
         * k2 = f(x + h/2, y1 + (k1 * h) / 2)
         * k3 = f(x + h/2, y1 + (k2 * h) / 2)
         * k4 = f(x + h, y1 + k3 * h)
         * 
         * h = step-size
         */

        private const double g = 9.81;

        public static double SolveEquation1(double h, double w1, double y1)
        {
            return y1 + w1 * h;
        }

        public static double SolveEquation2(double h, double w2, double y1)
        {
            return y1 + w2 * h;
        }

        public static double SolveEquation3(double h, double m1, double m2, double theta1, double theta2, double w1, double w2, double L1, double L2, double y1)
        {
            double numerator, denominator;
            double k1, k2, k3, k4;
            //k1
            numerator = -g * (2 * m1 + m2) * Math.Sin(theta1) - m2 * g * Math.Sin(theta1 - 2 * theta2) - 2 * Math.Sin(theta1 - theta2) * m2 * (w2 * w2 * L2 - w1 * w1 * L1 * Math.Cos(theta1 - theta2));
            denominator = L1 * (2 * m1 + m2 - m2 * Math.Cos(2 * theta1 - 2 * theta2));
            k1 = numerator / denominator;

            //k2
            numerator = -g * (2 * m1 + m2) * Math.Sin(theta1) - m2 * g * Math.Sin(theta1 - 2 * theta2) - 2 * Math.Sin(theta1 - theta2) * m2 * (w2 * w2 * L2 - (w1 + ((k1 * h) / 2)) * (w1 + ((k1 * h) / 2)) * L1 * Math.Cos(theta1 - theta2));            
            k2 = numerator / denominator;

            //k3
            numerator = -g * (2 * m1 + m2) * Math.Sin(theta1) - m2 * g * Math.Sin(theta1 - 2 * theta2) - 2 * Math.Sin(theta1 - theta2) * m2 * (w2 * w2 * L2 - (w1 + ((k2 * h) / 2)) * (w1 + ((k2 * h) / 2)) * L1 * Math.Cos(theta1 - theta2));            
            k3 = numerator / denominator;

            //k4
            numerator = -g * (2 * m1 + m2) * Math.Sin(theta1) - m2 * g * Math.Sin(theta1 - 2 * theta2) - 2 * Math.Sin(theta1 - theta2) * m2 * (w2 * w2 * L2 - (w1 + k3 * h) * (w1 + k3 * h) * L1 * Math.Cos(theta1 - theta2));            
            k4 = numerator / denominator;

            //return solution
            return y1 + (h / 6) * (k1 + 2 * k2 + 2 * k3 + k4);
        }

        public static double SolveEquation4(double h, double m1, double m2, double theta1, double theta2, double w1, double w2, double L1, double L2, double y1)
        {
            double numerator, denominator;
            double k1, k2, k3, k4;
            //k1
            numerator = 2 * Math.Sin(theta1 - theta2) * (w1 * w1 * L1 * (m1 + m2) + g * (m1 + m2) * Math.Cos(theta1) + w2 * w2 * L2 * m2 * Math.Cos(theta1 - theta2));
            denominator = L2 * (2 * m1 + m2 - m2 * Math.Cos(2 * theta1 - 2 * theta2));
            k1 = numerator / denominator;

            //k2
            numerator = 2 * Math.Sin(theta1 - theta2) * (w1 * w1 * L1 * (m1 + m2) + g * (m1 + m2) * Math.Cos(theta1) + (w2 + ((k1 * h) / 2)) * (w2 + ((k1 * h) / 2)) * L2 * m2 * Math.Cos(theta1 - theta2));
            k2 = numerator / denominator;

            //k3
            numerator = 2 * Math.Sin(theta1 - theta2) * (w1 * w1 * L1 * (m1 + m2) + g * (m1 + m2) * Math.Cos(theta1) + (w2 + ((k2 * h) / 2)) * (w2 + ((k2 * h) / 2)) * L2 * m2 * Math.Cos(theta1 - theta2));
            k3 = numerator / denominator;

            //k4
            numerator = 2 * Math.Sin(theta1 - theta2) * (w1 * w1 * L1 * (m1 + m2) + g * (m1 + m2) * Math.Cos(theta1) + (w2 + k3 * h) * (w2 + k3 * h) * L2 * m2 * Math.Cos(theta1 - theta2));
            k4 = numerator / denominator;

            //return solution
            return y1 + (h / 6) * (k1 + 2 * k2 + 2 * k3 + k4);
        }
    }
}
