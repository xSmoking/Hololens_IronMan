#if WINDOWS_UWP
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
#endif

public static class SerialIO
{
#if WINDOWS_UWP
    private static SerialDevice serial;
    private static DataReader reader;
    private static DataWriter writer;
#endif

    static SerialIO()
    {
#if WINDOWS_UWP
        //await serial = SerialDevice.FromIdAsync("COM1");
        serial.BaudRate = 9600;
        //serial.PortName = "COM5";
        //serial.ReadTimeout.Duration = 1000;
        serial.DataBits = 8;
        serial.StopBits = SerialStopBitCount.Two;
        serial.Parity = SerialParity.None;
        reader = new DataReader(serial.InputStream);
        writer = new DataWriter(serial.OutputStream);
        reader.UnicodeEncoding = UnicodeEncoding.Utf8;
        reader.ByteOrder = ByteOrder.LittleEndian;
#endif
    }

    public static void Write(string _message)
    {
#if WINDOWS_UWP
        writer.WriteString(_message);
#endif
    }

    public static string Read()
    {
#if WINDOWS_UWP
        return reader.ReadString(1);
#else
        return "";
#endif
    }
}