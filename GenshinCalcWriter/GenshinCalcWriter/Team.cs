using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using GenshinTeamCalc;
using System.Text;
using System.Threading.Tasks;

namespace GenshinTeamCalc
{
    public class Team
    {
        public string name;
        public string server;
        public List<Character> characters = new List<Character>();
        public double rotlen;
        public double dps;
        public double dpm;
        public double dpmAoe;
        public double som;

        /*
        public Team(string[,] stuff, Calc calc) // from DB
        {
            name = stuff[0, 0];
            rotlen = double.Parse(stuff[1, 0], CultureInfo.InvariantCulture);
            server = stuff[2, 0];
            for (int i = 1, j = 0; j < 4; i++, j++)
            {
                string temp = stuff[0, i];
                if (!calc.CharacterExists(temp))
                {
                    //i--; // skips empty or ignored characters
                    Character tempchar = new Character("no");
                    tempchar.dmg = 0;
                    tempchar.aoe = 1;
                    characters.Add(tempchar);
                }
                else
                {
                    Character tempchar = new Character(temp);
                    if (stuff[1, i].Contains("!")) // for relative damage
                    {
                        stuff[1, i] = stuff[1, i].Replace("!", "");
                        if (stuff[2, i] == null) // no offset multiplier
                            tempchar.dmg = calc.GetDmg(tempchar, int.Parse(stuff[1, i]));
                        else // with offset multiplier
                            tempchar.dmg = Math.Round(calc.GetDmg(tempchar, int.Parse(stuff[1, i])) * double.Parse(stuff[2, i], CultureInfo.InvariantCulture));
                    }
                    else // for raw damage
                    {
                        if (stuff[2, i] == null) // no offset multiplier
                            tempchar.dmg = Math.Round(double.Parse(stuff[1, i], CultureInfo.InvariantCulture));
                        else // with offset multiplier
                            tempchar.dmg = Math.Round(double.Parse(stuff[1, i], CultureInfo.InvariantCulture) * double.Parse(stuff[2, i], CultureInfo.InvariantCulture));
                    }
                    if (stuff[3, i] == null) // no AOE override
                    {
                        tempchar.aoe = calc.GetAoe(tempchar);
                    }
                    else // AOE override
                    {
                        tempchar.aoe = double.Parse(stuff[3, i], CultureInfo.InvariantCulture);
                    }
                    calc.AddCosmetic(tempchar);
                    characters.Add(tempchar);
                }
            }
            CalcDpm();
        }
        */

        public Team(string[,] stuff) // from DB
        {
            name = stuff[0, 0];
            rotlen = double.Parse(stuff[1, 0], CultureInfo.InvariantCulture);
            server = stuff[2, 0];
            for (int i = 1, j = 0; j < 4; i++, j++)
            {
                string temp = stuff[0, i];
                if (!Calc.CharacterExists(temp))
                {
                    //i--; // skips empty or ignored characters
                    Character tempchar = new Character("no");
                    tempchar.dmg = 0;
                    tempchar.aoe = 1;
                    characters.Add(tempchar);
                }
                else
                {
                    Character tempchar = new Character(temp);
                    if (stuff[1, i].Contains("!")) // for relative damage
                    {
                        stuff[1, i] = stuff[1, i].Replace("!", "");
                        if (stuff[2, i] == null) // no offset multiplier
                            tempchar.dmg = Calc.GetDmg(tempchar, int.Parse(stuff[1, i]));
                        else // with offset multiplier
                            tempchar.dmg = Math.Round(Calc.GetDmg(tempchar, int.Parse(stuff[1, i])) * double.Parse(stuff[2, i], CultureInfo.InvariantCulture));
                    }
                    else // for raw damage
                    {
                        if (stuff[2, i] == null) // no offset multiplier
                            tempchar.dmg = Math.Round(double.Parse(stuff[1, i], CultureInfo.InvariantCulture));
                        else // with offset multiplier
                            tempchar.dmg = Math.Round(double.Parse(stuff[1, i], CultureInfo.InvariantCulture) * double.Parse(stuff[2, i], CultureInfo.InvariantCulture));
                    }
                    if (stuff[3, i] == null) // no AOE override
                    {
                        tempchar.aoe = Calc.GetAoe(tempchar);
                    }
                    else // AOE override
                    {
                        tempchar.aoe = double.Parse(stuff[3, i], CultureInfo.InvariantCulture);
                    }
                    Calc.AddCosmetic(tempchar);
                    characters.Add(tempchar);
                }
            }
            CalcDpm();
        }
        public Team() // Template
        {
            name = "Team Name";
            server = "Default";
            rotlen = 20;
            for(int i = 0; i < 4; i++)
            {
                characters.Add(new Character("no"));
                characters[i].dmg = 1000;
            }
        } 

