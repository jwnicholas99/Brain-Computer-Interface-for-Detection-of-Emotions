using System;
using System.Collections.Generic;
using System.Text;

namespace BCILib.App
{
    public class BCICfg
    {
        public const string System = "System";
        public const string Train = "Train";
        public const string Test = "Test";

        // train
        public const string Current_Train = "Current_Train";
        public const string ID_Train = "ID_Train";

        public const string Number_Trials = "Number_Trials";
        public const string Time_WaitStart = "Time_WaitStart";
        public const string Time_Prepare = "Time_Prepare";
        public const string Time_Cue = "Time_Cue";
        public const string Time_Action = "Time_Action";
        public const string Time_Rest = "Time_Rest";
        public const string Time_Imagine_Wait = "Time_Imagine_Wait";

        public const string Stim_Prepare = "Stim_Prepare";
        public const string Stim_Task_Offset = "Stim_Task_Offset";
        public const string Stim_Click_Offset = "Stim_Click_Offset";
        public const string Stim_KeyDown_Offset = "Stim_KeyDown_Offset";
        public const string Stim_Rest = "Stim_Rest";

        public const string UsedChannelIdx = "UsedChannelIdx";

        public const string EEG = "EEG";
        public const string AppName = "AppName";

        // test
        public const string Score_Interval_MS = "Score_Interval_MS";
        public const string Score_Smooth_Factor = "Score_Smooth_Factor";
        public const string Score_Bias = "Score_Bias";
        public const string Score_Gain = "Score_Gain";
        public const string ModelFile = "ModelFile";
        public const string ModelUsedChannels = "ModelUsedChannels";
        public const string Amplifier = "Amplifier";
        public const string AmpChannels = "AmpChannels";
        public const string UseGlobalProfile = "UseGlobalProfile";

        // BCI Applications
        public const string AttentionDetection = "AttentionDetection";
        public const string TrainingFileListName = "TrainingFileListName";

        // file names
        public const string TrainingFileName = "TrainingFiles.txt";

        public const string NamePartSeperator = "-";
    }

    public enum BCIConstant {
        Data,
        Model,
        DataTraining,
    }
}
