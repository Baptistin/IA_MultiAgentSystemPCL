using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAgentSystemPCL
{
    /// <summary>
    /// </summary>
    /// <seealso cref="MultiAgentSystemPCL.ObjectInWorld" />
    public class LapinAgent : ObjectInWorld
    {
        /// <summary>
        ///     The step
        /// </summary>
        public const double STEP = 2;

        /// <summary>
        ///     The distanc e_ minimum
        /// </summary>
        public const double DISTANCE_MIN = 5;

        /// <summary>
        ///     The squar e_ distanc e_ minimum
        /// </summary>
        public const double SQUARE_DISTANCE_MIN = 25;

        //protected const double DISTANCE_MAX = 40;
        /// <summary>
        ///     The squar e_ distanc e_ maximum
        /// </summary>
        public const double SQUARE_DISTANCE_MAX = 1600;

        /// <summary>
        ///     The speed x
        /// </summary>
        public double speedX;

        /// <summary>
        ///     The speed y
        /// </summary>
        public double speedY;

        /// <summary>
        ///     The time to live alt
        /// </summary>
        public int timeToLiveAlt = Ocean.TimeToDeadAlter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LapinAgent" /> class.
        /// </summary>
        /// <param name="_x">The _x.</param>
        /// <param name="_y">The _y.</param>
        /// <param name="_dir">The _dir.</param>
        public LapinAgent(double _x, double _y, double _dir)
        {
            PosX = _x;
            PosY = _y;
            speedX = Math.Cos(_dir);
            speedY = Math.Sin(_dir);
        }

        /// <summary>
        ///     Gets the speed x.
        /// </summary>
        /// <value>
        ///     The speed x.
        /// </value>
        public double SpeedX => speedX;

        /// <summary>
        ///     Gets the speed y.
        /// </summary>
        /// <value>
        ///     The speed y.
        /// </value>
        public double SpeedY => speedY;

        /// <summary>
        ///     Updates the position.
        /// </summary>
        internal void UpdatePosition()
        {
            PosX += STEP * SpeedX;
            PosY += STEP * SpeedY;
        }

        /// <summary>
        ///     Deadbies the life.
        /// </summary>
        /// <returns></returns>
        public bool DeadbyLife()
        {
            return timeToLiveAlt <= 0;
        }

        /// <summary>
        ///     Nears the specified _lapin.
        /// </summary>
        /// <param name="_lapin">The _lapin.</param>
        /// <returns></returns>
        private bool Near(LapinAgent _lapin)
        {
            var squareDistance = SquareDistanceTo(_lapin);
            return squareDistance < SQUARE_DISTANCE_MAX && squareDistance > SQUARE_DISTANCE_MIN;
        }

        /// <summary>
        ///     Distances to wall.
        /// </summary>
        /// <param name="_wallXMin">The _wall x minimum.</param>
        /// <param name="_wallYMin">The _wall y minimum.</param>
        /// <param name="_wallXMax">The _wall x maximum.</param>
        /// <param name="_wallYMax">The _wall y maximum.</param>
        /// <returns></returns>
        internal double DistanceToWall(double _wallXMin, double _wallYMin, double _wallXMax, double _wallYMax)
        {
            var min = double.MaxValue;
            min = Math.Min(min, PosX - _wallXMin);
            min = Math.Min(min, PosY - _wallYMin);
            min = Math.Min(min, _wallYMax - PosY);
            min = Math.Min(min, _wallXMax - PosX);
            return min;
        }

        /// <summary>
        ///     Computes the average direction.
        /// </summary>
        /// <param name="_lapinList">The _lapin list.</param>
        internal void ComputeAverageDirection(List<LapinAgent> _lapinList)
        {
            //List<LapinAgent> lapinUsed = _lapinList.Where(x => Near(x)).ToList();
            //int nbLapin = lapinUsed.Count;
            //if (nbLapin >= 1)
            //{
            //    double speedXTotal = 0;
            //    double speedYTotal = 0;
            //    foreach (LapinAgent neighbour in lapinUsed)
            //    {
            //        speedXTotal += neighbour.SpeedX;
            //        speedYTotal += neighbour.SpeedY;
            //    }

            //    speedX = (speedXTotal / nbLapin + speedX) / 2;
            //    speedY = (speedYTotal / nbLapin + speedY) / 2;
            //    Normalize();
            //}
        }

        /// <summary>
        ///     Normalizes this instance.
        /// </summary>
        protected void Normalize()
        {
            var speedLength = Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY);
            speedX /= speedLength;
            speedY /= speedLength;
        }

        /// <summary>
        ///     Avoids the walls.
        /// </summary>
        /// <param name="_wallXMin">The _wall x minimum.</param>
        /// <param name="_wallYMin">The _wall y minimum.</param>
        /// <param name="_wallXMax">The _wall x maximum.</param>
        /// <param name="_wallYMax">The _wall y maximum.</param>
        /// <returns></returns>
        internal bool AvoidWalls(double _wallXMin, double _wallYMin, double _wallXMax, double _wallYMax)
        {
            // Stop at walls
            if (PosX < _wallXMin) PosX = _wallXMin;
            if (PosY < _wallYMin) PosY = _wallYMin;
            if (PosX > _wallXMax) PosX = _wallXMax;
            if (PosY > _wallYMax) PosY = _wallYMax;

            // Change direction
            var distance = DistanceToWall(_wallXMin, _wallYMin, _wallXMax, _wallYMax);
            if (distance < DISTANCE_MIN)
            {
                if (distance == PosX - _wallXMin)
                    speedX += 0.3;
                else if (distance == PosY - _wallYMin)
                    speedY += 0.3;
                else if (distance == _wallXMax - PosX)
                    speedX -= 0.3;
                else if (distance == _wallYMax - PosY) speedY -= 0.3;
                Normalize();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Avoids the lapin.
        /// </summary>
        /// <param name="_lapinAgent">The _lapin agent.</param>
        /// <returns></returns>
        internal bool AvoidLapin(LapinAgent _lapinAgent)
        {
            var squareDistanceToLapin = SquareDistanceTo(_lapinAgent);
            if (squareDistanceToLapin < SQUARE_DISTANCE_MIN)
            {
                var diffX = (_lapinAgent.PosX - PosX) / Math.Sqrt(squareDistanceToLapin);
                var diffY = (_lapinAgent.PosY - PosY) / Math.Sqrt(squareDistanceToLapin);

                speedX = SpeedX - diffX / 4;
                speedY = SpeedY - diffY / 4;
                Normalize();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Avoids the obstacle.
        /// </summary>
        /// <param name="_obstacles">The _obstacles.</param>
        /// <returns></returns>
        internal bool AvoidObstacle(List<BadZone> _obstacles)
        {
            var nearestObstacle = _obstacles.Where(x => SquareDistanceTo(x) < x.Radius * x.Radius).FirstOrDefault();

            if (nearestObstacle != null)
            {
                var distanceToObstacle = DistanceTo(nearestObstacle);
                var diffX = (nearestObstacle.PosX - PosX) / distanceToObstacle;
                var diffY = (nearestObstacle.PosY - PosY) / distanceToObstacle;

                speedX = SpeedX - diffX / 2;
                speedY = SpeedY - diffY / 2;
                Normalize();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Updates the specified _lapin list.
        /// </summary>
        /// <param name="_lapinList">The _lapin list.</param>
        /// <param name="_obstacles">The _obstacles.</param>
        /// <param name="_max_width">The _max_width.</param>
        /// <param name="_max_height">The _max_height.</param>
        internal void Update(List<LapinAgent> _lapinList, List<BadZone> _obstacles, double _max_width,
            double _max_height)
        {
            timeToLiveAlt--;
            if (!AvoidWalls(0, 0, _max_width, _max_height))
                if (!AvoidObstacle(_obstacles) && _lapinList.Count > 1)
                {
                    var squareDistanceMin = _lapinList.Where(x => !x.Equals(this)).Min(x => x.SquareDistanceTo(this));

                    if (
                        !AvoidLapin(
                            _lapinList.Where(x => x.SquareDistanceTo(this) == squareDistanceMin).FirstOrDefault()))
                        ComputeAverageDirection(_lapinList);
                }

            UpdatePosition();
        }
    }
}