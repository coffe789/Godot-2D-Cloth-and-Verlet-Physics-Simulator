using Godot;
using Godot.Collections;
using Cloth;
using System.Collections.Generic;
using System;

namespace Cloth
{
    // Dynamic 2d cloth simulation using verlet integration based off this python project: https://code.google.com/archive/p/pythoncloth/
    // TODO 1. COLLISIONS
    public class Cloth : Polygon2D
    {

        private Particle[,] grid;
        private List<Vector2> drawPoints;
        private List<Vector2> drawUvs;

        [Export]
        public int rows = 10;
        [Export]
        public int columns = 10;

        private Timer timer;

        [Export]
        public float refreshRate = 0.01f;
        [Export]
        public float timeStep = 3f;
        [Export]
        public float gravity = 0.005f;
        [Export]
        public bool debugFollowMouse = true;
        [Export]
        public float scaling = 15;
        [Export]
        public bool debugDrawGrid = true;
        [Export]
        public float damping = 1;


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            drawPoints = new List<Vector2>();
            drawUvs = new List<Vector2>();
            CreateGrid();

            timer = new Timer();
            timer.Connect("timeout", this, "_OnTimeout");
            timer.WaitTime = refreshRate;
            AddChild(timer);
            timer.Start();

            
        }

        public void _OnTimeout()
        {
            //CreateAlphaShape();
            Verlet();
            SatisfyConstraints();
            SatisfyConstraints();
            SatisfyConstraints();
        }

        public override void _Process(float delta)
        {
            Update();
            if (debugFollowMouse)
                FollowMouse();

        }