        public Team(Team team) // Dupe
        {
            name = team.name;
            server = team.server;
            rotlen = team.rotlen;
            foreach (Character character in team.characters)
            {
                characters.Add(new Character(character));
            }
            som = team.som;
            dpm = team.dpm;
            dpmAoe = team.dpmAoe;
            dps = team.dps;
        }

        /// <summary>
        /// Calculates the DPM and DPS of the team and stores it in the atributes.
        /// </summary>
        public void CalcDpm()
        {
            double som = 0;
            double somAoe = 0;
            for (int j = 0; j < characters.Count; j++)
            {
                som += characters[j].dmg;
                somAoe += characters[j].dmg * characters[j].aoe;
            }
            this.som = Math.Round(som);
            dps = Math.Round((som / rotlen), 2);
            dpm = Math.Round((som * (60 / rotlen)) / 1000000, 2);
            dpmAoe = Math.Round((somAoe * (60 / rotlen)) / 1000000, 2);
        }
        public void WriteInfo()
        {
            Console.WriteLine(name);
            Console.WriteLine(server);
            Console.WriteLine(rotlen);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine($"{characters[i].name}: {characters[i].dmg} ({characters[i].dmg * characters[i].aoe})");
            }
        }

        /// <summary>
        /// Returns a list of the percentages of the damage each character does in the team, with the same order.
        /// </summary>
        public List<double> GetPercentages()
        {
            List<double> result = new List<double>();
            foreach (Character character in characters)
            {
                result.Add(Math.Round((character.dmg / som * 100), 2));
            }
            return result;
        }
        
    }

    public class TeamCondensed
    {
        public Team team;
        public string innername;
        public bool active;
        public string[] text;
        public int pos;
        public double legacy;

        public TeamCondensed(Team team, string[] text, int pos)
        {
            this.team = team;
            innername = team.name;
            active = IsActive(team.name);
            this.text = text;
            text[0] = text[0].Replace("//", "");
            this.pos = pos;
        }

        public TeamCondensed(Team team) // TEMPLATE
        {
            this.team = team;
            active = true;
            text = new string[5];
            text[0] = $"{team.name}-{team.rotlen}-{team.server}";
            for(int i = 0; i < 4; i++)
            {
                text[i + 1] = "no-1000";
            }
        }

        bool IsActive(string name)
        {
            bool result = !(name.StartsWith("//"));
            team.name = team.name.Replace("//", "");
            return result;
        }

        public void Update()
        {
            Debug.WriteLine("updating!");
            string[] temp = text[0].Split('-'); // Team info
            temp[0] = team.name;
            temp[1] = team.rotlen.ToString();
            temp[2] = team.server;
            text[0] = temp[0];
            for (int j = 1; j < temp.Length; j++)
            {
                text[0] += $"-{temp[j]}";
            }
            for (int i = 1; i < text.Length; i++) // each character
            {
                List<string> temp2 = text[i].Split('-').ToList();
                temp2[0] = team.characters[i - 1].name;
                if (Calc.CharacterExists(temp2[0]))
                {
                    if (temp2[1].Contains("!")) // uses relative damage
                    {
                        int index = int.Parse(temp2[1].Replace("!", ""));
                        double mult = team.characters[i - 1].dmg / Calc.GetDmg(team.characters[i - 1], index);
                        Debug.WriteLine($"ITS RELATIVE {team.characters[i - 1].dmg} / {Calc.GetDmg(team.characters[i - 1], index)} {mult}");
                        if (temp2.Count <= 2) // no offset multiplier
                        {
                            if (mult != 1)
                            {
                                temp2.Add(mult.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                        else // with offset multiplier
                        {
                            temp2[2] = mult.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else // uses raw damage
                    {
                        temp2[1] = team.characters[i - 1].dmg.ToString();
                        //Debug.WriteLine($"ITS RAW {temp2[2]}");
                        try
                        {
                            temp2[2] = "1";
                        }
                        catch { }
                        //Debug.WriteLine($"ITS RAW {temp2[2]}");
                    }
                    if (temp2.Count >= 4) // AOE override
                    {
                        temp2[3] = team.characters[i - 1].aoe.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (team.characters[i - 1].aoe != Calc.GetAoe(team.characters[i - 1]))
                        {
                            if (temp2.Count == 2)
                            {
                                temp2.Add("1");
                            }
                            temp2.Add(team.characters[i - 1].aoe.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    text[i] = temp2[0];
                    for (int j = 1; j < temp2.Count; j++)
                    {
                        text[i] += $"-{temp2[j]}";
                    }
                }
            }
        }
    }

    public class Character
    {
        public Team Team;
        public string name;
        public string element;
        public string img;
        private double _dmg;
        public double aoe;
        public int relative = -1;

        public double dmg
        {
            get { return _dmg; }
            set
            {
                if (value < 0)
                {
                    Debug.WriteLine(name + " has invalid damage in some team!");
                    _dmg = 0;
                }
                else
                {
                    _dmg = value;
                }
            }
        }

        public double dmgAoe         
        {
            get { return _dmg * aoe; }
        }

        public Character(string name, string element, string img, double aoe)
        {
            this.name = name;
            this.element = element;
            this.img = img;
            this.aoe = aoe;
        }
        public Character(string name)
        {
            this.name = name;
            element = "element";
            img = "link";
            aoe = 1;
        }

        public Character(Character dupe)
        {
            name = dupe.name;
            element = dupe.element;
            img = dupe.img;
            aoe = dupe.aoe;
            dmg = dupe.dmg;
            relative = dupe.relative;
        }
    }

public class CharacterCondensed
{
    public Character character;
    public List<string> damages;
    public string innername;
    public List<string> text;

    public CharacterCondensed(Character character, List<string> damages, string innername, List<string> text)
    {
        this.character = character;
        this.damages = damages;
        this.innername = innername;
        this.text = text;
    }

    public CharacterCondensed(Character character) // TEMPLATE
    {
        this.character = character;
        text = new List<string>();
        damages = new List<string>();
        text.Add(character.name);
        text.Add(character.element);
        if (character.aoe == null)
        {
            text.Add("1");
        }
        else
        {
            text.Add(character.aoe.ToString());
        }
        text.Add(character.img);
    }
    public void Update()
    {
        text[0] = character.name;
        text[1] = character.element;
        for (int i = 2; i < damages.Count + 2; i++)
        {
            if (damages.Count > text.Count - 4)
            {
                //if (damages[i - 2].Length <= 1) // to not conflict with relative
                    //text.Insert(i, damages[i - 2] + "0");
                text.Insert(i, damages[i - 2]);
            }
            else
            {
                if (damages[i - 2] == "")
                {
                    text.RemoveAt(i);
                    damages.RemoveAt(i - 2);
                }
                else
                {
                    //if (damages[i - 2].Length <= 1) // to not conflict with relative
                        //text[i] = damages[i - 2] + "0";
                    text[i] = damages[i - 2];
                }
            }
        }
        text[text.Count - 2] = character.aoe.ToString(CultureInfo.InvariantCulture);
        text[text.Count - 1] = character.img;
    }
}

