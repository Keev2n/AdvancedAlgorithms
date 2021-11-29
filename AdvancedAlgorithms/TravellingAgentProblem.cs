using System;

namespace AdvancedAlgorithms
{
    class City
    {
        public double x { get; set; }
        public double y { get; set; }
        public City(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
    class SearchSpaceTravellingAgent
    {
        public City startAndLastCity { get; set; }
        public City[] cities { get; set; }
        public SearchSpaceTravellingAgent(City[] cities, City startAndLastCity)
        {
            this.cities = cities;
            this.startAndLastCity = startAndLastCity;
        }

    }

    class TravellingAgentProblem
    {
        Random r = new Random();
        public City[] HillClimbingRandomRestart(SearchSpaceTravellingAgent searchSpace, double epsilon, int stopCondition)
        {
            City[] p = createRandomCity(searchSpace);
            int number = 0;

            Console.WriteLine($"{number}. try: Current fitness: "+ fitnessCalculateDistance(p));

            while (number < stopCondition)
            {
                number++;
                int pIndex = r.Next(1, (p.Length - 1));
                bool stuck = false;
                while (!stuck)
                {
                    City[] q = argmin(p, pIndex, epsilon);
                    if (q[0]!= null && fitnessCalculateDistance(q) < fitnessCalculateDistance(p))
                    {
                        for (int i = 0; i < p.Length; i++)
                        {
                            p[i] = q[i];
                        }
                    }
                    else
                    {
                        stuck = true;
                    }

                    Console.WriteLine($"{number}. try: Current fitness: " + fitnessCalculateDistance(p));
                }
            }

            return p;
        }
        City[] createRandomCity(SearchSpaceTravellingAgent s)
        {
            City[] newRandomCity = new City[(s.cities.Length + 2)];
            newRandomCity[0] = s.startAndLastCity;
            newRandomCity[(newRandomCity.Length - 1)] = s.startAndLastCity;

            for (int i = 1; i < newRandomCity.Length - 1; i++)
            {
                bool notCorrectIndex;
                do
                {
                    notCorrectIndex = false;
                    int randomIndex = r.Next(0, s.cities.Length);

                    foreach (var city in newRandomCity)
                    {
                        if (city == s.cities[randomIndex])
                        {
                            notCorrectIndex = true;
                            break;
                        }
                    }

                    if (notCorrectIndex == false)
                    {
                        newRandomCity[i] = s.cities[randomIndex];
                    }

                } while (notCorrectIndex);
            }

            return newRandomCity;
        }

        double fitnessCalculateDistance(City[] cities)
        {
            double distanceCities = 0;

            for (int i = 0; i < cities.Length - 1; i++)
            {
                City cityOne = cities[i];
                City cityTwo = cities[(i + 1)];
                distanceCities += distanceTwoCity(cityOne, cityTwo);
            }

            return distanceCities;

        }
        double distanceTwoCity(City one, City two)
        {
            return Math.Sqrt(Math.Pow((two.x - one.x), 2) + Math.Pow((two.y - one.y), 2));
        }
        City[] argmin(City[] cities, int cityIndex, double epsilon)
        {
            City[] maybeBetterCity = new City[cities.Length];
            double bestFitness = -1;

            for (int i = 1; i < (cities.Length - 1); i++)
            {
                if (i == cityIndex)
                {
                    continue;
                }

                if (epsilon > 0)
                {
                    double distance = distanceTwoCity(cities[i], cities[cityIndex]);
                    if (distance > epsilon)
                    {
                        continue;
                    }
                }

                City[] newCity = new City[cities.Length];

                for (int j = 0; j < cities.Length; j++)
                {
                    newCity[j] = cities[j];
                }

                City change = newCity[cityIndex];
                newCity[cityIndex] = newCity[i];
                newCity[i] = change;

                double currentFitness = fitnessCalculateDistance(newCity);

                if (bestFitness == -1)
                {
                    for (int j = 0; j < newCity.Length; j++)
                    {
                        maybeBetterCity[j] = newCity[j];
                    }
                    bestFitness = currentFitness;
                }
                else
                {
                    if (currentFitness < bestFitness)
                    {
                        for (int j = 0; j < newCity.Length; j++)
                        {
                            maybeBetterCity[j] = newCity[j];
                        }
                        bestFitness = currentFitness;
                    }
                }
            }

            return maybeBetterCity;
        }
    }
}
