# job-shop-scheduling
This repository is an implementation of job-shop scheduling problem using C#.

## Command-line arguments:
- Name of input file (file from which the job shop input will be loaded) = *-f* or *--file* (required)
- Path to input directory = *-d* or *--dir* (optional)
- Number of threads that will be used = *-t* or *--threads* (optional)
- Number of generations of GA = *-g* or *--gen* (optional)
- Number of iterations of GA = *-i* or *--it* (optional)

If value for optional argument is not specified, default value defined in *Config* class will be used.

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
