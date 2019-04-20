namespace JobShopScheduling.JobShopStructures
{
    using System.Collections.Generic;

    public class Job
    {
        public IReadOnlyList<Operation> Operations { get; }

        public Job(List<Operation> operations)
        {
            Operations = operations;
        }
    }
}