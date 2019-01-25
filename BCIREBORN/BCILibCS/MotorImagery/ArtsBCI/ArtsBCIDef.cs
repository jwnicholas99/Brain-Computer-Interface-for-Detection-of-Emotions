using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCILib.MotorImagery.ArtsBCI
{
    /// <summary>
    /// Specify actions for motor imagery tasks
    /// </summary>
    public enum RehabAction
    {
        None,

        // hand movement
        OpenClose,
        Rotate,
        Alternative,

        //// tongue movement
        //TongueMove,

        //// swallow
        //Swallow,
        //DrinkWater
    }

    public enum RehabCommand
    {
        None,
        Reset, // return to start position
        SetWindow, // Set feedback window positiona and size

        // for cue
        FadeIn,
        FadeOut,

        // for HK
        PassiveMovement,
        AssistiveMovement,

        WaitForFinish,
        Stop
    }
}
