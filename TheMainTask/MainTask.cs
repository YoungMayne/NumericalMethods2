using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace TheMainTask
{
    class MainTask
    {
        // Variables
        private const double E  = 0.3;
        private const double m1 = 1.0;
        private const double m2 = 0.0;
        private double k1(double x) { return Math.Pow(x, 2.0) + 2; }
        private double k2(double x) { return Math.Pow(x, 2.0); }
        private double q1(double x) { return x; }
        private double q2(double x) { return Math.Pow(x, 2.0); }
        private double f1(double x) { return 1.0; }
        private double f2(double x) { return Math.Sin(Math.PI * x); }

        public void GetGrids(ref List<double> g1, ref List<double> g2, double h)
        {
            g1.Add(0.0);
            for (int i = 0; g1[i] < 1.0; ++i)
            {
                g1.Add((i + 1) * h);
            }

            g2.Add(h * 0.5);
            for (int i = 0; (g2[i] + h) < 1.0; ++i)
            {
                g2.Add((i + 1.5) * h);
            }
        }

        public List<double> GetAi(double h, int n, List<double> grid)
        {
            List<double> ai = new List<double>();
            double result;

            for (int i = 0; i < n; ++i)
            {
                if (E > grid[i + 1])
                {
                    result = Math.Log(Math.Sin((grid[i + 1] + 1.1) / 2.0)) -
                             Math.Log(Math.Cos((grid[i + 1] + 1.1) / 2.0)) -
                             Math.Log(Math.Sin((grid[i] + 1.1) / 2.0)) +
                             Math.Log(Math.Cos((grid[i] + 1.1) / 2.0));

                    ai.Add(h * Math.Sqrt(2.0) / result);
                }
                else if ((E < grid[i + 1]) && (E < grid[i]))
                {
                    result = Math.Tan(grid[i + 1]) - Math.Tan(grid[i]);

                    ai.Add(h / result);
                }
                else //if ((E < grid[i + 1]) && (E > grid[i]))
                {
                    result = (Math.Log(Math.Sin((E + 1.1) / 2.0)) -
                              Math.Log(Math.Cos((E + 1.1) / 2.0)) -
                              Math.Log(Math.Sin((grid[i] + 1.1) / 2.0)) +
                              Math.Log(Math.Cos((grid[i] + 1.1) / 2.0))) /
                              Math.Sqrt(2.0);
                    result = result + Math.Tan(grid[i + 1]) - Math.Tan(E);

                    ai.Add(h / result);
                }
            }

            return ai;
        }

        public List<double> GetDi(double h, List<double> grid)
        {
            List<double> di = new List<double>();
            double result;

            for (int i = 0; i < (grid.Count - 1); ++i)
            {
                if (E > grid[i + 1])
                {
                    di.Add(1.0);
                }
                else if ((E < grid[i + 1]) && (E < grid[i]))
                {
                    result = Math.Pow(grid[i + 1], 3.0) -
                             Math.Pow(grid[i], 3.0) /
                             (3.0 * h);

                    di.Add(result);
                }
                else if ((E < grid[i + 1]) && (E > grid[i]))
                {
                    result = (E - grid[i] + (Math.Pow(grid[i + 1], 3.0) -
                                             Math.Pow(E, 3.0)) / 3.0) / h;

                    di.Add(result);
                }
            }

            return di;
        }

        public List<double> GetPhi(double h, List<double> grid)
        {
            List<double> phi = new List<double>();
            double result;

            for (int i = 0; i < (grid.Count - 1); ++i)
            {
                if (E > grid[i + 1])
                {
                    result = -(Math.Cos(2 * grid[i + 1]) -
                               Math.Cos(2 * grid[i])) /
                               (2.0 * h);

                    phi.Add(result);
                }
                else if ((E < grid[i + 1]) && (E < grid[i]))
                {
                    result = (Math.Sin(grid[i + 1]) - Math.Sin(grid[i])) / h;

                    phi.Add(result);
                }
                else if ((E < grid[i + 1]) && (E > grid[i]))
                {
                    result = ((-Math.Cos(2 * E) +
                                Math.Cos(2 * grid[i])) / 2.0 +
                                Math.Sin(grid[i + 1]) -
                                Math.Sin(E)) / h;

                    phi.Add(result);
                }
            }

            return phi;
        }

        public List<List<double>> GetMatrix(int n,
                                            double h,
                                            List<double> ai,
                                            List<double> di,
                                            List<double> phi)
        {
            List<List<double>> matrix = new List<List<double>>();
            List<double> row;

            matrix.Add(new List<double>(4) { 1.0, 0.0, 0.0, 1.0 });
            for (int i = 1; i < n; ++i)
            {
                row = new List<double>() { ai[i - 1] / (h * h),
                                           -(((ai[i - 1] + ai[i]) / (h * h)) + di[i - 1]),
                                           ai[i] / (h * h),
                                           -phi[i - 1] };

                matrix.Add(row);
            }

            return matrix;
        }

        public List<double> GetDifSceme(int n, List<List<double>> matrix)
        {
            List<double> alphai = new List<double>();
            List<double> betai = new List<double>();

            double[] vi = new double[n + 1];
            double ai;
            double bi;
            double ci;
            double phi;

            alphai.Add(matrix[0][1]);
            betai.Add(matrix[0][3]);

            for (int i = 1; i < n; i++)
            {
                ai = matrix[i][0];
                bi = matrix[i][2];
                ci = matrix[i][1];
                phi = matrix[i][3];

                alphai.Add(-bi / (ci + alphai[i - 1] * ai));
                betai.Add((phi - ai * betai[i - 1]) / (ci + alphai[i - 1] * ai));
            }

            vi[0] = 1.0;
            vi[n] = 0.0;
            for (int i = n - 1; i > 0; i--)
            {
                vi[i] = alphai[i] * vi[i + 1] + betai[i];
            }

            return vi.OfType<double>().ToList();
        }

        public List<double> GetAbsDif(List<double> a, List<double> b)
        {
            List<double> eps = new List<double>();
            double result;

            for (int i = 0; i < a.Count; i++)
            {
                result = Math.Abs(b[i * 2] - a[i]);
                eps.Add(result);
            }

            return eps;
        }
    }
}