        public void Follow(Vector2 points, float distanceScale = 3, int numRows = 1)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < numRows; j++)
                {
                    grid[i,j].currentPos = new Vector2((points.x+distanceScale*(i-1))/scaling, (points.y+distanceScale*(j-1))/scaling);
                }
            }

        }

        private void FollowMouse()
        {
            Vector2 mousePos = GetGlobalMousePosition();
            //GD.Print(mousePos);
            Follow(mousePos, rows+4);
        }
        

        public override void _Draw()
        {
            SortGridForDrawing();
            //CreateAlphaShape();
            CreateMesh();   
            
        }

        private void CreateMesh()
        {
  
            IList<Point> points = new List<Point>();
            foreach (var dot in drawPoints)
            {
                points.Add(new Point(dot.x, dot.y));
            }

            IList<Point> hull = ConvexHull.MakeHull(points);
            List<Vector2> polygon = new List<Vector2>();

            List<Vector2> uvs = new List<Vector2>();

            foreach (var point in hull)
            {
                // ToLocal helps interface with other Godot 2D nodes
                polygon.Add(ToLocal(new Vector2((float)point.x,(float)point.y)));
                uvs.Add(new Vector2((float)point.x / 10, (float)point.y / 10));
            }
            Polygon = (polygon.ToArray());
            Uv = uvs.ToArray();


            drawPoints.Clear();
            drawUvs.Clear();


            
           
        }

        // private void AddEdge(int[] mesh, List<Vector2> edges, List<Vector2> edge_points, Vector2 coords)
        // {
        //     // if (edges.Contains(coords))
        //     // {
        //     //     return;
        //     // }

        //     edges.Add(coords);
        //     edge_points.Add(drawPoints[mesh[(int)coords.x]]);
        //     edge_points.Add(drawPoints[mesh[(int)coords.y]]);

        // }

        // private void CreateAlphaShape()
        // {
        //     float alpha = 1f;

        //     int[] mesh = Geometry.TriangulateDelaunay2d(drawPoints.ToArray());
            
        //     List<Vector2> edges = new List<Vector2>();
        //     List<Vector2> edge_points = new List<Vector2>();

        //     for (int i = 0; i < mesh.Length; i+=3)
        //     {
        //         //if (i+2 <= drawPoints.Count-1)
        //         //{
        //             //GD.Print("hhhhh");
        //             var pa = drawPoints[mesh[i]] / scaling;
        //             var pb = drawPoints[mesh[i+1]] / scaling;
        //             var pc = drawPoints[mesh[i+2]] / scaling;

        //             // Lengths of sides of triangle
        //             var a = Math.Sqrt(Math.Pow(pa.x-pb.x, 2) + Math.Pow(pa.y-pb.y, 2));
        //             var b = Math.Sqrt(Math.Pow(pb.x-pc.x, 2) + Math.Pow(pb.y-pc.y, 2));
        //             var c = Math.Sqrt(Math.Pow(pc.x-pa.x, 2) + Math.Pow(pc.y-pa.y, 2));

        //             // Semiperimeter of triangle
        //             var s = (a+b+c)/2.0f;
        //             // Area of triangle
        //             var area = Math.Sqrt(s*(s-a)*(s-b)*(s-c));
        //             var circum_r = a*b*c/(4.0*area);

        //             if (circum_r < 1.0f / alpha)
        //             {
        //                 GD.Print("egvdrv");
        //                 AddEdge(mesh, edges, edge_points, new Vector2(i, i+1));
        //                 AddEdge(mesh, edges, edge_points, new Vector2(i+1, i+2));
        //                 AddEdge(mesh, edges, edge_points, new Vector2(i+2, i));
        //             }
        //         //}
        //     }

        //     foreach (var edge in edge_points)
        //     {
        //         DrawCircle(edge, 2, new Color(1,0,0));
        //     }

            

        // }


        private void SortGridForDrawing()
        {
            Color lineColor = new Color(1, 0, 0);
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Particle particle = grid[i,j];
                    
                    grid[i,j].pixelPosition = new Vector2 (particle.currentPos.x*scaling, particle.currentPos.y*scaling);
                    List<Vector2> neighbors = particle.neighbors;

                    foreach (var neighbor in particle.neighbors)
                    {
                        Particle point = grid[(int)neighbor.x, (int)neighbor.y]; //!!! (int)
                        if (point == null)
                            return;
                        
                        Vector2 toLine = new Vector2(point.currentPos.x*scaling, point.currentPos.y*scaling);

                        
                        if (debugDrawGrid)
                            DrawLine(ToLocal(particle.pixelPosition), ToLocal(toLine), new Color(0,0,0));
                        drawPoints.Add(particle.pixelPosition);
                        drawUvs.Add(particle.pixelPosition / 10);
                        //drawUvs.Add(toLine);
                        
                    }
                    Particle prevParticle = particle;
                }
            }
            // grid[rows-1,0].stuck = true;
            // grid[0,0].stuck = true;

            
        }

        private void CreateGrid()
        {
            grid = new Particle[rows,columns];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                
                // Filling out the array
                //grid[x] = new Particle[columns];

                for (int y = 0; y < columns; y++)
                {
                    //grid[x,y] = new Particle();
                    Vector2 currentPos = new Vector2(x,y);
                    Particle particle = new Particle();
                    particle.currentPos = currentPos;
                    particle.previousPos = currentPos;
                    particle.forces = new Vector2(0, gravity);
                    particle.restLength = 1f;
                    particle.listPosition = new Vector2(x,y);
                    particle.stuck = false;
                    particle.pixelPosition = new Vector2(0,0);
                    particle.neighbors = new List<Vector2>();

                    grid[x,y] = particle;

                }
            }

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    List<Vector2> neighbors = FindNeighbors(grid[x,y].listPosition, grid);
                    grid[x,y].neighbors = neighbors;
                }
            }
        }

        private List<Vector2> FindNeighbors(Vector2 pointPosition, Particle[,] grid)
        {
            int columnLimit = grid.GetLength(1);
            int rowLimit = grid.GetLength(0);

            List<Vector2> possNeighbors = new List<Vector2>();
            possNeighbors.Add(new Vector2(pointPosition.x-1, pointPosition.y));
            possNeighbors.Add(new Vector2(pointPosition.x, pointPosition.y-1));
            possNeighbors.Add(new Vector2(pointPosition.x+1, pointPosition.y));
            possNeighbors.Add(new Vector2(pointPosition.x, pointPosition.y+1));


            List<Vector2> neig = new List<Vector2>();

            // Loop and find the qualified potential neighbors
            foreach (var coord in possNeighbors)
            {
                if (coord.x < 0 | (coord.x > rowLimit-1))
                {
                    
                } else if (coord.y < 0 | (coord.y > columnLimit - 1))
                {

                } 
                else
                {
                    neig.Add(coord);
                }
            }

            List<Vector2> finalNeighbors = new List<Vector2>();

            foreach (var point in neig)
            {
                finalNeighbors.Add(new Vector2(point.x, point.y));
            }

            return finalNeighbors;
        }


        private void Verlet()
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Particle particle = grid[x,y];
                    if (particle.stuck)
                        particle.currentPos = particle.previousPos;
                    else
                    {
                        Vector2 c_p = particle.currentPos;
                        Vector2 temp = c_p;
                        Vector2 p_p = particle.previousPos;
                        Vector2 f = particle.forces;

                        var fmultbytime = new Vector2(f.x*timeStep*timeStep, f.y*timeStep*timeStep);
                        var tempminusp_p = new Vector2(c_p.x-p_p.x, c_p.y-p_p.y);
                        var together = new Vector2(fmultbytime.x+tempminusp_p.x, fmultbytime.y+tempminusp_p.y);

                        // Simple multiplier for damping
                        together.x *= damping;
                        together.y *= damping;

                        c_p = new Vector2(c_p.x+together.x, c_p.y+together.y);

                        particle.currentPos = c_p;
                        particle.previousPos = temp;
                    }
                }
            }
        }

        private void SatisfyConstraints()
        {
            float m = 0.5f;
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Particle particle = grid[x,y];
                    if (particle.stuck)
                        particle.currentPos = particle.previousPos;
                    else
                    {
                        foreach(var constraint in particle.neighbors)
                        {
                            var c2 = grid[(int)constraint.x,(int)constraint.y].currentPos; //!! (int)
                            var c1 = particle.currentPos;
                            Vector2 delta = new Vector2(c2.x-c1.x, c2.y-c1.y);
                            float deltaLength = (float)Math.Sqrt(Math.Pow((c2.x-c1.x),2) + Math.Pow((c2.y-c1.y),2));
                            float diff = (deltaLength - 1.0f) / deltaLength;

                            var dtemp = new Vector2(delta.x*m*diff, delta.y*m*diff);

                            c1.x += dtemp.x;
                            c1.y += dtemp.y;
                            c2.x -= dtemp.x;
                            c2.y -= dtemp.y;

                            particle.currentPos = c1;
                            grid[(int)constraint.x,(int)constraint.y].currentPos = c2; // !! (int)
                        }
                        
                    }
                }
            }
        }



    }

}
