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
                BackingExpando = new ExpandoObject();
        }


        [JsonProperty]
        private ExpandoObject BackingExpando { get; set; }

        [JsonIgnore]
        public dynamic BagData
        {
            get
            {
                return BackingExpando;
            }
        }
    }
}
