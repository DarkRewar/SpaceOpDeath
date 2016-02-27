using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceOpDeath.Utils;
using System.Xml;
using SpaceOpDeath.Ships;

namespace SpaceOpDeath.Members
{
    /// <summary>
    /// Type de membre (Mechanic, Captain, Commander, Doctor).
    /// </summary>
    public enum MemberType
    {
        Member = 0,
        Doctor = 1,
        Mechanic = 2,
        Captain = 3,
        Commander = 4
    }

    /// <summary>
    /// Gère les membres.
    /// </summary>
    class Member
    {
        /// <summary>
        /// Le nombre de points de vie du membre.
        /// </summary>
        protected int _lifepoints;

        /// <summary>
        /// Si le membre a déjà été joué ce tour.
        /// </summary>
        public bool played;

        /// <summary>
        /// Si le membre a déjà bougé ce tour.
        /// </summary>
        public bool moved
        {
            get;
            private set;
        }

        /// <summary>
        /// Liste des dés que possède le membre.
        /// </summary>
        protected List<Dice> _dices;

        /// <summary>
        /// Type de membre (Mechanic, Captain, Commander, Doctor).
        /// </summary>
        protected MemberType _type = MemberType.Member;

        /// <summary>
        /// Si le membre a déjà joué ce tour.
        /// </summary>
        protected bool hasPlayed = false;

        /// <summary>
        /// Le module dans lequel se trouve actuellement le membre.
        /// </summary>
        public Module module
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructeur par défaut du membre.
        /// </summary>
        public Member()
        {
            played = false;
            moved = false;

            XmlAttributeCollection aleaLife = Alea.Config.Get( "MemberHealthStart" );
            XmlAttributeCollection aleaDices = Alea.Config.Get( "MemberDicesStart" );

            _lifepoints = Dice.Rand.Next( int.Parse( aleaLife["Min"].Value ), int.Parse( aleaLife["Max"].Value ) );
            _dices = new List<Dice>();
            for ( int i = 0; i < Dice.Rand.Next( int.Parse(aleaDices["Min"].Value), int.Parse( aleaDices["Max"].Value ) ); ++i )
            {
                _dices.Add( new Dice() );
            }
        }

        /// <summary>
        /// Récupère les points de vie du membre.
        /// </summary>
        /// <returns></returns>
        public int GetLifepoints()
        {
            return _lifepoints;
        }

        /// <summary>
        /// Ajoute des points de vie à un membre.
        /// </summary>
        /// <param name="lifepoints"></param>
        public void AddLifepoints( int lifepoints )
        {
            _lifepoints += lifepoints;
            int maxLifepoints = int.Parse(Alea.Config.Get( "MemberMaxHealth" )["Value"].Value);

            if( _lifepoints > maxLifepoints )
            {
                _lifepoints = maxLifepoints;
            }
        }

        /// <summary>
        /// Recupère les dés du membre.
        /// </summary>
        /// <returns></returns>
        public List<Dice> GetDices()
        {
            return _dices;
        }

        /// <summary>
        /// Récupère tous les dés qui peuvent être lancés.
        /// </summary>
        /// <returns></returns>
        public List<Dice> GetUsableDices()
        {
            List<Dice> dices = new List<Dice>();
            foreach( Dice dice in _dices )
            {
                if( !dice.putAside )
                {
                    dices.Add( dice );
                }
            }
            return dices;
        }

        /// <summary>
        /// Récupère tous les dés qui peuvent être joués.
        /// </summary>
        /// <returns></returns>
        public List<Dice> GetPlayableDices()
        {
            List<Dice> dices = new List<Dice>();
            foreach ( Dice dice in _dices )
            {
                if ( !dice.played )
                {
                    dices.Add( dice );
                }
            }
            return dices;
        }

        /// <summary>
        /// Ajoute un certain nombre de dés.
        /// </summary>
        /// <param name="numberOfDices"></param>
        /// <returns></returns>
        public Member AddDices( int numberOfDices )
        {
            int maxDices = int.Parse( Alea.Config.Get( "MemberMaxDices" )["Value"].Value );
            for( var i = 0; i < numberOfDices; ++i )
            {
                if( _dices.Count == maxDices )
                {
                    break;
                }

                Dice dice = new Dice();
                dice.played = true;
                _dices.Add( dice );
            }
            return this;
        }

        /// <summary>
        /// Retire un certain nombre de dés.
        /// </summary>
        /// <param name="numberOfDices"></param>
        /// <returns></returns>
        public Member RemoveDices( int numberOfDices )
        {
            int realNumber = numberOfDices;
            while ( realNumber - _dices.Count < 1 )
            {
                --realNumber;
            }

            _dices.RemoveRange( 0, numberOfDices );

            return this;
        }

        /// <summary>
        /// Récupère le type de membre.
        /// </summary>
        /// <returns></returns>
        public MemberType GetMemberType()
        {
            return _type;
        }

        /// <summary>
        /// Bouge un membre vers un module.
        /// </summary>
        /// <param name="module"></param>
        public void MoveTo( Module module, bool move = true )
        {
            if( this.module != null )
            {
                int damages = PreviewMoveDamages( module );
                AddLifepoints( -damages );

                this.module.RemoveMember( this );
            }

            module.AddMember( this );
            this.module = module;
            moved = move;
        }

