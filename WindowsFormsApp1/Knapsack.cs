using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1
{
    public class Knapsack
    {
        public int[] Values { get; private set; }
        public int[] Weights { get; private set; }
        public int Capacity { get; private set; }
        public bool[] BestSolution { get; private set; }
        public int BestValue { get; private set; }
        private StringBuilder iterationLog;

        public Knapsack()
        {
            iterationLog = new StringBuilder();
        }

        public Knapsack readProgramFromRTB(string text)
        {
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            if (lines.Length != 3)
            {
                throw new FormatException("Invalid input format. Expected 3 lines.");
            }

            // Parse objective (max/min and values)
            string[] objectiveParts = lines[0].Split(' ');
            Values = objectiveParts.Skip(1).Select(int.Parse).ToArray();

            // Parse weights and capacity
            string[] weightParts = lines[1].Split(' ');
            Weights = weightParts.Take(weightParts.Length - 2).Select(int.Parse).ToArray();
            Capacity = int.Parse(weightParts.Last());

            if (Values.Length != Weights.Length)
            {
                throw new FormatException("Mismatch in number of values and weights.");
            }

            iterationLog.AppendLine("Knapsack Problem Loaded:");
            iterationLog.AppendLine($"Values: {string.Join(", ", Values)}");
            iterationLog.AppendLine($"Weights: {string.Join(", ", Weights)}");
            iterationLog.AppendLine($"Capacity: {Capacity}");
            iterationLog.AppendLine();

            return this;
        }

        public void Solve()
        {
            BestSolution = new bool[Values.Length];
            BestValue = 0;
            bool[] currentSolution = new bool[Values.Length];

            BranchAndBound(0, 0, 0, currentSolution);

            iterationLog.AppendLine("Final Solution:");
            iterationLog.AppendLine($"Best Value: {BestValue}");
            iterationLog.AppendLine($"Items selected: {string.Join(", ", GetSelectedItems())}");
        }

        private void BranchAndBound(int index, int currentWeight, int currentValue, bool[] currentSolution)
        {
            if (index == Values.Length)
            {
                if (currentValue > BestValue)
                {
                    BestValue = currentValue;
                    Array.Copy(currentSolution, BestSolution, currentSolution.Length);
                }
                return;
            }

            iterationLog.AppendLine($"Node: Index={index}, Weight={currentWeight}, Value={currentValue}");

            // Calculate upper bound
            double upperBound = currentValue + FractionalKnapsack(index, Capacity - currentWeight);

            if (upperBound > BestValue)
            {
                // Try including the item
                if (currentWeight + Weights[index] <= Capacity)
                {
                    currentSolution[index] = true;
                    BranchAndBound(index + 1, currentWeight + Weights[index], currentValue + Values[index], currentSolution);
                    currentSolution[index] = false;
                }

                // Try excluding the item
                BranchAndBound(index + 1, currentWeight, currentValue, currentSolution);
            }
            else
            {
                iterationLog.AppendLine($"Pruning node: UpperBound={upperBound} <= BestValue={BestValue}");
            }
        }

        private double FractionalKnapsack(int startIndex, int remainingCapacity)
        {
            double value = 0;
            for (int i = startIndex; i < Values.Length; i++)
            {
                if (remainingCapacity >= Weights[i])
                {
                    value += Values[i];
                    remainingCapacity -= Weights[i];
                }
                else
                {
                    value += (double)Values[i] / Weights[i] * remainingCapacity;
                    break;
                }
            }
            return value;
        }

        private string GetSelectedItems()
        {
            List<int> selectedItems = new List<int>();
            for (int i = 0; i < BestSolution.Length; i++)
            {
                if (BestSolution[i])
                {
                    selectedItems.Add(i + 1);  // Adding 1 to make it 1-indexed
                }
            }
            return string.Join(", ", selectedItems);
        }

        public string PrintProgram()
        {
            return iterationLog.ToString();
        }
    }
}
