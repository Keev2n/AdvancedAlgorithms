using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedAlgorithms
{
    class Point
    {
        public int xCoordinates { get; set; }
        public int yCoordinates { get; set; }
        public Point(int xCoordinates, int yCoordinates)
        {
            this.xCoordinates = xCoordinates;
            this.yCoordinates = yCoordinates;
        }
    }

    class SearchSpaceSmallestBoundary
    {
        public int xCoordinates { get; set; }
        public int yCoordinates { get; set; }
        public List<Point> definedCoordinates { get; set; }
        public SearchSpaceSmallestBoundary(int xCoordinates, int yCoordinates, List<Point> definedCoordinates)
        {
            this.xCoordinates = xCoordinates;
            this.yCoordinates = yCoordinates;
            this.definedCoordinates = definedCoordinates;
        }
    }
    class Boundary
    {
        public Point[] points = new Point[4];
        public double fitness { get; set; }
        public double calculatePerimeter()
        {
            double perimeter = 0;

            perimeter += Math.Sqrt(Math.Pow((points[1].xCoordinates - points[0].xCoordinates), 2) + Math.Pow((points[1].yCoordinates - points[0].yCoordinates), 2));
            perimeter += Math.Sqrt(Math.Pow((points[2].xCoordinates - points[1].xCoordinates), 2) + Math.Pow((points[2].yCoordinates - points[1].yCoordinates), 2));
            perimeter += Math.Sqrt(Math.Pow((points[3].xCoordinates - points[2].xCoordinates), 2) + Math.Pow((points[3].yCoordinates - points[2].yCoordinates), 2));
            perimeter += Math.Sqrt(Math.Pow((points[0].xCoordinates - points[3].xCoordinates), 2) + Math.Pow((points[0].yCoordinates - points[3].yCoordinates), 2));

            return perimeter;
        }
        double distanceFromLine(Point linePoint1, Point linePoint2, Point p)
        {
            double distance = 0;
            distance = ((linePoint2.xCoordinates - linePoint1.xCoordinates) * (linePoint1.yCoordinates - p.yCoordinates) -
                (linePoint1.xCoordinates - p.xCoordinates) * (linePoint2.yCoordinates - linePoint1.yCoordinates)) /
                Math.Sqrt(Math.Pow((linePoint2.xCoordinates - linePoint1.xCoordinates), 2) + Math.Pow((linePoint2.yCoordinates - linePoint1.yCoordinates), 2));
            return distance;
        }
        bool pointInsideOrNot(Point p)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Point p1 = points[i];
                Point p2 = points[(i + 1) % points.Length];
                double distance = distanceFromLine(p1, p2, p);
                if (distance < 0)
                {
                    return false;
                }
            }

            return true;
        }
        public bool allPointsInsideOrNot(SearchSpaceSmallestBoundary s)
        {
            foreach (var definedPoints in s.definedCoordinates)
            {
                if (pointInsideOrNot(definedPoints) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }

    class SmallestBoundaryProblem
    {
        Random r = new Random();
        public (Point[], double) geneticAlgorithm(SearchSpaceSmallestBoundary searchSpace, int numberPopulation, int stopCondition, int mutateRate, int crossOverRate)
        {
            Boundary[] p = initializePopulation(searchSpace, numberPopulation);
            evaluation(p, searchSpace);

            var best = argMin(p);
            Point[] pBest = best.Item1;
            double bFitness = best.Item2;

            log(pBest, bFitness);

            int counter = 0;
            while (counter < stopCondition)
            {
                List<Boundary> p2 = new List<Boundary>();
                List<Boundary> m = selectParents(p); 
                normalizeFitnessPop(m);
                while (p2.Count < p.Length)
                {
                    List<Boundary> selected = selection(m);
                    List<Boundary> c = crossOver(selected, crossOverRate);
                    c = mutate(c, mutateRate);
                    p2.AddRange(c);
                }

                for (int i = 0; i < p.Length; i++)
                {
                    p[i] = p2[i];
                }

                evaluation(p, searchSpace);
                best = argMin(p);
                pBest = best.Item1;
                bFitness = best.Item2;

                counter++;
                log(pBest, bFitness);
            }

            return (pBest, bFitness);
         
        }
        Boundary[] initializePopulation(SearchSpaceSmallestBoundary searchSpace, int numberPopulation)
        {
            Boundary[] population = new Boundary[numberPopulation];

            for (int i = 0; i < numberPopulation; i++)
            {
                Boundary b = new Boundary();

                do
                {
                    population[i] = b;

                    int xCoordinates = r.Next(0, (searchSpace.xCoordinates + 1));
                    int yCoordinates = r.Next(0, (searchSpace.yCoordinates + 1));

                    Point p0 = new Point((xCoordinates + r.Next(-searchSpace.xCoordinates, 0)), (yCoordinates + r.Next(0, (searchSpace.yCoordinates + 1))));
                    Point p1 = new Point((xCoordinates + r.Next(0, (searchSpace.xCoordinates + 1))), (yCoordinates + r.Next(0, (searchSpace.yCoordinates + 1))));
                    Point p2 = new Point((xCoordinates + r.Next(0, (searchSpace.xCoordinates + 1))), (yCoordinates + r.Next(-searchSpace.yCoordinates, 0)));
                    Point p3 = new Point((xCoordinates + r.Next(-searchSpace.xCoordinates, 0)), (yCoordinates + r.Next(-searchSpace.yCoordinates, 0)));

                    population[i].points[0] = p0;
                    population[i].points[1] = p1;
                    population[i].points[2] = p2;
                    population[i].points[3] = p3;

                } while (!b.allPointsInsideOrNot(searchSpace));
            }

            return population;
        }
        void evaluation(Boundary[] population, SearchSpaceSmallestBoundary s) 
        {
            foreach (var item in population)
            {
                bool inside = item.allPointsInsideOrNot(s);

                if (inside == true)
                {
                    item.fitness = item.calculatePerimeter();
                }
                else
                {
                    item.fitness = 3 * 2 * (s.xCoordinates + s.yCoordinates);
                }
            }
        }
        (Point[], double) argMin(Boundary[] population)
        {
            Point[] pBest = population[0].points;
            double bFitness = population[0].fitness; 

            foreach (var item in population)
            {
                if (item.fitness < bFitness)
                {
                    bFitness = item.fitness;

                    for (int i = 0; i < 4; i++)
                    {
                        pBest[i] = item.points[i];
                    }
                }
            }

            return (pBest, bFitness);
        }

        List<Boundary> selectParents(Boundary[] population)
        {
            List<Boundary> population2 = new List<Boundary>();

            foreach (var item in population)
            {
                Boundary b = new Boundary();
                b.fitness = item.fitness;

                for (int i = 0; i < 4; i++)
                {
                    b.points[i] = item.points[i];
                }

                population2.Add(b);
            }

            return population2;
        }
        void normalizeFitnessPop(List<Boundary> M)
        {
            double fitnessMax = M.Max(x => x.fitness);
            double fitnessMin = M.Min(x => x.fitness);
            double fitnessSum = M.Sum(x => x.fitness);

            foreach (var item in M)
            {
                item.fitness = (item.fitness - fitnessMin) / (fitnessMax - fitnessMin);
            }
        }
        List<Boundary> selection(List<Boundary> M)
        {
            List<Boundary> selected = new List<Boundary>();
            double fitnessSum = M.Sum(x => x.fitness);

            foreach (var item in M)
            {
                double p = 1 - (item.fitness / fitnessSum);
                double number = r.Next(100);
                number = number / 100;
                if (number < p)
                {
                    selected.Add(item);
                }
            }

            return selected;
        }
        List<Boundary> crossOver(List<Boundary> C, int crossOverRate)
        {
            List<int> indexContains = new List<int>();
            List<Boundary> oldAndCrossed = new List<Boundary>();

            bool contains = false;
            int index = 0;
            int index2 = 0;

            if (C.Count % 2 != 0)
            {
                index = r.Next(0, C.Count);
                oldAndCrossed.Add(C[index]);
                indexContains.Add(index);
            }

            for (int i = 0; i < C.Count / 2; i++)
            {
                do
                {
                    index = r.Next(0, C.Count());
                    index2 = r.Next(0, C.Count());

                    if (index == index2 || indexContains.Contains(index) || indexContains.Contains(index2))
                    {
                        contains = true;
                        continue;
                    }
                    contains = false;

                    indexContains.Add(index);
                    indexContains.Add(index2);

                } while (contains);

                int doICrossOver = r.Next(0, 100);

                if (doICrossOver < crossOverRate)
                {
                    Boundary crossedOne = new Boundary();
                    Boundary crossedTwo = new Boundary();
                    crossedOne.points[0] = C[index].points[0];
                    crossedOne.points[1] = C[index].points[1];
                    crossedOne.points[2] = C[index2].points[2];
                    crossedOne.points[3] = C[index2].points[3];

                    crossedTwo.points[0] = C[index2].points[0];
                    crossedTwo.points[1] = C[index2].points[1];
                    crossedTwo.points[2] = C[index].points[2];
                    crossedTwo.points[3] = C[index].points[3];

                    oldAndCrossed.Add(crossedOne);
                    oldAndCrossed.Add(crossedTwo);
                }
                else 
                {
                    oldAndCrossed.Add(C[index]);
                    oldAndCrossed.Add(C[index2]);
                }
            }
            
            return oldAndCrossed;
        }
        List<Boundary> mutate(List<Boundary> C, int mutateRate)
        {
            int db = 0;
            foreach (var item in C)
            {
                int randomMutate = r.Next(0, 100);

                if (randomMutate < mutateRate)
                {
                    int coordinatesToMutate = r.Next(0, 4);

                    switch (coordinatesToMutate)
                    {
                        case 0:
                            item.points[0].xCoordinates += 1;
                            item.points[0].yCoordinates -= 1;
                            break;
                        case 1:
                            item.points[1].xCoordinates -= 1;
                            item.points[1].yCoordinates -= 1;
                            break;
                        case 2:
                            item.points[2].xCoordinates -= 1;
                            item.points[2].yCoordinates += 1;
                            break;
                        case 3:
                            item.points[3].xCoordinates += 1;
                            item.points[3].yCoordinates += 1;
                            break;
                    }
                }
                db++;
            }

            return C;
        }
        void log(Point[] pBest, double bFitness)
        {

            Console.WriteLine("The current best fitness: " + bFitness);
            for (int i = 0; i < pBest.Length; i++)
            {
                Console.WriteLine($" Point: {i} X: {pBest[i].xCoordinates} Y: {pBest[i].yCoordinates}");
            }
        }

    }
    
}
