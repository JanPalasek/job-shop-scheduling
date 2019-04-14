namespace JobShopScheduling
{
    using System.Collections.Generic;

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
        public List<Job> Jobs { get; }

        /// <summary>
        /// Number of machines.
        /// </summary>
        public int MachinesCount { get; }

        public void Verify()
        {
            // TODO: implement verifying the input
        }
    }
}