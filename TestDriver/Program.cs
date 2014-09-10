using System;
using System.Collections.Generic;
using System.Text;

using RDH2.SHArK.Interface;
using RDH2.SHArK.Interface.Enums;

namespace TestDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get an IMirror to move the motor
            IMirror mirror = ClassFactory.CreateIMirror();

            //Loop through commands
            String quit = "n";
            do
            {
                //Get the axis 
                Console.Write("Axis: (X)");
                String axisStr = Console.ReadLine();

                if (axisStr == String.Empty)
                    axisStr = "X";

                //Get the power
                Console.Write("Power: (25)");
                String powerStr = Console.ReadLine();

                if (powerStr == String.Empty)
                    powerStr = "25";

                //Get the degrees
                Console.Write("Degrees: (90)");
                String degStr = Console.ReadLine();

                if (degStr == String.Empty)
                    degStr = "90";

                //Move the Mirror
                MirrorAxis axis = (MirrorAxis)Enum.Parse(typeof(MirrorAxis), axisStr);
                Int32 power = Int32.Parse(powerStr);
                Int32 degrees = Int32.Parse(degStr);

                mirror.Move(axis, degrees);

                //Get the char to quit
                Console.Write("Quit: (n)");
                quit = Console.ReadLine();

                if (quit == String.Empty)
                    quit = "n";

                Console.WriteLine();
            }
            while (quit != "y");

            //Dispose of the Mirror
            mirror.Dispose();
        }
    }
}
