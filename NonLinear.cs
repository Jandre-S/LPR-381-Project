using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class NonLinear
    {
        private double GoldenRatio = (1 + Math.Sqrt(5)) / 2;

        // Delegate to represent the objective function
        public delegate double ObjectiveFunction(double[] x);

        public double GoldenSectionSearch(ObjectiveFunction f, double a, double b, double tolerance = 1e-6, int maxIterations = 1000)
        {
            double c = b - (b - a) / GoldenRatio;
            double d = a + (b - a) / GoldenRatio;

            for (int i = 0; i < maxIterations; i++)
            {
                if (Math.Abs(c - d) < tolerance)
                {
                    return (c + d) / 2;
                }

                if (f(new double[] { c }) < f(new double[] { d }))
                {
                    b = d;
                }
                else
                {
                    a = c;
                }

                c = b - (b - a) / GoldenRatio;
                d = a + (b - a) / GoldenRatio;
            }

            return (c + d) / 2;
        }

        public string SolveExamples()
        {
            // One-dimensional example: f(x) = x^2
            ObjectiveFunction f1D = x => x[0] * x[0];
            double result1D = GoldenSectionSearch(f1D, -10, 10);
            string resultString1D = $"Minimum of x^2 found at x = {result1D:F6}";

            // One-dimensional slice of Rosenbrock function: f(x) = (1-x)^2 + 100(1-x^2)^2
            ObjectiveFunction rosenbrock1D = x => Math.Pow(1 - x[0], 2) + 100 * Math.Pow(1 - x[0] * x[0], 2);
            double resultRosenbrock1D = GoldenSectionSearch(rosenbrock1D, -2, 2);
            string resultStringRosenbrock1D = $"Minimum of 1D slice of Rosenbrock function found at x = {resultRosenbrock1D:F6}";

            // One-dimensional quadratic function: f(x) = (x-3)^2
            ObjectiveFunction quadratic1D = x => Math.Pow(x[0] - 3, 2);
            double resultQuadratic1D = GoldenSectionSearch(quadratic1D, 0, 6);
            string resultStringQuadratic1D = $"Minimum of (x-3)^2 found at x = {resultQuadratic1D:F6}";

            // One-dimensional exponential function: f(x) = e^x - 2x
            ObjectiveFunction exponential1D = x => Math.Exp(x[0]) - 2 * x[0];
            double resultExponential1D = GoldenSectionSearch(exponential1D, 0, 2);
            string resultStringExponential1D = $"Minimum of e^x - 2x found at x = {resultExponential1D:F6}";

            // Concatenate all the results into a single string
            return $"{resultString1D}\n{resultStringRosenbrock1D}\n{resultStringQuadratic1D}\n{resultStringExponential1D}";
        }

    }

}
