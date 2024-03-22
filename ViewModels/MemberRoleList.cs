using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;
using BrainBoost.Parameter;

namespace BrainBoost.ViewModels
{
    public class MemberRoleList
    {
        Member member {get;set;} = new();

        Member_Role member_Role {get;set;} = new();
    }
}