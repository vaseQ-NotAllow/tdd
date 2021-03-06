using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private Point center;
        private SortedSet<Point> cornerPoints;
        private HashSet<Rectangle> rectangles;
        private Dictionary<Point, int> countOfRectanglesOnPoint;

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
            cornerPoints = new SortedSet<Point>(new PointRadiusComparer(center)) {center};
            rectangles = new HashSet<Rectangle>();
            countOfRectanglesOnPoint = new Dictionary<Point, int>{{center, 4}};
        }

        public CircularCloudLayouter() : this(Point.Empty)
        {
        }

        public HashSet<Rectangle> Centreings()
        {
            var centreingsCloudLayout = new CircularCloudLayouter(center);
            var newRectangles = new HashSet<Rectangle>();
            foreach (var rectangle in rectangles.OrderBy(x =>  -x.Width * x.Height))
                newRectangles.Add(centreingsCloudLayout.PutNextRectangle(rectangle.Size));

            return newRectangles;
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                throw new ArgumentException("rectangleSize");
            
            foreach (var possibleLocation in cornerPoints)
            {
                foreach (var rectangle in RectanglesHelper.GetAllPossibleRectangles(possibleLocation,
                    rectangleSize))
                {
                    if (RectanglesHelper.HaveRectangleIntersectWithAnother(rectangle, rectangles))
                        continue;
                    rectangles.Add(rectangle);
                    foreach (var corner in RectanglesHelper.GetCorners(rectangle))
                    {
                        cornerPoints.Add(corner);
                        if (corner != possibleLocation)
                            countOfRectanglesOnPoint[corner] = 3;
                    }
                    if (--countOfRectanglesOnPoint[possibleLocation] != 0) 
                        return rectangle;
                    cornerPoints.Remove(possibleLocation);
                    countOfRectanglesOnPoint.Remove(possibleLocation);
                    return rectangle;
                }
            }

            throw new Exception("UnExcepted Error");
        }

        public Bitmap Visualization() =>
            TagsCloudVisualization.Visualization.VisualizationFil(rectangles, center.X * 2, center.Y * 2);
    }
}