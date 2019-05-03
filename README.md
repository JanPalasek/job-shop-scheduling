# job-shop-scheduling
This repository is an implementation of job-shop scheduling problem using C#.

## Command-line arguments:
- Name of input file (file from which the job shop input will be loaded) = *-f* or *--file*
- Path to input directory = *-d* or *--dir*
- Number of threads that will be used = *-t* or *--threads*
- Number of generations of GA = *-g* or *--gen*
- Number of iterations of GA = *-i* or *--it*

Example:
```
dotnet JobShopScheduling.dll -d "Examples" --file="la19.in" --threads=4
```

## Libraries used:
For genetic algorithm evaluation:
- [GeneticSharp](https://github.com/giacomelli/GeneticSharp)
- [AdvancedAlgorithms](https://github.com/justcoding121/Advanced-Algorithms)

Others:
- [NUnit](https://github.com/nunit)
- [OxyPlot](https://github.com/oxyplot/oxyplot)
- [Serilog](https://github.com/serilog/serilog)
