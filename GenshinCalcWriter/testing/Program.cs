using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenshinTeamCalc;

namespace ConsoleApp
{
    internal class Program
    {
        static Calc calc;

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    calc = new Calc();
                    break;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Could not locate files");
                    Settings(true);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("file paths invalid");
                    Settings(true);
                }
                catch (Exception d)
                {
                    Console.WriteLine(d);
                    Console.ReadLine();
                    break;
                }
            }
            int menu;
            do
            {
                Console.WriteLine("Welcome to the Genshin Calc Helper!\n\nWhat do you want?\n1- Full Teams Calculations (database)\n2- Quick Team Calculation\n3- All Account Team Pairing Ranking\n4- Specific Team Pairing Ranking\n\n5- Add/edit character damage\n6- Add Teams\n7- Settings\n\n0- Close");
                menu = int.Parse(Console.ReadLine());
                int teamsa;
                switch (menu)
                {
                    case 1:
                        File.Delete(calc.OUTlocation);
                        Console.Clear();
                        calc.teams.Clear();
                        int amount = calc.Teams();
                        TeamCalc(amount, true);
                        break;
                    case 2:
                        Console.Clear();
                        Console.Write("Team Calculation and Ranking\n\nAmount of teams: ");
                        teamsa = int.Parse(Console.ReadLine());
                        TeamCalc(teamsa, false);
                        break;
                    case 3:
                        if (calc.teams.Count <= 0)
                        {
                            calc.Teams();
                        }
                        TeamScore(calc.AccountTeamRanker(calc.teams, null), true);
                        Console.ReadLine();
                        break;
                    case 4:
                        if (calc.teams.Count <= 0)
                        {
                            calc.Teams();
                        }
                        ShowTeams(1);
                        Console.WriteLine("Which one do you wish to choose?");
                        int h = int.Parse(Console.ReadLine());
                        TeamScore(calc.AccountTeamRanker(calc.teams, calc.teams[h - 1]), true); // could check for amount of teams of the server?
                        Console.ReadLine();
                        break;
                    case 5:
                        Console.WriteLine("Type the name of the character.");
                        string c = Console.ReadLine().ToLower();
                        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                        c = textInfo.ToTitleCase(c);
                        DamageManager(c, calc);
                        break;
                    case 6:
                        TeamManager(calc);
                        break;
                    case 7:
                        Settings(false);
                        break;
                    case 0:
                        break;
                    default:
                        Console.WriteLine("Erm... what the heavenly principplesss??");
                        break;
                }
                Console.Clear();
            } while (menu != 0);
        }  // For Console App
        static void TeamCalc(int teamamount, bool rank)
        {
            if (!rank)
            {
                List<Team> localteams = new List<Team>();
                for (int i = 0; i < teamamount; i++)
                {
                    localteams.Add(new Team());
                    Console.WriteLine($"TEAM {i + 1}\n");
                    Console.Write("Rotation Length: ");
                    localteams[i].server = "the List";
                    try
                    {
                        localteams[i].rotlen = double.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("You stupid");
                        localteams[i].rotlen = double.Parse(Console.ReadLine());
                    }
                    for (int j = 0; j < 4; j++)
                    {
                        string temp;
                        Console.Write("Name: ");
                        temp = Console.ReadLine();
                        if (temp == "no")
                        {

                        }
                        else
                        {
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            temp = textInfo.ToTitleCase(temp);
                            Character tempchar = new Character(temp);
                            double tempdmg;
                            Console.Write("Damage: ");
                            tempdmg = double.Parse(Console.ReadLine());
                            double tempaoe;
                            tempaoe = calc.GetAoe(tempchar);
                            if (tempaoe <= -1)
                            {
                                Console.Write("Aoe: ");
                                tempaoe = double.Parse(Console.ReadLine());
                            }
                            tempchar.dmg = tempdmg; tempchar.aoe = tempdmg * tempaoe;
                            localteams[i].characters.Add(tempchar);
                        }
                    }
                    localteams[i].name = $"{localteams[i].characters[0].name} Team";
                    localteams[i].CalcDpm();
                }
                DrawCalc(localteams);
                DrawRank(localteams);
            }
            else
            {
                DrawCalc(calc.teams);
                DrawRank(calc.teams);
            }
            Console.WriteLine("Teams Calc'ed!");

            Process.Start(new ProcessStartInfo
            {
                FileName = calc.OUTlocation,
                UseShellExecute = true
            });
            Console.ReadLine();
        }
        static void DrawCalc(List<Team> teams)
        {
            using (StreamWriter writer = new StreamWriter(calc.OUTlocation, true)) // TXT WRITTING 
            {
                string tempserver = "uh";
                for (int i = 0; i < teams.Count; i++)
                {
                    if (teams[i].server != tempserver)
                    {
                        tempserver = teams[i].server;
                        writer.WriteLine($"\n---------------------------------------------- {tempserver} ----------------------------------------------");
                    }
                    writer.WriteLine($"\n{teams[i].name} ({teams[i].rotlen} sec)\n");
                    List<double> percentages = teams[i].GetPercentages();
                    for (int j = 0; j < teams[i].characters.Count; j++) // draw every character damage, name and percentage relative to team damage
                    {
                        if (teams[i].characters[j].name == "no" || teams[i].characters[j].dmg <= 0)
                        {

                        }
                        else
                        {
                            writer.WriteLine($"{teams[i].characters[j].dmg} {teams[i].characters[j].name}    {percentages[j]}%");
                        }
                    }
                    string tempstring = teams[i].som.ToString(); // getting sum to show as x.xxx.xxx
                    if (tempstring.Length <= 3)
                        writer.WriteLine($"\n{teams[i].som}");
                    else if (tempstring.Length <= 6)
                    {
                        tempstring = tempstring.Insert(3, ".");
                        writer.WriteLine($"\n{tempstring}");
                    }
                    else
                    {
                        tempstring = tempstring.Insert(1, ".");
                        tempstring = tempstring.Insert(5, ".");
                        writer.WriteLine($"\n{tempstring}");
                    }
                    writer.WriteLine($"{teams[i].dpm} mi dpm");
                    writer.WriteLine($"Aoe: {teams[i].dpmAoe}");
                    writer.WriteLine($"{teams[i].dps} DPS");
                }
                writer.WriteLine();
            }
        }
        static void DrawRank(List<Team> teams)
        {
            List<Team>[] Ordered = calc.TeamRank(teams);
            using (StreamWriter writer = new StreamWriter(calc.OUTlocation, true))
            {
                string caption = "DMG IN 1 MIN RANKING (single target)    *DIFF*            AOE\r\n- AMAZING DAMAGE\t\t\t\t\t- REALLY GOOD IN AOE\n";
                Console.Write(caption);
                writer.Write(caption);
                for (int i = 0; i < teams.Count; i++)
                {
                    string spacing, spacing2;
                    if (Ordered[0][i].name.Length >= 20)
                        spacing = "\t";
                    else if (Ordered[0][i].name.Length >= 12)
                        spacing = "\t\t";
                    else spacing = "\t\t\t";
                    if (Ordered[1][i].name.Length >= 20)
                        spacing2 = "\t";
                    else if (Ordered[1][i].name.Length >= 12)
                        spacing2 = "\t\t";
                    else spacing2 = "\t\t\t";
                    string template1 = $" {Ordered[0][i].name}{spacing}{Ordered[0][i].dpm}";//template1 = $".t{j+1}. {Ordered[0][i].dpm}";
                    string template2 = $" {Ordered[1][i].name}{spacing2}{Ordered[1][i].dpmAoe}";//template2 = $".t{k+1}. {dmgAoe[i]}";
                    if (i < teams.Count - 1)
                    {
                        writer.WriteLine($"#{i + 1}{template1}\t({(((Ordered[0][i].dpm / Ordered[0][i+1].dpm) - 1) * 100).ToString("F2")}%) \t#{i + 1}{template2}\t({(((Ordered[1][i].dpmAoe / Ordered[1][i+1].dpmAoe) - 1) * 100).ToString("F2")}%)");
                        ColorRank(Ordered[0][i].dpm, 0);
                        Console.Write($"#{i + 1}{template1}\t({(((Ordered[0][i].dpm / Ordered[0][i+1].dpm) - 1) * 100).ToString("F2")}%) \t");
                        ColorRank(Ordered[1][i].dpmAoe, 1);
                        Console.WriteLine($"#{i + 1}{template2}\t({(((Ordered[1][i].dpmAoe / Ordered[1][i+1].dpmAoe) - 1) * 100).ToString("F2")}%)");
                    }
                    else
                    {
                        writer.WriteLine($"#{i + 1}{template1}\t(Nan%) \t\t#{i + 1}{template2}\t(NaN%)");
                        Console.WriteLine($"#{i + 1}{template1}\t(Nan%) \t\t#{i + 1}{template2}\t(NaN%)");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Dictionary<string, SortedDictionary<double, Team[]>[]> scores = calc.AccountTeamRanker(teams, null);
            TeamScore(scores, false);
        }
        static void TeamScore(Dictionary<string, SortedDictionary<double, Team[]>[]> scores, bool full)
        {
            if (full)
            {
                foreach (KeyValuePair<string, SortedDictionary<double, Team[]>[]> account in scores) // server
                {
                    Console.WriteLine($"Best teams in {account.Key}:");
                    int type = 0;
                    foreach (SortedDictionary<double, Team[]> datatype in account.Value) // dmg type
                    {
                        try
                        {
                            switch (type)
                            {
                                case 0:
                                    Console.WriteLine("Single Target:\n");
                                    break;
                                case 1:
                                    Console.WriteLine("Aoe Scenario:\n");
                                    break;
                                case 2:
                                    type--;
                                    Console.WriteLine("Best Overall:\n");
                                    break;
                            }
                            int pos = 1;
                            foreach (KeyValuePair<double, Team[]> pair in datatype.Reverse()) // pairing
                            {
                                ColorRank(pair.Key / 2, type);
                                Console.WriteLine($"#{pos++} - \t{pair.Value[0].name} and {pair.Value[1].name} ({pair.Key})");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            type++;
                        }
                        catch
                        {
                            Console.WriteLine("This account doesn't have 2 full teams without overlap :(");
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter yeah = new StreamWriter(calc.OUTlocation, true)) 
                {
                    foreach (KeyValuePair<string, SortedDictionary<double, Team[]>[]> account in scores) // account
                    {
                        List<Team[]> Pairs = new List<Team[]>();
                        List<double> Pairdmg = new List<double>();
                        Console.WriteLine($"\nBest teams in {account.Key}:");
                        yeah.WriteLine($"\nBest teams in {account.Key}:");
                        try
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        ColorRank(account.Value[i].First().Key / 2, 0);
                                        Console.Write("Single Target: ");
                                        break;
                                    case 1:
                                        ColorRank(account.Value[i].First().Key / 2, 1);
                                        Console.Write("Aoe Scenario: ");
                                        break;
                                    case 2:
                                        ColorRank(account.Value[i].First().Key / 2, 1);
                                        Console.Write("Best Overall: ");
                                        break;
                                }
                                Console.WriteLine($"\t{account.Value[i].Last().Value[0].name} and {account.Value[i].Last().Value[1].name} ({account.Value[i].Last().Key})");
                                yeah.WriteLine($"\t{account.Value[i].Last().Value[0].name} and {account.Value[i].Last().Value[1].name} ({account.Value[i].Last().Key})");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            Console.WriteLine();
                            yeah.WriteLine();
                        }
                        catch
                        {
                            Console.WriteLine("This account doesn't have 2 full teams without overlap :(");
                            yeah.WriteLine("This account doesn't have 2 full teams without overlap :(");
                        }

                    }
                }
            }
        }
        static void ColorRank(double dmg, int type) // changes color of ranking at specified thresholds
        {
            if (type == 0)
            {
                if (dmg >= 3.5)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (dmg >= 2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
            }
            else
            {
                if (dmg >= 8)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (dmg >= 5)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
            }
        }
        public void DamageManager(string c, Calc calc)
        {
            Console.Clear();
            bool on = true;
            if (calc.DB.ContainsKey(c))
            {
                string[] tmpdb = File.ReadAllLines(calc.DBlocation);
                do
                {
                    int i;
                    for (i = 0; !tmpdb[i].Contains(c); i++) ;
                    Console.WriteLine(tmpdb[i].Replace("\t", "  "));
                    Console.WriteLine($"Which value do you want to change? (1 - 4 for damages, 5 for Aoe, 0 to return)");
                    int menu = int.Parse(Console.ReadLine());
                    string[] tmp = tmpdb[i].Split(',');
                    if (menu == 0) { on = false; }
                    else
                    {
                        int value;
                        string[] subtmp;
                        switch (menu)
                        {
                            case 1:
                                Console.WriteLine("Change to what value?");
                                value = int.Parse(Console.ReadLine());
                                subtmp = tmp[1].Split(' ');
                                tmp[1] = tmp[1].Replace(subtmp[subtmp.Length - 1], value.ToString());
                                DB[c][0] = value;
                                break;
                            case 2:
                                Console.WriteLine("Change to what value?");
                                value = int.Parse(Console.ReadLine());
                                subtmp = tmp[2].Split(' ');
                                tmp[2] = tmp[2].Replace(subtmp[subtmp.Length - 1], value.ToString());
                                Console.WriteLine(tmp[2]);
                                calc.DB[c][1] = value;
                                break;
                            case 3:
                                Console.WriteLine("Change to what value?");
                                value = int.Parse(Console.ReadLine());
                                tmp[3] = tmp[3].Replace(calc.DB[c][2].ToString(), value.ToString());
                                calc.DB[c][2] = value;
                                break;
                            case 4:
                                Console.WriteLine("Change to what value?");
                                value = int.Parse(Console.ReadLine());
                                tmp[4] = tmp[4].Replace(calc.DB[c][3].ToString(), value.ToString());
                                calc.DB[c][3] = value;
                                break;
                            case 5:
                                Console.WriteLine("Change to what value?");
                                string uhh = Console.ReadLine().Replace('.', ',');
                                double uh = Math.Round(double.Parse(uhh), 2);
                                string dbtrans = DB[c][4].ToString("N2");
                                dbtrans = dbtrans.Replace(',', '.');
                                calc.DB[c][4] = uh;
                                uhh = uh.ToString("N2");
                                uhh = uhh.Replace(',', '.');
                                string failsafe = tmp[5];
                                tmp[5] = tmp[5].Replace(dbtrans, uhh);
                                break;
                            default:
                                break;
                        }
                        tmpdb[i] = "";
                        for (int k = 0; k < tmp.Length; k++)
                        {
                            if (k == tmp.Length - 1)
                            {
                                tmpdb[i] += tmp[k];
                            }
                            else
                            {
                                tmpdb[i] += tmp[k] + ",";
                            }
                        }
                    }
                } while (on);
                File.WriteAllLines(DBlocation, tmpdb);
            }
            else
            {
                Console.WriteLine("This character was not found in the database. Do you wish to add a new character? (Y/N)");
                if (Console.ReadLine().ToLower() == "y")
                {
                    string[] tmpdb = File.ReadAllLines(DBlocation);
                    StreamWriter w = new StreamWriter(DBlocation, false);
                    string text = c + ",";
                    if (c.Length >= 7)
                    {
                        text += "\t";
                    }
                    else
                    {
                        text += "\t\t";
                    }
                    text += "0, 0, 0, 0, 1.00;";
                    int i;
                    bool done = false;
                    for (i = 0; i < tmpdb.Length; i++)
                    {
                        if (!done)
                        {
                            string[] tmptmp = tmpdb[i].Split(',');
                            switch (tmptmp[0].CompareTo(c))
                            {
                                case -1:
                                    w.WriteLine(tmpdb[i]);
                                    break;
                                case 1:
                                    w.WriteLine(text);
                                    w.WriteLine(tmpdb[i]);
                                    done = true;
                                    break;
                            }
                        }
                        else
                        {
                            w.WriteLine(tmpdb[i]);
                        }
                    }
                    w.Close();
                    double[] tmp = { 0, 0, 0, 0, 1 };
                    DB.Add(c, tmp);
                    DamageManager(c);
                }
            }
        }
        public void TeamManager()
        {
            Console.Clear();
            bool on = true;
            //string[] tmpdb = File.ReadAllLines(TeamDBlocation);
            do
            {
                Console.WriteLine("\n1- Add a new Team\n2- /////////\n3- Toggle a Team\n\n0- Return"); // 2 will be edit
                int menu = int.Parse(Console.ReadLine());
                string joker = "";
                string temp;
                int i, choice;
                switch (menu)
                {
                    case 0:
                        on = false;
                        break;
                    case 1:

                        string[] tmpdb2 = File.ReadAllLines(DBlocation);
                        Console.Clear();
                        Console.Write("Team name: ");
                        joker += Console.ReadLine();
                        joker += "-";
                        Console.Write("\nRotation Length: ");
                        joker += Console.ReadLine();
                        joker += "-";
                        Console.Write("\nAccount/Server: ");
                        string server = Console.ReadLine().ToUpper();
                        joker += server;
                        joker += ";\n";
                        for (int j = 1; j <= 4; j++)
                        {
                            Console.WriteLine("(Type 'no' to skip a member)");
                            Console.WriteLine($"\n\nTeam member {j}:");
                            temp = Console.ReadLine();
                            joker += temp;
                            temp = temp.ToLower();
                            Console.Clear();
                            if (!(temp == "no"))
                            {
                                bool on2;
                                try
                                {
                                    for (i = 0; !tmpdb2[i].Contains(temp); i++) ;
                                }
                                catch
                                {
                                    Console.WriteLine("Erm we didn't find it :(");
                                    Console.ReadLine();
                                    break;
                                }
                                do
                                {
                                    on2 = false;
                                    Console.WriteLine(tmpdb2[i].Replace("\t", "  "));
                                    Console.WriteLine("Which value to use? (1-4)");
                                    int menu2 = int.Parse(Console.ReadLine());
                                    if (menu2 == 0) { on = false; }
                                    else
                                    {
                                        if (menu2 >= 5) { }
                                        else
                                        {
                                            joker += $"-{menu2 - 1}";
                                        }
                                        if (j <= 3)
                                        {
                                            joker += ";\n";
                                        }
                                        else
                                        {
                                            joker += "\n";
                                        }
                                    }
                                    Console.Clear();
                                } while (on2);
                            }
                            else
                            {
                                if (j <= 3)
                                {
                                    joker += "-0;\n";
                                }
                                else
                                {
                                    joker += "-0\n";
                                }
                            }
                        }
                        Console.WriteLine(joker);
                        string tmp = File.ReadAllText(TeamDBlocation);
                        string[] tmpdb = tmp.Split(',');
                        bool Innit = false;
                        for (i = 0; i < tmpdb.Length; i++)
                        {
                            string[] tmp2 = tmpdb[i].Split(';');
                            string[] tmp3 = tmp2[0].Split('-');
                            if (tmp3[2] == server)
                            {
                                Innit = true;
                            }
                            else if (Innit)
                            {
                                break;
                            }
                        }
                        i = (i * 5) + i + 1;
                        Console.WriteLine($"Line to insert: line {i}");
                        tmpdb = File.ReadAllLines(TeamDBlocation);
                        if (i < tmpdb.Length - 3)
                        {
                            joker += ",";
                        }
                        StreamWriter w = new StreamWriter(TeamDBlocation, false);
                        bool ducttape = true;
                        for (int j = 0; j < tmpdb.Length - 1; j++)
                        {
                            if (j == i - 1)
                            {
                                ducttape = false;
                                w.WriteLine(joker);
                            }
                            w.WriteLine(tmpdb[j]);
                        }
                        Console.Write(tmpdb[tmpdb.Length - 1]);
                        Console.ReadLine();
                        w.Write(tmpdb[tmpdb.Length - 1]);
                        if (ducttape)
                        {
                            Console.Write("\n,\n" + joker);
                            Console.WriteLine("thatsn  it");
                            Console.ReadLine();
                            w.Write("\n,\n" + joker);

                        }
                        w.Close();
                        break;
                    case 2:
                        /*while (true)
                        {
                            Console.Clear();
                            ShowTeams(0);
                            Console.WriteLine("Which one to Edit? (-1 to leave)");
                            choice = int.Parse(Console.ReadLine());
                            if (choice <= -1) { break; }
                            while (true) // ------------- TEAM
                            {
                                string t = "";
                                string[] teams = File.ReadAllText(TeamDBlocation).Split(',');
                                Console.Clear();
                                Console.WriteLine(teams[choice - 1]);
                                Console.WriteLine("\n Change what line? (1-5) (-1 to return)");
                                int choice2 = int.Parse(Console.ReadLine());
                                if (choice2 <= -1 || choice2 >= 6) { break; }
                                if (choice2 == 1) 
                                {
                                    Console.WriteLine("(1) Change name (2) Change Rotation Length (3) Change Server");
                                    int choice3 = int.Parse(Console.ReadLine());
                                    t = teams[choice];
                                    string[] spots = t.Split(';')[0].Split('-');
                                    switch (choice3)
                                    {
                                        case 1:
                                            Console.Write("Type altered name: ");
                                            spots[0] = $"{Console.ReadLine()}";
                                            break;
                                        case 2:
                                            Console.Write("Type altered rotation length: ");
                                            spots[1] = $"{int.Parse(Console.ReadLine())}";
                                            break;
                                        case 3:
                                            Console.Write("Type altered server: ");
                                            spots[2] = $"{Console.ReadLine().ToUpper()}";
                                            break;
                                    }
                                    joker = $"{spots[0]}-{spots[1]}-{spots[2]};";
                                    t = teams[choice];
                                    string[] h = t.Split(';');
                                    h[0] = joker;
                                    joker = "";
                                    foreach (string he in h)
                                    {
                                        joker += he;
                                    }
                                    teams[choice] = joker;
                                    File.WriteAllLines(TeamDBlocation, teams);
                                }
                                else // -------------- DAMAGES
                                {
                                    tmpdb2 = File.ReadAllLines(DBlocation);
                                    Console.WriteLine("(1) Change Character (2) Change Damage (3) Add/Edit Weight");
                                    int choice3 = int.Parse(Console.ReadLine());
                                    t = teams[choice2];
                                    string[] spots = t.Split(';')[0].Split('-');
                                    temp = spots[0];
                                    if (choice3 == 1)
                                    {
                                        Console.Write("Type Character: ");
                                        spots[0] = $"\n{Console.ReadLine()}";
                                        joker = spots[0];
                                        temp = spots[0].ToLower();
                                        Console.Clear();
                                    }
                                    if (choice3 <= 2)
                                    {
                                        if (!(temp == "no"))
                                        {
                                            bool on2;
                                            try
                                            {
                                                for (i = 0; !tmpdb2[i].Contains(temp); i++) ;
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Erm we didn't find it :(");
                                                Console.ReadLine();
                                                break;
                                            }
                                            do
                                            {
                                                on2 = false;
                                                Console.WriteLine(tmpdb2[i].Replace("\t", "  "));
                                                Console.WriteLine("Which value to use? (1-4)");
                                                int menu2 = int.Parse(Console.ReadLine());
                                                if (menu2 == 0) { on = false; }
                                                else
                                                {
                                                    if (menu2 >= 5) { }
                                                    else
                                                    {
                                                        joker += $"-{menu2 - 1}";
                                                    }
                                                    if (spots[2].IndexOf(';') <= -1)
                                                    {
                                                        joker += "\n";
                                                    }
                                                    else
                                                    {
                                                        joker += ";\n";
                                                    }
                                                }
                                                Console.Clear();
                                            } while (on2);
                                        }
                                        else
                                        {
                                            if (tmpdb2[0].IndexOf(';') <= -1)
                                            {
                                                joker += "-0\n";
                                            }
                                            else
                                            {
                                                joker += "-0;\n";
                                            }
                                        }
                                    }
                                    joker = $"{spots[0]}-{spots[1]}{spots[2]};";
                                    t = teams[choice];
                                    string[] h = t.Split(';');
                                    h[0] = joker;
                                    joker = "";
                                    foreach (string he in h)
                                    {
                                        joker += he;
                                    }
                                    teams[choice] = joker;
                                }

                            }
                        }*/
                        break;
                    case 3:
                        while (true)
                        {
                            Console.Clear();
                            //ShowTeams(0);   ---------------
                            Console.WriteLine("Which one to toggle? (-1 to leave)");
                            choice = int.Parse(Console.ReadLine());
                            string[] lines = File.ReadAllLines(TeamDBlocation);
                            if (choice <= -1)
                            {
                                break;
                            }
                            if (lines[(choice - 1) * 6].IndexOf("//") >= 0)
                            {
                                lines[(choice - 1) * 6] = lines[(choice - 1) * 6].Replace("//", "");
                            }
                            else
                            {
                                lines[(choice - 1) * 6] = $"//{lines[(choice - 1) * 6]}";
                            }
                            File.WriteAllLines(TeamDBlocation, lines);
                        }
                        teams.Clear();
                        Teams();
                        break;
                    default:
                        break;

                }
            } while (on);
        }
        static void Settings(bool forced)
        {
            bool on = true;
            string[] temp = new string[3];
            StreamWriter writer = new StreamWriter("config.cfg", false);
            if (forced)
            {
                do
                {
                    Console.WriteLine("Please set the locations of all three files.");
                    Console.WriteLine("1- Set damage/Aoe DB location\n2- Set teams DB location\n3- Set Output location\n");
                    int menu = int.Parse(Console.ReadLine());
                    switch (menu)
                    {
                        case 1:
                            Console.Write("Type damage and Aoe DB location: ");
                            temp[0] = Console.ReadLine();
                            break;
                        case 2:
                            Console.Write("Type Team DB location: ");
                            temp[1] = Console.ReadLine();
                            break;
                        case 3:
                            Console.Write("Type Output location: ");
                            temp[2] = Console.ReadLine();
                            break;
                        default:
                            break;
                    }
                } while (temp[0] == null || temp[1] == null || temp[2] == null);
                writer.WriteLine($"DB; {temp[0]};\nTeamDB; {temp[1]};\nOutput; {temp[2]};\ndmgtype; dps");
                writer.Close();
            }
            else
            {
                temp[0] = calc.DBlocation; temp[1] = calc.TeamDBlocation; temp[2] = calc.OUTlocation;
                string dmgtype = calc.dmgType;
                do
                {
                    int menu;
                    Console.WriteLine("1- Set damage/Aoe DB location\n2- Set teams DB location\n3- Set Output location\n4- See Current locations\n0- Return");
                    try { menu = int.Parse(Console.ReadLine()); }
                    catch (FormatException) { menu = -1; }
                    switch (menu)
                    {
                        case 1:
                            Console.Write("Type damage and Aoe DB location: ");
                            temp[0] = Console.ReadLine();
                            break;
                        case 2:
                            Console.Write("Type Team DB location: ");
                            temp[1] = Console.ReadLine();
                            break;
                        case 3:
                            Console.Write("Type Output location: ");
                            temp[2] = Console.ReadLine();
                            break;
                        case 4:
                            Console.WriteLine($"Damage/Aoe DB: {temp[0]}\nTeams DB: {temp[1]}\nOutput file location: {temp[2]}\n");
                            break;
                        case 0:
                            on = false;
                            break;
                        default:
                            break;
                    }
                } while (on);
                writer.WriteLine($"DB; {temp[0]};\nTeamDB; {temp[1]};\nOutput; {temp[2]};\ndmgtype; {calc.dmgType}");
                writer.Close();
                calc = new Calc();
            }
            Console.Clear();
        }
        static void ShowTeams(int mode) // 0 - Only Names and if they are disabled. 1 - Show only Enabled
        {
            string[] temp = File.ReadAllText(calc.TeamDBlocation).Split(',');
            switch (mode)
            {
                case 0:
                    string server = "";
                    for (int i = 0; i < temp.Length; i++)
                    {
                        string t = temp[i].Split(';')[0].Split('-')[2];
                        if (t != server)
                        {
                            server = t;
                            Console.WriteLine($"\n------------- {server} -------------");
                        }
                        t = temp[i];
                        t = t.Remove(temp[i].IndexOf('-'));
                        t = t.Replace(Environment.NewLine, "");
                        Console.Write($"({i + 1}) \t {t} \n ");
                    }
                    break;
                case 1:
                    string server1 = "";
                    int offset = 0;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        string t = temp[i].Split(';')[0].Split('-')[2];
                        if (t != server1)
                        {
                            server1 = t;
                            Console.WriteLine($"\n------------- {server1} -------------");
                        }
                        t = temp[i];
                        if (t.IndexOf("//") < 0)
                        {
                            t = t.Remove(temp[i].IndexOf('-'));
                            t = t.Replace(Environment.NewLine, "");
                            Console.Write($"({i + 1 - offset}) \t {t} \n ");
                        }
                        else
                        {
                            offset++;
                        }

                    }
                    break;
            }
        }
    }
}
