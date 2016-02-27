using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpaceOpDeath.Utils
{
    class Alea
    {
        public static XmlDocument AleaDocument = new XmlDocument();
        public static Alea Config = new Alea();

        private Alea()
        {
            AleaDocument.Load( "Config/Alea.xml" );
        }

        public XmlAttributeCollection Get( string key )
        {
            string result = string.Empty;

            XmlNode node = AleaDocument.SelectSingleNode( string.Format(@"/Config/Param[@Index=""{0}""]", key) );

            if ( node == null )
            {
                throw new SpaceOpDeathException( "La configuration des variables aléatoires n'est pas bonne. Le jeu ne peut pas continuer." );
            }

            return node.Attributes;
        }
    }
}
