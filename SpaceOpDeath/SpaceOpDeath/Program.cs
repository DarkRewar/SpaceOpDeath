using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SpaceOpDeath.Utils;

namespace SpaceOpDeath
{
    class Program
    {
        static void Main( string[] args )
        {
            string appName = ConfigurationManager.AppSettings["AppName"];
            Console.WriteLine( appName );
            Console.WriteLine( "Réalisé par Curtis Pelissier\n-----------------------------\n" );

            try
            {
                GameEvent ge = GameEvent.gameEvent;
                ge.StartGame();
            }
            catch ( SpaceOpDeathException e )
            {
                Console.WriteLine( "Une erreur est survenue : " + e.Message );
            }
            catch ( Exception e )
            {
                Console.WriteLine( "Une erreur est survenue : le jeu ne peut continuer son exécution." );
                Console.WriteLine( e.Message );
            }

            Console.ReadLine();
        }
    }
}
