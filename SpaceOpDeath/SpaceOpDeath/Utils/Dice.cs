using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpDeath.Utils
{
    /// <summary>
    /// La class s'occupe des dés.
    /// </summary>
    class Dice
    {
        /// <summary>
        /// La variable aléatoire qui s'occupe de l'Aléa du jeu.
        /// </summary>
        public static Random Rand = new Random();

        /// <summary>
        /// Le nombre de faces sur le dé.
        /// </summary>
        public int faces
        {
            get;
            private set;
        }

        /// <summary>
        /// Si le dé a été joué.
        /// </summary>
        public bool played = false;

        /// <summary>
        /// Si le dé a été mis de côté ou non.
        /// </summary>
        public bool putAside = false;

        /// <summary>
        /// Le nombre obtenu au lancé du dé.
        /// </summary>
        public int number
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructeur. Instancie un dé. Met son nombre de faces à 6
        /// par défaut.
        /// </summary>
        public Dice()
        {        
            int maxFaces = int.Parse( Alea.Config.Get( "DiceDefaultFaces" )["Value"].Value );
            faces = maxFaces;
        }

        /// <summary>
        /// Constructeur. Instancie un dé avec un nombre
        /// de faces prédéfini.
        /// </summary>
        /// <param name="numberOfFaces"></param>
        public Dice( int numberOfFaces )
        {
            faces = numberOfFaces;
        }

        /// <summary>
        /// Lance un dé et garde le nombre obtenu en mémoire.
        /// </summary>
        /// <returns></returns>
        public Dice Roll()
        {
            if ( faces < 1 )
            {
                throw new SpaceOpDeathException( "Le dé ne possède pas assez de faces." );
            }

            number = Rand.Next( 1, faces + 1 );
            return this;
        }

        public override string ToString()
        {
            string res = string.Empty;

            string asideStr = string.Empty;
            if( putAside )
            {
                asideStr = "[Mis de côté] ";
            }

            res = asideStr + number;
            return res;
        }
    }
}
