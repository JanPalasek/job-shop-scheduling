namespace JobShopScheduling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class JobShopLoader
    {
        public JobShop Load(string path)
        {
            using (var reader = new StreamReader(path))
            {
                int[] jobsAndMachinesCount = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                int jobsCount = jobsAndMachinesCount[0];
                var jobs = new List<Job>();
                int operationId = 0;
                for (int jobId = 0; jobId < jobsCount; jobId++)
                {
                    var operations = new List<Operation>();
                    string[] lineArgs = reader.ReadLine().Split(new []{ ' '}, StringSplitOptions.RemoveEmptyEntries);

                    for (int order = 0; order < lineArgs.Length / 2; order += 2)
                    {
                        int machineId = int.Parse(lineArgs[order * 2]);
                        double cost = double.Parse(lineArgs[order * 2 + 1]);
                        cost = Math.Abs(cost) < 10e-12 ? cost + 10e-6 : cost;
                        operations.Add(new Operation(operationId, jobId, machineId, order, cost));
                        operationId++;
                    }

                    jobs.Add(new Job(operations));
                }

                var jobShop = new JobShop(jobs);

                return jobShop;
            }
        }
    }
}