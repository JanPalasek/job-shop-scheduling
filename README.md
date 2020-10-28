# job-shop-scheduling
This repository is an implementation of job-shop scheduling problem using C#. Algorithm is described <a href="http://janpalasek.com/jobshop.html">here</a>.

[![CircleCI](https://circleci.com/gh/JanPalasek/job-shop-scheduling.svg?style=svg)](https://circleci.com/gh/JanPalasek/job-shop-scheduling)

## How to run the project
Command-line arguments:
- Path to the input file (file from which the job shop input will be loaded) = *-f* or *--file* (required)
- Number of threads that will be used = *-t* or *--threads* (optional)
- Number of generations of GA = *-g* or *--gen* (optional)
- Number of iterations of GA = *-i* or *--it* (optional)

If value for optional argument is not specified, default value defined in *Config* class will be used.

Example:
```
dotnet JobShopScheduling.dll --file="Examples/la19.in" --threads=4
```

The program will output three files into the execution directory:
1. *{inputFileName}.log* - contains all logs from evaluation,
2. *{inputFileName}.svg* - plot showing progression of the best individual,
3. *{outputFileName}.out* - contains the best schedules found both for the adaptive and non-adaptive algorithm and their lengths. The schedule is described as list of machines and their operations in the order of the execution.

Schedule example:
```
...
Machine 0: 0 8 6
Machine 1: ...
...
```
This describes that on machine 0 the first operation executing will be the operation 0, then 8, then 6,...

### Input file
Input file is expected to have first line specifying jobs and machines count in the following format.
```
{jobs count} {machines count}
```
Every new line specifies new jobs operations and costs.
```
{machine ID} {cost} {machine ID} {cost} ...
```

Example of input file:
```
3 3
0 2 1 3 2 1
1 13 2 10 0 1
1 2 0 3 2 3
```

The example input file specifies, that number of jobs is 3 and number of machines is also 3. Additionally, the first job has three operations, where the first must be performed on machine 0 and has cost 2, the second one must be performed on the 1st machine and has cost 3 and the third one must be performed on 2nd machine and must have cost 1, the second job has three operations, where the first one must be performed on 1st machine and has cost 13, etc.

Further constraints have on the input file have to be met:
1. Machine IDs have to be indexed from 0 to M-1 continuously, where M is number of machines (e.g. there must not be machine 1 missing in between machine 0 and machine 2, and there must not be machine 0 missing in the data),
2. each machine has to have at least 3 operations,
3. each job has to have at least 3 operations.

## Libraries used:
For the genetic algorithm evaluation:
- [GeneticSharp](https://github.com/giacomelli/GeneticSharp)
- [AdvancedAlgorithms](https://github.com/justcoding121/Advanced-Algorithms)

Others:
- [NUnit](https://github.com/nunit)
- [OxyPlot](https://github.com/oxyplot/oxyplot)
- [Serilog](https://github.com/serilog/serilog)
