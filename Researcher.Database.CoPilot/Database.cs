using System.Globalization;
using System.Text;

namespace Researcher.Insights;

public static class Database
{
    public record struct ChatProcessed(DateTime Timestamp, string Country);

    public static List<ChatProcessed> Records;

    public static void LoadRecords()
    {
         Records = Deserialize("Database.csv");
    }

    public static List<ChatProcessed> Deserialize(string csvFilePath)
    {
        var records = new List<ChatProcessed>();
        var lines = File.ReadAllLines(csvFilePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Timestamp"))
                continue;

            var fields = SplitCsvLine(line);
            var timestamp = DateTime.ParseExact(fields[0].Trim('"'), "MM/dd/yyyy, h.mm.ss.fff tt", CultureInfo.InvariantCulture);
            var country = fields[1].Trim('"');

            records.Add(new ChatProcessed { Timestamp = timestamp, Country = country });
        }

        return records;
    }

    public static void Serialize(string csvFilePath, List<ChatProcessed> records)
    {
        using (var writer = new StreamWriter(csvFilePath))
        {
            writer.WriteLine("Timestamp,Countries");

            foreach (var record in records)
            {
                var timestamp = record.Timestamp.ToString("MM/dd/yyyy, h.mm.ss.fff tt", CultureInfo.InvariantCulture);
                writer.WriteLine($"\"{timestamp}\",\"{record.Country}\"");
            }
        }
    }

    private static string[] SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;

        foreach (var ch in line)
        {
            if (ch == '"' && !inQuotes)
            {
                inQuotes = true;
            }
            else if (ch == '"' && inQuotes)
            {
                inQuotes = false;
            }
            else if (ch == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(ch);
            }
        }

        fields.Add(currentField.ToString());
        return fields.ToArray();
    }
}
