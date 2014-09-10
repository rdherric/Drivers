using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using RDH2.USB;
using RDH2.USB.Enums;

namespace RDH2.SHArK.Interface.Mirror
{
    /// <summary>
    /// LEGOMirror is used to manipulate a Mirror using the 
    /// LEGO Mindstorms NXT Brick.
    /// </summary>
    internal class LEGOMirror : IMirror
    {
        #region Member Variables
        private const String _name = "LEGO Mirror Interface";
        private const Double _secondsPerStep = 0.025;

        //USB\VID_0694&PID_0002
        private const Int32 _vendorID = 0x0694;
        private const Int32 _productID = 0x0002;
        private UsbInterface _device = null;
        private UsbInterface.UsbPipe _inPipe = null;
        private UsbInterface.UsbPipe _outPipe = null;

        private const Byte _mailboxID = 0;
        private const Byte _readMailboxID = 10;
        private const Byte _motorPower = 25;
        private const String _programName = "SHArK.rxe";
        private const String _resourceName = "RDH2.SHArK.Interface.Resources." + LEGOMirror._programName;
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor for the LEGOMirror class.
        /// </summary>
        public LEGOMirror()
        {
            //Create the DeviceInterface
            this._device = new UsbInterface(LEGOMirror._vendorID, LEGOMirror._productID);

            //Figure out the pipes
            foreach (UsbInterface.UsbPipe pipe in this._device.Pipes)
            {
                if (pipe.Type == UsbdPipeType.UsbdPipeTypeBulk && pipe.Direction == UsbPipeDirection.In)
                    this._inPipe = pipe;
                else if (pipe.Type == UsbdPipeType.UsbdPipeTypeBulk && pipe.Direction == UsbPipeDirection.Out)
                    this._outPipe = pipe;
            }

            //Setup the NXT brick by downloading the program 
            //and starting it running
            this.SetupNXTBrick();
        }
        #endregion


        #region IMirror methods
        /// <summary>
        /// Name defines the name of the Mirror interface if
        /// one was successfully created.
        /// </summary>
        public String Name
        {
            get { return LEGOMirror._name; }
        }


        /// <summary>
        /// SecondsPerStep defines the number of seconds required
        /// for the mirror to perform a step.
        /// </summary>
        public Double SecondsPerStep
        {
            get { return LEGOMirror._secondsPerStep; }
        }


        /// <summary>
        /// Move moves a particular mirror to the specified position.
        /// </summary>
        /// <param name="axis">The MirrorAxis to move -- X or Y</param>
        /// <param name="relativePos">The amount by which to move the mirror relative to its current position</param>
        public void Move(Enums.MirrorAxis axis, Int32 relativePos)
        {
            //Flip the direction of the position if this 
            //is the X-Axis mirror -- this is required because
            //the X-Axis mirror is inverted in gears
            if (axis == Enums.MirrorAxis.X)
                relativePos *= -1;

            //Get the Mirror Move command
            Byte[] command = this.CreateMirrorMoveCommand(axis, relativePos);

            //Write the command and check the result
            this._device.Write(this._outPipe, command);
            this.CheckReturnCode((Byte)LEGODirectCommand.MessageWrite, 3, false);

            //Get the Mirror Move Complete command
            command = this.CreateMirrorMoveCompleteCommand();

            //Write the command the check the result until 
            //a response comes back
            Byte[] output = null;
            do
            {
                this._device.Write(this._outPipe, command);
                output = this.CheckReturnCode((Byte)LEGODirectCommand.MessageRead, 64, true);
            }
            while (output != null && output[4] == 0);
        }
        #endregion


        #region IDisposable methods
        /// <summary>
        /// Dispose is called when the program is shutting 
        /// down so that LEGO brick can be cleaned up.
        /// </summary>
        public void Dispose()
        {
            //Clean up the brick
            this.CleanupNXTBrick();

            //Clean up the UsbInterface
            this._device.Dispose();
            this._device = null;
        }
        #endregion


