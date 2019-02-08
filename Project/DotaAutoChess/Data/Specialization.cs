using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace DotaAutoChess.Data
{
    public class Specialization
    {
        public string Name { get; set; }
        public Specialization (string Name)
        {
            this.Name = Name;
        }

        public static List<Specialization> LoadFromCsv(string dataFileCsv)
        {
            List<Specialization> allSpecializationsList = new List<Specialization>();

            using (TextFieldParser csvParser = new TextFieldParser(dataFileCsv))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    string Name = fields[0];
                    allSpecializationsList.Add(new Specialization(Name));
                }
            }

            return allSpecializationsList;
        }


    }
}
