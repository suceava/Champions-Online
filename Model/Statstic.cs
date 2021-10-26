using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CharacterBuilder.ChampionsOnline.Model
{
    public class Statistic
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }

        public string Code { get; set; }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }

}
