using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Text;
public class GameUtility
{

    public const float ResolutionDelayTime = 1;
    public const string SavePrefKey = "Level_";

    public const string FileName = "Questions_level";
    public static string FileDir
    {
        get
        {
            return Application.dataPath + "/Resources/";
        }
    }
}
[System.Serializable()]
public class Data
{
    public Question[] Questions = new Question[0];
    public int Level = 0;

    public Data() { }

    public static void Write(Data data, string path)
    {
        if (File.Exists(path))
            File.Delete(path);
        XmlSerializer serializer = new XmlSerializer(typeof(Data));
        var encoding = Encoding.GetEncoding("UTF-8");
        // using (Stream stream = new FileStream(path, FileMode.Create))
        using (TextWriter stream = new StreamWriter(path, true, System.Text.Encoding.UTF8))
        {
            serializer.Serialize(stream, data);
        }
    }
    public static Data Fetch(string filePath)
    {
        return Fetch(out bool result, filePath);
    }
    public static Data Fetch(out bool result, string filePath)
    {
        if (!File.Exists(filePath)) { result = false; return new Data(); }

        XmlSerializer deserializer = new XmlSerializer(typeof(Data));
        using (TextReader stream = new StreamReader(filePath, System.Text.Encoding.UTF8))
        {
            var data = (Data)deserializer.Deserialize(stream);

            result = true;
            return data;
        }
    }
}