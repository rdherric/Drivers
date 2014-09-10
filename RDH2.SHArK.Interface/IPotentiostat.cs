using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.SHArK.Interface
{
    /// <summary>
    /// IPotentiostat is used by an application to manipulate
    /// the potentiostat of a SHArK instrument.
    /// </summary>
    public interface IPotentiostat : IDisposable
    {
        /// <summary>
        /// Name defines the name of the Mirror interface
        /// to the end user.
        /// </summary>
        String Name { get; }


        /// <summary>
        /// SetPotential sets the voltage on the Working
        /// Electrode.
        /// </summary>
        /// <param name="potential">The potential to set</param>
        void SetPotential(Double potential);


        /// <summary>
        /// GetCurrent returns the value of the Current 
        /// that is at the Working Electrode.
        /// </summary>
        /// <returns>Double value of the current</returns>
        Double GetCurrent(Int32 numSamples);


        /// <summary>
        /// MaximumCurrent defines the greatest amount of current
        /// that can be detected from the Potentiostat.
        /// </summary>
        Double MaximumCurrent { get; }


        /// <summary>
        /// TargetDarkCurrent defines the amount of current that
        /// should be allowable prior to a scan being performed.
        /// </summary>
        Double TargetDarkCurrent { get; }


        /// <summary>
        /// IsCurrentSettled determines if the electrode
        /// has settled its charging current.
        /// </summary>
        Boolean IsCurrentSettled { get; }
    }
}
