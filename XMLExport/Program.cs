using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Xml.Serialization;
using ExcelLoading;

namespace XMLExport
{
    /// <summary>
    /// Exports data from excel and simply test it
    /// </summary>
    class Program
    {
        /// <summary>
        /// Start point of application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string curDir = Directory.GetCurrentDirectory();
            string filename = "ColonyRuler.xlsm";
            while (!File.Exists(curDir + "\\" + filename))
                curDir = Directory.GetParent(curDir).FullName;

            Application app = new Application();
            app.Workbooks.Open((curDir + "\\" + filename),
                               Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                               Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                               Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                               Type.Missing, Type.Missing);
            foreach (XmlMap map in app.ActiveWorkbook.XmlMaps)
            {
                string data = string.Empty;
                if (map.IsExportable)
                {
                    map.ExportXml(out data);
                    StreamWriter file = new StreamWriter(curDir + "\\" + map.Name + ".xml");
                    file.Write(data);
                    file.Flush();
                    file.Close();
                }
            }
            app.Workbooks.Close();
            app.Quit();

            string destPath = curDir + "\\ColonyRuler\\Assets\\Resources";

            CopyFiles("*.xml", destPath, curDir);
            CopyFiles("*.cs", curDir + "\\ColonyRuler\\Assets\\Scripts\\XMLScripts", curDir);

            filename = destPath + "\\items_map.xml";
            Test<items>( filename );
            filename = destPath + "\\army_map.xml";
            Test<army>(filename);
            filename = destPath + "\\buildings_map.xml";
            Test<buildings>(filename);
            filename = destPath + "\\materials_map.xml";
            Test<materials>(filename);
            filename = destPath + "\\science_map.xml";
            Test<science>(filename);
            filename = destPath + "\\celebrations_map.xml";
            Test<celebrations>(filename);
            filename = destPath + "\\resource_map.xml";
            Test<resource>(filename);
            filename = destPath + "\\process_map.xml";
            Test<process>(filename);
            filename = destPath + "\\effect_map.xml";
            Test<effect>(filename);

            Console.Out.WriteLine(DateTime.Now.ToString() + " done ");

        }

        /// <summary>
        /// Copy all the files with extention from source dir to dest dir
        /// </summary>
        /// <param name="destFolder"></param>
        /// <param name="curDir"> source dir </param>
        static void CopyFiles(string extention, string destFolder, string curDir)
        {

            foreach (string file in Directory.GetFiles(curDir, extention))
            {
                string destFile = destFolder + "\\" + Path.GetFileName(file);
                if (File.Exists(destFile))
                    File.Delete(destFile);

                File.Copy(file, destFile);
            }
        }

        /// <summary>
        /// Base testing for deserialization of data
        /// </summary>
        /// <typeparam name="T"> class for testing </typeparam>
        /// <param name="filename"> data for testing </param>
        static void Test<T> (string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    XmlSerializer x = new XmlSerializer(typeof(T));
                    TextReader reader = new StreamReader(filename);

                    T itm = (T)x.Deserialize(reader);
                }
                catch (Exception e)
                {
                    var cons = Console.Out;
                    cons.WriteLine("Some error found:" + e.Message + " in the file:" + filename);
                }
            }
        }
    }
}
