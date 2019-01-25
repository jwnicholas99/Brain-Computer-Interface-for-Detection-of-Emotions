using System.IO;

public enum P300_WMCMD : byte
{
    None,
    Output_Code_Score,
    Prepare_Start,
    Set_Button_State
}

public enum ButtonState
{
    Normal, Highlight, Cue, Result, NewLine, Clear
};


public class P300CMDReader
{
    private BinaryReader br = null;

    public P300CMDReader(byte[] cmd_buf)
    {
        br = new BinaryReader(new MemoryStream(cmd_buf));
    }
    
    public P300_WMCMD ReadCommand()
    {
        if (br != null)
            return (P300_WMCMD) (br.ReadInt32());
        else
            return P300_WMCMD.None;
    }

    public int ReadInt32()
    {
        if (br != null) return br.ReadInt32();
        else return int.MinValue;
    }

    public double ReadDouble()
    {
        if (br != null) return br.ReadDouble();
        else return double.NaN;
    }
}
