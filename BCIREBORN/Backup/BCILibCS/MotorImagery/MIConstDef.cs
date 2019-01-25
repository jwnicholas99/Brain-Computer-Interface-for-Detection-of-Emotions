using System;
using System.Collections.Generic;
using System.Text;

using BCILib.App;

namespace BCILib.MotorImagery
{
    public class MIConstDef : BCICfg
    {
        public const string BCIType = "BCIMotorImagery";
        public const string TrainingCfgFileName = "MITrain.cfg";

        public const string MITraining = "MITraining";
        public const string ModelTrainingSetting = "ModelTrainingSetting";
        public const string Train_MTS_Para = "Train_MTS_Para";
        public const string Train_MTS_Channels = "Train_MTS_Channels";
        public const string Used_Channels = "Used_Channels";

        public const string MIProcess = "MIProcess";
        public const string MultiTimeModel = "MultiTimeModel";
        public const string ClassLabels = "ClassLabels";
        public const string ModelPattern = "ModelPattern";

        public const string Number_tasks = "Number_tasks";
        public const string Num_Moving_Steps = "Num_Moving_Steps";
        public const string Finger_Click = "Finger_Click";
        public const string Finger_LeftClick = "Finger_LeftClick";
        public const string Finger_RightClick = "Finger_RightClick";
        public const string Tongue_Move = "Tongue_Move";
        public const string Foot_Move = "Foot_Move";
        public const string BeepAfterRest = "BeepAfterRest";

        public const string Task_Configure = "Task_Configure";
        public const string Key_Configure = "Key_Configure";
        public const string Rehab_Action = "Rehab_Action";
        public const string Rehab_Repeat = "Rehab_Repeat";

        public const string MITest = "MITest";
        public const string MI_Num_Score = "MI_Num_Score";
        public const string MI_Shift_Score = "MI_Shift_Score";
        public const string MI_Num_Rehab = "MI_Num_Rehab";
        public const string MI_Num_Supervise = "MI_Num_Supervise";

        public static string DefChannels
        {
            get
            {
                return "F7,F3,Fz,F4,F8,FT7,FC3,FCz,FC4,FT8,T7,C3,Cz,C4,T8,TP7,CP3,CPz,CP4,TP8,P7,P3,Pz,P4,P8";
            }
        }

        public const string ModelList = "Models.lst";
    }

    /// <summary>
    /// Motor Imagery Steps
    /// </summary>
    public enum MI_STEP
    {
        MI_None = 0,
        MI_Prepare = 1,
        MI_Cue = 2,
        MI_Action = 3,
        MI_Rest = 4,
        MI_Rehab = 5,

        MI_Success = 6,
        MI_Fail = 7,
    };

    public enum MIAction
    {
        None = 0,
        Left = 1,
        Right = 2,
        MITongue = 3,
        Feet = 4,
        Idle = 5,

        // added on 25 May 2014 - for Cybathlon data collection
        LRH = 6, // left and right hand
        LHF = 7, // left hand and foot
        RHF = 8, // right hand and foot

        // added on 24 Sept 2012
        //MISwallow = 6, // Moter Imagery of Swallow
        //TongueMove = 7,
        //ActualSwallow = 8,
    };

    public enum MI_MODEL_TYPE
    {
        None,
        FB_ParzenWindow,//Feature Selection using MI based on Best Individual Features with Parzen Window
        FB_FLD
    };

}
