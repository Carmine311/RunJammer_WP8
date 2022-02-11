using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Common.Model.Implementation;

namespace RunJammer.WP.Model
{
    [Table]
    [DataContract]
    public class RunJammerUser : LocalEntityBase
    {
        [Column]
        [DataMember]
        public override int ID { get; set; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        [IgnoreDataMember]
        public override int LocalID { get; set; }

        [Column]
        [DataMember]
        public string UserID { get; set; }
    }
}
