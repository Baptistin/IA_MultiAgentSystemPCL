using System;
using System.Collections.Generic;

namespace MultiAgentSystemPCL
{
    /// <summary>
    /// </summary>
    /// <param name="_lapin">The _lapin.</param>
    /// <param name="_renard">The _renard.</param>
    /// <param name="_obstacles">The _obstacles.</param>
    public delegate void OceanUpdated(List<LapinAgent> _lapin, List<RenardAgent> _renard, List<BadZone> _obstacles);

    /// <summary>
    /// </summary>
    public class Ocean
    {
        /// <summary>
        ///     The vision renard global
        /// </summary>
        public static int visionRenardGlobal = 20;

        /// <summary>
        ///     The renard time to eat dead
        /// </summary>
        public static int renardTimeToEatDead = 1000;

        /// <summary>
        ///     The time to dead alter
        /// </summary>
        public static int TimeToDeadAlter = 5000;

        /// <summary>
        ///     The lapin list
        /// </summary>
        public readonly List<LapinAgent> lapinList;

        //LapinAgent[] lapinList = null;
        //RenardAgent[] renardList = null;
        /// <summary>
        ///     The obstacles
        /// </summary>
        public readonly List<BadZone> obstacles;

        /// <summary>
        ///     The random generator
        /// </summary>
        public readonly Random randomGenerator;

        /// <summary>
        ///     The renard list
        /// </summary>
        public readonly List<RenardAgent> renardList;

        /// <summary>
        ///     The ma x_ height
        /// </summary>
        public double MAX_HEIGHT;

        /// <summary>
        ///     The ma x_ width
        /// </summary>
        public double MAX_WIDTH;

        /// <summary>
        ///     The time to appear
        /// </summary>
        public int timeToAppear = 100;

        /// <summary>
        ///     The timetolive
        /// </summary>
        public int timetolive;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ocean" /> class.
        /// </summary>
        /// <param name="_lapinNb">The _lapin nb.</param>
        /// <param name="_renardNb">The _renard nb.</param>
        /// <param name="_width">The _width.</param>
        /// <param name="_height">The _height.</param>
        public Ocean(int _lapinNb, int _renardNb, double _width, double _height)
        {
            MAX_WIDTH = _width;
            MAX_HEIGHT = _height;
            randomGenerator = new Random();

            lapinList = new List<LapinAgent>();
            renardList = new List<RenardAgent>();
            obstacles = new List<BadZone>();
            for (var i = 0; i < _lapinNb; i++)
                lapinList.Add(new LapinAgent(randomGenerator.NextDouble() * MAX_WIDTH,
                    randomGenerator.NextDouble() * MAX_HEIGHT, randomGenerator.NextDouble() * 2 * Math.PI));
            for (var i = 0; i < _renardNb; i++)
                renardList.Add(new RenardAgent(randomGenerator.NextDouble() * MAX_WIDTH,
                    randomGenerator.NextDouble() * MAX_HEIGHT, randomGenerator.NextDouble() * 2 * Math.PI));
        }

        /// <summary>
        ///     Occurs when [ocean updated event].
        /// </summary>
        public event OceanUpdated oceanUpdatedEvent;

        /// <summary>
        ///     Sets the vision renard.
        /// </summary>
        /// <param name="mavar">The mavar.</param>
        public static void setVisionRenard(int mavar)
        {
            visionRenardGlobal = mavar;
        }

        /// <summary>
        ///     Gets the vision renard.
        /// </summary>
        /// <returns></returns>
        public static int getVisionRenard()
        {
            return visionRenardGlobal;
        }

        /// <summary>
        ///     Sets the time life.
        /// </summary>
        /// <param name="mavar">The mavar.</param>
        public static void setTimeLife(int mavar)
        {
            TimeToDeadAlter = mavar;
        }

        /// <summary>
        ///     Gets the time life.
        /// </summary>
        /// <returns></returns>
        public static int getTimeLife()
        {
            return TimeToDeadAlter;
        }

        /// <summary>
        ///     Sets the time eat.
        /// </summary>
        /// <param name="mavar">The mavar.</param>
        public static void setTimeEat(int mavar)
        {
            renardTimeToEatDead = mavar;
        }

        /// <summary>
        ///     Gets the time eat.
        /// </summary>
        /// <returns></returns>
        public static int getTimeEat()
        {
            return renardTimeToEatDead;
        }

        /// <summary>
        ///     Updates the environnement.
        /// </summary>
        public void UpdateEnvironnement()
        {
            UpdateObstacles();
            UpdateFish();
            UpdateRenard();
            if (oceanUpdatedEvent != null) oceanUpdatedEvent(lapinList, renardList, obstacles);
            timetolive++;

            double test = timetolive;

            if (Math.Abs(test / timeToAppear - Math.Round(test / timeToAppear)) <= 0 && lapinList.Count > 1)
                lapinList.Add(new LapinAgent(randomGenerator.NextDouble() * MAX_WIDTH,
                    randomGenerator.NextDouble() * MAX_HEIGHT, randomGenerator.NextDouble() * 2 * Math.PI));

            if (Math.Abs(test / (timeToAppear * 10) - Math.Round(test / (timeToAppear * 10))) <= 0 &&
                renardList.Count > 1)
                renardList.Add(new RenardAgent(randomGenerator.NextDouble() * MAX_WIDTH,
                    randomGenerator.NextDouble() * MAX_HEIGHT, randomGenerator.NextDouble() * 2 * Math.PI));
        }

        /// <summary>
        ///     Updates the obstacles.
        /// </summary>
        private void UpdateObstacles()
        {
            foreach (var obstacle in obstacles) obstacle.Update();
            obstacles.RemoveAll(x => x.Dead());
        }

        /// <summary>
        ///     Updates the fish.
        /// </summary>
        private void UpdateFish()
        {
            foreach (var fish in lapinList) fish.Update(lapinList, obstacles, MAX_WIDTH, MAX_HEIGHT);
            lapinList.RemoveAll(x => x.DeadbyLife());
        }

        /// <summary>
        ///     Updates the renard.
        /// </summary>
        private void UpdateRenard()
        {
            foreach (var renard in renardList) renard.Update(lapinList, obstacles, MAX_WIDTH, MAX_HEIGHT);
            renardList.RemoveAll(x => x.Dead());
            renardList.RemoveAll(x => x.DeadbyLife());
        }

        /// <summary>
        ///     Adds the obstacle.
        /// </summary>
        /// <param name="_posX">The _pos x.</param>
        /// <param name="_posY">The _pos y.</param>
        /// <param name="_radius">The _radius.</param>
        public void AddObstacle(double _posX, double _posY, double _radius)
        {
            obstacles.Add(new BadZone(_posX, _posY, _radius));
        }
    }
}