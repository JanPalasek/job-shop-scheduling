using System;

namespace JobShopScheduling
{
    using GeneticSharp.Domain;
    using GeneticSharp.Domain.Crossovers;

    class Program
    {
        static void Main(string[] args)
        {
            new GeneticAlgorithm().Start();
        }
    }
}
