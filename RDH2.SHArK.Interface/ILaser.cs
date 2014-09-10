using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.SHArK.Interface
{
    /// <summary>
    /// ILaser exposes the methods required to operate
    /// the Laser in a SHArK kit.
    /// </summary>
    public interface ILaser : IDisposable
    {
        /// <summary>
        /// Name defines the name of the Object that is
        /// operating the Laser.
        /// </summary>
        String Name { get; }

        
        /// <summary>
        /// SetLaserState turns the laser on or off based
        /// on the Boolean input
        /// </summary>
        /// <param name="state">The value to set on the laser</param>
        void SetLaserState(Boolean state);
    }
}
