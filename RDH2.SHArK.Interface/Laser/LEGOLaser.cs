using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using RDH2.SHArK.Interface.Daq;

namespace RDH2.SHArK.Interface.Laser
{
    /// <summary>
    /// LEGOLaser implements the ILaser interface for the
    /// custom SHArK electronics box.
    /// </summary>
    internal class LEGOLaser : ILaser
    {
        #region Member Variables
        private const String _name = "Custom SHArK Laser Interface";

        //MCCDaq object to do the Laser output
        private MCCDaq _daq = null;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the LEGOLaser object.
        /// </summary>
        public LEGOLaser()
        {
            //Create the MCCDaq object
            this._daq = new MCCDaq();
        }
        #endregion


        #region ILaser Methods
        /// <summary>
        /// Name returns the name of the type of Object
        /// that is created if successful.
        /// </summary>
        public String Name
        {
            get { return LEGOLaser._name; }
        }


        /// <summary>
        /// SetLaserState sets the value of the input on the
        /// laser DIO.
        /// </summary>
        /// <param name="state">The value to set on the Laser</param>
        public void SetLaserState(Boolean state)
        {
            this._daq.SetLaserState(state);
        }
        #endregion


        #region IDisposable Methods
        /// <summary>
        /// Dispose is used to clean up any resources that
        /// the object makes to operate.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
    }
}
