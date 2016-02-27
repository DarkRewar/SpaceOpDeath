using SpaceOpDeath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpDeath.Members
{
    class Doctor : Member
    {
        public Doctor()
            :base()
        {
            _type = MemberType.Doctor;
        }

        /// <summary>
        /// La méthode va executer le pouvoir spécial du membre
        /// concerné.
        /// </summary>
        public override void Action()
        {
            base.Action();

            int lifeToHeal = int.Parse( Alea.Config.Get( "DoctorActionHeal" )["Value"].Value );
            List<Member> members = module.ship.GetMembers();
            foreach( Member member in members )
            {
                member.AddLifepoints(1);
            }
        }

        public override string ToStringAction()
        {
            string res = "Donne 1 point de vie à tous les membres de l'équipage";
            return res;
        }
    }
}
