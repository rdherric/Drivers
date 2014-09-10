using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.SHArK.Interface
{
    /// <summary>
    /// IMirror is used by an application to move a Mirror
    /// on a SHArK instrument.
    /// </summary>
    public interface IMirror : IDisposable
    {
        /// <summary>
        /// Name defines the name of the Mirror interface
        /// to the end user.
        /// </summary>
        String Name { get; }


        /// <summary>
        /// Move moves a mirror to a particular position.
        /// </summary>
        /// <param name="axis">The Axis that should be moved -- X or Y</param>
        /// <param name="position">The position to which it should be moved</param>
        void Move(Enums.MirrorAxis axis, Int32 position);


        /// <summary>
        /// SecondsPerStep defines the amount of time that it
        /// takes to move a mirror by one step.
        /// </summary>
        Double SecondsPerStep { get; }
    }
}
