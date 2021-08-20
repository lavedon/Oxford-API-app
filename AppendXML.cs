using System;
using System.Xml;
using System.Text;
using System.IO;

namespace oed {
    public static class AppendXML{
        
        public static void Append(string filePath)
        {
        }

        public static void Append(string filePath, CurrentQuery query) 
        {
        }

        private static XmlDocument getXmlDocument(string filePath) {
            if (!File.Exists(filePath)) {
                Console.WriteLine($"Trying to append {filePath}.");
                Console.WriteLine($"But {filePath} does not exist.");
            } else {

            }
            XmlDocument xmlDoc = new XmlDocument();
            FileStream fs = new FileStream(filePath, FileMode.Open);
            xmlDoc.Load(fs);
            return xmlDoc;
        }
    }
}