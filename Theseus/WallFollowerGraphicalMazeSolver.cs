﻿using System;
using System.Drawing;

namespace Theseus
{
    /// <summary>
    /// Graphical maze solver implementing a left-handed "wall follower" maze solution algorithm.
    /// This solver is capable of generating a single solution for any "simply connected" maze where all walls are
    /// connected together or to the outer walls.
    /// The solution is not guaranteed to be optimal.
    /// </summary>
    public class WallFollowerGraphicalMazeSolver : IGraphicalMazeSolver
    {
        public static WallFollowerGraphicalMazeSolver Create()
        {
            return new WallFollowerGraphicalMazeSolver();
        }

        protected WallFollowerGraphicalMazeSolver()
        {
        }

        Image IGraphicalMazeSolver.GenerateSolution(GraphicalMaze maze)
        {
            if (maze == null) throw new ArgumentNullException(nameof(maze));

            // Find start pixel
            var startPixel = FindStartPixel(maze);
            if (startPixel == null)
            {
                return maze.MazeImage; // No start pixel found
            }

            // Move up from start pixel until wall reached
            var currPixel = startPixel;
            var nextPixel = maze.GetPixelTop(currPixel);
            while (!nextPixel.IsColor(maze.WallColor))
            {
                currPixel = nextPixel;
                nextPixel = maze.GetPixelTop(currPixel);

                if (nextPixel.IsColor(maze.FinishColor))
                {
                    return maze.MazeImage;
                }

                maze.SetPixel(currPixel, maze.SolutionColor);
            }

            var travelDirection = Direction.Right;
            var wallDirection = Direction.Top;

            while (!currPixel.IsColor(maze.FinishColor))
            {
                maze.SetPixel(currPixel, maze.SolutionColor);
                var nextStep = FollowWall(maze, currPixel, travelDirection, wallDirection);
                currPixel = nextStep.Item1;
                travelDirection = nextStep.Item2;
                wallDirection = nextStep.Item3;

                if (currPixel.IsColor(maze.FinishColor) && currPixel.X == startPixel.X && currPixel.Y == startPixel.Y)
                {
                    return maze.MazeImage; // No solution found
                }
            }

            return maze.MazeImage;
        }

        private static GraphicalMazePixel FindStartPixel(GraphicalMaze maze)
        {
            var startingY = 0;
            var endingY = maze.Height;
            var startingX = 0;
            var endingX = maze.Width;

            // Spiral iterate, since the start point is most likely on the outside of the maze
            while (startingY < endingY && startingX < endingX)
            {
                // Iterate over the first row from the remaining rows
                for (var x = startingX; x < endingX; x++)
                {
                    if (maze.GetPixel(x, startingY).IsColor(maze.StartColor))
                    {
                        return maze.GetPixel(x, startingY);
                    }
                }
                startingY++;

                // Iterate over the last column from the remaining columns
                for (var y = startingY; y < endingY; y++)
                {
                    if (maze.GetPixel(endingX - 1, y).IsColor(maze.StartColor))
                    {
                        return maze.GetPixel(endingX - 1, y);
                    }
                }
                endingX--;

                // Iterate over the last row from the remaining rows
                for (var x = startingX; x < endingX; x++)
                {
                    if (maze.GetPixel(x, endingY - 1).IsColor(maze.StartColor))
                    {
                        return maze.GetPixel(x, endingY - 1);
                    }
                }
                endingY--;

                // Iterate over the first column from the remaining columns
                for (var y = startingY; y < endingY; y++)
                {
                    if (maze.GetPixel(startingX, y).IsColor(maze.StartColor))
                    {
                        return maze.GetPixel(startingX, y);
                    }
                }
                startingX++;
            }

            return null;
        }

        private static Tuple<GraphicalMazePixel, Direction, Direction> FollowWall(GraphicalMaze maze,
            Point currentPoint, Direction travelDirection, Direction wallDirection)
        {
            // Left-hand rule conditions only
            while (maze.GetPixel(currentPoint, travelDirection).IsColor(maze.WallColor))
            {
                // Turn at interior corners
                switch (wallDirection)
                {
                    case Direction.Top:
                        travelDirection = Direction.Bottom;
                        wallDirection = Direction.Right;
                        break;
                    case Direction.Right:
                        travelDirection = Direction.Left;
                        wallDirection = Direction.Bottom;
                        break;
                    case Direction.Bottom:
                        travelDirection = Direction.Top;
                        wallDirection = Direction.Left;
                        break;
                    case Direction.Left:
                        travelDirection = Direction.Right;
                        wallDirection = Direction.Top;
                        break;
                    case Direction.TopRight:
                        break;
                    case Direction.BottomRight:
                        break;
                    case Direction.BottomLeft:
                        break;
                    case Direction.TopLeft:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(wallDirection), wallDirection, null);
                }
            }

            var nextPixel = maze.GetPixel(currentPoint, travelDirection);
            // Wrap-around if moving away from a wall
            switch (travelDirection)
            {
                case Direction.Top:
                    if (!maze.GetPixel(nextPixel, Direction.Left).IsColor(maze.WallColor))
                    {
                        travelDirection = Direction.Left;
                        wallDirection = Direction.Bottom;
                    }
                    break;
                case Direction.Right:
                    if (!maze.GetPixel(nextPixel, Direction.Top).IsColor(maze.WallColor))
                    {
                        travelDirection = Direction.Top;
                        wallDirection = Direction.Left;
                    }
                    break;
                case Direction.Bottom:
                    if (!maze.GetPixel(nextPixel, Direction.Right).IsColor(maze.WallColor))
                    {
                        travelDirection = Direction.Right;
                        wallDirection = Direction.Top;
                    }
                    break;
                case Direction.Left:
                    if (!maze.GetPixel(nextPixel, Direction.Bottom).IsColor(maze.WallColor))
                    {
                        travelDirection = Direction.Bottom;
                        wallDirection = Direction.Right;
                    }
                    break;
                case Direction.TopRight:
                    break;
                case Direction.BottomRight:
                    break;
                case Direction.BottomLeft:
                    break;
                case Direction.TopLeft:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(travelDirection), travelDirection, null);
            }

            return new Tuple<GraphicalMazePixel, Direction, Direction>(nextPixel, travelDirection, wallDirection);
        }
    }
}
