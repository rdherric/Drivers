using System;
using System.Collections.Generic;
using System.Text;

using MccDaq;

namespace RDH2.SHArK.Interface.Daq
{
    /// <summary>
    /// MCCDaq encapsulates all of the operations of the 
    /// SHArK data interface box, which uses a Measurement
    /// Computing DAQ card for communication.
    /// </summary>
    internal class MCCDaq : IDisposable
    {
        #region Member Variables
        private Boolean _dioPortConfigured = false;

        //Values set in the SHArK boxes for data communication
        private const DigitalPortType _laserPortType = DigitalPortType.FirstPortA;
        private const Int32 _laserBit = 0;
        private const Int32 _potentialPort = 0;
        private const Int32 _currentPort = 0;

        //Static variable so that only one actual 
        //MCC data acquisistion object is ever used
        private static MccBoard _board = null;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the MCCDaq object.
        /// </summary>
        public MCCDaq()
        {
            //Create the static member variable
            this.CreateMccBoard();

            //Set up the DIO port
            this.ConfigDIOPort();
        }
        #endregion


        #region Data Acquisition Methods
        /// <summary>
        /// SetLaserState turns the Laser either on or off
        /// based on the Boolean input.
        /// </summary>
        /// <param name="bitValue">The value to set the bit</param>
        /// <returns>Boolean TRUE if successful, FALSE otherwise</returns>
        public Boolean SetLaserState(Boolean bitValue)
        {
            //Declare a variable to return
            Boolean rtn = true;

            //Turn the bitValue into a DigitalLogicState
            DigitalLogicState state = DigitalLogicState.High;

            if (bitValue == false)
                state = DigitalLogicState.Low;

            //Set the bit
            ErrorInfo ei = MCCDaq._board.DBitOut(MCCDaq._laserPortType, MCCDaq._laserBit, state);

            //If an error occurred, set the return value
            if (ei.Value != ErrorInfo.ErrorCode.NoErrors)
                rtn = false;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// SetPotential sets the value of the potential
        /// on the Electrode.
        /// </summary>
        /// <param name="potential">The potential to set</param>
        /// <returns>Boolean TRUE if successful, FALSE otherwise</returns>
        public Boolean SetPotential(Double potential)
        {
            //Declare a variable to return
            Boolean rtn = true;

            //Set the potential
            ErrorInfo ei = MCCDaq._board.VOut(MCCDaq._potentialPort, Range.Uni4Volts, Convert.ToSingle(potential), VOutOptions.Default);

            //If an error occurred, set the return value
            if (ei.Value != ErrorInfo.ErrorCode.NoErrors)
                rtn = false;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// GetCurrent gets the value of the current from the
        /// electrode.
        /// </summary>
        /// <param name="numSamples">The number of samples to acquire and average</param>
        /// <returns>Double Current value</returns>
        public Double GetCurrent(Int32 numSamples)
        {
            //Declare a variable to hold the sum of the data
            Double sum = 0.0;

            //Iterate through the number of samples and acquire
            //the data
            for (Int32 i = 0; i < numSamples; i++)
            {
                UInt16 rawData = 0;
                ErrorInfo ei = MCCDaq._board.AIn(MCCDaq._currentPort, Range.Bip10Volts, out rawData);

                //If the value came back successfully, convert it
                if (ei.Value == ErrorInfo.ErrorCode.NoErrors)
                {
                    Single sngData = 0.0f;
                    ei = MCCDaq._board.ToEngUnits(Range.Bip10Volts, rawData, out sngData);

                    //Set the value of the output if the value was converted
                    if (ei.Value == ErrorInfo.ErrorCode.NoErrors)
                        sum += Convert.ToDouble(sngData);
                }
            }

            //Return the result
            return sum / numSamples;
        }
        #endregion


        #region IDisposable Methods
        /// <summary>
        /// Dispose is called to clean up the Background scan
        /// if it has been started.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// CreateMccBoard does the detection and creation
        /// of the MccBoard object and stores it in the 
        /// static variable.
        /// </summary>
        private void CreateMccBoard()
        {
            //If the board has already been created, just return
            //with no further processing
            if (MCCDaq._board != null)
                return;

            //Figure out the number of boards that 
            //have been installed
            //Get the max number of boards
            Int32 maxBoards = GlobalConfig.NumBoards;

            //If the number isn't at least one, throw an Exception
            if (maxBoards < 1)
                throw new System.ApplicationException("Could not find MCC DAQ Boards installed.");

            //Iterate through the boards and take the first
            //one that works
            for (Int32 i = 0; i < maxBoards; i++)
            {
                //Attempt to make the MccBoard
                MccBoard board = new MccBoard(i);

                //If the Board doesn't appear to exist, continue
                ErrorInfo ei = board.FlashLED();
                if (ei.Value == ErrorInfo.ErrorCode.BadBoard)
                    continue;

                //Save the board as the member variable and break
                MCCDaq._board = board;
                break;
            }
        }


        /// <summary>
        /// ConfigDIOPort is used to configure the Digital
        /// I/O port to perform output.
        /// </summary>
        private void ConfigDIOPort()
        {
            //If the port has already been configured, just
            //return with no further processing
            if (this._dioPortConfigured == true)
                return;

            //Configure the port
            MCCDaq._board.DConfigPort(MCCDaq._laserPortType, DigitalPortDirection.DigitalOut);

            //Set the flag to show the port is configured
            this._dioPortConfigured = true;
        }
        #endregion
    }
}
