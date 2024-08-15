using System;
using System.Text;

namespace LPR_381_Project
{
    internal class NonLinear
    {
        private double GoldenRatio = (1 + Math.Sqrt(5)) / 2;

        // Delegate to represent the objective function
        public delegate double ObjectiveFunction(double[] x);

        public string GoldenSectionSearch(ObjectiveFunction f, double a, double b, double tolerance = 1e-6, int maxIterations = 1000)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("Iter\tLower\tUpper\tx1\tx2\tf(x1)\tf(x2)\tGap");

            double c = b - (b - a) / GoldenRatio;
            double d = a + (b - a) / GoldenRatio;

            for (int i = 0; i < maxIterations; i++)
            {
                double fx1 = f(new double[] { c });
                double fx2 = f(new double[] { d });
                double gap = Math.Abs(c - d);

                output.AppendLine($"{i}\t{a:F6}\t{b:F6}\t{c:F6}\t{d:F6}\t{fx1:F6}\t{fx2:F6}\t{gap:F6}");

                if (gap < tolerance)
                {
                    output.AppendLine($"Converged after {i + 1} iterations.");
                    output.AppendLine($"Minimum found at x = {(c + d) / 2:F6}");
                    return output.ToString();
                }

                if (fx1 < fx2)
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

            output.AppendLine($"Maximum iterations ({maxIterations}) reached without convergence.");
            output.AppendLine($"Best estimate of minimum: x = {(c + d) / 2:F6}");
            return output.ToString();
        }

        public string SolveExamples()
        {
            StringBuilder results = new StringBuilder();

            // One-dimensional example: f(x) = x^2
            ObjectiveFunction f1D = x => x[0] * x[0];
            results.AppendLine("Minimizing f(x) = x^2:");
            results.AppendLine(GoldenSectionSearch(f1D, -10, 10));
            results.AppendLine("\n\n");

            // One-dimensional quadratic function: f(x) = (x-3)^2
            ObjectiveFunction quadratic1D = x => Math.Pow(x[0] - 3, 2);
            results.AppendLine("Minimizing f(x) = (x-3)^2:");
            results.AppendLine(GoldenSectionSearch(quadratic1D, 0, 6));
            results.AppendLine("\n\n");

            // One-dimensional exponential function: f(x) = e^x - 2x
            ObjectiveFunction exponential1D = x => Math.Exp(x[0]) - 2 * x[0];
            results.AppendLine("Minimizing f(x) = e^x - 2x:");
            results.AppendLine(GoldenSectionSearch(exponential1D, 0, 2));

            return results.ToString();
        }
    }
}
