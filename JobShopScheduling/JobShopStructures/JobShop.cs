namespace JobShopScheduling.JobShopStructures
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents job shop task.
    ///
    /// Contains list of jobs <see cref="Jobs"/>. Job consists of operations (<see cref="Operation"/>).
    /// Each operation can be done on specific machine (<see cref="Operation.MachineId"/>).
    /// Machines must be indexed from 0 to <see cref="MachinesCount"/>.
    /// </summary>
    public class JobShop
    {
        /// <summary>
        /// List of jobs that need to be scheduled in order to fulfill the goal.
        /// </summary>
        public IReadOnlyList<Job> Jobs { get; }

        /// <summary>
        /// List of operations.
        /// </summary>
        public Operation[] Operations { get; }

        /// <summary>
        /// Index represents machine of Id and maps to list of operations that must be done on that particular machine.
        /// </summary>
        public Operation[][] MachineOperations { get; }

        /// <summary>
        /// Number of machines.
        /// </summary>
        public int MachinesCount { get; }

        public void Verify()
        {
            // TODO: implement verifying the input
        }

        public JobShop(List<Job> jobs)
        {
            Jobs = jobs;
            Operations = Jobs.SelectMany(x => x.Operations).ToArray();
            MachinesCount = Operations.Max(x => x.MachineId) + 1;

            MachineOperations = new Operation[MachinesCount][];
            for (int i = 0; i < MachineOperations.Length; i++)
            {
                MachineOperations[i] = Operations.Where(x => x.MachineId == i).ToArray();
            }
        }

        public JobShop(List<Operation> operations)
        {
            var groupedByJob = operations.GroupBy(x => x.JobId).OrderBy(x => x.Key);
            var jobs = groupedByJob.Select(x => new {JobId = x.Key, JobOperations = x.OrderBy(y => y.Order).ToList()})
                .Select(x => new Job(x.JobOperations)).ToList();

            Operations = operations.ToArray();
            Jobs = jobs;
            MachinesCount = Operations.Max(x => x.MachineId) + 1;

            MachineOperations = new Operation[MachinesCount][];
            for (int i = 0; i < MachineOperations.Length; i++)
            {
                MachineOperations[i] = Operations.Where(x => x.MachineId == i).ToArray();
            }
        }
    }
}