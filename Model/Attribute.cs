using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CharacterBuilder.ChampionsOnline.Model
{
    public class Attribute:Statistic
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Presence { get; set; }
        public int Ego { get; set; }
        public int Constitution { get; set; }
        public int Recovery { get; set; }
        public int Endurance { get; set; }
        
    }
}
