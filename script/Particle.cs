using System;
using System.Collections.Generic;
using Godot;

namespace Cloth
{

    public class Particle
    {
        
        public Vector2 currentPos;
        public Vector2 previousPos;
        public Vector2 forces;
        public float restLength;
        public Vector2 listPosition;
        public bool stuck;
        public Vector2 pixelPosition;
        public List<Vector2> neighbors;
    }
}