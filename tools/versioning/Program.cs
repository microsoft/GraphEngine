using System;

public class Versioning
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine(GetFileBuildNumber());
    }

    /// <summary>
    /// 16 bit build number: [year: 7 bits][month: 4 bits][day: 5 bits]
    /// </summary>
    public static string GetFileBuildNumber()
    {
        DateTime dt = DateTime.Now;
        uint year = (uint)(dt.Year - 2000);
        year = year << 9;

        uint month = (uint)dt.Month;
        month = month << 5;

        uint day = (uint)dt.Day;

        uint time = year | month | day;

        return time.ToString();
    }

    public static string BuildNumber2Date(string build)
    {
        uint time = uint.Parse(build);
        uint day = time & 0x1f;
        uint month = (time >> 5) & 0xf;
        uint year = (time >> 9) & 0x7f;
        return $"20{year}-{month}-{day}";
    }
}
