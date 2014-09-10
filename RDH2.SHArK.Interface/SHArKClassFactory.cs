using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.SHArK.Interface
{
    /// <summary>
    /// ClassFactory is used to retrieve an interface
    /// supported by the class library.
    /// </summary>
    public class SHArKClassFactory
    {
        #region IMirror Create Method
        /// <summary>
        /// CreateIMirror returns an IMirror interface based
        /// on the current configuration.
        /// </summary>
        /// <returns>IMirror object if one is configured; NULL otherwise</returns>
        public static IMirror CreateIMirror()
        {
            //Declare a variable to return
            IMirror rtn = null;

            //Try to get an RDH2Mirror object
            //try
            //{
            //    rtn = new Mirror.RDH2Mirror();
            //}
            //catch { }

            //Try to get a LEGOMirror object if the RDH2Mirror
            //couldn't be created
            if (rtn == null)
            {
                try
                {
                    rtn = new Mirror.LEGOMirror();
                }
                catch { }
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region IPotentiostat Create Method
        /// <summary>
        /// CreateIPotentiostat returns an IPotentiostat interface
        /// based on the current configuration.
        /// </summary>
        /// <returns>IPotentiostat object if one is configured; NULL otherwise</returns>
        public static IPotentiostat CreateIPotentiostat()
        {
            //Declare a variable to return
            IPotentiostat rtn = null;

            //Try to get an RDH2Potentiostat object
            //try
            //{
            //    rtn = Potentiostat.RDH2Potentiostat();
            //}
            //catch { }

            //Try to get a LEGOPotentiostat object if the RDH2Potentiostat
            //could not be created
            if (rtn == null)
            {
                try
                {
                    rtn = new Potentiostat.LEGOPotentiostat();
                }
                catch { }
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region ILaser Create Method
        /// <summary>
        /// CreateILaser returns an ILaser interface
        /// based on the current configuration.
        /// </summary>
        /// <returns>ILaser object if one is configured; NULL otherwise</returns>
        public static ILaser CreateILaser()
        {
            //Declare a variable to return
            ILaser rtn = null;

            //Try to get an RDH2Laser object
            try
            {
                //rtn = new Laser.RDH2Laser();
            }
            catch { }

            //Try to get a LEGOLaser object if the RDH2Laser
            //could not be created
            if (rtn == null)
            {
                try
                {
                    rtn = new Laser.LEGOLaser();
                }
                catch { }
            }

            //Return the result
            return rtn;
        }
        #endregion
    }
}
