using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace GenshinTeamCalc
{
    public static class Calc
    {
        public static List<Team> teams = new List<Team>();
        static Dictionary<string, List<double>> DB = new Dictionary<string, List<double>>(); // dmg, dmg, dmg, dmg, aoe
        static Dictionary<string, string[]> CosmeticDB = new Dictionary<string, string[]>(); // element, img
        public static string dmgType;
        public static string TeamDBlocation;
        public static string DBlocation;
        public static string OUTlocation;
        public static string configlocation;
        static string desktoppath;

        public static List<Team>[] TeamRank(List<Team> teams)
        {
            SortedDictionary<double, Team> Ordered = new SortedDictionary<double, Team>();
            SortedDictionary<double, Team> OrderedAoe = new SortedDictionary<double, Team>();
            foreach (Team team in teams)
            {
                bool needsfix = true;
                double dmg = team.dpm;
                while (needsfix)
                {
                    try
                    {
                        Ordered.Add(dmg, team);
                        needsfix = false;
                    }
                    catch
                    {
                        dmg -= 0.001;
                    }
                }
                dmg = team.dpmAoe;
                needsfix = true;
                while (needsfix)
                {
                    try
                    {
                        OrderedAoe.Add(dmg, team);
                        needsfix = false;
                    }
                    catch
                    {
                        dmg -= 0.001;
                    }
                }
            }
            List<Team>[] order = new List<Team>[2];
            order[0] = new List<Team>(); order[1] = new List<Team>();
            foreach (KeyValuePair<double, Team> kv in Ordered.Reverse())
            {
                order[0].Add(kv.Value);
            }
            foreach (KeyValuePair<double, Team> kv in OrderedAoe.Reverse())
            {
                order[1].Add(kv.Value);
            }
            return order;
        }

        /// <summary>
        /// Returns amount of unique accounts or servers registered in the database
        /// </summary>
        public static int GetAcountAmount(List<Team> teams)
        {
            Dictionary<string, int> servers = new Dictionary<string, int>();
            foreach (Team team in teams)
            {
                if (!servers.ContainsKey(team.server))
                {
                    servers.Add(team.server, 1);
                }
            }
            return servers.Count;
        }

        /// <summary>
        /// Loads all the active teams from the TeamDB file, and stores those teams in the teams list
        /// </summary>
        public static int Teams()
        {
            StreamReader r = new StreamReader(TeamDBlocation);
            if (r.Peek() <= -1)
            {
                return 0;
            }
            string temp = "";
            string full = "";
            while (temp != null)
            {
                temp = r.ReadLine();
                full += temp;
            }
            r.Close();
            string[] teaminfo = full.Split(',');
            int teamamount = 0;

            for (int i = 0; i < teaminfo.Length; i++)
            {
                string[,] everything = new string[4, 5]; // name, char1, char2, char3, char4
                                                         // rot,  index, index, index, index
                                                         // serv, mult,   mult, mult,  mult
                                                         // pos,  aoeO,  aoeO,  aoeO,  aoeO
                if (teaminfo[i].StartsWith("//"))
                {
                    // skip team
                }
                else
                {
                    string[] teaminfo2 = teaminfo[i].Split(';');
                    for (int j = 0; j < teaminfo2.Length; j++)
                    {
                        string[] uhh = teaminfo2[j].Split('-');
                        for (int k = 0; k < uhh.Length; k++)
                        {
                            everything[k, j] = uhh[k];
                        }
                    }
                    Team tempteam = new Team(everything);
                    teams.Add(tempteam);
                    teamamount++;
                }
            }
            Debug.WriteLine("Teams loaded!");
            return teamamount;
        }

        /// <summary>
        /// Loads all teams from the TeamDB file, even if inactive, stores them in the parameter list, and returns all the text from the file
        /// </summary>
        public static List<string[]> ALLTeams(List<Team> result)
        {

            if (result == null)
            {
                throw new Exception("Team List reference is not instantialized!");
            }
            StreamReader r = new StreamReader(TeamDBlocation);
            List<string[]> resultraw = new List<string[]>();
            if (r.Peek() <= -1)
            {
                return new List<string[]>();
            }
            string temp = "";
            string full = "";
            while (temp != null)
            {
                temp = r.ReadLine();
                full += temp;
            }
            r.Close();
            string[] teaminfo = full.Split(',');
            int teamamount = 0;
            for (int i = 0; i < teaminfo.Length; i++)
            {

                string[,] everything = new string[4, 5]; // name, char1, char2, char3, char4
                                                         // rot,  index, index, index, index
                                                         // serv, mult,   mult, mult,  mult
                                                         // pos,  aoeO,  aoeO,  aoeO,  aoeO
                string[] teaminfo2 = teaminfo[i].Split(';');
                resultraw.Add(teaminfo2);
                for (int j = 0; j < teaminfo2.Length; j++)
                {
                    string[] uhh = teaminfo2[j].Split('-');
                    for (int k = 0; k < uhh.Length; k++)
                    {
                        everything[k, j] = uhh[k];
                    }
                }
                Team tempteam = new Team(everything);
                result.Add(tempteam);
                teamamount++;
            }
            return resultraw;
        }

        /// <summary>
        /// Returns a list of all the characters in the DB.
        /// </summary>
        public static List<Character> GetAllCharacters()
        {
            List<Character> result = new List<Character>();
            foreach (KeyValuePair<string, List<double>> kv in DB)
            {
                Character temp = new Character(kv.Key, CosmeticDB[kv.Key][0], CosmeticDB[kv.Key][1], kv.Value[kv.Value.Count - 1]);
                temp.relative = -1;
                result.Add(temp);
            }
            return result;
        }

        /// <summary>
        /// Returns a dictionary of all the characters in the DB, with their name as the key. Used for GUI.
        /// </summary>
        public static Dictionary<string, CharacterCondensed> GetAllCharactersCondensed()
        {
            Dictionary<string, CharacterCondensed> result = new Dictionary<string, CharacterCondensed>();
            StreamReader r = new StreamReader(DBlocation);
            if (r.Peek() <= -1)
            {
                return new Dictionary<string, CharacterCondensed>();
            }
            string temp = "";
            string full = "";
            while (temp != null)
            {
                temp = r.ReadLine();
                full += temp;
            }
            r.Close();
            string[] characters = full.Split(';');
            foreach (string character in characters)
            {
                //Debug.WriteLine(character);
                List<string> text = character.Replace(" ", "").Split(',').ToList();
                List<string> damages = new List<string>();
                for (int i = 2; i < text.Count - 2; i++)
                {
                    damages.Add(text[i]);
                }
                result.Add(text[0], new CharacterCondensed(
                    new Character(text[0], text[1], text[text.Count - 1], (double.Parse(text[text.Count - 2], CultureInfo.InvariantCulture))),
                    damages, text[0], text));
            }
            return result;
        }

        /// <summary>
        /// Returns the relative damage of a character from an index, from the DB.
        /// </summary>
        public static double GetDmg(Character character, int v)
        {
            try
            {
                if (DB[character.name][v] <= 0)
                {
                    throw new Exception($"Yo {character.name} has a null dmg in variant {v} in some team!!!\n");
                }
                character.relative = v;
                return DB[character.name][v];
            }
            catch
            {
                throw new Exception($"Hey! it seems {character.name} is NOT in the DB! or doesn't have dmg in pos {v}\n");
            }
        }

        /// <summary>
        /// Returns the Aoe coefficient of a character, from the DB.
        /// </summary>
        public static double GetAoe(Character character) /// Gets Aoe from the Dictionary stored DB
        {
            try
            {
                try
                {
                    return DB[character.name][DB[character.name].Count - 1];
                }
                catch
                {
                    throw new Exception($"{character.name} has an invalid amount of damage values in DB!");
                }
            }
            catch
            {
                throw new Exception($"Hey! {character.name} is NOT in the DB or has an invalid value!\n");
            }
        }

        /// <summary>
        /// Applies the cosmetic atributes of the character class, for visuals in GUI.
        /// </summary>
        public static void AddCosmetic(Character character)
        {
            try
            {
                try
                {
                    character.element = CosmeticDB[character.name][0];
                    character.img = CosmeticDB[character.name][1];
                }
                catch
                {
                    throw new Exception($"{character.name} has an invalid amount of damage values in DB!");
                }
            }
            catch
            {
                throw new Exception($"Hey! {character.name} is NOT in the DB or has an invalid value!\n");
            }
        }

        /// <summary>
        /// Loads all data from database files, configuration files, and stores them in the DB dictionary (for relative damages and aoe), and the CosmeticDB dictionary (element and icon).
        /// </summary>
        public static void LoadStuff(bool auto)
        {
            desktoppath = AppDomain.CurrentDomain.BaseDirectory + @"DB\";//AppContext.BaseDirectory;//.Replace(" ", "");
            Directory.CreateDirectory(desktoppath);
            string cfg;
            string folder = "";
            if (auto)
            {
                folder = desktoppath;
            }
            try
            {
                cfg = File.ReadAllText($@"{folder}config.cfg");
                configlocation = $@"{folder}config.cfg";
            }
            catch
            {
                if (auto)
                {
                    StreamWriter writer = new StreamWriter($@"{desktoppath}config.cfg", false);
                    writer.WriteLine($"DB;{desktoppath}db.txt;\nTeamDB;{desktoppath}teams.txt;\nOutput;{desktoppath}log.txt;\ndmgtype;dps");
                    configlocation = $@"{desktoppath}config.cfg";
                    if (!File.Exists(desktoppath + "db.txt"))
                    {
                        Debug.WriteLine("AY!");
                        File.WriteAllText(desktoppath + "db.txt", GetTemplate());
                    }
                    if (!File.Exists(desktoppath + "teams.txt"))
                    {
                        //File.Create(desktoppath + "teams.txt");
                        File.WriteAllText(desktoppath + "teams.txt", "");
                    }
                    //File.WriteAllText($"{desktoppath}db.txt", GetTemplate());
                    writer.Close();
                }
                else
                {
                    throw new FileNotFoundException("Welcome to your first time!");
                }
                LoadStuff(auto);
                return;
            }
            cfg = cfg.Replace("\n", "").Replace("\r", "").Replace("\n\r", "");//.Replace(" ", "")
            string[] cfg2 = cfg.Split(';');
            if (cfg2.Length <= 7)
            {
                throw new FileNotFoundException("invalid config!");
            }
            DBlocation = cfg2[1];
            TeamDBlocation = cfg2[3];
            OUTlocation = cfg2[5];
            dmgType = cfg2[7]; // dps or dpm
            string all;
            try
            {
                all = File.ReadAllText(DBlocation);
            }
            catch (FileNotFoundException db)
            {
                throw new FileNotFoundException("The DB location is invalid!", db);
            }
            try
            {
                File.ReadAllText(TeamDBlocation);
            }
            catch (FileNotFoundException team)
            {
                throw new FileNotFoundException("The Team DB location is invalid!", team);
            }
            /*try
            {
                File.Exists(OUTlocation);
            }
            catch
            {
                OUTlocation = "output.txt";
            }*/
            all = all.Replace(" ", "");
            all = all.Replace("\t", "");
            string[] sep = all.Split(';');
            for (int i = 0; i < sep.Length; i++)
            {
                int start = sep[i].IndexOf('(');
                while (start >= 0)
                {

                    int end = sep[i].IndexOf(')');
                    sep[i] = sep[i].Remove(start, (end - start + 1));
                    start = sep[i].IndexOf('(');
                }
                string[] individual = sep[i].Split(',');
                individual[0] = individual[0].Replace("\r\n", "");
                if (individual.Length < 4 || individual.Length > 14) // 4 mandatory, max 10 relative damages
                {
                    throw new Exception($"{individual[0]} has invalid amount of data in the database");
                }
                List<double> stuff = new List<double>();
                for (int j = 2; j < individual.Length - 2; j++) // to store relative damage
                {
                    try
                    {
                        stuff.Add(double.Parse(individual[j]));
                    }
                    catch
                    {
                        Debug.WriteLine($"Information from {individual[0]} in the database has an error");
                        throw new Exception($"Information from {individual[0]} in the database has an error");
                    }
                }
                if (double.Parse(individual[individual.Length - 2], CultureInfo.InvariantCulture) <= 0)
                {
                    Debug.WriteLine(individual[0] + "'s Aoe is wrong!");
                    throw new Exception(individual[0] + "'s Aoe is wrong!");
                }
                else
                {
                    stuff.Add(double.Parse(individual[individual.Length - 2], CultureInfo.InvariantCulture));
                }
                DB.Add(individual[0], stuff);
                CosmeticDB.Add(individual[0], new string[] { individual[1], individual[individual.Length - 1] });
            }
            Teams();
        }
        public static Dictionary<string, SortedDictionary<double, Team[]>[]> AccountTeamRanker(List<Team> teams, Team special)// Teams, Team to use. RETURN: Returns dictionary based on server, with 3 sorted dictionaries of pairing ranked in each category
        {
            Dictionary<string, SortedDictionary<double, Team[]>[]> Everything = new Dictionary<string, SortedDictionary<double, Team[]>[]>(); // jesus christ..
            // Server, Array of Pairings (one for reach type of calc)
            List<List<Team>> accounts = AccountSeparate(teams, special);
            foreach (List<Team> account in accounts)
            {
                SortedDictionary<double, Team[]>[] temp = new SortedDictionary<double, Team[]>[3];
                for (int i = 0; i < 3; i++)
                {
                    temp[i] = TeamPairer(account, i, special);
                }
                Everything.Add(account[0].server, temp);
            }
            return Everything;
        }
        private static SortedDictionary<double, Team[]> TeamPairer(List<Team> teams, int type, Team special) // type 0- single target, type 1- Aoe, type 2- Average, full rank or nah
        {
            SortedDictionary<double, Team[]> result = new SortedDictionary<double, Team[]>();
            Dictionary<double, Team> joker = new Dictionary<double, Team>();
            bool needsfix;
            double specialdmg = -1;
            switch (type)
            {
                case 0:
                    if (special != null)
                        specialdmg = special.dpm;
                    foreach (Team team in teams)
                    {
                        needsfix = true;
                        double dmg = team.dpm;
                        while (needsfix)
                        {
                            try
                            {
                                joker.Add(dmg, team);
                                needsfix = false;
                            }
                            catch
                            {
                                dmg -= 0.001;
                            }
                        }
                    }
                    break;
                case 1:
                    if (special != null)
                        specialdmg = special.dpmAoe;
                    foreach (Team team in teams)
                    {
                        needsfix = true;
                        double dmg = team.dpmAoe;
                        while (needsfix)
                        {
                            try
                            {
                                joker.Add(dmg, team);
                                needsfix = false;
                            }
                            catch
                            {
                                dmg -= 0.001;
                            }
                        }
                    }
                    break;
                case 2:
                    if (special != null)
                        specialdmg = Math.Round((special.dpm + special.dpmAoe) / 2, 2);
                    foreach (Team team in teams)
                    {
                        needsfix = true;
                        double dmg = Math.Round((team.dpm + team.dpmAoe) / 2, 2);
                        while (needsfix)
                        {
                            try
                            {
                                joker.Add(dmg, team);
                                needsfix = false;
                            }
                            catch
                            {
                                dmg -= 0.001;
                            }
                        }
                    }
                    break;
            }
            if (special != null)
            {
                InnerPair(new KeyValuePair<double, Team>(specialdmg, special), joker, result, true); // checks only pairings of the specific team
            }
            else
            {
                foreach (KeyValuePair<double, Team> kv1 in joker) // checks every pair within the same account
                {
                    InnerPair(kv1, joker, result, false);
                }
            }
            return result;
        }
        private static void InnerPair(KeyValuePair<double, Team> kv1, Dictionary<double, Team> joker, SortedDictionary<double, Team[]> result, bool special)
        {
            bool startAdding;
            if (special) { startAdding = true; } else { startAdding = false; }
            foreach (KeyValuePair<double, Team> kv2 in joker)
            {
                if (!special)
                {
                    if (kv1.Equals(kv2))
                    {
                        startAdding = true;
                        continue;
                    }
                }
                else { startAdding = true; }
                if (startAdding)
                {
                    var kv1Characters = new HashSet<string>(kv1.Value.characters.Select(c => c.name));
                    bool conflict = kv2.Value.characters.Any(character2 => kv1Characters.Contains(character2.name));
                    if (!conflict)
                    {
                        double dmg = kv1.Key;
                        bool needsfix = true;
                        while (needsfix)
                        {
                            try
                            {
                                result.Add(dmg + kv2.Key, new Team[] { kv1.Value, kv2.Value });
                                needsfix = false;
                            }
                            catch
                            {
                                Team[] teamfix = new Team[] { kv2.Value, kv1.Value };
                                bool isDuplicate = result.Values.Any(existingArray =>
                                existingArray.SequenceEqual(teamfix));
                                if (isDuplicate)
                                {
                                    break;
                                }
                                dmg -= 0.001;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of lists of teams, separated by account.
        /// </summary>
        public static List<List<Team>> AccountSeparate(List<Team> teams, Team special)
        {
            List<List<Team>> accounts = new List<List<Team>>();
            if (special != null)
            {
                accounts.Add(new List<Team>());
                foreach (Team team in teams)
                {
                    if (team.server == special.server)
                    {
                        accounts[0].Add(team);
                    }
                }
            }
            else
            {
                foreach (Team team in teams)
                {
                    int i = 1;
                    foreach (List<Team> accountlist in accounts)
                    {
                        if (team.server == accountlist[0].server)
                        {
                            accountlist.Add(team);
                            break;
                        }
                        i++;
                    }
                    if (i > accounts.Count)
                    {
                        accounts.Add(new List<Team> { team });
                    }
                }
            }
            return accounts;
        }
        public static string NumberClarity(double number)
        {
            return number.ToString("N0");
        }

        /// <summary>
        /// Transforms Damage per Minute to Damage per Second.
        /// </summary>
        public static double ToDps(double dpm, double rotlen)
        {
            return ((dpm * 1000000) * (rotlen / 60)) / rotlen;
        }

        /// <summary>
        /// Checks if the character is registered in the database.
        /// </summary>
        public static bool CharacterExists(string name)
        {
            return DB.ContainsKey(name);
        }

        static string GetTemplate()
        {
            return "Amber, pyro,1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-1-12.png; \r\n Arlecchino, pyro,2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-32-8.png; \r\n Bennett, pyro,3.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-13.png; \r\n Chevreuse, pyro,1.20, https://cdn3.emoji.gg/emojis/2257-chevreuse-delicious.png; \r\n Dehya, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-5.png; \r\n Diluc, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-3.png; \r\n Klee, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-1.png; \r\n Gaming, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-1.png; \r\n Hutao, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-5-6.png; \r\n Lyney, pyro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-5.png; \r\n Mavuika, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-11.png; \r\n Pmc, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png; \r\n Thoma, pyro, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-10-11.png; \r\n Xiangling, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-16.png; \r\n Xinyan, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-3-11.png; \r\n Yanfei, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-8.png; \r\n Yoimiya, pyro, 1.10, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-5.png; \r\n Ayato, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-2.png; \r\n Barbara, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-11.png; \r\n Candace, hydro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-11.png; \r\n Childe,hydro, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-8.png; \r\n Furina, hydro, 1.75, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-6.png; \r\n Hmc, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png; \r\n Kokomi, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-8.png; \r\n Mona, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-12.png; \r\n Mualani, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-15.png; \r\n Neuvillette, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-15.png; \r\n Nilou, hydro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-1.png; \r\n Sigewinne, hydro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-8.png; \r\n Xingqiu, hydro,1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-16.png; \r\n Yelan, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-1.png; \r\n Amc, anemo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png; \r\n Chasca,anemo, 1.05, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-5.png; \r\n Faruzan, anemo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-16.png; \r\n Heizou, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-16-4.png; \r\n Ifa,anemo,2,https://static.icy-veins.com/images/genshin-impact/characters/ifa.webp; \r\n Jean, anemo, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-16.png; \r\n Kazuha,anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-8.png; \r\n Lynette, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-11.png; \r\n Sayu, anemo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-9.png; \r\n Sucrose, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-12.png; \r\n Venti, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-12.png; \r\n Wanderer, anemo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-3.png; \r\n Xianyun, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-12.png; \r\n Xiao, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-14.png; \r\n Yumemizuki,anemo,3,https://act-webstatic.hoyoverse.com/event-static-hoyowiki-admin/2025/02/08/11662c12fb353d88bd2b20a2726c9f1c_8013472600677983943.png; \r\n Beidou,electro, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-15.png; \r\n Clorinde, electro, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-13.png; \r\n Cyno, electro,2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-5.png; \r\n Dori, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-17-9.png; \r\n Emc, electro, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png; \r\n Fischl,electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-16.png; \r\n Iansan,electro, 2.5, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-37-3.png; \r\n Keqing,electro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-7.png; \r\n Kuki, electro,1.10, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-11.png; \r\n Lisa, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-2.png; \r\n Ororon, electro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-4.png; \r\n Raiden, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-4.png; \r\n Razor, electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-14.png; \r\n Sara, electro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-9.png; \r\n Sethos, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-14.png; \r\n Varesa,electro,3.20,https://static.icy-veins.com/images/genshin-impact/characters/varesa.webp; \r\n Yae, electro, 1.01, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-9.png; \r\n Alhaitham, dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-13.png; \r\n Baizhu, dendro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-3.png; \r\n Collei, dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-13.png; \r\n Dmc, dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png; \r\n Emilie, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-15.png; \r\n Kaveh, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-7.png; \r\n Kinich,dendro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-13.png; \r\n Kirara, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-3.png; \r\n Nahida, dendro,4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-19-5.png; \r\n Tighnari, dendro, 1.10, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-15.png; \r\n Yaoyao,dendro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-2.png; \r\n Ayaka, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-3.png; \r\n Charlotte, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-16.png; \r\n Chongyun, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-14.png; \r\n Citlali, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-11.png; \r\n Diona, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-11.png; \r\n Escoffier,cryo,1.20,https://static.icy-veins.com/images/genshin-impact/characters/escoffier.webp; \r\n Eula, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-2.png; \r\n Freminet, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-1.png; \r\n Ganyu, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-9.png; \r\n Kaeya, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-4.png; \r\n Layla, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-13.png; \r\n Mika, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-16.png; \r\n Qiqi, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-9.png; \r\n Rosaria, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-13.png; \r\n Shenhe, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-4.png; \r\n Wriothesley, cryo, 1.60, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-1.png; \r\n Albedo,geo, 1.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-9.png; \r\n Chiori, geo, 1.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-16.png; \r\n Gmc, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png; \r\n Gorou, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-5.png; \r\n Kachina, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-3.png; \r\n Itto, geo, 2.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-2.png; \r\n Navia, geo, 1.10, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-8.png; \r\n Ningguang, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-10.png; \r\n Noelle, geo, 3.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-2.png; \r\n Xilonen, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-12.png; \r\n Yunjin, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-6.png; \r\n Zhongli, geo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-9.png";
        }
            /*
        public static Calc()
        {
            LoadStuff(false);
        }
        public Calc(bool yeah) // for Blazor Hybrid App, non static stuff
        {
            desktoppath = AppDomain.CurrentDomain.BaseDirectory + @"DB\";//AppContext.BaseDirectory;//.Replace(" ", "");
            Directory.CreateDirectory(desktoppath);
            LoadStuff(yeah);
            Teams();
        }*/

            /// <summary>
            /// Refreshes the databases.
            /// </summary>
        public static void Reload()
        {
            teams.Clear();
            DB.Clear();
            CosmeticDB.Clear();
            LoadStuff(true);
            //Teams();
            Debug.Write($"({teams.Count})");
        }
    }
}