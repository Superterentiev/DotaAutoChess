using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DotaAutoChess.Data
{

    public enum ColorConditionName
    {
        Black,
        //BonusGrid -> if rule Works 
        BonusRuleWorking,
        //BonusGrid -> if rule DOESNT Work 
        BonusRuleNotWorkingAtAll,
        //BonusGrid -> if rule can soon work 
        BonusRuleNearlyWorkingAtAll,
        //BonusGrid -> if rule can soon work 
        FilterLabel,
        //Hint about double clicking
        HintForDgAllHeroes,
        //Background for SetUp AND Header
        SetUpBackground,
        //SetUp Header color
        SetUpHeader,
        //Column setup headers
        SetUpColumnsHeader,
        //InfoBlock
        InformationText,
        //Bonus headers
        BonusHeader,

        //HeroesNCosts
        Cost1,
        Cost2,
        Cost3,
        Cost4,
        Cost5,
        Beast,
        Goblin,
        Undead,
        Mage,
        Demon,
        DemonHunter,
        Dwarf,
        Human,
        Assassin,
        Mech,
        Dragon,
        Naga,
        Druid,
        Shaman,
        Element,
        Ogre,
        Orc,
        Hunter,
        Warlock,
        Elf,
        Troll,
        Knight,
        Warrior

    }


    public class DacColor
    {
        public ColorConditionName ConditionName { get; set; }
        public string Hex { get; set; }
        public SolidColorBrush Brush { get; set; }

        public DacColor(string ConditionName, string Hex)
        {
            this.ConditionName = (ColorConditionName)Enum.Parse(typeof(ColorConditionName), ConditionName);
            this.Hex = Hex;
            this.Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(Hex)); //"#ffaacc"
        }

        public static DacColor GetColorByEnumCondition(List<DacColor> allColorList, ColorConditionName ConditionName)
        {
            foreach (DacColor color in allColorList)
            {
                if (color.ConditionName == ConditionName)
                    return color;
            }
            return null;
        }

        public static List<DacColor> LoadFromCsv(string dataFileCsv)
        {
            List<DacColor> allColorsList = new List<DacColor>();

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
                    string conditionName = fields[0];
                    string hex = fields[1];
                    allColorsList.Add(new DacColor(conditionName, hex));
                }
            }

            return allColorsList;
        }


    }
}
