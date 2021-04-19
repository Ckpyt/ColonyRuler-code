using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Xml.Serialization;
using ExcelLoading;
using Newtonsoft.Json;

namespace XMLExport
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "ExcelLoading", IsNullable = false)]
    public partial class XmlObject
    {

        private AbstractObject[] repetativeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public AbstractObject[] repetative
        {
            get
            {
                return this.repetativeField;
            }
            set
            {
                this.repetativeField = value;
            }
        }
    }
    /// <summary>
    /// Exports data from excel and simply test it
    /// </summary>
    class Program
    {
        public const string itmsMap = "\\items_map.xml";
        public const string ArmyMap = "\\army_map.xml";
        public const string buildingsMap = "\\buildings_map.xml";
        public const string materialsMap = "\\materials_map.xml";
        public const string scienceMap = "\\science_map.xml";
        public const string resourceMap = "\\resource_map.xml";
        public const string mineralResourceMap = "\\mineralResource_map.xml";
        public const string processMap = "\\process_map.xml";
        public const string wildAnimalMap = "\\wildAnimal_map.xml";
        public const string domesticAnimalMap = "\\domesticAnimal_map.xml";
        public const string trapsMap = "\\traps_map.xml";

        public const string gameFolder = "\\ColonyRuler\\Assets\\Scripts\\XMLScripts";
        const string filename = "ColonyRuler.xlsm";

        public static string LocateXMLFile()
        {
            string curDir = Directory.GetCurrentDirectory();
            while (!File.Exists(curDir + "\\" + filename))
                curDir = Directory.GetParent(curDir).FullName;
            return curDir;
        }

        static ItemsLocalization localizItems = new ItemsLocalization();
        /// <summary>
        /// Start point of application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string curDir = LocateXMLFile();

            DeleteFiles("*.xml", curDir);

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
                    //fixing with Russian Excel
                    const string wrongName = "карта";
                    string lastName = map.Name.Substring(map.Name.Length - wrongName.Length);
                    if(lastName == wrongName)
                    {
                        string name = map.Name.Substring(0, map.Name.Length - wrongName.Length);
                        name += "Map";
                        map.Name = name;
                    }

                    StreamWriter file = new StreamWriter(curDir + "\\" + map.Name + ".xml");
                    file.Write(data);
                    file.Flush();
                    file.Close();
                }
            }
            app.Workbooks.Close();
            app.Quit();

            string DestPath = curDir + "\\ColonyRuler\\Assets\\Resources";

            DeleteFiles("*.xml", DestPath);
            DeleteFiles("*.meta", DestPath);

            CopyFiles("*.xml", DestPath, curDir);
            CopyFiles("*.cs", curDir + gameFolder, curDir);

            FullTesting(DestPath);
            ExportJson(DestPath);

            Console.Out.WriteLine(DateTime.Now.ToString() + " done ");

        }

        /// <summary>
        /// get all names and descriptions from xml and put it in the localization file
        /// </summary>
        /// <param name="DestPath"> there are xml files </param>
        static void ExportJson(string DestPath)
        {

            ReadFile(DestPath + itmsMap, "items"); 
            ReadFile(DestPath + ArmyMap, "army"); 
            ReadFile(DestPath + buildingsMap, "buildings");
            ReadFile(DestPath + materialsMap, "materials");
            ReadFile(DestPath + scienceMap, "science");
            ReadFile(DestPath + resourceMap, "resource");
            ReadFile(DestPath + mineralResourceMap, "mineralResource");
            ReadFile(DestPath + processMap, "process");
            ReadFile(DestPath + wildAnimalMap, "wildAnimal");
            ReadFile(DestPath + domesticAnimalMap, "domesticAnimal");
            ReadFile(DestPath + trapsMap, "traps");

            localizItems.m_itemList.Add(new LocalizationItem("Population", "Population"));
            localizItems.m_itemList.Add(new LocalizationItem("People", "People"));

            localizItems.Sort();
            string json = JsonConvert.SerializeObject(localizItems);
            File.WriteAllText(DestPath + "\\EN_en\\items_new.json", json);
        }

        static void ReadFile(string filename, string className)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(XmlObject));
                TextReader reader = new StreamReader(filename);
                string fl = reader.ReadToEnd();
                fl = fl.Replace(className, "XmlObject");
                StringReader strReader = new StringReader(fl);
                XmlObject itm = (XmlObject)x.Deserialize(strReader);
                foreach(var obj in itm.repetative)
                {
                    LocalizationItem litm = new LocalizationItem(obj.name, obj.description);
                    localizItems.m_itemList.Add(litm);
                }
            }
            catch (Exception e)
            {
                var cons = Console.Out;
                cons.WriteLine("Some error found:" + e.Message + " in the file:" + filename);
            }
        }

        /// <summary>
        /// Simple testing of excel data.
        /// </summary>
        /// <param name="DestPath"> there are xml files </param>
        static void FullTesting(string DestPath)
        {
            //non-abstractObject children
            string filename = DestPath + "\\effect_map.xml";
            test<effect>(filename);

            //abstractObject children
            filename = DestPath + itmsMap;
            test<items>(filename);
            filename = DestPath + ArmyMap;
            test<army>(filename);
            filename = DestPath + buildingsMap;
            test<buildings>(filename);
            filename = DestPath + materialsMap;
            test<materials>(filename);
            filename = DestPath + scienceMap;
            test<science>(filename);
            //filename = DestPath + "\\celebrations_map.xml";
            //test<celebrations>(filename);
            filename = DestPath + resourceMap;
            test<resource>(filename);
            filename = DestPath + mineralResourceMap;
            test<mineralResource>(filename);
            filename = DestPath + processMap;
            test<process>(filename);
            filename = DestPath + wildAnimalMap;
            test<wildAnimal>(filename);
            filename = DestPath + domesticAnimalMap;
            test<domesticAnimal>(filename);
            filename = DestPath + trapsMap;
            test<traps>(filename);
        }

        /// <summary>
        /// delete files with the same extension in the folder
        /// </summary>
        /// <param name="extention"> files' extention </param>
        /// <param name="folder"> folder for deleting files </param>
        static void DeleteFiles(string extention, string folder)
        {
            foreach (string file in Directory.GetFiles(folder, extention))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Copy all the files with extention from source dir to dest dir
        /// </summary>
        /// <param name="destFolder">destination folder</param>
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
        static void test<T> (string filename)
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
