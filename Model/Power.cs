using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CharacterBuilder.ChampionsOnline.Model
{
    public class Power:Statistic
    {
        public int InsetRequirement { get; set; }
        public int OutsetRequirement { get; set; }

        public List<Advantage> Advantages { get; set; }

        public HashSet<string> Tags { get; set; }

        public int FloatingGroupPower { get; set; }

        //  This is used to group the powers by a set
        public string PowerType { get; set; }

    
        public Power()
        {
            Tags = new HashSet<string>();
            Advantages = new List<Advantage>();
        }
    }


}
