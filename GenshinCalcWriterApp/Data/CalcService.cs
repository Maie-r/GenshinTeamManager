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
        public static string hashmemstring;
        static int[] memory = { -1, -1}; // 0, 1: Team table
        //private static TeamCondensed memory2; // Team manage
        private static TeamPair[] teamPairs = new TeamPair[2]; // 0: selected, 1: compare

        public static int NormalSelected
        {
            get { return memory[0]; }
            set { memory[0] = value; }
        }

        public static int NormalCompare
        {
            get { return memory[1]; }
            set { memory[1] = value; }
        }

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
            /*int[] hashmemory = new int[2]; // 0: tableselect 1: tablecompare
            for (int i = 0; i < memory.Length; i++)
            {
                hashmemory[i] = Calc.teams.IndexOf(memory[i]);
            }
            for (int i = 0; i < 2; i++)
            {
                if (teamPairs[i] != null)
                {
                    pairnames[i] = teamPairs[i].Name;
                }
                else
                {
                    Debug.WriteLine("Stored team pairs null:");
                    Debug.WriteLine(teamPairs[i] == null);
                }
            }*/
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
                if (teamPairs[i] != null)
                {
                    teamPairs[i] = TeamPairer.GetPairByName(teamPairs[i].Name);
                }
            }
            //Debug.WriteLine(memory[0].characters[0].dmg);
        }
    }
}
