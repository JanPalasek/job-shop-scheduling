namespace JobShopScheduling
{
    using System;

    /// <summary>
    /// Represents one operation of a job.
    /// </summary>
    public class Operation : IEquatable<Operation>
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

        /// <summary>
        /// Order in the job described by <see cref="JobId"/>.
        /// </summary>
        public int Order { get; }

        public double Cost { get; }

        public Operation(int id, int jobId, int machineId, int order, double cost)
        {
            Id = id;
            JobId = jobId;
            MachineId = machineId;
            Order = order;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Job: {JobId}, Order: {Order}, Machine: {MachineId}";
        }

        public bool Equals(Operation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Operation) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}