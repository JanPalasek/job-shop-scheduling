namespace JobShopScheduling
{
    /// <summary>
    /// Represents one operation of a job.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Unique Id across all operations.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Id of the job this operation is part of.
        /// </summary>
        public int JobId { get; }

        /// <summary>
        /// Id of the machine this operations needs to be performed on.
        /// </summary>
        public int MachineId { get; }

        public Operation(int id, int jobId, int machineId)
        {
            Id = id;
            JobId = jobId;
            MachineId = machineId;
        }
    }
}