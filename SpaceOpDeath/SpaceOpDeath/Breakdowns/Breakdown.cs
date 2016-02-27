using System;
using SpaceOpDeath.Utils;
using System.Xml;

namespace SpaceOpDeath.Breakdowns
{
    /// <summary>
    /// Type de la panne.
    /// </summary>
    public enum BreakdownType
    {
        Small = 1,
        Medium = 2,
        Big = 3
    }

    /// <summary>
    /// La classe Breakdown gère une panne sur un module du vaisseau.
    /// La panne a un type, un effet à appliquer en fin de tour
    /// et peut être encore active ou non.
    /// </summary>
    public class Breakdown
    {
        /// <summary>
        /// Type de la panne.
        /// </summary>
        private BreakdownType _type;

        /// <summary>
        /// Si la panne est encore active ou non.
        /// </summary>
        public bool active = true;

        /// <summary>
        /// La puissance de la panne (entre 1 et 35).
        /// Plus précisement, les points de réparation
        /// qu'il faut pour la réparer.
        /// </summary>
        public int strength
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructeur de la panne. Il faut un type de faille
        /// afin de déterminer sa puissance.
        /// </summary>
        /// <param name="type"></param>
        public Breakdown( BreakdownType type )
        {
            _type = type;

            XmlAttributeCollection config = Alea.Config.Get( "Breakdown" + type + "Range" );
            strength = Dice.Rand.Next( int.Parse( config["Min"].Value ), int.Parse( config["Max"].Value ) );
        }

        /// <summary>
        /// Récupère le type de panne.
        /// </summary>
        /// <returns></returns>
        public BreakdownType GetBreakdownType()
        {
            return _type;
        }

        /// <summary>
        /// Retire de la puissance à la panne.
        /// </summary>
        /// <param name="v"></param>
        public void SubStrength( int v )
        {
            strength -= v;
            if( strength < 0 )
            {
                strength = 0;
            }
        }

        /// <summary>
        /// Active l'effet de la panne.
        /// </summary>
        public void Trigger()
        {

        }

        /// <summary>
        /// J'ai vraiment besoin d'expliquer ce que ça fait ? --'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "La panne est de type " + _type + " et a une puissance de " + strength;
        }

        /// <summary>
        /// Retourne les infos essentielles concernant la panne.
        /// </summary>
        /// <returns></returns>
        public string ToInfoString()
        {
            return _type + " (puissance : " + strength + ")";
        }
    }
}