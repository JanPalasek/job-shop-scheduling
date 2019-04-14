namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Linq;
    using GeneticSharp.Domain.Chromosomes;

    public class ScheduleChromosome : ChromosomeBase
    {
        /// <summary>
        /// List of jobs that have to be done.
        /// </summary>
        private JobShop jobShop;

        public ScheduleChromosome(JobShop jobShop, int machinesCount) : base(machinesCount)
        {
            this.jobShop = jobShop;

            // TODO: initialize genes
        }

        public override Gene GenerateGene(int geneIndex)
        {
            throw new NotSupportedException();
        }

        public override IChromosome CreateNew()
        {
            var machineChromosomes = GetGenes().Select(x => x.Value).Cast<MachineChromosome>();

            var newScheduleChromosome = new ScheduleChromosome(jobShop, Length);
            // create new machine chromosomes and assign it to new schedule
            newScheduleChromosome.ReplaceGenes(0, machineChromosomes
                .Select(x => x.CreateNew()).Select(x => new Gene(x)).ToArray());

            return newScheduleChromosome;
        }

        public override IChromosome Clone()
        {
            var scheduleChromosome = (ScheduleChromosome)base.Clone();
            scheduleChromosome.jobShop = this.jobShop;
            return scheduleChromosome;
        }
    }
}