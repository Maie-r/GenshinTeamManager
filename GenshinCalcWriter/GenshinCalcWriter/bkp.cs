using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace GenshinCalcWriter
{
    class Calc
    {
        //wishlist: select server/account, update database
        static double[] OrderGuideYou(int teamamount, double[]dmg) // orders array list from highest to lowest
        {
            for (int i = 0; i < teamamount - 1; i++)
            {
                // Find the minimum element 
                // in unsorted array 
                int min_idx = i;
                for (int j = i + 1; j < teamamount; j++)
                    if (dmg[j] > dmg[min_idx])
                        min_idx = j;

                // Swap the found minimum 
                // element with the first 
                // element 
                double temp = dmg[min_idx];
                dmg[min_idx] = dmg[i];
                dmg[i] = temp;
            }
            return dmg;
        }
        static void TeamCalc (int teams, bool rank) // (amount of teams, using database?) reads team names, damage numbers and Aoe, calculates total damage, dps, dpm.
        {
            string filePath = @"C:\Users\user\Desktop\projects\GENSHIN\product.txt";
            double[,] Damage = new double[teams, 4];
            double[,] Aoe = new double[teams, 4];
            double[] Rot = new double[teams];
            string[,] Names = new string[teams, 4];
            double[,] everything = new double[10, teams]; // id id id... 20 > rot lengh ... > dmg1... > dmg2...
            if (rank == false)
            {
                for (int i = 0; i < teams; i++) // INPUT READING
                {
                    Console.WriteLine($"TEAM {i + 1}\n");
                    Console.Write("Rotation Length: ");
                    Rot[i] = double.Parse(Console.ReadLine());
                    for (int j = 0; j < 4; j++)
                    {
                        Console.Write("Name: ");
                        Names[i, j] = Console.ReadLine();
                        if (Names[i, j] == "no")
                        {

                        }
                        else
                        {
                            Console.Write("Damage: ");
                            Damage[i, j] = double.Parse(Console.ReadLine());
                            Aoe[i, j] = GetAoe(Names[i, j]);
                            if (GetAoe(Names[i, j]) == -1)
                            {
                                Console.Write("Aoe: ");
                                Aoe[i, j] = double.Parse(Console.ReadLine());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < teams; i++) // GET FROM DATABASE
                {
                    everything = Teams();
                    Rot[i] = everything[1, i];
                    for (int j = 2, k=0; j < 10; j+=2, k++)
                    {
                        Damage[i, k] = everything[j, i];
                        Aoe[i, k] = everything[(j+1), i];
                    }
                }
            }
            double[] dpm = new double[teams + 1];
            double[] dpmAoe = new double[teams + 1];
            using (StreamWriter writer = new StreamWriter(filePath, true)) // TXT WRITTING 
            {
                if (rank == false) // from input
                {
                    for (int i = 0; i < teams; i++)
                    {
                        double som = 0;
                        double somAoe = 0;
                        writer.WriteLine($"\n{Names[i, 0]} Team\n");
                        for (int j = 0; j < 4; j++)
                        {
                            if (Names[i, j] == "no" || Damage[i, j] <= 0)
                            {

                            }
                            else
                            {
                                writer.WriteLine($"{Damage[i, j]} {Names[i, j]}");
                                som += Damage[i, j];
                                somAoe += Damage[i, j] * Aoe[i, j];
                            }
                        }
                        dpm[i] = Math.Round((som * (60 / Rot[i])) / 1000000, 2);
                        dpmAoe[i] = Math.Round((somAoe * (60 / Rot[i])) / 1000000, 2);
                        writer.WriteLine($"\n{som}");
                        writer.WriteLine($"{dpm[i]} mi dpm");
                        writer.WriteLine($"Aoe Ranking: {dpmAoe[i]}");
                        writer.WriteLine($"{Math.Round(som / Rot[i], 2)} DPS");
                    }
                }
                else // from database
                {
                    for (int i = 0; i < teams; i++)
                    {
                        double som = 0;
                        double somAoe = 0;
                        writer.WriteLine($"\n.t{everything[0, i]}.\n");
                        for (int j = 0; j < 4; j++)
                        {
                            if (Names[i, j] == "no" || Damage[i, j] <= 0)
                            {

                            }
                            else
                            {
                                writer.WriteLine($"{Damage[i, j]} {Names[i, j]}");
                                som += Damage[i, j];
                                somAoe += Damage[i, j] * Aoe[i, j];
                            }
                        }
                        dpm[i] = Math.Round((som * (60 / Rot[i])) / 1000000, 2);
                        dpmAoe[i] = Math.Round((somAoe * (60 / Rot[i])) / 1000000, 2);
                        writer.WriteLine($"\n{som}");
                        writer.WriteLine($"{dpm[i]} mi dpm");
                        writer.WriteLine($"Aoe Ranking: {dpmAoe[i]}");
                        writer.WriteLine($"{Math.Round(som / Rot[i], 2)} DPS");
                    }
                }

                writer.WriteLine();
            }
            Console.WriteLine("Teams Calc'ed!");
            TeamRank(dpm, teams, dpmAoe, Names, rank);
        }
        static void TeamRank(double[] dmg, int teamamount, double[] dmgAoe, string[,] Names, bool rank) // (dpm, amount of teams, aoedpm, names, fromDB?) ranks teams on their dpm. If used after teamcalc, also ranks Aoe.
        {
            string filePath = @"C:\Users\user\Desktop\projects\GENSHIN\product.txt";
            string template1, template2;
            double[] og = new double[teamamount + 1];
            double[] og2 = new double[teamamount + 1];
            if (dmgAoe == null) // only ranking 
            {
                double [] dmg1 = new double[teamamount + 1];
                for (int i = 0; i<teamamount; i++)
                {
                    Console.Write($"Team {i + 1} dmg: ");
                    dmg1[i] = double.Parse(Console.ReadLine());
                    dmg1.CopyTo(og, 0);
                }
                dmg1 = OrderGuideYou(teamamount, dmg1);
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    for (int i = 0; i < teamamount; i++)
                    {
                        for (int j = 0; j < teamamount; j++)
                        {
                            if (og[j] == dmg1[i])
                            {
                                writer.WriteLine($"{i + 1}. Team {j+1}\t{dmg1[i]} ({(Math.Round(dmg1[i] / dmg1[i + 1], 3) - 1) * 100}%)");
                            }
                        }
                    }
                }
                Console.WriteLine("Rankings Written!");
                Console.ReadLine();
            }
            else
            {
                for (int i = 0; i<dmg.Length; i++)
                {
                    dmg.CopyTo(og, 0);
                    dmgAoe.CopyTo(og2, 0);
                }
                dmg = OrderGuideYou(teamamount, dmg);
                dmgAoe = OrderGuideYou(teamamount, dmgAoe);
                using (StreamWriter writer = new StreamWriter(filePath, true)) // this is SO fucking unoptimized
                {
                    for (int i = 0; i < teamamount; i++)
                    {
                        for (int j = 0; j < teamamount; j++)
                        {
                            if (og[j] == dmg[i])
                            {
                                for (int k = 0; k < teamamount; k++)
                                {
                                    if (rank == true) // as of now if the team has the same damage, duplicates will be in the rankings, i'm too lazy to fix that right now so just do it manually (exclude the 2 between them)
                                    {
                                        template1 = $".t{j+1}. {dmg[i]}";
                                        template2 = $".t{k+1}. {dmgAoe[i]}";
                                    }
                                    else
                                    {
                                        template1 = $" {Names[j, 0]} {dmg[i]}";
                                        template2 = $" {Names[k, 0]} {dmgAoe[i]}";
                                    }
                                    if (og2[k] == dmgAoe[i])
                                    {
                                        writer.WriteLine($"{i + 1}.{template1} ({(Math.Round(dmg[i] / dmg[i + 1], 2) - 1) * 100}%)\t\t {i + 1}.{template2} ({(Math.Round(dmgAoe[i] / dmgAoe[i + 1], 3) - 1) * 100}%)");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Teams Ranked!");
            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\user\Desktop\projects\GENSHIN\product.txt";
            Console.WriteLine("Welcome to the Genshin Calc Helper!\n\nWhat do you want?\n1- Team Calculation (add)\n2- Ranking only (add)\n3- Team Calculation (database)\n4- Clear Contents");
            int menu = int.Parse(Console.ReadLine());
            int teams;
            do
            {
                switch (menu)
                {
                    case 1:
                        Console.Clear();
                        Console.Write("Team Calculation and Ranking\n\nAmount of teams: ");
                        teams = int.Parse(Console.ReadLine());
                        TeamCalc(teams, false);
                        break;
                    case 2:
                        Console.Clear();
                        Console.Write("Team Ranking\n\nAmount of teams: ");
                        teams = int.Parse(Console.ReadLine());
                        TeamRank(null, teams, null, null, false);
                        break;
                    case 3:
                        Console.Clear();
                        TeamCalc(17, true);
                        break;
                    case 4:
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {

                        }
                        Console.WriteLine("Contents Cleared! Type another number to continue or close the application.");
                        menu = int.Parse(Console.ReadLine());
                        break;
                    default:
                        Console.WriteLine("Erm... what the heavenly principplesss??");
                        break;
                }
            } while (true);
        }
        static double GetAoe(string name) // (character name) QoL, saving time while placing teams
        {
            string[] names = { "albedo", "alhaitham", "arle", "ayaka", "ayato", "beidou", "benny", "childe", "cyno", "dehya", "diluc", "eula", "fish", "furina", "gorou", "hutao", "itto", "kaeya", "kazuha", "kuki", "lyney", "nahida", "navia", "neuvi", "raiden", "rosaria", "sucrose", "venti", "wanderer", "xiao", "xl", "xq", "yae", "yaoyao", "yelan", "yunjin", "zhong" };
            double[] aoe = { 1.5, 2, 2.5, 3, 2, 3, 3, 2.2, 2.5, 3, 2, 2, 1.2, 1.75, 2, 1.5, 2.7, 2, 3, 1, 1.5, 2.2, 1.5, 4, 1, 2.5, 4, 4, 2, 4, 3, 1.5, 1.5, 1, 1.5, 1, 3 };
            for (int i = 0; i < names.Length; i++)
            {
                if (name == names[i])
                {
                    return aoe[i];
                }
            }
            return -1;

        }
        static double GetDmg(string name, int v) // (character name, version) mostly for complete rankings. Quite messy
        {
            string[] names = {                  "albedo",                    "alhaitham",                             "arle",                "ayaka",                       "ayato",                   "beidou",     "benny",          "childe",               "cyno",              "dehya",                    "diluc",         "eula",          "fish",                   "furina",             "gorou",                   "hutao",                     "itto",          "kaeya",                    "kazuha",                           "kuki",                     "lyney",                 "nahida",                  "navia",               "neuvi",                   "raiden",            "rosaria",           "sucrose",                "venti",                  "wanderer",                    "xiao",                        "xl",                      "xq",                   "yae",           "yaoyao",              "yelan",         "yunjin",        "zhong" };
            double[,] dmg = { {/*albedo monogeo*/276233, /*alhaitham unbuffed*/ 202432, /*arle double hydro C2 (TBA)*/782848, /*ayaka frozen*/480611, /*ayato international*/318283, /*beidou (no teams)*/ 0, /*benny*/5000, /*Childe*/ 459590, /*Cyno Chaos*/ 329158, /*Burgeons*/ 246664,    /*Diluc Plunge*/ 846330, /*Eula*/ 212000, /*Fish*/ 120798, /*C2 Furina 2Hyd*/ 583861, /* Full Geo */ 11726,     /*Hutao 2Hyd*/ 906933,    /*C1 Itto mono*/ 869305, /*kayak*/ 15000,   /*kazuha burstless*/59283,        /*quickbloom (10)*/ 348891,  /*Lyney Offensive*/ 734039,   /*NA quicken*/ 134097, /*in neuvi GdnTp*/220700, /*Hypercarry*/ 1148145, /*leo's 21 seeds*/ 715124,   /*205% ER*/ 97590,  /* Leo's */  28334,   /* Asia pyro */ 62473, /*NA spam yunjin */ 526900,  /* Leo's ceiling */ 741750, /* EU international */ 571316,   /* C6 Db Hyd */ 245519,  /* ElCharged */ 136347, /* why */ 23491,  /* Db Hydro */ 342151, /* why */ 6762, /* lol */ 5024},  
                              { /*albedo benny*/ 262718,  /*alhaitham buffed*/  431695, /*arle Ceiling (benny) C2  */ 1544344, /*ayaka cryo */409904, /*ayato bloom*/        469662,                       0,          0,               0,        /*Cyno TF*/  316446,              0,      /*Diluc National*/  421096,          0,               0,         /*C2 Furina */  482336,   /* Benny */  10696, /*Hutao ben kaz*/  1552318, /*C1 Itto benny*/  968990,           0,       /*kazuha burstful*/ 76759, /*furina quickbloom (7)*/  222361, /*Lyney Defensive*/  614196, /*Leo onfield*/  61983, /*in neuvi petra*/ 143783,      /*Vape*/  1129006,                    0,                    0,                   0,                       0,    /* CA spam benny */  570258,                      0,     /* AS international */  296445,   /* C6 Leo's */  173045,                  0,                0,                     0,                0,              0},
                              {                  0,                             0,                    /*Leo's Arle*/  810193,                 0,                             0,                            0,          0,               0,                     0,                   0,                          0,               0,               0,     /*Leo's Furina */   403710,                0,                        0,                          0,                0,/*both double swirl btw*/  0,                                0,                           0,                       0,   /*as Main dps (?)*/  889121,  /* Leo's */   743745,                     0,                    0,                   0,                       0,                         0,                           0,    /*   AS Mono Pyro   */   181367, /* C2 Db Hyd */   150375,                  0,                0,                     0,                0,              0}
                             };
            for (int i = 0; i < names.Length; i++)
            {
                if (name == names[i])
                {
                    return dmg[v, i];
                }
            }
            return 0;

        }

        static double[,] Teams() // preset teams
        {
            /* Team ID, rot lenght, dmg1, aoe1, dmg2, aoe2, dmg3, aoe3, dmg4, aoe4 */
            double[,] everything = {{1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15, 16, 17},
                                    {22, 22, 22, 25, 25, 25, 23, 23, 22, 20, 22, 22, 22, 25, 25, 20, 22},
                                    {GetDmg("hutao", 0), GetDmg("neuvi", 0),    GetDmg("itto", 0),  GetDmg("childe", 0), GetDmg("alhaitham", 1), GetDmg("arle", 0),  GetDmg("ayaka", 0), GetDmg("ayato", 0),  GetDmg("lyney", 0), GetDmg("wanderer", 0), GetDmg("xiao", 0),   GetDmg("raiden", 0), GetDmg("neuvi", 2),  GetDmg("arle", 2),   GetDmg("eula", 0), GetDmg("diluc", 1), GetDmg("diluc", 0)   },
                                    {GetAoe("hutao"),    GetAoe("neuvi"),       GetAoe("itto"),     GetAoe("childe"),    GetAoe("alhaitham"),    GetAoe("arle"),     GetAoe("ayaka"),    GetAoe("ayato"),     GetAoe("lyney"),    GetAoe("wanderer"),    GetAoe("xiao"),      GetAoe("raiden"),    GetAoe("neuvi"),     GetAoe("arle"),      GetAoe("eula"),    GetAoe("diluc"),    (GetAoe("diluc") + 1)},
                                    {GetDmg("xq", 0),    GetDmg("rosaria", 0),  GetDmg("albedo", 0),GetDmg("xl", 0),     GetDmg("furina", 1),    GetDmg("xq", 2),   (GetDmg("xq", 1)/2), GetDmg("xl", 1),     GetDmg("xl", 2),    GetDmg("yunjin", 0),   GetDmg("furina", 2), GetDmg("xq", 1),     GetDmg("furina", 1), GetDmg("xq", 1),     0,                 GetDmg("xq", 1),    GetDmg("xq", 1),     },
                                    {GetAoe("xq"),       GetAoe("rosaria"),     GetAoe("albedo"),   GetAoe("xl"),        GetAoe("furina"),       GetAoe("xq"),       GetAoe("xq"),       GetAoe("xl"),        GetAoe("xl"),       GetAoe("yunjin"),      GetAoe("furina"),    GetAoe("xq"),        GetAoe("furina"),    GetAoe("xq"),        0,                 GetAoe("xq"),       GetAoe("xq"),        },
                                    {GetDmg("yelan", 0), GetDmg("navia", 0),    GetDmg("gorou", 0), GetDmg("kazuha", 1), GetDmg("kuki", 1),      GetDmg("furina", 0),GetDmg("venti", 0), GetDmg("benny", 0),  GetDmg("venti", 0), 0,                     GetDmg("benny", 0),  GetDmg("nahida", 1), GetDmg("zhong", 0),  GetDmg("sucrose", 0),0,                 0,                  0                    },
                                    {GetAoe("yelan"),    GetAoe("navia"),       GetAoe("gorou"),    GetAoe("kazuha"),    GetAoe("kuki"),         GetAoe("furina"),   GetAoe("venti"),    GetAoe("benny"),     GetAoe("venti"),    0,                     GetAoe("benny"),     GetAoe("nahida"),    GetAoe("zhong"),     GetAoe("sucrose"),   0,                 0,                  0                    },
                                    {GetDmg("zhong", 0), GetDmg("kazuha", 1),   GetDmg("zhong", 0), GetDmg("benny", 0),  GetDmg("yaoyao", 0),    0,                  0,                  GetDmg("sucrose", 0),0,                  0,                     0,                   GetDmg("zhong", 0),  0,                   0,                   0,                 0,                  0                    },
                                    {GetAoe("zhong"),    GetAoe("kazuha"),      GetAoe("zhong"),    GetAoe("benny"),     GetAoe("yaoyao"),       0,                  0,                  GetAoe("sucrose"),   0,                  0,                     0,                   GetAoe("zhong"),     0,                   0,                   0,                 0,                  0                    }     
                                    };
            return everything;
        }
    }
}
