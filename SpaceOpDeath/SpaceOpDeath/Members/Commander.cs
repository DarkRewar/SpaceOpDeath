using SpaceOpDeath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpDeath.Members
{
    class Commander : Member
    {
        public Commander()
            :base()
        {
            _type = MemberType.Commander;
        }

        public override void Action()
        {
            base.Action();

            int pointsToRepair = int.Parse( Alea.Config.Get( "CommanderActionRepair" )["Value"].Value );
            module.Repair( pointsToRepair );
        }

        public override string ToStringAction()
        {
            string res = "Peut fournir 10 points de réparation supplémentaire";
            return res;
        }
    }
}
