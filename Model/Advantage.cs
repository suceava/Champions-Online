using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CharacterBuilder.ChampionsOnline.Model
{
    public class Advantage:Statistic
    {
        public int Cost { get; set; }
        public string RequiredAdvantage { get; set; }
    }
}
