# Search based Manual - Organica Wild

## General Concept

In the search based approach you search for the best solution within i certain population. 
The population consists of individuals which are ranked based on their fitness. The best one survive the worst ones get sortet out until we stop with some optimal solutions called the pareto front.
Out of those you choose the one you want.

## How to use this to generate things

For PCG you can use this to tame other techniques and add constraints on the generation.
For example you can test for playability in the of levels or quality of the result.
In the samples there is a example where a search based approach is used to constrain a completely random generation of a forest.
The constraints try to make the forest as big as possible and move the start and end of the forest level as far away from each other as possible.

## How to make your own search

In the framework you can use the implemented class `Nsga2Algorithm` to search for your an optimal solution.
To use it simply pass your starting population and your fitness functions.
Your individual can be anything. It simply needs to inherit from `INsga2Invididual` or `AbstractNsga2Indivdual` (has some boring bookkeeping pre implemented).
For your fitness functions you need to create classes that inherit from `AbstractNsga2FitnessFunction<T>`.
Implement so that the result of the fitness function is smaller the better the fitness is.
NSGA-II optimize into the negative.