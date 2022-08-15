using LiteNetLib.Utils;

public class Utils
{
    public static NetDataWriter singleWriter;

    public static NetDataWriter GetNetDataWriter(string message)
    {
        if (singleWriter == null)
            singleWriter = new NetDataWriter();
        singleWriter.Reset();
        singleWriter.Put(message);
        return singleWriter;
    }

    public static NetDataWriter GetNetDataWriter(byte type)
    {
        if (singleWriter == null)
            singleWriter = new NetDataWriter();
        singleWriter.Reset();
        singleWriter.Put(type);
        return singleWriter;
    }
}