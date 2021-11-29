using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AdvancedAlgorithms
{
    class Pixel
    {
        public int xCoordinate { get; set; }
        public int yCoordinate { get; set; }
        public byte colorRed { get; set; }
        public byte colorGreen { get; set; }
        public byte colorBlue { get; set; }
        public Pixel(byte colorRed, byte colorGreen, byte colorBlue)
        {
            this.colorRed = colorRed;
            this.colorGreen = colorGreen;
            this.colorBlue = colorBlue;
        }
        public Pixel(int xCoordinate, int yCoordinate, byte colorRed, byte colorGreen, byte colorBlue)
        {
            this.xCoordinate = xCoordinate;
            this.yCoordinate = yCoordinate;
            this.colorRed = colorRed;
            this.colorGreen = colorGreen;
            this.colorBlue = colorBlue;
        }
    }
    class Cluster
    {
        public List<Pixel> pixels { get; set; }
        public Cluster()
        {
            pixels = new List<Pixel>();
        }

    }
    static class Image
    {
        static public unsafe List<Pixel> makeImage(Bitmap image)
        {
            List<Pixel> imagePixels = new List<Pixel>();
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width,
                 image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();
            int stride = imageData.Stride;

            for (int y = 0; y < imageData.Height; y++)
            {
                byte* row = scan0 + (y * stride);

                for (int x = 0; x < imageData.Width; x++)
                {
                    int bIndex = x * bytesPerPixel;
                    int gIndex = bIndex + 1;
                    int rIndex = bIndex + 2;

                    byte pixelR = row[rIndex];
                    byte pixelG = row[gIndex];
                    byte pixelB = row[bIndex];

                    Pixel p = new Pixel(x, y, pixelR, pixelG, pixelB);
                    imagePixels.Add(p);
                }
            }
            image.UnlockBits(imageData);
            return imagePixels;
        }
    }
    class ImageSegmentation
    {
        Random r;
        public ImageSegmentation()
        {
            r = new Random();
        }
        public void kmeansAlgorithm(Bitmap image, int clusterNumber, List<Pixel> population)
        {
            Cluster[] clusters = new Cluster[clusterNumber];
            Pixel[] centroids = initializeCentroids(image, clusterNumber);

            Pixel[] centroids2;
            do
            {
                centroids2 = centroids.Select(pixel => new Pixel(pixel.xCoordinate, pixel.yCoordinate, pixel.colorRed,
                    pixel.colorGreen, pixel.colorBlue)).ToArray();

                clusters = Enumerable.Range(0, clusterNumber).Select(x => new Cluster()).ToArray();


                foreach (var pixel in population)
                {
                    int nearest = argMinDS(pixel, centroids2);
                    clusters[nearest].pixels.Add(pixel);
                }

                for (int i = 0; i < centroids.Length; i++)
                {
                    if(clusters[i].pixels.Count != 0)
                    {
                        centroids[i] = centroidMeanCalculate(centroids2[i], clusters[i].pixels);
                    }
                }

            } while (!centroidsAreSame(centroids, centroids2));

            newImageCreate(image, clusters, centroids);
        }
        Pixel[] initializeCentroids(Bitmap image, int clusterNumber)
        {
            Pixel[] centroids = new Pixel[clusterNumber];

            for (int i = 0; i < clusterNumber; i++)
            {
                int xCoordinate = r.Next(0, image.Width);
                int yCoordinate = r.Next(0, image.Height);
                Color color = image.GetPixel(xCoordinate, yCoordinate);
                Pixel centroid = new Pixel(color.R, color.G, color.B);
                centroids[i] = centroid;
            }

            return centroids;
        }
        int argMinDS(Pixel point, Pixel[] centroids)
        {
            int nearest = 0;
            double minLength = Math.Sqrt(Math.Pow((point.colorRed - centroids[0].colorRed), 2) +
                Math.Pow((point.colorBlue - centroids[0].colorBlue), 2) +
                Math.Pow((point.colorGreen - centroids[0].colorGreen), 2));

            for (int i = 1; i < centroids.Length; i++)
            {
                double currentCentroidLength = Math.Sqrt(Math.Pow((point.colorRed - centroids[i].colorRed), 2) +
                    Math.Pow((point.colorBlue - centroids[i].colorBlue), 2) +
                    Math.Pow((point.colorGreen - centroids[i].colorGreen), 2));
                if (currentCentroidLength < minLength)
                {
                    minLength = currentCentroidLength;
                    nearest = i;
                }
            }

            return nearest;
        }
        Pixel centroidMeanCalculate(Pixel centroid, List<Pixel> cluster)
        {
            byte colorRed = Convert.ToByte(cluster.Sum(x => x.colorRed) / cluster.Count);
            byte colorGreen = Convert.ToByte(cluster.Sum(x => x.colorGreen) / cluster.Count);
            byte colorBlue = Convert.ToByte(cluster.Sum(x => x.colorBlue) / cluster.Count);

            return new Pixel(colorRed, colorGreen, colorBlue);
        }
        bool centroidsAreSame(Pixel[] centroid1Collection, Pixel[] centroid2Collection)
        {
            for (int i = 0; i < centroid1Collection.Length; i++)
            {
                if (centroid1Collection[i].colorBlue != centroid2Collection[i].colorBlue ||
                    centroid1Collection[i].colorRed != centroid2Collection[i].colorRed ||
                    centroid1Collection[i].colorGreen != centroid2Collection[i].colorGreen)
                {
                    return false;
                }
            }

            return true;
        }
        unsafe void newImageCreate(Bitmap image, Cluster[] clusters, Pixel[] centroids) 
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width,
                image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();
            int stride = imageData.Stride;

            for (int i = 0; i < clusters.Length; i++)
            {
                if (clusters[i].pixels.Count() != 0)
                {
                    foreach (var pixel in clusters[i].pixels)
                    {
                        Color c = Color.FromArgb(centroids[i].colorRed, centroids[i].colorGreen, centroids[i].colorBlue);
                        byte* row = scan0 + (pixel.yCoordinate * stride);

                        int bIndex = pixel.xCoordinate * bytesPerPixel;
                        int gIndex = bIndex + 1;
                        int rIndex = bIndex + 2;

                        row[rIndex] = centroids[i].colorRed;
                        row[gIndex] = centroids[i].colorGreen;
                        row[bIndex] = centroids[i].colorBlue;
                    }
                }
            }
            image.UnlockBits(imageData);
        }
    }
}