        #region Command Methods
        /// <summary>
        /// CreateProgramStartCommand creates a Byte Array to 
        /// be written to the NXT brick to start a program going.
        /// </summary>
        /// <returns>Byte Array of command data</returns>
        private Byte[] CreateProgramStartCommand()
        {
            //Declare a variable to return
            List<Byte> rtn = new List<Byte>();

            //Add the command type and purpose
            rtn.Add((Byte)LEGOCommandType.DirectCommand);
            rtn.Add((Byte)LEGODirectCommand.StartProgram);

            //Turn the name of the Program to run into a Char Array
            Char[] nameChars = LEGOMirror._programName.ToCharArray();
            
            //Add each of the chars to the List
            foreach (Char c in nameChars)
                rtn.Add((Byte)c);

            //Add null chars up to 22 bytes
            while (rtn.Count < 22)
                rtn.Add(0);

            //Return the result
            return rtn.ToArray();
        }


        /// <summary>
        /// CreateProgramStopCommand creates a Byte Array to be
        /// written to the NXT brick to stop the currently running
        /// program.
        /// </summary>
        /// <param name="requireReply">Boolean to determine if a success code should be returned</param>
        /// <returns>Byte Array of command data</returns>
        private Byte[] CreateProgramStopCommand(Boolean requireReply)
        {
            //Declare a variable to return
            Byte[] rtn = new Byte[2];

            //Add the command type and purpose
            if (requireReply == true)
                rtn[0] = (Byte)LEGOCommandType.DirectCommand;
            else
                rtn[0] = (Byte)LEGOCommandType.DirectCommandWithoutReply;

            rtn[1] = (Byte)LEGODirectCommand.StopProgram;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// CreateProgramFileCreateCommand creates a Byte Array
        /// to be written to the NXT brick to open a file for
        /// writing on the brick.
        /// </summary>
        /// <returns>Byte Array of command data</returns>
        private Byte[] CreateProgramFileCreateCommand()
        {
            //Declare a List to hold the Byte data
            List<Byte> rtn = new List<Byte>();

            //Add the command type and purpose
            rtn.Add((Byte)LEGOCommandType.SystemCommand);
            rtn.Add((Byte)LEGOSystemCommand.OpenWriteLinear);

            //Turn the name of the Program to run into a Char Array
            Char[] nameChars = LEGOMirror._programName.ToCharArray();

            //Add each of the chars to the List
            foreach (Char c in nameChars)
                rtn.Add((Byte)c);

            //Add nulls until the Length is 22
            while (rtn.Count < 22)
                rtn.Add(0);

            //Get the size of the file
            Byte[] fileBytes = this.GetSHArKFileResource();
            Int32 fileSize = fileBytes.GetLength(0);

            //Add the size of the file to the Byte Array
            rtn.Add((Byte)(fileSize & 0xFF));
            rtn.Add((Byte)((fileSize >> 8) & 0xFF));
            rtn.Add((Byte)((fileSize >> 16) & 0xFF));
            rtn.Add((Byte)((fileSize >> 24) & 0xFF));

            //Return the result
            return rtn.ToArray();
        }


        /// <summary>
        /// CreateProgramLoadCommand creates a Byte Array 
        /// to be written to the NXT brick to write the 
        /// SHArK program file to the brick.
        /// </summary>
        /// <param name="fileHandle">The Handle of the NXT program file</param>
        /// <returns>Byte Array of command data</returns>
        private List<Byte[]> CreateProgramLoadCommands(Byte fileHandle)
        {
            //Declare a List to hold the Byte data
            List<Byte[]> rtn = new List<Byte[]>();

            //Get the file from Resources
            Byte[] fileBytes = this.GetSHArKFileResource();

            //Split the file into 61 byte chunks so that it can
            //be written in pieces to the device
            Int32 numChunks = Convert.ToInt32(fileBytes.GetLength(0) / 61);
            Int32 remainder = fileBytes.GetLength(0) % 61;

            //Iterate through the chunks and add them to commands
            for (Int32 i = 0; i <= numChunks; i++)
            {
                //Create a new Byte Array to hold the command
                List<Byte> currCommand = new List<Byte>();

                //Add the command type and purpose
                currCommand.Add((Byte)LEGOCommandType.SystemCommand);
                currCommand.Add((Byte)LEGOSystemCommand.WriteData);

                //Add the Handle number
                currCommand.Add(fileHandle);

                //Determine the Array size based on the loop index
                Int32 arraySize = 61;
                if (i == numChunks)
                    arraySize = remainder;

                //Copy the bytes to a temp Array
                Byte[] currFileBytes = new Byte[arraySize];
                Array.Copy(fileBytes, (i * 61), currFileBytes, 0, arraySize);

                //Add the Bytes to the Array
                currCommand.AddRange(currFileBytes);

                //Add the command to the return List
                rtn.Add(currCommand.ToArray());
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// CreateFileCloseCommand creates a Byte Array to be written
        /// to the NXT Brick to close a file that has been written.
        /// </summary>
        /// <param name="fileHandle">The File Handle to close</param>
        /// <returns>Byte Array of command data</returns>
        private Byte[] CreateFileCloseCommand(Byte fileHandle)
        {
            //Declare a variable to return
            Byte[] rtn = new Byte[3];

            //Set the command type and purpose
            rtn[0] = (Byte)LEGOCommandType.SystemCommand;
            rtn[1] = (Byte)LEGOSystemCommand.CloseHandle;

            //Set the file handle
            rtn[2] = fileHandle;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// CreateProgramDeleteCommand creates a Byte Array to write 
        /// to the NXT brick to delete the SHArK program file.
        /// </summary>
        /// <returns></returns>
        private Byte[] CreateProgramDeleteCommand(Boolean requireReply)
        {
            //Declare a variable to return
            List<Byte> rtn = new List<Byte>();

            //Add the command type and purpose
            if (requireReply == true)
                rtn.Add((Byte)LEGOCommandType.SystemCommand);
            else
                rtn.Add((Byte)LEGOCommandType.SystemCommandWithoutReply);

            rtn.Add((Byte)LEGOSystemCommand.DeleteFile);

            //Turn the name of the Program to run into a Char Array
            Char[] nameChars = LEGOMirror._programName.ToCharArray();

            //Add each of the chars to the List
            foreach (Char c in nameChars)
                rtn.Add((Byte)c);

            //Add nulls until the Length is 22
            while (rtn.Count < 22)
                rtn.Add(0);

            //Return the result
            return rtn.ToArray();
        }


        /// <summary>
        /// CreateMirrorMoveCommand is used to generate the Byte
        /// Array that will be sent to the NXT Brick to move a 
        /// Mirror.
        /// </summary>
        /// <param name="axis">The Axis that needs to be moved -- X or Y</param>
        /// <param name="relativePos">The amount by which to move the mirror relative to the current position</param>
        /// <returns>Byte Array of LEGO Command</returns>
        private Byte[] CreateMirrorMoveCommand(Enums.MirrorAxis axis, Int32 relativePos)
        {
            //Declare a List to hold the command
            List<Byte> rtn = new List<Byte>();

            //Set the command type -- Direct with reply
            rtn.Add((Byte)LEGOCommandType.DirectCommand);

            //Set the MessageWrite command
            rtn.Add((Byte)LEGODirectCommand.MessageWrite);

            //Set the Mailbox to which to send message
            rtn.Add(LEGOMirror._mailboxID);

            //Create a string with the data in it
            String msg = Convert.ToByte(axis).ToString() + "|" + LEGOMirror._motorPower.ToString() + "|" + relativePos.ToString();

            //Add the length of the string for message size -- add
            //one for the null character added below
            rtn.Add(Convert.ToByte(msg.Length + 1));

            //Turn the String to a Char array
            Char[] msgChars = msg.ToCharArray();

            //Put each byte into the command
            foreach (Char c in msgChars)
                rtn.Add((Byte)c);

            //Add a null byte for the end of the string
            rtn.Add(0);

            //Return the result
            return rtn.ToArray();
        }


        /// <summary>
        /// CreateMirrorMoveCompleteCommand creates a Byte Array
        /// that can be sent to the NXT Brick to read a complete
        /// message after the motor has been moved.
        /// </summary>
        /// <returns></returns>
        private Byte[] CreateMirrorMoveCompleteCommand()
        {
            //Declare a variable to return
            Byte[] rtn = new Byte[5];

            //Set the command type and purpose
            rtn[0] = (Byte)LEGOCommandType.DirectCommand;
            rtn[1] = (Byte)LEGODirectCommand.MessageRead;

            //Set the remote Mailbox from which to read a message
            rtn[2] = LEGOMirror._readMailboxID;

            //Set the local Mailbox to 0x00
            rtn[3] = 0x00;

            //Set the Read to remove the message
            rtn[4] = 0x01;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// CreateGetOutputStateCommand creates a Byte Array to 
        /// send to the Brick to query the current state of the
        /// motors.
        /// </summary>
        /// <param name="axis">The Motor to query</param>
        private Byte[] CreateGetOutputStateCommand(Enums.MirrorAxis axis)
        {
            //Declare a variable to return
            Byte[] rtn = new Byte[3];

            //Set the command type -- Direct with reply
            rtn[0] = (Byte)LEGOCommandType.DirectCommand;

            //Set the GetOutputState command
            rtn[1] = (Byte)LEGODirectCommand.GetOutputState;

            //Set the Motor to query
            rtn[2] = Convert.ToByte(axis);

            //Return the result
            return rtn;
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// SetupNXTBrick downloads the SHArK program to the 
        /// NXT brick and starts it running.
        /// </summary>
        private void SetupNXTBrick()
        {
            //Get the command to stop the program if it is running -- 
            //don't worry about return codes
            Byte[] command = this.CreateProgramStopCommand(false);

            //Write the command to the brick
            this._device.Write(this._outPipe, command);

            //Get the command to delete the file if it exists -- 
            //don't worry about return codes
            command = this.CreateProgramDeleteCommand(false);

            //Write the command to the brick
            this._device.Write(this._outPipe, command);

            //Get the command to open the file write
            command = this.CreateProgramFileCreateCommand();

            //Write the command to the brick
            this._device.Write(this._outPipe, command);
            Byte[] rtnCode = this.CheckReturnCode((Byte)LEGOSystemCommand.OpenWriteLinear, 4, false);

            //Read the result and get the file Handle
            //out of it
            Byte fileHandle = rtnCode[3];

            //Get the List of commands to write the program to the brick.
            //THis is required so that the 64-byte limit isn't overflowed.
            List<Byte[]> loadCommands = this.CreateProgramLoadCommands(fileHandle);

            //Write the data to the device
            foreach (Byte[] fileWriteCommand in loadCommands)
            {
                //Write the command to the brick and check the result
                this._device.Write(this._outPipe, fileWriteCommand);
                this.CheckReturnCode((Byte)LEGOSystemCommand.WriteData, 6, false);
            }

            //Get the command to close the file handle
            command = this.CreateFileCloseCommand(fileHandle);

            //Write the command to the brick and check the result
            this._device.Write(this._outPipe, command);
            this.CheckReturnCode((Byte)LEGOSystemCommand.CloseHandle, 4, false);

            //Get the command to start the program
            command = this.CreateProgramStartCommand();

            //Wait 500 ms to allow the brick to catch up
            Thread.Sleep(500);

            //Write the command to the brick and check the result
            this._device.Write(this._outPipe, command);
            this.CheckReturnCode((Byte)LEGODirectCommand.StartProgram, 3, false);

            //Wait 500 ms to allow the brick to catch up
            Thread.Sleep(500);
        }


        /// <summary>
        /// CleanupNXTBrick stops the SHArK program from running
        /// and deletes it from the brick.
        /// </summary>
        private void CleanupNXTBrick()
        {
            //Get the command to stop the program -- require a reply
            Byte[] command = this.CreateProgramStopCommand(true);

            //Write the command to the brick and check the result
            this._device.Write(this._outPipe, command);
            this.CheckReturnCode((Byte)LEGODirectCommand.StopProgram, 3, false);

            //Get the command to delete the program
            command = this.CreateProgramDeleteCommand(true);

            //Delete the program from the NXT brick and check
            //the result
            this._device.Write(this._outPipe, command);
            this.CheckReturnCode((Byte)LEGOSystemCommand.DeleteFile, 23, false);
        }


        /// <summary>
        /// CheckReturnCode iterates through the Bytes of a return
        /// Array and throws Exceptions as necessary.
        /// </summary>
        /// <param name="command">The type of command that was sent</param>
        /// <param name="bytesToRead">The amount of memory to read from the Brick</param>
        /// <param name="suppressExceptions">Determines if an exception is thrown or just a return code</param>
        private Byte[] CheckReturnCode(Byte command, Int32 bytesToRead, Boolean suppressExceptions)
        {
            //Create a return code
            Byte[] rtnCode = this._device.Read(this._inPipe, bytesToRead);

            //Create an Exception object to throw
            Exception e = null;

            //If no return code was found...Exception
            if (rtnCode == null)
                e = new System.ApplicationException("Could not read Error Code from NXT Brick.");

            //If the code isn't exactly right...Exception
            else if (rtnCode.GetLength(0) != bytesToRead)
                e = new System.ApplicationException("Error Code return was not returned properly.");

            //If the first Bytes isn't 2...Exception
            else if (rtnCode[0] != 2)
                e = new System.ApplicationException("Error Code return is not valid LEGO return code.");

            //IF the commands don't match up...Exception
            else if (rtnCode[1] != Convert.ToByte(command))
                e = new System.ApplicationException("Command was not replied successfully.");

            //Finally, if the error code isn't 0...Exception
            else if (rtnCode[2] != 0)
                e = new System.ApplicationException("Error in performing Command -- Error Code: " + rtnCode[2].ToString());

            //Throw the Exception if it shouldn't be suppressed
            if (e != null && suppressExceptions == false)
                throw e;

            //Return the code
            return rtnCode;
        }


        /// <summary>
        /// GetSHArKFileResource pulls the RXE file out of 
        /// the DLL and returns it as a Byte Array.
        /// </summary>
        /// <returns>Byte Array of file data</returns>
        private Byte[] GetSHArKFileResource()
        {
            //Declare a variable to return
            List<Byte> rtn = new List<Byte>();

            //Set the Assembly that is currently executing
            Assembly assembly = Assembly.GetExecutingAssembly();

            //Pull the resource out of the Assembly
            Stream fileStream = assembly.GetManifestResourceStream(LEGOMirror._resourceName);

            //If the stream is null, throw an Exception
            if (fileStream == null)
                throw new System.ApplicationException("Could not Load SHArK.RXE File from Resources.");

            //Read the resource into the Array
            for (Int32 i = 0; i < fileStream.Length; i++)
                rtn.Add((Byte)fileStream.ReadByte());
            
            //Return the result
            return rtn.ToArray();
        }
        #endregion


        #region Private Enums to run the LEGO Brick
        /// <summary>
        /// LEGOCommandType determines the type of Command
        /// that needs to be sent to the Brick.  
        /// </summary>
        private enum LEGOCommandType
        {
            DirectCommand = 0x00,
            SystemCommand = 0x01,
            ReplyCommand = 0x02,
            DirectCommandWithoutReply = 0x80,
            SystemCommandWithoutReply = 0x81
        }


        /// <summary>
        /// LEGOSystemCommand is used to send a System Command
        /// to the Brick.
        /// </summary>
        private enum LEGOSystemCommand
        {
            OpenWrite = 0x81,
            WriteData = 0x83,
            CloseHandle = 0x84,
            DeleteFile = 0x85,
            GetFirmwareVersion = 0x88,
            OpenWriteLinear = 0x89,
            RequestIOMapInfo = 0x90,
            CloseIOMapHandle = 0x92,
            ReadIOMap = 0x94,
            WriteIOMap = 0x95,
            SetBrickName = 0x98,
            GetDeviceInfo = 0x9B
        }


        /// <summary>
        /// LEGODirectCommand is used to send a Direct Command
        /// to the Brick.
        /// </summary>
        private enum LEGODirectCommand
        {
            StartProgram = 0x00,
            StopProgram = 0x01,
            PlayTone = 0x03,
            SetOutputState = 0x04,
            SetInputMode = 0x05,
            GetOutputState = 0x06,
            GetInputValues = 0x07,
            ResetInputScaledValue = 0x08,
            MessageWrite = 0x09,
            ResetMotorPosition = 0x0A,
            GetBatteryLevel = 0x0B,
            KeepAlive = 0x0D,
            LsGetStatus = 0x0E,
            LsWrite = 0x0F,
            LsRead = 0x10,
            MessageRead = 0x13
        }
        #endregion
    }
}
