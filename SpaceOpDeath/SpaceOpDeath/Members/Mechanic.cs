using System;
using SpaceOpDeath.Ships;
using SpaceOpDeath.Utils;

namespace SpaceOpDeath.Members
{
    class Mechanic : Member
    {
        public Mechanic()
            :base()
        {
            _type = MemberType.Mechanic;   
        }

        public override void Action()
        {
            base.Action();

            int lifeToGive = int.Parse( Alea.Config.Get( "MechanicActionRepair" )["Value"].Value );
            module.ship.AddLifepoints( 1 );
        }

        public override string ToStringAction()
        {
            string res = "Donne 1 point de vie au vaisseau";
            return res;
        }
    }
}