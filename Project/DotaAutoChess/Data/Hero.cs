using DotaAutoChess.Data;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaAutoChess.DataClass
{

    public class HeroForListBox
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }


        public class Hero
    {
        public string Name;
        public int Cost;
        internal List<Specialization> Specializations { get; set; }
        //abillity
        //stat-lvl

        public Hero()
        {
            Specializations = new List<Specialization>();
        }
        public Hero(string name, string cost, string specializations)
        {
            this.Name = name;
            this.Cost = int.Parse(cost);

            Specializations = new List<Specialization>();
            List<string> strSpecializationsList = specializations.Split(',').ToList();
            foreach (string strSpecializations in strSpecializationsList)
            {
                Specializations.Add(new Specialization(strSpecializations));
            }
          
        }


        public static string HerosToCsv(List<Hero> heroes)
        {
            List<string> fields = new List<string>();
            fields.Add("Name");
            fields.Add("Cost");
            fields.Add("Specializations");

            string separator = ";";
            string header = String.Join(separator, fields);

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header+separator);


            foreach (var Hero in heroes)
            {
                StringBuilder linie = new StringBuilder();
                linie.Append(Hero.Name);
                linie.Append(separator);
                linie.Append(Hero.Cost.ToString());
                linie.Append(separator);
                linie.Append(String.Join(",", Hero.Specializations));
                linie.Append(separator);
                csvdata.AppendLine(linie.ToString());
            }

            return csvdata.ToString();

        }

        public static List<Hero> LoadFromCsv(string dataFileCsv)
        {
            List<Hero> allHeroesList = new List<Hero>();

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
                    string Cost = fields[1];
                    string Specializations = fields[2];

                    allHeroesList.Add(new Hero(Name, Cost, Specializations));
                }
            }

            return allHeroesList;
        }


        public static Hero GetHeroByName( List<Hero> allHeroes,string heroName)
        {
          foreach (Hero hero in allHeroes)
          {
              if (hero.Name == heroName)
              {
                  return hero;
              }
          }

          throw new Exception("Hero not found");
        }


        public static bool HeroGotSpec (Hero hero, string specializationStr)
        {
            foreach (Specialization specialization in hero.Specializations)
            {
                if (specialization.Name == specializationStr)
                    return true;
            }
            return false;
        }

        //private void LoadToCsvTest()
        //{
        //    string dataFilePath = ConfigurationManager.AppSettings["datafile"];

        //    List<Hero> heroesList = new List<Hero>();

        //    Hero hero = new Hero();
        //    hero.Name = "Abaddon";
        //    hero.Cost = 3;
        //    hero.Specializations.Add(Specialization.Undead);
        //    hero.Specializations.Add(Specialization.Knight);
        //    heroesList.Add(hero);

        //    hero = new Hero();
        //    hero.Name = "Alchemist";
        //    hero.Cost = 4;
        //    hero.Specializations.Add(Specialization.Goblin);
        //    hero.Specializations.Add(Specialization.Warlock);
        //    heroesList.Add(hero);

        //    hero = new Hero();
        //    hero.Name = "DrowRanger";
        //    hero.Cost = 1;
        //    hero.Specializations.Add(Specialization.Undead);
        //    hero.Specializations.Add(Specialization.Hunter);
        //    heroesList.Add(hero);

        //    //FileContent = CsvTools.ToCsv(";", heroesList);
        //    string FileContent = Hero.HerosToCsv(heroesList);
        //    File.WriteAllText(dataFilePath, FileContent);

        //}


    }
}
