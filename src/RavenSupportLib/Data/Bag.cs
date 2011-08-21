using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GeniusCode.RavenDb.Data
{
    public class Bag
    {
        public Bag()
        {
                Expando = new ExpandoObject();
        }


        [JsonProperty]
        private ExpandoObject Expando { get; set; }

        [JsonIgnore]
        public dynamic Data
        {
            get
            {
                return Expando;
            }
        }
    }
}