        /// <summary>
        /// Récupère le nombre de dégats que se prendra le membre
        /// s'il veut se rendre à un module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public int PreviewMoveDamages( Module module )
        {
            int damages = 0;
            foreach( Module mods in PathFind( module ) )
            {
                if( mods.breakdown != null )
                {
                    ++damages;
                }
            }            

            return damages;
        }

        /// <summary>
        /// Cherche le meilleur chemin pour accéder au module voulu.  
        /// Retourn la liste des modules à parcourir.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public List<Module> PathFind( Module moduleToFind, List<Module> path = null, Module currentCursor = null )
        {
            if( path == null )
            {
                path = new List<Module>();
                path.Add( this.module );
            }

            if( currentCursor == null )
            {
                currentCursor = this.module;
            } else if( currentCursor == moduleToFind )
            {
                path.Add( moduleToFind );
            } else
            {
                while ( currentCursor != moduleToFind )
                {
                    foreach ( Module linked in currentCursor.GetModulesLinked() )
                    {
                        if ( PathFind( moduleToFind, path, linked ).Contains( moduleToFind ) )
                        {
                            path.Add( linked );
                        }
                    }
                }
            }           

            return path;
        }

        /// <summary>
        /// Lance tous les dés jouable du membre.
        /// </summary>
        public void RollDices()
        {
            foreach( Dice dice in _dices )
            {
                if( !dice.putAside && !dice.played )
                {
                    dice.Roll();
                }
            }
        }

        /// <summary>
        /// Vérifie si le membre peut dépenser un dé dans son 
        /// action spéciale.
        /// </summary>
        /// <returns></returns>
        internal bool HasMinDiceForAction()
        {
            int minPower = int.Parse(Alea.Config.Get( "SpecialPowerMinValue" )["Value"].Value);
            foreach( Dice dice in _dices )
            {
                if( dice.number >= minPower && !dice.played )
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Vérifie si le membre peut dépenser ses dés.
        /// </summary>
        /// <returns></returns>
        public bool CanSpendDices()
        {
            foreach ( Dice dice in _dices )
            {
                if ( dice.played )
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Marque tous les dés comme étant joués.
        /// </summary>
        /// <param name="played"></param>
        public void PlayDices( bool played = true )
        {
            foreach ( Dice dice in _dices )
            {
                dice.played = played;
            }
        }

        /// <summary>
        /// L'action que va effectuer le membre.
        /// Elle doit être réécrite dans les classes enfants.
        /// L'action va enlever le dé qui a la valeur minimale
        /// requise ou le mettre en statut "played" s'il n'en
        /// reste plus qu'un au joueur.
        /// </summary>
        virtual public void Action()
        {
            int minValueDice = int.Parse( Alea.Config.Get( "SpecialPowerMinValue" )["Value"].Value );
            int maxDice = 0;
            foreach( Dice dice in _dices )
            {
                if( dice.number > maxDice )
                {
                    maxDice = dice.number;
                }
            }

            int i = minValueDice;
            List<Dice> playableDices = GetPlayableDices();
            while( i <= maxDice )
            {
                foreach ( Dice dice in playableDices )
                {
                    if ( dice.number == i )
                    {
                        if( playableDices.Count == 1 )
                        {
                            dice.played = true;
                        }
                        else
                        {
                            _dices.Remove( dice );
                        }

                        return;
                    }
                }
                ++i;
            }
        }

        /// <summary>
        /// Reset les variables nécessaires pour jouer dans un tour.
        /// </summary>
        public void ResetPlay()
        {
            played = false;
            moved = false;

            foreach( Dice dice in _dices )
            {
                dice.played = false;
                dice.putAside = false;
            }
        }
        
        /// <summary>
        /// Vous avez vraiment besoin de savoir ce que ça fait ?
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "Le " + _type + " (" + _lifepoints + " PV) possède " + _dices.Count + " dés";
            return str;
        }

        /// <summary>
        /// Retourne les infos concernant le membre.
        /// </summary>
        /// <returns></returns>
        public string ToStringInfo()
        {
            string locked = string.Empty;
            if ( played )
            {
                locked = "[Verrouillé] ";
            }

            string movedText = string.Empty;
            if ( moved )
            {
                movedText = "[Amovible] ";
            }

            string str = locked + movedText + _type + " (" + _lifepoints + " PV) " + "[" + _dices.Count + " Dés]";
            return str;
        }

        /// <summary>
        /// Retourne la somme des dés.
        /// </summary>
        /// <returns></returns>
        public int TotalDices()
        {
            int total = 0;
            foreach( Dice dice in _dices )
            {
                if( !dice.played )
                {
                    total += dice.number;
                }
            }
            return total;
        }

        /// <summary>
        /// Retourne la liste des dés que possède le membre.
        /// </summary>
        /// <returns></returns>
        public string ToStringListDices()
        {
            string list = string.Empty;
            List<Dice> playableDices = GetPlayableDices();
            int i = 1;
            foreach ( Dice dice in playableDices )
            {
                list += dice.number;
                if ( i < playableDices.Count )
                {
                    list += ", ";
                }
                ++i;
            }
            return list;
        }

        /// <summary>
        /// Doit être réecrite dans les classes filles.
        /// </summary>
        /// <returns></returns>
        virtual public string ToStringAction()
        {
            return null;
        }
    }
}
