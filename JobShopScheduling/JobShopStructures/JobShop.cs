using System;
using JobShopScheduling.Utils;

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

        public void Validate()
        {
            if (MachinesCount < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(MachinesCount), $"There has to be at least 3 machines. You submitted {MachinesCount}");
            }

            foreach (var operation in Operations)
            {
                if (operation.Order < 0)
                {
                    throw new ArgumentException();
                }

                if (operation.MachineId < 0 || operation.MachineId >= MachinesCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(operation.MachineId), $"Invalid machine id {operation.MachineId} of operation {operation.Id}");
                }

                var machineOperations = MachineOperations[operation.MachineId];

                if (!machineOperations.Any(x => x.Equals(operation)))
                {
                    throw new ArgumentException($"Operation {operation.Id} was not found on machine {operation.MachineId}");
                }
            }

            int[] notFilledEnoughMachines = MachineOperations.IndicesOf(x => x.Length < 3).ToArray();
            if (notFilledEnoughMachines.Length > 0)
            {
                throw new ArgumentException($"Every machine must have at least 2 operations. " +
                                            $"Machines not fulfilling this are: {string.Join(',', notFilledEnoughMachines)}");
            }
        }

        public JobShop(List<Job> jobs, bool validate = true)
        {
            Jobs = jobs;
            Operations = Jobs.SelectMany(x => x.Operations).ToArray();
            MachinesCount = Operations.Max(x => x.MachineId) + 1;

            MachineOperations = new Operation[MachinesCount][];
            for (int i = 0; i < MachineOperations.Length; i++)
            {
                MachineOperations[i] = Operations.Where(x => x.MachineId == i).ToArray();
            }

            if (validate)
            {
                Validate();
            }
        }
    }
}