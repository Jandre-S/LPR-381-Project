using System;
using System.Collections.Generic;
using System.Text;

namespace LPR_381_Project
{
    public class KnapsackModel
    {
        public List<KnapsackItem> Items { get; set; }
        public double Capacity { get; set; }
    }

    public class KnapsackItem
    {
        public double Value { get; set; }
        public double Weight { get; set; }
        public int Index { get; set; }
    }

    public class PriorityQueue<T> where T : IComparable<T>
    {
        private SortedSet<T> set = new SortedSet<T>();

        public void Enqueue(T item)
        {
            set.Add(item);
        }

        public T Dequeue()
        {
            if (set.Count == 0)
                throw new InvalidOperationException("The queue is empty.");

            T item = set.Max;
            set.Remove(item);
            return item;
        }

        public int Count => set.Count;
    }

    public class BranchAndBoundKnapsackAlgorithm
    {
        private KnapsackModel model;
        private double bestValue;
        private List<int> bestSolution;
        private List<TableInfo> tables;

        public BranchAndBoundKnapsackAlgorithm(KnapsackModel model)
        {
            this.model = model;
            this.bestValue = 0;
            this.bestSolution = new List<int>();
            this.tables = new List<TableInfo>();
        }

        public void Solve()
        {
            var priorityQueue = new PriorityQueue<Node>();
            var root = new Node
            {
                Level = -1,
                Solution = new List<int>(),
                UpperBound = CalculateUpperBound(new List<int>(), -1)
            };

            priorityQueue.Enqueue(root);

            int tableNumber = 1;
            while (priorityQueue.Count > 0)
            {
                var node = priorityQueue.Dequeue();

                var tableInfo = new TableInfo
                {
                    TableNumber = tableNumber++,
                    Level = node.Level,
                    Solution = new List<int>(node.Solution),
                    UpperBound = node.UpperBound,
                    CurrentValue = CalculateValue(node.Solution),
                    CurrentWeight = CalculateWeight(node.Solution)
                };
                tables.Add(tableInfo);

                if (node.Level == model.Items.Count - 1)
                {
                    continue;
                }

                var nextLevel = node.Level + 1;
                var item = model.Items[nextLevel];

                // Include the item
                var includedSolution = new List<int>(node.Solution) { 1 };
                if (IsFeasible(includedSolution))
                {
                    var includedNode = new Node
                    {
                        Level = nextLevel,
                        Solution = includedSolution,
                        UpperBound = CalculateUpperBound(includedSolution, nextLevel)
                    };
                    priorityQueue.Enqueue(includedNode);

                    var includedValue = CalculateValue(includedSolution);
                    if (includedValue > bestValue)
                    {
                        bestValue = includedValue;
                        bestSolution = new List<int>(includedSolution);
                    }
                }

                // Exclude the item
                var excludedSolution = new List<int>(node.Solution) { 0 };
                var excludedNode = new Node
                {
                    Level = nextLevel,
                    Solution = excludedSolution,
                    UpperBound = CalculateUpperBound(excludedSolution, nextLevel)
                };
                priorityQueue.Enqueue(excludedNode);
            }
        }

        private double CalculateValue(List<int> solution)
        {
            double value = 0;
            for (int i = 0; i < solution.Count; i++)
            {
                if (solution[i] == 1)
                {
                    value += model.Items[i].Value;
                }
            }
            return value;
        }

        private double CalculateWeight(List<int> solution)
        {
            double weight = 0;
            for (int i = 0; i < solution.Count; i++)
            {
                if (solution[i] == 1)
                {
                    weight += model.Items[i].Weight;
                }
            }
            return weight;
        }

        private bool IsFeasible(List<int> solution)
        {
            return CalculateWeight(solution) <= model.Capacity;
        }

        private double CalculateUpperBound(List<int> solution, int level)
        {
            double bound = CalculateValue(solution);
            double totalWeight = CalculateWeight(solution);

            for (int i = level + 1; i < model.Items.Count; i++)
            {
                if (totalWeight + model.Items[i].Weight <= model.Capacity)
                {
                    bound += model.Items[i].Value;
                    totalWeight += model.Items[i].Weight;
                }
                else
                {
                    var fraction = (model.Capacity - totalWeight) / model.Items[i].Weight;
                    bound += model.Items[i].Value * fraction;
                    break;
                }
            }
            return bound;
        }

        public double GetBestValue() => bestValue;
        public List<int> GetBestSolution() => bestSolution;
        public List<TableInfo> GetTables() => tables;

        public class Node : IComparable<Node>
        {
            public int Level { get; set; }
            public List<int> Solution { get; set; }
            public double UpperBound { get; set; }

            public int CompareTo(Node other)
            {
                return other.UpperBound.CompareTo(UpperBound);
            }
        }
    }

    public class TableInfo
    {
        public int TableNumber { get; set; }
        public int Level { get; set; }
        public List<int> Solution { get; set; }
        public double UpperBound { get; set; }
        public double CurrentValue { get; set; }
        public double CurrentWeight { get; set; }
    }
}