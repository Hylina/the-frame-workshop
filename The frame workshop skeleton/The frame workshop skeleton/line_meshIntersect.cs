using System;
using System.Collections;
using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace Line_meshIntersect
{
    internal class Program
    {
        public static List<Vector3> line_meshIntersect(Mesh mesh, Vector3 linePointA, Vector3 linePointB)
        // returns a list of all the intersections of a line and an object mesh between A and B
        //     if there are no intersections, returns an empty list
        //     if there is an infinity of intersections with a specific triangle of the mesh, will only keep one of those points
        {
            // init output list
            List<Vector3> points = new List<Vector3>();

            // init recursively used constant variable and recursive counter
            int meshTrianglesLength = mesh.triangles.Length;
            int counter = 0;

            while (counter < mesh.triangles.Length)
            // traversal of all the triangles of the mesh
            {
                // init triangle
                Vector3[] triangle = new Vector3[] {mesh.vertices[mesh.triangles[counter]], mesh.vertices[mesh.triangles[counter + 1]], mesh.vertices[mesh.triangles[counter + 2]]};

                // get intersection point between the plane defined by the thiangle and the line defined by A and B
                Vector4 point = __getIntersectPoint(__getPlaneEquation(__getNormalVect(triangle), triangle[0]),
                    new[] {linePointA, linePointB});

                if (__isInTriangle(triangle, point))
                {
                    points.Add(point);
                }
            }

            points = __removeOutOfInterval(points, linePointA, linePointB);
            
            return __sort(points, linePointA);
        }

        private static Vector3 __getNormalVect(Vector3[] triangle)
        // returns a Vector3 with the coordinates of a normal vector to the plane defined by the triangle in input
        {
            // init vectors AB and AC
            Vector3 AB = new Vector3(triangle[2].x - triangle[1].x, triangle[2].y - triangle[1].y, triangle[2].z - triangle[1].z);
            Vector3 AC = new Vector3(triangle[3].x - triangle[1].x, triangle[3].y - triangle[1].y, triangle[3].z - triangle[1].z);

            // init the coordinates of the output
            float a, b, c;
            
            // finding a, b, c, cases depending on null values to avoid division by 0
            if (AB.x == 0 && AC.x == 0)
            {
                if (AB.z != 0)
                {
                    b = (AC.z * AB.x / AB.z - AC.x) / (AC.y - AC.z * AB.y / AB.z);
                    c = (AB.x - AB.y * b) / AB.z;
                    a = 1;
                }
                else
                {
                    b = (AB.z * AC.x / AC.z - AB.x) / (AB.y - AB.z * AC.y / AC.z);
                    c = (AC.x - AC.y * b) / AC.z;
                    a = 1;
                }
            }

            if (AB.y == 0 && AC.y == 0)
            {
                if (AB.x != 0)
                {
                    c = (AC.x * AB.y / AB.x - AC.y) / (AC.z - AC.x * AB.z / AB.x);
                    a = (AB.y - AB.z * c) / AB.x;
                    b = 1;
                }
                else
                {
                    c = (AB.x * AC.y / AC.x - AB.y) / (AB.z - AB.x * AC.z / AC.x);
                    a = (AC.y - AC.z * c) / AC.x;
                    b = 1;
                }
            }
            
            // else

            if (AB.x != 0)
            {
                b = (AC.x * AB.z / AB.x - AC.z) / (AC.y - AC.x * AB.y / AB.x);
                a = (AB.z - AB.y * b) / AB.x;
                c = 1;
            }
            else
            {
                b = (AB.x * AC.z / AC.x - AB.z) / (AB.y - AB.x * AC.y / AC.x);
                a = (AC.z - AC.y * b) / AC.x;
                c = 1;
            }
            
            return new Vector3(a, b, c);
        }

        private static Vector4 __getPlaneEquation(Vector3 normalVect, Vector3 pointOfPlane)
        // returns a Vector4 with the 4 coefficients of the carthesian equation of a plane
        {
            // init Vector4 output
            Vector4 res = new Vector4();
            
            //init coordinates
            float a = normalVect.x;
            float b = normalVect.y;
            float c = normalVect.z;
            float d;
            
            // finding d
            d = -a * pointOfPlane.x - b * pointOfPlane.y - c * pointOfPlane.z;
            
            // setting w, x, y, z
            res.w = a;
            res.x = b;
            res.y = c;
            res.z = d;
            
            //output
            return res;
        }

        private static Vector4 __getIntersectPoint(Vector4 plane, Vector3[] line)
        // returns the intersections point between a plane and a line with ({-1 if none; 1 if inf, x, y, z})
        //     if the coordinate_0 is -1 or 1, x = y = z = 0.  Else coordinate_0 = 0
        {
            // find denominator of t
            float tDenom = plane.w * (line[0].x - line[1].x) + plane.x * (line[0].y - line[1].y) + plane.y * (line[0].z - line[1].z);
            
            // test if line is strictly parallel or included in the plane.  If true returns corresponding Vector

            // test if line is included in plane (infinite number of solutions)
            if (plane.w*line[0].x + plane.x*line[0].y + plane.y*line[0].z + plane.z == 0 &&
                plane.w*line[1].x + plane.x*line[1].y + plane.y*line[1].z + plane.z == 0)
            {
                return new Vector4(1, 0, 0, 0);
            }

            // test if there are no solutions (denominator of t is null)
            if (tDenom == 0)
            {
                return new Vector4(-1, 0, 0, 0);
            }
            
            // find t
            float t = -(plane.w * line[1].x + plane.x * line[1].y + plane.y * line[1].z + plane.z) / tDenom;

            if (t == float.PositiveInfinity || t == float.NegativeInfinity)
            {
                return new Vector4(1, 0, 0, 0);
            }
            
            // find Xm, Ym, Zm
            Vector4 res = new Vector4();

            res.x = (line[0].x - line[1].x) * t + line[1].x;
            res.y = (line[0].y - line[1].y) * t + line[1].y;
            res.z = (line[0].z - line[1].z) * t + line[1].z;
            
            //output
            return res;
        }

        private static bool __isInTriangle(Vector3[] triangle, Vector4 pointIntersect)
        // return true if the interection point is contained in the triangle, else return false
        {
            if (pointIntersect.w == -1)
            // if there is no intersection
            {
                return false;
            }

            if (pointIntersect.w == 1)
            // if the intersection is infinite
            {
                // return a point of the line that is in the triangle
                return false;
            }
            Vector3 point = new Vector3(pointIntersect.x, pointIntersect.y, pointIntersect.z);
            return __isSameSide(point, triangle[1], triangle[2], triangle[3])
                   && __isSameSide(point, triangle[2], triangle[3], triangle[1])
                   && __isSameSide(point, triangle[3], triangle[1], triangle[2]);
        }
        
        

        private static bool __isSameSide(Vector3 point1, Vector3 point2, Vector3 linePoint1, Vector3 linePoint2)
        // return true if point1 and point2 are on the same side the line defined by the points linePoint1 and linePoint2, assuming all four poitns are on teh same plane
        {
            return __ScalarMultMult(__VectorMult(linePoint2 - linePoint1, point1 - linePoint1), __VectorMult(linePoint2 - linePoint1, point2 - linePoint1)) >= 0;
        }

        private static Vector3 __VectorMult(Vector3 vect1, Vector3 vect2)
        // returns the vector product of two vectors
        {
            return new Vector3(vect1.y*vect2.z - vect1.z*vect2.y, vect1.z*vect2.x - vect1.x*vect2.z, vect1.x*vect2.y - vect1.y*vect2.x);
        }
        
        private static float __ScalarMultMult(Vector3 vect1, Vector3 vect2)
        // returns the product of all teh coeeficients of two vectors
        {
            return vect1.x * vect1.y * vect1.z * vect2.x * vect2.y * vect2.z;
        }

        private static List<Vector3> __removeOutOfInterval(List<Vector3> points, Vector3 pointA, Vector3 pointB)
        //  assuming that all points considered are aligned, removes all the points that are not between A and B and returns the resulting list
        {
            // init output list
            List<Vector3> newPoints = new List<Vector3>();
            
            // init temporary values
            float xMin = Math.Min(pointA.x, pointB.x);
            float xMax = Math.Max(pointA.x, pointB.x);
            
            
            if (pointA.x != pointB.x)
            // gerenal case
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (xMin < points[i].x && points[i].x < xMax)
                    {
                        newPoints.Add(points[i]);
                    }
                }
            }
            else if(pointA.y != pointB.y)
            // specific case 1
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (xMin < points[i].y && points[i].y < xMax)
                    {
                        newPoints.Add(points[i]);
                    }
                }
            }
            else
            // specific case 2
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (xMin < points[i].z && points[i].z < xMax)
                    {
                        newPoints.Add(points[i]);
                    }
                }
            }
            
            // output
            return newPoints;
        }

        private static List<Vector3> __sort(List<Vector3> points, Vector3 point)
        // sorts all the points according to their proximity to the point
        {
            // init of list of point that has tuples of distance and index
            List<double[]> distancePoints = new List<double[]>();
            for (int i = 0; i < points.Count; i++)
            {
                distancePoints.Add(new []{__Distance(points[i], point), i});
            }

            // sorts the list distanceList by increasing distance (parameter 1 of the "tuples")
            bool isSorted = false;
            while (!isSorted)
            {
                isSorted = true;
                for (int i = 0; i < distancePoints.Count - 1; i++)
                {
                    if (distancePoints[i][0] > distancePoints[i + 1][0])
                    {
                        isSorted = false;
                        double[] temp = distancePoints[i];
                        distancePoints[i] = distancePoints[i + 1];
                        distancePoints[i + 1] = temp;
                    }
                }
            }

            // creating the output list, which has the Vector3 s ordered by distance
            List<Vector3> res = new List<Vector3>();
            for (int i = 0; i < distancePoints.Count; i++)
            {
                res.Add(points[(int) (distancePoints[i][1])]);
            }

            return res;
        }

        private static double __Distance(Vector3 point, Vector3 distancePoint)
        // returns the distance between two point and distancePoint as a double
        {
            return Math.Sqrt(((distancePoint.x - point.x) * (distancePoint.x - point.x) +
                              (distancePoint.y - point.y) * (distancePoint.y - point.y) +
                              (distancePoint.z - point.z) * (distancePoint.z - point.z)));
        }
    }
}