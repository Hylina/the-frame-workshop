using System;
using System.Collections;
using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace Theframeworkshopskeleton
{
    public class Point
    {

        
        private Vector3 coordinates;
        private Vector3 rotation;
        private readonly string name;
        private bool fixedPoint;
        private float weight;
        private int treePos;


        public Point(string name, float x, float y, float z)
        {
            this.name = name;
            fixedPoint = false;

            coordinates.x = x;
            coordinates.y = y;
            coordinates.z = z;

            rotation.x = 0;
            rotation.y = 0;
            rotation.z = 0;

            treePos = -1;
            weight = 1;
        }




        public string Name
        {
            get { return name; }
        }

        public Vector3 Coordinates
        {
            get { return coordinates; }
            set { coordinates = value; }
        }
    
        

        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        
        public bool FixedPoint
        {
            get { return fixedPoint; }
            set { fixedPoint = value; }
        }

        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public int TreePos
        {
            get { return treePos; }
            set { treePos = value; }
        }

        public void AddValuePos(float x, float y, float z)
        {
            coordinates.x += x;
            coordinates.y += y;
            coordinates.z += z;
        }

        public void AddValueRotPos(float rotX, float rotY, float rotZ)
        {
            rotation.x += rotX;
            rotation.y += rotY;
            rotation.z += rotZ;
        }

    }
}
