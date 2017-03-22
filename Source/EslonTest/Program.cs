using Eslon;
using Eslon.Java;
using System;
using System.Diagnostics;
using System.IO;

namespace EslonTest
{
    class Program
    {
        static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\EslonTest";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(Path);
            Eslon_AccessPoint();
            Eslon_Batch();
            Eslon_Opaque();
            Java_AccessPoint();
            Process.Start(Path);
        }

        // Test access point
        static void Eslon_AccessPoint()
        {
            var name = String.Format("{0}\\{1}.txt", Path, nameof(Eslon_AccessPoint));

            // Write
            File.WriteAllText(name, EslonAccess.Write(new Coalition()));

            // Read
            var volume = EslonAccess.Read<Coalition>(File.ReadAllText(name));

            // Write
            // File.WriteAllText(name, EslonAccess.Write(volume));
        }

        // Test batch objects
        static void Eslon_Batch()
        {
            var name = String.Format("{0}\\{1}.txt", Path, nameof(Eslon_Batch));
            var batch = new EslonBatch().Configure(new Coalition(), EslonAccess.Engine, true);

            // Write
            using (Stream stream = File.Create(name))
            {
                batch.Write(stream);
            }

            // Read
            using (Stream stream = File.OpenRead(name))
            {
                batch.Read(stream);
            }

            // Write
            using (Stream stream = File.Create(name))
            {
                batch.Write(stream);
            }
        }

        // Test opaque strings
        static void Eslon_Opaque()
        {
            var name = String.Format("{0}\\{1}.txt", Path, nameof(Eslon_Opaque));
            var text = "(A='Alpha',B='Bravo',C='Charlie')";

            File.WriteAllText(name, EslonCanon.Opaque.Read(text).ToString());
        }

        // Test Java access point
        static void Java_AccessPoint()
        {
            var name = String.Format("{0}\\{1}.txt", Path, nameof(Java_AccessPoint));

            // Write
            File.WriteAllText(name, JavaAccess.Write(new Coalition()));

            // Read
            var volume = JavaAccess.Read<Coalition>(File.ReadAllText(name));

            // Write
            // File.WriteAllText(name, JavaAccess.Write(volume));
        }
    }
}
