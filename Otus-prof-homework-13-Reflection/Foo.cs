using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Otus_prof_homework_13_Reflection
{    
    public class Foo
    {
        [XmlAttribute]
        [JsonConverter(typeof(MyConverter))]
        public int i1, i2, i3, i4, i5;
        public Foo Get() => new()
        {
            i1 = 1,
            i2 = 2,
            i3 = 3,
            i4 = 4,
            i5 = 5
        };
    }
}
