using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcViewer;
using GenshinTeamCalc;

namespace CalcViewer.Data
{
    public class CalcService : ICalc
    {
        private Calc _calc;
        private Team _selected;

        public CalcService()
        {
            //try
            //{
                _calc = new Calc(true);
            /*}
            catch (FileNotFoundException db)
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\DB");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\DB\db.txt", "Amber, pyro,1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-1-12.png;\r\nArlecchino, pyro,2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-32-8.png;\r\nBennett, pyro,3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-13.png;\r\nChevreuse, pyro,1.20, https://cdn3.emoji.gg/emojis/2257-chevreuse-delicious.png;\r\nDehya, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-5.png;\r\nDiluc, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-3.png;\r\nKlee, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-1.png;\r\nGaming, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-1.png;\r\nHutao, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-5-6.png;\r\nLyney, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-5.png;\r\nMavuika, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-11.png;\r\nPmc, pyro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nThoma, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-10-11.png;\r\nXiangling, pyro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-16.png;\r\nXinyan, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-3-11.png;\r\nYanfei, pyro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-8.png;\r\nYoimiya, pyro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-5.png;\r\nAyato, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-2.png;\r\nBarbara, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-11.png;\r\nCandace, hydro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-11.png;\r\nChilde,hydro, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-8.png;\r\nFurina, hydro, 1.75, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-6.png;\r\nHmc, hydro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nKokomi, hydro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-8.png;\r\nMona, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-12.png;\r\nMualani, hydro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-15.png;\r\nNeuvillette, hydro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings--set-26-15.png;\r\nNilou, hydro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-1.png;\r\nSigewinne, hydro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-8.png;\r\nXingqiu, hydro,1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-16.png;\r\nYelan, hydro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-1.png;\r\nAmc, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nChasca,anemo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-5.png;\r\nFaruzan, anemo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-16.png;\r\nHeizou, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-16-4.png;\r\nJean, anemo, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-16.png;\r\nKazuha,anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-7-8.png;\r\nLynette, anemo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-11.png;\r\nSayu, anemo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-9.png;\r\nSucrose, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-12.png;\r\nVenti, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-14-12.png;\r\nWanderer, anemo, 2.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-3.png;\r\nXianyun, anemo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-12.png;\r\nXiao, anemo, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-14.png;\r\nBeidou,electro, 2.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-15.png;\r\nClorinde, electro, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-13.png;\r\nCyno, electro,2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-5.png;\r\nDori, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-17-9.png;\r\nEmc, electro, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nFischl,electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-16.png;\r\nKeqing,electro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-7.png;\r\nKuki, electro,1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-15-11.png;\r\nLisa, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-2.png;\r\nOroron, electro, 4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-4.png;\r\nRaiden, electro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-4.png;\r\nRazor, electro, 1.30, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-18-14.png;\r\nSara, electro, 2.25, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-9-9.png;\r\nSethos, electro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-33-14.png;\r\nYae, electro, 1.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-13-9.png;\r\nAlhaitham, dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-13.png;\r\nBaizhu, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-3.png;\r\nCollei, dendro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-13.png;\r\nDmc, dendro, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nEmilie, dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-15.png;\r\nKaveh, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-7.png;\r\nKinich,dendro, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-13.png;\r\nKirara, dendro, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-24-3.png;\r\nNahida, dendro,4.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-19-5.png;\r\nTighnari, dendro, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-15.png;\r\nYaoyao,dendro, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-2.png;\r\nAyaka, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-8-3.png;\r\nCharlotte, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-16.png;\r\nChongyun, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-14.png;\r\nCitlali, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-11.png;\r\nDiona, cryo, 1.40, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-20-11.png;\r\nEula, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-2.png;\r\nFreminet, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-28-1.png;\r\nGanyu, cryo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-4-9.png;\r\nKaeya, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-25-4.png;\r\nLayla, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-23-13.png;\r\nMika, cryo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-22-16.png;\r\nQiqi, cryo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-2-9.png;\r\nRosaria, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-6-13.png;\r\nShenhe, cryo, 2.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-30-4.png;\r\nWriothesley, cryo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-1.png;\r\nAlbedo,geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-9.png;\r\nChiori, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-31-16.png;\r\nGmc, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/dancing-beasts-and-soaring-kites--series-emojis-13.png;\r\nGorou, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-5.png;\r\nKachina, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-35-3.png;\r\nItto, geo, 2.70, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-11-2.png;\r\nNavia, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-34-8.png;\r\nNingguang, geo, 1.20, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-21-10.png;\r\nNoelle, geo, 3.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintingsset-27-2.png;\r\nXilonen, geo, 2.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-36-12.png;\r\nYunjin, geo, 1.50, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-6.png;\r\nZhongli, geo, 3.00, https://cdn.wanderer.moe/genshin-impact/emotes/paimon-s-paintings-set-12-9.png\r\n"); 
                File.Create(AppDomain.CurrentDomain.BaseDirectory + @"\DB\teams.txt");
                //try
                //{
                    _calc = new Calc(true);
                }
                catch
                {
                    throw new FileNotFoundException("Files not found", db);
                }
        }*/
        }

        public Task<Calc> Start()
        {
            return Task.FromResult(_calc);
        }

        public async Task Reload()
        {
            _selected = null;
            await Task.Run(() => _calc = new Calc(true));
        }

        public void Setlect(Team team)
        {
            _selected = team;
        }

        public Team Getlect()
        {
            return _selected;
        }
    }
}
