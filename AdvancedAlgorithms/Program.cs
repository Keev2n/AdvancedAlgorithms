using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AdvancedAlgorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            //kmeansProgram();
            //smallestBoundryProblem();
            travellingAgentProblem(); 

        }
        static void kmeansProgram()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ImageSegmentation kmeans = new ImageSegmentation();
            Bitmap image = new Bitmap("image.jpg");
            List<Pixel> population = Image.makeImage(image);
            kmeans.kmeansAlgorithm(image, 3, population);
            image.Save("image3.jpg");
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
        static void smallestBoundryProblem()
        {
            List<Point> definedPoints = new List<Point>();
            Point p1 = new Point(5, 6);
            Point p2 = new Point(6, 4);
            Point p3 = new Point(7, 7);
            definedPoints.Add(p1);
            definedPoints.Add(p2);
            definedPoints.Add(p3);
            SearchSpaceSmallestBoundary s = new SearchSpaceSmallestBoundary(20, 20, definedPoints);

            SmallestBoundaryProblem problem = new SmallestBoundaryProblem();
            var result = problem.geneticAlgorithm(s, 1000, 10, 50, 5);
            Console.ReadKey();
        }
        static void travellingAgentProblem()
        {
            string[] something = File.ReadAllLines("travellingAgentProblem.txt");
            string[] firstAndLast = something[0].Split(",");
            City cFirstAndLast = new City(double.Parse(firstAndLast[0]), double.Parse(firstAndLast[1]));
            City[] cities = new City[something.Length - 1];

            for (int i = 1; i < something.Length; i++)
            {
                string[] coordinates = something[i].Split(",");
                City newCity = new City(double.Parse(coordinates[0]), double.Parse(coordinates[1]));
                cities[i - 1] = newCity; 
            }

            SearchSpaceTravellingAgent sT = new SearchSpaceTravellingAgent(cities, cFirstAndLast);
            TravellingAgentProblem p = new TravellingAgentProblem();
            City[] citiesResult = p.HillClimbingRandomRestart(sT, -1, 15);

            StreamWriter sw = new StreamWriter("travellingAgentProblemResult.txt");

            foreach (var city in citiesResult)
            {
                sw.WriteLine(city.x + ", " + city.y);
            }

            sw.Close();
            Console.ReadKey();
        }

    }
}
