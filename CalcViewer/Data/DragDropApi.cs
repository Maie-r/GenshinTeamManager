using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenshinTeamCalc;
using MaieBlazorLib;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace CalcViewer.Data
{
    // This is only here for some backups not to break

    public class DragDropApi
    {
        // 0, 1 For regular teams, 2, 3 for team pairs
        static int[] dragged = new int[] { -1, -1, -1, -1 };
        static int[] over = new int[] { -1, -1, -1, -1 };
        // For team pairs switching
        static int[] draggedteam = new int[] { -1, -1 };
        static int[] overteam = new int[] { -1, -1 };

        public static void Clear()
        {
            dragged[0] = dragged[1] = dragged[2] - dragged[3] -1;
            over[0] = over[1] = over[2] = over[3] -1;
            draggedteam[0] = draggedteam[1] = -1;
            overteam[0] = overteam[1] = -1;
        }

        // --------------------------- CHARACTER DRAG ----------------------------
        public static void StartDrag(int i)
        {
            dragged[0] = i;
        }
        public static void StartDrag(Character chara, Team t)
        {
            //Logger.LogInformation($"Started drag with {chara.name}!");
            dragged[0] = t.characters.IndexOf(chara);
        }

        public static void StartDrag(Character chara, Team t, int i) // (0 for select, 1 for compare) Characacter Draggin'
        {
            //Logger.LogInformation($"Started drag with {chara.name}!");
            dragged[i] = t.characters.IndexOf(chara);
        }
        public static void StopDrag(Team selected)
        {
            if (over[0] >= 0)
            {
                if (over[0] != dragged[0] && dragged[0] >= 0)
                {
                    Debug.WriteLine($"Switcheroo!");
                    Character temp = selected.characters[over[0]];
                    selected.characters[over[0]] = selected.characters[dragged[0]];
                    selected.characters[dragged[0]] = temp;
                }
            }
            Clear();
        }
        public static void StopDrag(Team selected, int type)
        {
            if (over[type] >= 0)
            {
                if (over[type] != dragged[type] && dragged[type] >= 0)
                {
                    Debug.WriteLine($"Switcheroo!");
                    Character temp = selected.characters[over[type]];
                    selected.characters[over[type]] = selected.characters[dragged[type]];
                    selected.characters[dragged[type]] = temp;
                }
            }
            Clear();
        }

        public static void OverHere(int i)
        {
            over[0] = i;
        }
        public static void OverHere(Character overchar, Team t, int i) // (0 for select, 1 for compare)
        {
            Debug.WriteLine($"Mouse over {overchar.name}!");
            over[i] = t.characters.IndexOf(overchar);
        }

        public static void OuttaHere()
        {
            Debug.WriteLine($"Mouse out!");
            over[0] = over[1] = over[2] = over[3] = -1;
        }

        // --------------------------- TEAM DRAG ----------------------------

        public static void StartDragTeam(int team, int type)
        {
            draggedteam[type] = team;
            Debug.WriteLine("Draggin team " + team + "" + type);
        }

        public static void StopDragTeam(TeamPair selected, int type)
        {
            if (overteam[type] >= 0)
            {
                if (overteam[type] != draggedteam[type] && draggedteam[type] >= 0)
                {
                    Team temp = selected.teams[overteam[type]];
                    selected.teams[overteam[type]] = selected.teams[draggedteam[type]];
                    selected.teams[draggedteam[type]] = temp;
                }
            }
            Clear();
        }

        public static void OverHereTeam(int team, int type) // selected pair: i = 0; compaired pair: i = 1
        {
            overteam[type] = team;
            Debug.WriteLine("Over team " + team + "" + type);
        }

        public static void OuttaHereTeam()
        {
            overteam[0] = overteam[1] = -1;
        }
    }
    //
}
