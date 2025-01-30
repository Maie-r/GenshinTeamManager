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
    public class Calc
    {
        public List<Team> teams = new List<Team>();
        readonly Dictionary<string, List<double>> DB = new Dictionary<string, List<double>>(); // dmg, dmg, dmg, dmg, aoe
        readonly Dictionary<string, string[]> CosmeticDB = new Dictionary<string, string[]>(); // element, img
        public string dmgType;
        public string TeamDBlocation;
        public string DBlocation;
        public string OUTlocation;
        public string configlocation;
        string desktoppath;
        
        public List<Team>[] TeamRank(List<Team> teams)
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
        public int GetAcountAmount(List<Team> teams)
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
        public int Teams() // Loads Teams from TeamDB
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
                    Team tempteam = new Team(everything, this);
                    teams.Add(tempteam);
                    teamamount++;
                }
            }
            Debug.WriteLine("Teams loaded!");
            return teamamount;
        }
        public List<string[]> ALLTeams(List<Team> result) // Returns all Teams from TeamDB in result, and all text as return value, even the ignored ones
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
                    Team tempteam = new Team(everything, this);
                    result.Add(tempteam);
                    teamamount++;
            }
            return resultraw;
        }
        public List<Character> GetAllCharacters()
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
        public Dictionary<string, CharacterCondensed> GetAllCharactersCondensed()
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
                Debug.WriteLine(character);
                List<string> text = character.Replace(" ", "").Split(',').ToList();
                List<string> damages = new List<string>();
                for (int i = 2; i < text.Count -2; i++)
                {
                    damages.Add(text[i]);
                }
                result.Add(text[0], new CharacterCondensed(
                    new Character(text[0], text[1], text[text.Count - 1], (double.Parse(text[text.Count - 2])/100)),
                    damages, text[0], text));
            }
            return result;
        }
        public double GetDmg(Character character, int v) // Gets Damage from the Dictionary stored DB
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
        public double GetAoe(Character character) // Gets Aoe from the Dictionary stored DB
        {
            try
            {
                try
                {
                    return DB[character.name][DB[character.name].Count-1];
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
        public void AddCosmetic(Character character) // Gets element and image from the Dictionary stored DB
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
        public void LoadStuff(bool auto) // Loads all info from DB into a Dictionary
        {
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
                        File.WriteAllText(desktoppath + "db.txt", "Amber, pyro,1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-1-12.png;\r\nArlecchino, pyro,2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-32-8.png;\r\nBennett, pyro,3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-13.png;\r\nChevreuse, pyro,1.20, https://cdn3.emoji.gg/emojis/2257-chevreuse-delicious.png;\r\nDehya, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-5.png;\r\nDiluc, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-3.png;\r\nKlee, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-1.png;\r\nGaming, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-1.png;\r\nHutao, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-5-6.png;\r\nLyney, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-5.png;\r\nMavuika, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-11.png;\r\nPmc, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nThoma, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-10-11.png;\r\nXiangling, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-16.png;\r\nXinyan, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-3-11.png;\r\nYanfei, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-8.png;\r\nYoimiya, pyro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-5.png;\r\nAyato, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-2.png;\r\nBarbara, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-11.png;\r\nCandace, hydro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-11.png;\r\nChilde,hydro, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-8.png;\r\nFurina, hydro, 1.75, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-6.png;\r\nHmc, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nKokomi, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-8.png;\r\nMona, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-12.png;\r\nMualani, hydro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-15.png;\r\nNeuvillette, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-15.png;\r\nNilou, hydro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-1.png;\r\nSigewinne, hydro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-8.png;\r\nXingqiu, hydro,1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-16.png;\r\nYelan, hydro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-1.png;\r\nAmc, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nChasca,anemo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-5.png;\r\nFaruzan, anemo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-16.png;\r\nHeizou, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-16-4.png;\r\nJean, anemo, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-16.png;\r\nKazuha,anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-8.png;\r\nLynette, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-11.png;\r\nSayu, anemo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-9.png;\r\nSucrose, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-12.png;\r\nVenti, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-12.png;\r\nWanderer, anemo, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-3.png;\r\nXianyun, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-12.png;\r\nXiao, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-14.png;\r\nBeidou,electro, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-15.png;\r\nClorinde, electro, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-13.png;\r\nCyno, electro,2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-5.png;\r\nDori, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-17-9.png;\r\nEmc, electro, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nFischl,electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-16.png;\r\nKeqing,electro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-7.png;\r\nKuki, electro,1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-11.png;\r\nLisa, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-2.png;\r\nOroron, electro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-4.png;\r\nRaiden, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-4.png;\r\nRazor, electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-14.png;\r\nSara, electro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-9.png;\r\nSethos, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-14.png;\r\nYae, electro, 1.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-9.png;\r\nAlhaitham, dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-13.png;\r\nBaizhu, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-3.png;\r\nCollei, dendro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-13.png;\r\nDmc, dendro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nEmilie, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-15.png;\r\nKaveh, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-7.png;\r\nKinich,dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-13.png;\r\nKirara, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-3.png;\r\nNahida, dendro,4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-19-5.png;\r\nTighnari, dendro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-15.png;\r\nYaoyao,dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-2.png;\r\nAyaka, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-3.png;\r\nCharlotte, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-16.png;\r\nChongyun, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-14.png;\r\nCitlali, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-11.png;\r\nDiona, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-11.png;\r\nEula, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-2.png;\r\nFreminet, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-1.png;\r\nGanyu, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-9.png;\r\nKaeya, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-4.png;\r\nLayla, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-13.png;\r\nMika, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-16.png;\r\nQiqi, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-9.png;\r\nRosaria, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-13.png;\r\nShenhe, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-4.png;\r\nWriothesley, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-1.png;\r\nAlbedo,geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-9.png;\r\nChiori, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-16.png;\r\nGmc, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nGorou, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-5.png;\r\nKachina, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-3.png;\r\nItto, geo, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-2.png;\r\nNavia, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-8.png;\r\nNingguang, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-10.png;\r\nNoelle, geo, 3.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-2.png;\r\nXilonen, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-12.png;\r\nYunjin, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-6.png;\r\nZhongli, geo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-9.png\r\n");
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
            if (cfg2.Length <=7)
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
                for (int j = 2; j < individual.Length-2; j++) // to store relative damage
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
                    stuff.Add(double.Parse(individual[individual.Length-2], CultureInfo.InvariantCulture));
                }
                DB.Add(individual[0], stuff);
                CosmeticDB.Add(individual[0], new string[] { individual[1], individual[individual.Length - 1]});
            }
        }
        public Dictionary<string, SortedDictionary<double, Team[]>[]> AccountTeamRanker(List<Team> teams, Team special)// Teams, Team to use. RETURN: Returns dictionary based on server, with 3 sorted dictionaries of pairing ranked in each category
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
        SortedDictionary<double, Team[]> TeamPairer(List<Team> teams, int type, Team special) // type 0- single target, type 1- Aoe, type 2- Average, full rank or nah
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
        void InnerPair(KeyValuePair<double, Team> kv1, Dictionary<double, Team> joker, SortedDictionary<double, Team[]> result, bool special)
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
        public List<List<Team>> AccountSeparate(List<Team> teams, Team special)
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
        public static double ToDps(double dpm, double rotlen)
        {
            return ((dpm * 1000000) * (rotlen / 60)) / rotlen;
        }
        public bool CharacterExists(string name)
        {
            return DB.ContainsKey(name);
        }

        string GetTemplate()
        {
            return "Amber, pyro,1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-1-12.png;\r\nArlecchino, pyro,2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-32-8.png;\r\nBennett, pyro,3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-13.png;\r\nChevreuse, pyro,1.20, https://cdn3.emoji.gg/emojis/2257-chevreuse-delicious.png;\r\nDehya, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-5.png;\r\nDiluc, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-3.png;\r\nKlee, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-1.png;\r\nGaming, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-1.png;\r\nHutao, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-5-6.png;\r\nLyney, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-5.png;\r\nMavuika, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-11.png;\r\nPmc, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nThoma, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-10-11.png;\r\nXiangling, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-16.png;\r\nXinyan, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-3-11.png;\r\nYanfei, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-8.png;\r\nYoimiya, pyro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-5.png;\r\nAyato, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-2.png;\r\nBarbara, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-11.png;\r\nCandace, hydro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-11.png;\r\nChilde,\thydro, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-8.png;\r\nFurina, hydro, 1.75, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-6.png;\r\nHmc, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nKokomi, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-8.png;\r\nMona, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-12.png;\r\nMualani, hydro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-15.png;\r\nNeuvillette, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-15.png;\r\nNilou, hydro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-1.png;\r\nSigewinne, hydro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-8.png;\r\nXingqiu, hydro,\t1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-16.png;\r\nYelan, hydro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-1.png;\r\nAmc, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nChasca,\tanemo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-5.png;\r\nFaruzan, anemo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-16.png;\r\nHeizou, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-16-4.png;\r\nJean, anemo, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-16.png;\r\nKazuha,\tanemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-8.png;\r\nLynette, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-11.png;\r\nSayu, anemo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-9.png;\r\nSucrose, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-12.png;\r\nVenti, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-12.png;\r\nWanderer, anemo, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-3.png;\r\nXianyun, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-12.png;\r\nXiao, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-14.png;\r\nBeidou,\telectro, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-15.png;\r\nClorinde, electro, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-13.png;\r\nCyno, electro,\t2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-5.png;\r\nDori, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-17-9.png;\r\nEmc, electro, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nFischl,\telectro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-16.png;\r\nKeqing,\telectro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-7.png;\r\nKuki, electro,\t1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-11.png;\r\nLisa, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-2.png;\r\nOroron, electro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-4.png;\r\nRaiden, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-4.png;\r\nRazor, electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-14.png;\r\nSara, electro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-9.png;\r\nSethos, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-14.png;\r\nYae, electro, 1.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-9.png;\r\nAlhaitham, dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-13.png;\r\nBaizhu, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-3.png;\r\nCollei, dendro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-13.png;\r\nDmc, dendro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nEmilie, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-15.png;\r\nKaveh, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-7.png;\r\nKinich,\tdendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-13.png;\r\nKirara, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-3.png;\r\nNahida, dendro,\t4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-19-5.png;\r\nTighnari, dendro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-15.png;\r\nYaoyao,\tdendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-2.png;\r\nAyaka, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-3.png;\r\nCharlotte, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-16.png;\r\nChongyun, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-14.png;\r\nCitlali, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-11.png;\r\nDiona, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-11.png;\r\nEula, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-2.png;\r\nFreminet, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-1.png;\r\nGanyu, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-9.png;\r\nKaeya, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-4.png;\r\nLayla, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-13.png;\r\nMika, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-16.png;\r\nQiqi, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-9.png;\r\nRosaria, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-13.png;\r\nShenhe, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-4.png;\r\nWriothesley, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-1.png;\r\nAlbedo,\tgeo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-9.png;\r\nChiori, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-16.png;\r\nGmc, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nGorou, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-5.png;\r\nKachina, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-3.png;\r\nItto, geo, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-2.png;\r\nNavia, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-8.png;\r\nNingguang, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-10.png;\r\nNoelle, geo, 3.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-2.png;\r\nXilonen, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-12.png;\r\nYunjin, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-6.png;\r\nZhongli, geo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-9.png\r\n";
        }
        public Calc() 
        {
            LoadStuff(false);
        }
        public Calc(bool yeah) // for Blazor Hybrid App, non static stuff
        {
            desktoppath = AppDomain.CurrentDomain.BaseDirectory + @"DB\";//AppContext.BaseDirectory;//.Replace(" ", "");
            Directory.CreateDirectory(desktoppath);
            LoadStuff(yeah);
            Teams();
        }
        public void Reload()
        {
            teams.Clear();
            DB.Clear();
            CosmeticDB.Clear();
            LoadStuff(true);
            Teams();
        }
    }
}