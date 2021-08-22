using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace oed {
    public static class AppendXML{
        
        public static void AppendTxtFile(string filePath)
        {
            try {
                using (Stream input = File.OpenRead(filePath))
                using (Stream output = new FileStream("OED-Export.txt", FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    input.CopyTo(output); 
                }
                File.Delete(filePath);
            } catch (Exception e) {
                Console.WriteLine("Error appending temporary file to OED-Export.txt");
                Console.WriteLine(e.Message);
            }
        }
        public static void Append(string filePath)
        {
            try {
            (XmlDocument appendDoc, FileStream fs) = getXmlDocument(filePath);
            appendDoc = updateMaster(appendDoc);
            fs.Close();
            File.Delete(filePath);
            } catch (Exception e) {
                Console.WriteLine("Error: appending to XML file.");
                Console.WriteLine("Is your file opened by another process? Open in SuperMemo?");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.Message.ToString());
                Console.ReadKey();
            }


        }

        private static XmlDocument updateMaster(XmlDocument appendDoc)
        {
            // XmlNode countElement = appendDoc.SelectSingleNode("/SuperMemoCollection/Count");
            XmlNodeList appendElements = appendDoc.SelectNodes("/SuperMemoCollection/SuperMemoElement");
            int appendElementsCount = appendElements.Count;

            // Get the count from the file we are going to append to our main file
            // Lets count the number again ourselves, as the XmlWriter count is somewhat unreliable
            var masterFilePath = Path.Combine(Environment.CurrentDirectory, SavedQueries.ExportFileName);
            FileStream fs = new FileStream(masterFilePath, FileMode.Open);
            XmlDocument masterDoc = new XmlDocument();
            masterDoc.Load(fs);
            XmlNodeList masterElements = masterDoc.SelectNodes("/SuperMemoCollection/SuperMemoElement");
            int masterElementsCount = masterElements.Count;

            // Update the count
            int newCount = masterElementsCount + appendElementsCount; 
            masterDoc.SelectSingleNode("/SuperMemoCollection/Count").InnerText = newCount.ToString();

            foreach (XmlNode appendElement in appendElements) {
                masterDoc.DocumentElement.AppendChild(masterDoc.ImportNode(appendElement, true));
            }
            fs.Close();
            masterDoc.Save(masterFilePath);
            
            return appendDoc;
        }

        private static (XmlDocument, FileStream) getXmlDocument(string filePath) {
            if (!File.Exists(filePath)) {
                Console.WriteLine($"ERROR: Trying append what is in {filePath}.");
                Console.WriteLine($"But {filePath} does not exist.");
            } else {
                Trace.WriteLine($"Appending {filePath} to {SavedQueries.ExportFileName}.");
            }
            XmlDocument appendDoc = new XmlDocument();
            FileStream fs = new FileStream(filePath, FileMode.Open);
            appendDoc.Load(fs);
            return (appendDoc, fs);
        }

    }
}