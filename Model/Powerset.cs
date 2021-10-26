using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CharacterBuilder.ChampionsOnline.Model
{
    public class Powerset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public string Code { get; set; }
        public string PowersetGroup { get; set; }

        public List<Power> Powers { get; set; }

        public Powerset()
        {
            Powers = new List<Power>();
        }

    }
}
