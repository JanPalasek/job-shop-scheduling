namespace JobShopScheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain.Randomizations;

    /// <summary>
    /// Component that can generate <see cref="JobShop"/>.
    /// </summary>
    public class JobShopGenerator
    {
        /// <summary>
        /// Creates new instance of <see cref="JobShop"/>.
        /// </summary>
        /// <param name="jobOperationCounts">Array where index i contains number of operations that i-th job has.</param>
        /// <param name="machinesCount">Total number of machines.</param>
        /// <returns>Returns new instance of <see cref="JobShop"/>.</returns>
        public JobShop Generate(int[] jobOperationCounts, int machinesCount)
        {
            var jobs = new List<Job>();
            int maximumOperationCost = 50;

            int lastFreeOperationId = 0;
            for (int jobId = 0; jobId < jobOperationCounts.Length; jobId++)
            {
                int operationCount = jobOperationCounts[jobId];
                var operations = new List<Operation>();
                for (int operationOrder = 0; operationOrder < operationCount; operationOrder++)
                {
                    int machineId = RandomizationProvider.Current.GetInt(0, machinesCount);
                    double operationCost = RandomizationProvider.Current.GetDouble(0, maximumOperationCost);

                    var operation = new Operation(lastFreeOperationId, jobId, machineId, operationOrder, operationCost);
                    operations.Add(operation);

                    lastFreeOperationId++;
                }

                var job = new Job(operations);
                jobs.Add(job);
            }

            var jobShop = new JobShop(jobs);
            return jobShop;
        }
    }
}