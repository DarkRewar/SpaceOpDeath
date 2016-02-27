using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SpaceOpDeath.Breakdowns;

namespace SpaceOpDeath.Utils
{
    class Tours
    {
        public static XmlDocument ToursDocument = new XmlDocument();

        public Tours( string pathFile )
        {
            ToursDocument.Load( pathFile );
        }

        private XmlNode Get( string path )
        {
            string result = string.Empty;

            XmlNode node = ToursDocument.SelectSingleNode( path );

            if ( node == null )
            {
                throw new SpaceOpDeathException( "La configuration des tours n'est pas bonne. Le jeu ne peut pas continuer." );
            }

            return node;
        }

        public List<Breakdown> GetTourBreakdowns( int tour )
        {
            List<Breakdown> breakdownsList = new List<Breakdown>();

            XmlNode small = Get( string.Format( @"/Tours/Tour[@Number=""{0}""]/Pannes/Panne[@Type=""{1}""]", tour, (int)BreakdownType.Small ) );
            XmlNode medium = Get( string.Format( @"/Tours/Tour[@Number=""{0}""]/Pannes/Panne[@Type=""{1}""]", tour, (int)BreakdownType.Medium ) );
            XmlNode big = Get( string.Format( @"/Tours/Tour[@Number=""{0}""]/Pannes/Panne[@Type=""{1}""]", tour, (int)BreakdownType.Big ) );

            try
            {
                for ( int i = 0; i < int.Parse( small.Attributes["Number"].Value ); ++i )
                {
                    breakdownsList.Add( new Breakdown( BreakdownType.Small ) );
                }

                for ( int i = 0; i < int.Parse( medium.Attributes["Number"].Value ); ++i )
                {
                    breakdownsList.Add( new Breakdown( BreakdownType.Medium ) );
                }

                for ( int i = 0; i < int.Parse( big.Attributes["Number"].Value ); ++i )
                {
                    breakdownsList.Add( new Breakdown( BreakdownType.Big ) );
                }
            }
            catch( Exception e )
            {
                throw e;
            }

            return breakdownsList;
        }
    }
}
