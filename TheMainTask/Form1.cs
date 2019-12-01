using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheMainTask
{
    public partial class Form1 : Form
    {
        private MainTask task;
        private int n = 10000;

        public Form1()
        {
            InitializeComponent();

            task = new MainTask();

            List<double> ai;
            List<double> di;
            List<double> phi;
            List<double> vi;
            List<double> ai2;
            List<double> di2;
            List<double> phi2;
            List<double> vi2;
            List<double> dif;
            List<double> xi = new List<double>();
            List<double> xi2 = new List<double>();
            List<double> x2i = new List<double>();
            List<double> x2i2 = new List<double>();

            List<List<double>> matrix;
            List<List<double>> matrix2;

            double h = 1.0 / n;
            task.GetGrids(ref xi, ref xi2, h);

            ai = task.GetAi(h, n, xi);
            di = task.GetDi(h, xi2);
            phi = task.GetPhi(h, xi2);
            matrix = task.GetMatrix(n, h, ai, di, phi);
            vi = task.GetDifSceme(n, matrix);

            int n2 = n * 2;
            double h2 = 1.0 / n2;
            task.GetGrids(ref x2i, ref x2i2, h2);

            ai2 = task.GetAi(h2, n2, x2i);
            di2 = task.GetDi(h2, x2i2);
            phi2 = task.GetPhi(h2, x2i2);
            matrix2 = task.GetMatrix(n2, h2, ai2, di2, phi2);
            vi2 = task.GetDifSceme(n2, matrix2);
            dif = task.GetAbsDif(vi, vi2);

            DrawSolve(xi, vi2, vi);
            DrawError(xi, dif);
        }

        void DrawSolve(List<double> xi, List<double> ui, List<double> vi)
        {
            ZedGraph.PointPairList v_list = new ZedGraph.PointPairList();
            ZedGraph.PointPairList u_list = new ZedGraph.PointPairList();

            for (int i = 0; i < xi.Count; i++)
            {
                v_list.Add(xi[i], vi[i]);
                u_list.Add(xi[i], ui[i * 2]);
            }

            zedGraphControl2.GraphPane.XAxis.Min = -0.01;
            zedGraphControl2.GraphPane.XAxis.Max = 1.0;

            zedGraphControl2.GraphPane.CurveList.Clear();

            zedGraphControl2.GraphPane.Title = "Решение";

            ZedGraph.LineItem CurveV = zedGraphControl2.GraphPane.AddCurve("v(x)", v_list, Color.FromName("Blue"), ZedGraph.SymbolType.None);
            ZedGraph.LineItem CurveU = zedGraphControl2.GraphPane.AddCurve("v2(x)", u_list, Color.FromName("Red"), ZedGraph.SymbolType.None);

            zedGraphControl2.AxisChange();
            zedGraphControl2.Invalidate();
        }

        void DrawError(List<double> xi, List<double> dif)
        {
            ZedGraph.PointPairList err_list = new ZedGraph.PointPairList();

            for (int i = 1; i < xi.Count; i++)
            {
                err_list.Add(xi[i], dif[i]);
            }

            zedGraphControl1.GraphPane.XAxis.Min = -0.01;
            zedGraphControl1.GraphPane.XAxis.Max = 1.0;

            zedGraphControl1.GraphPane.CurveList.Clear();

            zedGraphControl1.GraphPane.Title = "Ошибка";

            ZedGraph.LineItem CurveV = zedGraphControl1.GraphPane.AddCurve("error in scheme`s values", err_list, Color.FromName("Green"), ZedGraph.SymbolType.None);

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

    }
}
