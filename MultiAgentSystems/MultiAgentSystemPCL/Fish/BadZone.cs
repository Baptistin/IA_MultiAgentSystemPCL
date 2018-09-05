namespace MultiAgentSystemPCL
{
    /// <summary>
    /// </summary>
    /// <seealso cref="MultiAgentSystemPCL.ObjectInWorld" />
    public class BadZone : ObjectInWorld
    {
        /// <summary>
        ///     The radius
        /// </summary>
        protected double radius;

        /// <summary>
        ///     The time to live
        /// </summary>
        protected int timeToLive = 100;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BadZone" /> class.
        /// </summary>
        /// <param name="_posX">The _pos x.</param>
        /// <param name="_posY">The _pos y.</param>
        /// <param name="_radius">The _radius.</param>
        public BadZone(double _posX, double _posY, double _radius)
        {
            PosX = _posX;
            PosY = _posY;
            radius = _radius;
        }

        /// <summary>
        ///     Gets the radius.
        /// </summary>
        /// <value>
        ///     The radius.
        /// </value>
        public double Radius => radius;

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        public void Update()
        {
            timeToLive--;
        }

        /// <summary>
        ///     Deads this instance.
        /// </summary>
        /// <returns></returns>
        public bool Dead()
        {
            return timeToLive <= 0;
        }
    }
}