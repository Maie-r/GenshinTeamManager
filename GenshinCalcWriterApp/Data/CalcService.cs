using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CalcViewer;
using GenshinTeamCalc;

namespace CalcViewer.Data
{
    public static class CalcService
    {
        static int[] hashmemory;
        public static string hashmemstring;
        private static int[] memory = { -1, -1}; // 0, 1: Team table. 2, 3: team pairs
        //private static TeamCondensed memory2; // Team manage
        private static TeamPair[] teamPairs = new TeamPair[2]; // 0: selected, 1: compare

        public static int NormalSelected
        {
            get { return memory[0]; }
            set {
                /*if (memory[0] != null && value != null)
                    Debug.WriteLine("same: " + (memory[0] == value) + $" {memory[0].name} {value.name}");
                else
                    Debug.WriteLine("1" + memory[0] + " " + value);*/
                memory[0] = value; }
        }

        public static int NormalCompare
        {
            get { return memory[1]; }
            set { memory[1] = value; }
        }

        /*public static int PairSelected
        {
            get { return memory[2]; }
            set { memory[2] = value; }
        }

        public static int PairCompare
        {
            get { return memory[3]; }
            set { memory[3] = value; }
        }*/

        public static void PairSelected(TeamPair t)
        {
            teamPairs[0] = t;
        }

        public static TeamPair PairSelected()
        {
            return teamPairs[0];
        }

        public static void PairCompare(TeamPair t)
        {
            teamPairs[1] = t;
        }

        public static TeamPair PairCompare()
        {
            return teamPairs[1];
        }

        public static void Clear()
        {
            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = -1;
            }
            for (int i = 0; i < teamPairs.Length; i++)
            {
                teamPairs[i] = null;
            }
        }

        public static void StorePositions()
        {
            hashmemory = new int[4]; // 0: tableselect 1: tablecompare 3: pairselect1 4: pairselect2 5: paircompare1 6: paircompare2
            /*for (int i = 0; i < memory.Length; i++)
            {
                hashmemory[i] = Calc.teams.IndexOf(memory[i]);
            }*/
            for (int i = 0; i < 2; i++)
            {
                if (teamPairs[i] != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        /*Debug.WriteLine(hashmemory == null);
                        Debug.WriteLine(Calc.teams == null);
                        Debug.WriteLine(teamPairs == null);
                        Debug.WriteLine(teamPairs[0] == null);
                        Debug.WriteLine(teamPairs[1] == null);*/

                        hashmemory[(j + (i * 2))] = Calc.teams.IndexOf(teamPairs[i].teams[j]);
                    }
                }
                else
                {
                    Debug.WriteLine("Stored team pairs null:");
                    Debug.WriteLine(teamPairs[i] == null);
                }
            }
        }

        public static void RestorePositions()
        {
            //Debug.WriteLine(memory[0].characters[0].dmg);
            /*for (int i = 0; i < memory.Length; i++)
            {
                if (hashmemory[i] == -1)
                {
                    memory[i] = null;
                }
                else
                {
                    memory[i] = Calc.teams[hashmemory[i]];
                }
            }*/
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (hashmemory[(j + (i * 2))] == -1)
                    {
                        /*try
                        {
                            teamPairs[i][j] = null;
                        }
                        catch
                        {

                        }*/
                    }
                    else
                    {
                        if (teamPairs[i] == null)
                        {
                            //teamPairs[i] = new TeamPair();
                        }
                        teamPairs[i].teams[j] = Calc.teams[hashmemory[(j + (i * 2))]];
                    }
                    
                }
            }
            //Debug.WriteLine(memory[0].characters[0].dmg);
        }
    }
}
