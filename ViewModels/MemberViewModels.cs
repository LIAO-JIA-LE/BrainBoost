using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;

namespace BrainBoost.ViewModels
{
    public class MemberViewModels
    {
        public string search {get;set;}
        public Forpaging forpaging {get;set;} = new();
        public List<Member> member {get;set;} = [];
    }
}