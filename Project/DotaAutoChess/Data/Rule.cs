using DotaAutoChess.DataClass;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace DotaAutoChess.Data
{
    public enum BonusCondition { EqualOrMore, Equal, Less, Solo }
    //Todo...
    public enum ComninationCondition { Or, And , OnlyCombination}


    public class Rule
    {

        public string RuleBonus;
        public List<HeroCobmination> HeroCobminations;
        public ComninationCondition ComninationCondition;

        public Rule(string RuleBonus, List<HeroCobmination> HeroCobminations)
        {
            ComninationCondition = ComninationCondition.OnlyCombination;
            this.RuleBonus = RuleBonus;
            this.HeroCobminations = HeroCobminations;
        }
        public Rule(string RuleBonus, List<HeroCobmination> HeroCobminations, string ComninationCondition)
        {
            this.ComninationCondition = (ComninationCondition)Enum.Parse(typeof(ComninationCondition), ComninationCondition);
            this.RuleBonus = RuleBonus;
            this.HeroCobminations = HeroCobminations;
        }

        public static List<Rule> LoadFromCsv(string dataFileCsv)
        {
            List<Rule> allRulesList = new List<Rule>();

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

                    string ruleBonus = fields[0];
                    List<HeroCobmination> heroCobminations = new List<HeroCobmination>();
                    string comninationCondition  = "OnlyCombination";
                    if (fields.Length > 2)
                    {

                        for (int i = 1; i < fields.Length - 1; i++)
                        {
                            string heroCobminationString = fields[i];
                            string[] heroCobminationFields = heroCobminationString.Split(new char[] { ',' });
                            HeroCobmination heroCobmination = new HeroCobmination(heroCobminationFields[0], heroCobminationFields[1], heroCobminationFields[2]);
                            heroCobminations.Add(heroCobmination);
                        }
                        comninationCondition = fields[fields.Length - 1];
                    }
                    else
                    {
                        string heroCobminationString = fields[1];
                        string[] heroCobminationFields = heroCobminationString.Split(new char[] { ',' });
                        HeroCobmination heroCobmination = new HeroCobmination(heroCobminationFields[0], heroCobminationFields[1], heroCobminationFields[2]);
                        heroCobminations.Add(heroCobmination);
                    }

                    allRulesList.Add(new Rule(ruleBonus, heroCobminations, comninationCondition));

                }
            }

            return allRulesList;
        }

        public static bool IsSameSpecRule(Rule rule1, Rule rule2)
        {
            if (rule1.HeroCobminations.Count != rule2.HeroCobminations.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < rule1.HeroCobminations.Count; i++)
                {
                    if (rule1.HeroCobminations[i].Specialization.Name != rule2.HeroCobminations[i].Specialization.Name)
                        return false;

                    if (rule1.HeroCobminations[i].Condition != rule2.HeroCobminations[i].Condition)
                        return false;

                }
                return true;
            }
        }


        public static string GetSpecializationForHint(List<Rule> allRules, string specialization)
        {

            foreach (Rule rule in allRules)
            {
                foreach (HeroCobmination heroCobmination in rule.HeroCobminations)
                {
                    if (heroCobmination.Specialization.Name == specialization)
                    {
                        return "For "+ heroCobmination.NumberOfHeroes +" "+ specialization +" - " + rule.RuleBonus;
                    }
                }

            }

            return "";
        }

        //todo
            public static bool RuleWorks(Rule rule, Hero[] setupHeroArray)
        {
            //Delete Same Heroes (Same heroes dont stack)
            Hero[] processingSetupHeroList = new Hero[10];
            for (int i = 0; i < setupHeroArray.Length; i++)
            {
                Hero iHero = setupHeroArray[i];
                int pos = Array.IndexOf(processingSetupHeroList, iHero);

                //If not added
                if (!(pos > -1))
                {
                    processingSetupHeroList[i] = iHero;
                }
            }

            if (rule.ComninationCondition == ComninationCondition.OnlyCombination || rule.ComninationCondition == ComninationCondition.And)
            {
                foreach (HeroCobmination heroCobminations in rule.HeroCobminations)
                {
                    if (!HeroCombinationWorks(heroCobminations, processingSetupHeroList))
                    {
                        return false;
                    }
                }
            }

            if (rule.ComninationCondition == ComninationCondition.Or )
            {
                foreach (HeroCobmination heroCobminations in rule.HeroCobminations)
                {
                    if (HeroCombinationWorks(heroCobminations, processingSetupHeroList))
                    {
                        return true;
                    }
                }
                return false;
            }



            return true;
        }

        public static bool HeroCombinationWorks(HeroCobmination heroCobmination, Hero[] setupHeroList)
        {

            int countFromSetupList = 0;
            foreach (Hero hero in setupHeroList)
            {
                if (hero != null)
                {
                    foreach (Specialization specialization in hero.Specializations)
                    {
                        if (specialization.Name == heroCobmination.Specialization.Name)
                        {
                            countFromSetupList++;
                        }
                    }
                }

            }

            if (heroCobmination.Condition == BonusCondition.EqualOrMore)
            {
                if (countFromSetupList >= heroCobmination.NumberOfHeroes)
                {
                    return true;
                }
            }

            if (heroCobmination.Condition == BonusCondition.Equal)
            {
                if (countFromSetupList == heroCobmination.NumberOfHeroes)
                {
                    return true;
                }
            }


            return false;
        }




    }




    public class HeroCobmination
    {
        public BonusCondition Condition { get; set; }
        public int NumberOfHeroes { get; set; }
        public Specialization Specialization { get; set; }

        public HeroCobmination(string bonusCondition, string numberOfHeroes, string specialization)
        {
            BonusCondition curCondition;
            Enum.TryParse(bonusCondition, out curCondition);
            this.Condition = curCondition;
            this.NumberOfHeroes = int.Parse(numberOfHeroes);
            this.Specialization = new Specialization(specialization);
        }

    }






}
