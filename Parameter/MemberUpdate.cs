using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Parameter
{
    public class MemberUpdate
    {
        public string member_name { get; set; }
        public IFormFile file{ get; set; }
        public string member_photo ;
        public int member_id ;
    }
}