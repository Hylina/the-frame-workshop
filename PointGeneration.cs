using System;
using System.Collections;
using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace Theframeworkshopskeleton
{
    public class Pointgen
    {
       
        
        private Point __BinarySearchInMesh(Vector3 A, Vector3 B, int n, int count, ref bool linked, string name, Mesh mesh)
        /*
         * Find a link between the center of gravity of the mesh and a point in between the root and the a node in the skeleton
         
         * A : first point of collision with the mesh
         * B : second point of collison with the mesh
         * n : number of wanted try in binary search
         * i : number of tries already made
         * linked : to know wether the link between the root and middle of our node has been made
         * name : name of our point
         * mesh : mesh for which we want to make a skeleton         
         */
        {
            
            Vector3[] vertices = mesh.vertices;

            float middleX = (A.x + B.x) / 2;
            float middleY = (A.y + B.y) / 2;
            float middleZ = (A.z + B.z) / 2;
            Vector3 testPoint = new Vector3(middleX, middleY, middleZ);

            List<Vector3> intersections = Line_meshIntersect.Program.line_meshIntersect(mesh, A, B);
            if (intersections.Count == 0 ) //funcEtienne between root and test point
            {
                linked = true;
                Point intermediate = new Point(name, middleX, middleY, middleZ);
                return intermediate;
            }

            else
            {
                if (count < n)
                {
                    /* We call the binary search on the rigth and the left part of our line. */

                    Point firstPossibility = __BinarySearchInMesh(A, testPoint, n, count + 1, ref linked,  name, mesh);
                    Point SecondPossibility = __BinarySearchInMesh(testPoint, B, n, count + 1, ref linked, name, mesh);

                    /* We compute the two distances between the first possibility and the second. 
                     We want the smallest distance between our current middle point and the next, thus we will keep the point
                     with the smallest distance to our middle point. */

                    float firstDistance = (float) Math.Sqrt((firstPossibility.Coordinates.x - testPoint.x)
                        * (firstPossibility.Coordinates.x - testPoint.x) + (firstPossibility.Coordinates.y - testPoint.y)
                        * (firstPossibility.Coordinates.y - testPoint.y) + (firstPossibility.Coordinates.z - testPoint.z)
                        * (firstPossibility.Coordinates.z - testPoint.z));

                    float secondDistance = (float)Math.Sqrt((SecondPossibility.Coordinates.x - testPoint.x)
                        * (SecondPossibility.Coordinates.x - testPoint.x) + (SecondPossibility.Coordinates.y - testPoint.y)
                        * (SecondPossibility.Coordinates.y - testPoint.y) + (SecondPossibility.Coordinates.z - testPoint.z)
                        * (SecondPossibility.Coordinates.z - testPoint.z));

                    if (firstDistance < secondDistance)
                        return firstPossibility;
                    else
                        return SecondPossibility;
                    
                }

                else
                {
                    /* If we haven't found any possible, we create a point whose coordinates can't be possible*/
                    Point impossible = new Point(name, (float)Double.PositiveInfinity, (float)Double.PositiveInfinity, (float)Double.PositiveInfinity);
                    linked = false;
                    return impossible;
                }

            }
           
            
        }

        private List<Vector3> NormalToLine(Mesh mesh, Vector3 A, Vector3 B, Vector3 C, ref float phi, ref float teta, float radius)
        /*
        * Find the list of collision with the mesh in the circle of radius radius and center C, which is normal to the line AB.

        * mesh : mesh for which we want to make a skeleton  
        * A : first point of collision with the mesh 
        * B : second point of collison with the mesh
        * C : point belonging to the segment [AB]
        * phi : polar angle = colatitude 
        * teta : azimuthal angle in the xy-plane from the x-axis = longitude
        * radius : wanted radius of the circle     
        */

        {
            Vector3 AB = new Vector3(B.x - A.x, B.y - A.y, B.z - A.z);
            Vector3 tempPoint = new Vector3();
            
            // if A and B are the same point, no point can be found in between them
            if (AB.x == 0 && AB.y == 0 && AB.z == 0)
                throw new Exception("NormalToLines : A and B are the same point, invalid argument, line AB doesn't exist");

            if (AB.x != 0)
            {
                tempPoint.x = (float)((-((2 * C.y + radius * Math.Sin(phi) * Math.Sin(teta)) * (AB.y) + (2 * C.z + radius * Math.Cos(phi)) * AB.z)
                      / AB.x) - C.x);
                tempPoint.y = (float)(C.y + radius * Math.Sin(phi) * Math.Sin(teta));
                tempPoint.z = (float)(C.z + radius * Math.Cos(phi));

                List<Vector3> intersections = Line_meshIntersect.Program.line_meshIntersect(mesh, tempPoint, C);
                if (intersections.Count == 0 && phi < 6.283185) 
                {
                    if (phi < 5.7f)
                    {
                        phi += 0.523599f;
                        return NormalToLine(mesh, A, B, C, ref phi , ref teta, radius);
                    }
                        
                    else
                    {
                        if (teta < 3.14159)
                        {
                            teta += 0.523599f;
                            phi = 0;
                            return NormalToLine(mesh, A, B, C, ref phi, ref teta, radius);
                        }
  
                        else
                            throw new Exception("NormalToLines : (x != 0) No collision with the mesh, unable to find a point in the mesh");

                    }
                }

                else
                    return intersections;
                    
            }

            else
            {
                if (AB.z != 0)
                {
                    
                    tempPoint.x = (float)(C.x + radius * Math.Sin(phi) * Math.Cos(teta));
                    tempPoint.y = (float)((-((2 * C.x + radius * Math.Sin(phi) * Math.Cos(teta)) * AB.x + 
                        (2 * C.y + radius * Math.Sin(phi) * Math.Sin(teta)) * AB.z) / AB.y) - C.y);
                    tempPoint.z = (float)(C.z + radius * Math.Cos(phi));

                    List<Vector3> intersections = Line_meshIntersect.Program.line_meshIntersect(mesh, tempPoint, C);

                    if (intersections.Count == 0 && phi < 6.283185)
                    {
                        if (phi < 5.7f)
                        {
                            phi += 0.523599f;
                            return NormalToLine(mesh, A, B, C, ref phi, ref teta, radius);
                        }

                        else
                        {
                            if (teta < 3.14159)
                            {
                                teta += 0.523599f;
                                phi = 0;
                                return NormalToLine(mesh, A, B, C, ref phi, ref teta, radius);
                            }

                            else
                                throw new Exception("NormalToLines : (y != 0) No collision with the mesh, unable to find a point in the mesh");

                        }
                    }

                    else
                        return intersections;
                }

                else
                {
                    tempPoint.z = (float)((-((2 * C.x + radius * Math.Sin(phi) * Math.Cos(teta)) * (AB.x) + (2 * C.y + radius * Math.Cos(phi)) * AB.y)
                      / AB.z) - C.z);
                    tempPoint.x = (float)(C.x + radius * Math.Sin(phi) * Math.Cos(teta));
                    tempPoint.z = (float)(C.z + radius * Math.Cos(phi));

                    List<Vector3> intersections = Line_meshIntersect.Program.line_meshIntersect(mesh, tempPoint, C);

                    if (intersections.Count == 0 && phi < 6.283185)
                    {
                        if (phi < 5.7f)
                        {
                            phi += 0.523599f;
                            return NormalToLine(mesh, A, B, C, ref phi, ref teta, radius);
                        }

                        else
                        {
                            if (teta < 3.14159)
                            {
                                teta += 0.523599f;
                                phi = 0;
                                return NormalToLine(mesh, A, B, C, ref phi, ref teta, radius);
                            }

                            else
                                throw new Exception("NormalToLines : (z != 0) No collision with the mesh, unable to find a point in the mesh");

                        }
                    }

                    else
                        return intersections;
                }
            } 
        }
         
        

        private Point BinarySearchExtMesh(Mesh mesh, Vector3 root, Vector3 extremum, int maxNbTries, int Nbtries, int maxTry, string name)
        /*
        * Find a link between the center of gravity of the mesh and a point in between the root and a node in the skeleton

        * mesh : mesh for which we want to make a skeleton
        * root : first point of collision with the mesh
        * extremum : second point of collision with the mesh
        * maxNbTries : number of wanted try of binary search out of the mesh
        * Nbtries : number of tries already made
        * maxTry : number of tries of binary search wanted in the mesh 
        * name : name of our point
                 
        */
        {
            List<Vector3> intersections = Line_meshIntersect.Program.line_meshIntersect(mesh, root, extremum);
            
            // Takes the middle of the closest position to the root and the next collision
            float middleX = (root.x + extremum.x) / 2; //to change
            float middleY = (root.y + extremum.y) / 2; // to change
            float middleZ = (root.z + extremum.z) / 2; // to change
            Vector3 testPoint = new Vector3(middleX, middleY, middleZ);

            float phi = 0f;
            float teta = 0f;
            
            List<Vector3> plan = NormalToLine(mesh ,root, extremum, testPoint, ref phi, ref teta, 30); // to change root and extremum

            if (plan.Count == 1)
            {
                phi += 0.523599f;
                plan = NormalToLine(mesh, root, extremum, testPoint, ref phi, ref teta, 30);
            }

            bool linked = false;

            Point possibiliy = __BinarySearchInMesh(plan[0], plan[1], maxTry, 0, ref linked, "name", mesh);

            if (linked)
            {
                return possibiliy;
            }

            else
            {
                if (Nbtries < maxNbTries)
                {
                    Point firstPossibility = BinarySearchExtMesh(mesh, intersections[0], testPoint, maxNbTries, Nbtries + 1, maxTry, name);
                    Point SecondPossibility = BinarySearchExtMesh(mesh, testPoint, intersections[1], maxNbTries, Nbtries + 1, maxTry, name);

                    float firstDistance = (float)Math.Sqrt((firstPossibility.Coordinates.x - testPoint.x)
                            * (firstPossibility.Coordinates.x - testPoint.x) + (firstPossibility.Coordinates.y - testPoint.y)
                            * (firstPossibility.Coordinates.y - testPoint.y) + (firstPossibility.Coordinates.z - testPoint.z)
                            * (firstPossibility.Coordinates.z - testPoint.z));

                    float secondDistance = (float)Math.Sqrt((SecondPossibility.Coordinates.x - testPoint.x)
                        * (SecondPossibility.Coordinates.x - testPoint.x) + (SecondPossibility.Coordinates.y - testPoint.y)
                        * (SecondPossibility.Coordinates.y - testPoint.y) + (SecondPossibility.Coordinates.z - testPoint.z)
                        * (SecondPossibility.Coordinates.z - testPoint.z));

                    if (firstDistance < secondDistance)
                        return firstPossibility;
                    else
                        return SecondPossibility;
                }
                else
                    throw new Exception("BinarySearchExtMesh : no possible point to create a link with the root");
                
            }
            
        }

        private Tree Branch(Mesh mesh, Tree currBranch, Point extremum, int MaxNbPoints, int NbPoints, int maxNbTries, int maxTry, int nbSubtrees,
                List<Point> extremes, List<bool> linked, Point root)
        /*
         * Find a link between the center of gravity of the mesh and a point in between the root and an extremum in the skeleton.

         * mesh : mesh for which we want to make a skeleton
         * currBranch : tree containing only the previous point
         * extremum : point at one extreme of the mesh
         * MaxNbPoints : maximum number of points wanted in one branch of the skeleton
         * NbPoints : current number of points between the root and the extremum
         * maxNbTries : number of wanted try of binary search out of the mesh
         * maxTry : number of tries of binary search wanted in the mesh
         * nbsubtrees : number of subtrees of the currBranch
         * extremes : list of all the extremes of the mesh
         * linked : list of bool containing wether the extreme at the same index is already linked to a point
         * root : root of the tree
         */
        {
            string name = NbPoints.ToString();
            
            List<Vector3> intersections = Line_meshIntersect.Program.line_meshIntersect(mesh, currBranch.Root.Coordinates, extremum.Coordinates);

            if (intersections.Count == 0)
            {
                currBranch.AddPoint(extremum);
                return currBranch;
            }
            
            else
            {

                if (NbPoints < MaxNbPoints)
                {
                    Point intermediatePoint = BinarySearchExtMesh(mesh, currBranch.Root.Coordinates, extremum.Coordinates, maxNbTries, 0, maxTry, name);
                    currBranch.AddPoint(intermediatePoint);
                    intermediatePoint.TreePos = NbPoints;
                    
                    //Check wether another extremum can ce linked to this intermediate point
                    for (int i = 0; i < extremes.Count; i++)
                    {
                        if (!linked[i])
                        {
                            if (Line_meshIntersect.Program.line_meshIntersect(mesh, root.Coordinates, extremes[i].Coordinates).Count != 0
                                && Line_meshIntersect.Program.line_meshIntersect(mesh, intermediatePoint.Coordinates, extremes[i].Coordinates).Count == 0)
                            {
                                    currBranch.Subtrees[nbSubtrees - 1].AddPoint(extremes[i]);
                            }
                        }
                    }
                    
                    return Branch(mesh, currBranch.Subtrees[nbSubtrees - 1], extremum, MaxNbPoints, NbPoints + 1, maxNbTries, maxTry, nbSubtrees,
                        extremes, linked, root);
                }
                else
                    throw new Exception("Branch : Unable to create a branch between the root and the extremum");
            }
             
        }

        public Tree PointGeneration(Mesh mesh, Point root, List<Point> extremes)
		/*
        * Creates a skeleton using the already placed exremes and the wanted root of the skeleton of the mesh.

        * mesh : mesh for which we want to make a skeleton
        * root : point linking all the branches of the skeleton
        * extremes : list of points containing all the extremes
        */
        {
        
            Tree skeleton = new Tree("skeleton", root);
            List<bool> linked = new List<bool>();

            for (int i = 0; i < extremes.Count; i++)
            {
                linked.Add(false);
            }

            for (int i = 0; i < extremes.Count; i++)
            {
                if (!linked[i])
                    Branch(mesh, skeleton, extremes[i], 20, 1, 5, 5, i, extremes, linked, root);
            }

            return skeleton;
        }
    }
}

