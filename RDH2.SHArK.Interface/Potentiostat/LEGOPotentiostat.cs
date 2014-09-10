using System;
using System.Collections.Generic;
using System.Text;

using RDH2.SHArK.Interface.Daq;

namespace RDH2.SHArK.Interface.Potentiostat
{
    /// <summary>
    /// LEGOPotStat is used to set potential and read
    /// current using the MCC data acquisition board.
    /// </summary>
    internal class LEGOPotentiostat : IPotentiostat
    {
        #region Member Variables
        private const String _name = "Custom SHArK Potentiostat Interface";
        private const Double _darkThreshold = 2.0;
        private MCCDaq _daq = null;
        #endregion


        #region Constructor 
        /// <summary>
        /// Default constructor for the LEGOPotentiostat object.
        /// </summary>
        public LEGOPotentiostat()
        {
            //Create the MCCDaq object
            this._daq = new MCCDaq();
        }
        #endregion


        #region IPotentiostat Methods
        /// <summary>
        /// Name defines the name of the Potentiostat if one was
        /// sucessfully created.
        /// </summary>
        public String Name
        {
            get { return LEGOPotentiostat._name; }
        }


        /// <summary>
        /// SetPotential is used to set the voltage on the electrode.
        /// </summary>
        /// <param name="potential">The value of the voltage to set</param>
        public void SetPotential(Double potential)
        {
            //Set the potential on the DAQ
            this._daq.SetPotential(potential);
        }


        /// <summary>
        /// GetCurrent is used to retrieve the current that is
        /// detected.
        /// </summary>
        /// <param name="numSamples">The number of samples to read and average</param>
        /// <returns>Double Current value that is detected</returns>
        public Double GetCurrent(Int32 numSamples)
        {
            //Get the Current value
            return this._daq.GetCurrent(numSamples);
        }


        /// <summary>
        /// MaximumCurrent returns the highest amount of current 
        /// that can be detected by this Potentiostat.
        /// </summary>
        public Double MaximumCurrent
        {
            get { return 10.0; }
        }


        /// <summary>
        /// TargetDarkCurrent returns the highest amount of dark current 
        /// that should be detected by this Potentiostat prior to performing
        /// a Spectrum scan.
        /// </summary>
        public Double TargetDarkCurrent
        {
            get { return LEGOPotentiostat._darkThreshold; }
        }


        /// <summary>
        /// IsCurrentSettled determines if the electrode charging
        /// current has settled yet based on the dark current and 
        /// the Maximum Current available.
        /// </summary>
        public Boolean IsCurrentSettled
        {
            get
            {
                //Declare a variable to return
                Boolean rtn = false;

                //If the Dark Current is less than the set value, 
                //change the return
                if (Math.Abs(this._daq.GetCurrent(1)) < this.TargetDarkCurrent)
                    rtn = true;

                //Return the result
                return rtn;
            }
        }
        #endregion


        #region IDisposable methods
        /// <summary>
        /// Dispose is called when the program is shutting 
        /// down so that hardware can be cleaned up.
        /// </summary>
        public void Dispose()
        {
            //Clean up the other objects
            if (this._daq != null)
                this._daq.Dispose();
        }
        #endregion
    }
}
