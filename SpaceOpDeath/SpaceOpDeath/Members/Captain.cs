using SpaceOpDeath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpDeath.Members
{
    class Captain : Member
    {
        public Captain()
            :base()
        {
            _type = MemberType.Captain;
        }

        /// <summary>
        /// L'action du Capitaine.
        /// Donne X dé à tout les membres d'équipage.
        /// </summary>
        public override void Action()
        {
            base.Action();

            int dicesToGive = int.Parse( Alea.Config.Get( "CaptainActionDicesToGive" )["Value"].Value );
            foreach( Member member in module.ship.GetMembers() )
            {
                member.AddDices( dicesToGive );
            }
        }

        public override string ToStringAction()
        {
            string res = "Donne 1 dé à tous les membres d'équipage";
            return res;
        }
    }
}
