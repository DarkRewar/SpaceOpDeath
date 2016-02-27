using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceOpDeath.Breakdowns;
using SpaceOpDeath.Utils;
using SpaceOpDeath.Members;

namespace SpaceOpDeath.Ships
{
    class Ship
    {
        private int _lifepoints;
        private List<Module> _modules;

        public Ship()
        {
            _lifepoints = Dice.Rand.Next( 2, 6 );
            _modules = new List<Module>();            
        }

        public int GetLifepoints()
        {
            return _lifepoints;
        }

        public void AddLifepoints( int lifepoints )
        {
            _lifepoints += lifepoints;

            int maxLifepoints = int.Parse(Alea.Config.Get( "ShipMaxHealth" )["Value"].Value);
            if( lifepoints > maxLifepoints )
            {
                _lifepoints = maxLifepoints;
            } else
            {
                Console.WriteLine( "Le vaisseau a gagné " + lifepoints + " point de vie." );
            }
        }

        public List<Module> GetModules()
        {
            return _modules;
        }

        public Ship AddModule( Module module )
        {
            _modules.Add( module );
            module.ship = this;
            return this;
        }

        public Ship AddModules( params Module[] modules )
        {
            modules.Select( x => AddModule( x ) );
            return this;
        }

        /// <summary>
        /// Retire le module voulu du vaiseau.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public Ship RemoveModule( Module module )
        {
            module.ship = null;
            _modules.Remove( module );
            return this;
        }

        /// <summary>
        /// Retire du vaisseau le module se trouvant à un index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Ship RemoveModule( int index )
        {
            _modules.RemoveAt( index );
            return this;
        }

        /// <summary>
        /// Récupère tous les membres du vaisseau.
        /// </summary>
        /// <returns></returns>
        public List<Member> GetMembers()
        {
            List<Member> members = new List<Member>();
            foreach( Module module in _modules )
            {
                members.AddRange( module.GetMembers() );
            }
            return members;
        }

        /// <summary>
        /// Place aléatoirement une panne dans le vaisseau.
        /// Si un module a déjà une panne, on en cherche un autre.
        /// Si tous les modules sont en panne. On n'ajoute pas la panne.
        /// </summary>
        /// <param name="breakdown"></param>
        /// <returns></returns>
        public Ship PlaceBreakdown( Breakdown breakdown )
        {
            bool placed = false;
            bool fullBreakdowns = true;

            foreach( Module module in _modules )
            {
                if( module.breakdown == null )
                {
                    fullBreakdowns = false;
                    break;
                }
            }

            if( fullBreakdowns == false )
            {
                while ( !placed )
                {
                    Module module = _modules.ElementAt( Dice.Rand.Next( 0, _modules.Count ) );
                    if ( module.breakdown == null )
                    {
                        module.breakdown = breakdown;
                        placed = true;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Vérifie si toutes les pannes sont réparées.
        /// </summary>
        public void CheckAllBreakdowns()
        {
            foreach( Module module in _modules )
            {
                if( module.breakdown != null && module.breakdown.strength <= 0 )
                {
                    module.breakdown = null;
                }
            }
        }
        
        /// <summary>
        /// Déclenche et applique les effets des pannes.
        /// </summary>
        public void ApplyBreakdownEffects()
        {
            foreach ( Module module in _modules )
            {
                if ( module.breakdown != null && module.breakdown.strength > 0 )
                {
                    module.breakdown.Trigger();
                }
            }
        }

        public void DisplayStatus()
        {
            Console.WriteLine( "Le vaisseau a " + _lifepoints + " points de vie." );
            DisplayAllBreakdowns();
        }

        public void DisplayAllBreakdowns()
        {
            foreach( Module module in _modules )
            {
                if( module.breakdown != null )
                {
                    Console.WriteLine( 
                        "Il y a une panne de type " + module.breakdown.GetBreakdownType() + 
                        " (puissance : " + module.breakdown.strength + ")" +
                        " dans le module " + module.name + "."
                    );
                }
            }
        }

        public void DisplayAllMembers()
        {
            foreach ( Module module in _modules )
            {
                foreach( Member member in module.GetMembers() )
                {
                    Console.WriteLine(
                        "- " + member.ToStringInfo() +
                        " se trouve dans le module " + module.name + "."                        
                    );
                }
            }
        }

        public override string ToString()
        {
            string str = "Il y a ";
            str += _modules.Count;
            str += " modules.\n";

            foreach( Module m in _modules )
            {
                str += m;
            }
            return str;
        }
    }
}
