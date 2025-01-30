using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcViewer;
using GenshinTeamCalc;

namespace CalcViewer.Data
{
    public interface ICalc
    {
        Task<Calc>? Start();
        Task Reload();
        void Setlect(Team team);
        Team Getlect();
    }
}
