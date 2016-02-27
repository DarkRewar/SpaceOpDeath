using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceOpDeath.Ships;
using SpaceOpDeath.Members;
using System.Xml;
using SpaceOpDeath.Breakdowns;

namespace SpaceOpDeath.Utils
{
    class GameEvent
    {
        /// <summary>
        /// L'objet GameEvent unique. Gère tous les évènements du jeu.
        /// </summary>
        public static GameEvent gameEvent = new GameEvent();

        /// <summary>
        /// Le vaisseau.
        /// </summary>
        private Ship _ship;

        /// <summary>
        /// La liste des membres du vaisseau.
        /// </summary>
        private List<Member> _members;

        /// <summary>
        /// L'objet de gestion des tours en jeu.
        /// </summary>
        private Tours _tours;

        /// <summary>
        /// La variable contient le numéro du tour actuel.
        /// </summary>
        public int tour
        {
            get;
            private set;
        }

        /// <summary>
        /// Si la partie est finie ou non.
        /// </summary>
        public bool finished
        {
            get;
            private set;
        }

        /// <summary>
        /// Si la partie est gagnée ou non.
        /// </summary>
        public bool won
        {
            get;
            private set;
        }

        /// <summary>
        /// Instancie un GameEvent. Il s'agit de la classe permettant de
        /// gérer tous les évènements en jeu.
        /// </summary>
        private GameEvent()
        {
            finished = false;
            won = false;
            _ship = new Ship();
            _members = new List<Member>();
            
            Module laboratoire = new Module( "Laboratoire" );
            Module serre = new Module( "Serre" );
            Module pilotage = new Module( "Pilotage" );
            Module infirmerie = new Module( "Infirmerie" );
            Module detente = new Module( "Detente" );
            Module maintenance = new Module( "Maintenance" );
            Module survie = new Module( "Survie" );
            
            //_ship.AddModules( laboratoire, serre, pilotage, infirmerie, detente, maintenance, survie );
            _ship.AddModule( laboratoire )
                .AddModule( serre )
                .AddModule( pilotage )
                .AddModule( infirmerie )
                .AddModule( detente )
                .AddModule( maintenance )
                .AddModule( survie );
            
            pilotage.LinkModule( serre );
            serre.LinkModule( infirmerie );
            infirmerie.LinkModule( laboratoire );
            infirmerie.LinkModule( detente );
            detente.LinkModule( survie );
            detente.LinkModule( maintenance );

            _members.Add( new Captain() );
            _members.Add( new Commander() );
            _members.Add( new Doctor() );
            _members.Add( new Mechanic() );

            _tours = new Tours( "Config/Tours.xml" );

            tour = 1;
        }

        /// <summary>
        /// Récupère le vaisseau
        /// </summary>
        /// <returns></returns>
        public Ship GetShip()
        {
            return _ship;
        }

        /// <summary>
        /// Récupère les membres.
        /// </summary>
        /// <returns></returns>
        public List<Member> GetMembers()
        {
            return _members;
        }

        /// <summary>
        /// Place aléatoirement tous les membres.
        /// </summary>
        /// <returns></returns>
        public GameEvent PlaceMembers()
        {
            foreach( Member member in _members )
            {
                int randomInt = Dice.Rand.Next( 0, _ship.GetModules().Count );
                Module randomModule = _ship.GetModules().ElementAt( randomInt );
                member.MoveTo( randomModule, false );
            }

            return this;
        }

        /// <summary>
        /// Retire des dés à tous les membres.
        /// </summary>
        /// <param name="numberOfDices"></param>
        /// <returns></returns>
        public GameEvent RemoveDicesFromMembers( int numberOfDices = 1 )
        {
            foreach( Member member in _members )
            {
                member.RemoveDices( numberOfDices );
            }

            return this;
        }

        /// <summary>
        /// Début une partie. (Ah bon ? Je ne pensais pas...)
        /// </summary>
        public void StartGame()
        {
            Console.WriteLine( "Un mystérieux message venu de Mars vous est parvenu sur Terre. Des installations terrestres s'y trouvent." );
            Console.WriteLine( "Vous êtes alors chargés de vous rendre sur la planète rouge afin d'enquêter sur ce signal." );
            Console.WriteLine( "Alors que vous voyagez, une pluie inattendue d'astéroides vient frapper votre vaisseau de plein fouet." );
            Console.WriteLine( "Vous perdez alors deux membres de votre équipage et vous ne vous retrouvez plus qu'à quatre seulement.");
            Console.WriteLine( "Le retour sur Terre est impossible. Il vous faut continuer et survivre jusqu'à votre arrivée, " );
            Console.WriteLine( "prévue dans dix semaines, en voguant à travers un espace infiniment claustrophobique..." );                

            PlaceMembers();

            while ( !finished )
            {
                StartTour();
                ++tour;
            }
        }

        /// <summary>
        /// Débute un tour. (Non ! Sérieux ?)
        /// </summary>
        public void StartTour()
        {
            Watcher();

            if ( finished )
            {
                return;
            }

            ResetPlay();

            Console.WriteLine( "\n|----- Semaine " + tour + " -----|\n" );

            foreach( Breakdown b in _tours.GetTourBreakdowns( tour ) )
            {
                _ship.PlaceBreakdown( b );
            }

            MainMenu();
        }

        /// <summary>
        /// Applique les effets de fin de tour.
        /// </summary>
        private void EndTour()
        {
            _ship.ApplyBreakdownEffects();
            RemoveDicesFromMembers( 1 );
        }

        /// <summary>
        /// Vérifie toutes les conditions nécessaires pour pouvoir
        /// continuer la partie.
        /// </summary>
        public void Watcher()
        {
            if ( _ship.GetLifepoints() <= 0 && tour > 10 )
            {
                finished = true;
                return;
            }
            else if(  _ship.GetLifepoints() > 0 && tour > 10 && _members.Count > 0 )
            {
                finished = true;
                won = true;
                return;
            }

            finished = ( _members.Count == 0 );
        }

        /// <summary>
        /// Reset les variables nécessaires pour jouer dans un tour.
        /// </summary>
        public void ResetPlay()
        {
            foreach( Member member in _members )
            {
                member.ResetPlay();
            }
        }

        /// <summary>
        /// Récupère et sécurise les entrées clavier d'un utilisateur.
        /// </summary>
        /// <returns></returns>
        public int GetIntInput()
        {
            Console.Write( "Action> " );

            string input = Console.ReadLine();
            int number;
            Int32.TryParse( input, out number );
            return number;
        }

        /// <summary>
        /// Le menu principal. La base quoi.
        /// </summary>
        public void MainMenu()
        {
            Console.WriteLine( "Menu Principal. Quelle action voulez-vous effectuer ?" );
            Console.WriteLine( "1. Voir l'état du vaisseau" );
            Console.WriteLine( "2. Voir l'état des membres de l'équipage" );
            Console.WriteLine( "3. Bouger un membre de l'équipage" );
            Console.WriteLine( "4. Effectuer une action sur un module" );
            Console.WriteLine( "0. Quitter" );

            int action;
            action = GetIntInput();
            int[] actionsPossibles = { 0, 1, 2, 3, 4 };
            while ( !actionsPossibles.Contains( action ) )
            {
                Console.WriteLine( "L'action entrée n'existe pas." );
                action = GetIntInput();
            }

            switch ( action )
            {
                case 0:
                    finished = true;
                    return;
                case 1:
                    _ship.DisplayStatus();
                    break;
                case 2:
                    _ship.DisplayAllMembers();
                    break;
                case 3:
                    MoveMember();
                    break;
                case 4:
                    ActionMember();
                    break;
            }

            Console.WriteLine( "" );
            if( AllPlayed() )
            {
                ++tour;
                EndTour();
                StartTour();
            }
            else
            {
                MainMenu();
            }
        }

        /// <summary>
        /// Vérifie si tous les membres ont joué ce tour.
        /// </summary>
        /// <returns></returns>
        public bool AllPlayed()
        {
            foreach( Member member in _members )
            {
                if( !member.played )
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// L'action permettant à un membre d'utiliser son pouvoir spécial.
        /// </summary>
        private void ActionMember()
        {
            Console.WriteLine( "\nQuel membre voulez-vous utiliser ?" );
            int i = 1;
            foreach ( Member member in _members )
            {
                Console.WriteLine( i + ". " + member.ToStringInfo() );
                ++i;
            }

            int choice = GetIntInput();
            while ( choice < 1 || choice > _members.Count || _members.ElementAt( choice - 1 ).played )
            {
                Console.WriteLine( "Vous ne pouvez pas utiliser cet astronaute." );
                choice = GetIntInput();
            }

            Member memberChosen = _members.ElementAt( choice - 1 );

            // Choix de l'action à effectuer
            int[] actionsPossibles = { 0, 1 };
            Console.WriteLine( "Quelle action doit-il effectuer ?" );
            Console.WriteLine( "1. Lancer les dés [" + memberChosen.GetUsableDices().Count + " dés]" );
            Console.WriteLine( "0. Annuler" );

            int action;
            action = GetIntInput();
            while ( !actionsPossibles.Contains( action ) )
            {
                Console.WriteLine( "L'action entrée n'existe pas." );
                action = GetIntInput();
            }

            switch ( action )
            {
                case 0:
                    return;
                case 1:
                    RollDicesAction( memberChosen );
                    break;
            }

            memberChosen.played = true;
        }

        /// <summary>
        /// On choisi un membre à déplacer puis on choisi 
        /// le module vers lequel on veut le déplacer.
        /// </summary>
        private void MoveMember()
        {
            Console.WriteLine( "\nQuel membre voulez-vous bouger ?" );
            int i = 1;
            foreach( Member member in _members )
            {
                Console.WriteLine( i + ". " + member.ToStringInfo() );
                ++i;
            }

            int choice = GetIntInput();
            while( choice < 1 || choice > _members.Count ||
                _members.ElementAt( choice - 1 ).moved || 
                _members.ElementAt( choice - 1 ).played )
            {
                Console.WriteLine( "Vous ne pouvez pas bouger cet astronaute." );
                choice = GetIntInput();
            }

            Member memberChosen = _members.ElementAt( choice - 1 );

            // On choisi le module
            Console.WriteLine( "Veuillez choisir le module dans lequel déplacer " + memberChosen.GetMemberType() + ":");

            List<Module> modules = _ship.GetModules();
            i = 1;
            foreach ( Module module in modules )
            {
                string damagesStr = string.Empty;
                int takeDamages = memberChosen.PreviewMoveDamages( module ); 
                if( takeDamages > 0 )
                {
                    damagesStr = string.Format("(-{0} PV)", takeDamages);
                }

                Console.WriteLine( i + ". " + module.name + damagesStr );
                ++i;
            }

            choice = GetIntInput();
            while ( choice < 1 || choice > modules.Count )
            {
                Console.WriteLine( "Ce module n'existe pas." );
                choice = GetIntInput();
            }

            Module moduleChosen = modules.ElementAt( choice - 1 );
            memberChosen.MoveTo( moduleChosen );

            if( memberChosen.GetLifepoints() <= 0 )
            {
                //On prévient que le membre est décédé de manière tragique et héroïque
                moduleChosen.RemoveMember( memberChosen );
                _members.Remove( memberChosen );
                Console.WriteLine( memberChosen.GetMemberType() + " est mort lors de son déplacement. Il n'avait pas assez de points de vie." );
            } else
            {
                // On prévient que le membre a bien été déplacé
                Console.WriteLine( memberChosen.GetMemberType() + " a bien été déplacé dans le module " + moduleChosen.name + "." );
            }
        }

        /// <summary>
        /// L'action permettant de lancer les dés.
        /// </summary>
        /// <param name="memberChosen"></param>
        private void RollDicesAction( Member memberChosen )
        {
            int diceRollPerTour = int.Parse( Alea.Config.Get( "DiceRollsPerTour" )["Value"].Value );
            int rollsCount = 1;
            do
            {
                memberChosen.RollDices();
                List<Dice> dices = memberChosen.GetUsableDices();
                int dicePosition = 1;
                Console.Write( "Le résultat des dés est : " );
                foreach ( Dice dice in dices )
                {
                    Console.Write( dice.number );
                    if ( dicePosition < dices.Count )
                    {
                        Console.Write( "," );
                    }
                    else
                    {
                        Console.Write( ".\n" );
                    }
                    ++dicePosition;
                }

                // Sélection des dés à mettre de côté
                Console.WriteLine( "Veuillez choisir les dés à garder. Vous n'êtes pas obligé d'en garder." );

                int selectAction = -1;
                do
                {
                    int index = 1;
                    foreach ( Dice dice in dices )
                    {
                        Console.WriteLine( index + ". " + dice );
                        ++index;
                    }
                    Console.WriteLine( "0. Finir la sélection" );

                    selectAction = GetIntInput();
                    while ( selectAction < 0 && selectAction > dices.Count )
                    {
                        Console.WriteLine( "Le dé choisi n'existe pas." );
                        selectAction = GetIntInput();
                    }

                    if ( selectAction > 0 )
                    {
                        Dice chosenDice = dices.ElementAt( selectAction - 1 );
                        if ( chosenDice != null && chosenDice.putAside )
                        {
                            Console.WriteLine( "Le dé choisi ne peut être utilisé." );
                        }
                        else if ( chosenDice != null )
                        {
                            chosenDice.putAside = true;
                        }
                    }
                } while ( selectAction != 0 );

                ++rollsCount;
            }
            while ( memberChosen.GetUsableDices().Count > 0 && rollsCount <= diceRollPerTour );

            // Sélection des actions à effectuer
            int finalAction = -1;
            do
            {
                Console.WriteLine( "\nVous avez " + memberChosen.TotalDices() + " (" + memberChosen.ToStringListDices() + ")." );
                Console.WriteLine( "Que voulez-vous faire ?" );

                List<int> possibleFinalActions = new List<int>();
                possibleFinalActions.Add( 0 );

                if ( memberChosen.module.breakdown != null && memberChosen.CanSpendDices() )
                {
                    Console.WriteLine( "1. Investir dans la panne : " + memberChosen.module.breakdown.ToInfoString() );
                    possibleFinalActions.Add( 1 );
                }

                if ( memberChosen.HasMinDiceForAction() )
                {
                    Console.WriteLine( "2. Utiliser le pouvoir spécial (consomme un dé) : " + memberChosen.ToStringAction() );
                    possibleFinalActions.Add( 2 );
                }

                Console.WriteLine( "0. Finir les actions" );

                finalAction = GetIntInput();
                while ( !possibleFinalActions.Contains( finalAction) )
                {
                    Console.WriteLine( "L'action choisie ne peut être effectuée." );
                    finalAction = GetIntInput();
                }

                switch ( finalAction )
                {
                    case 1:
                        memberChosen.PlayDices();
                        memberChosen.module.breakdown.SubStrength( memberChosen.TotalDices() );
                        if( memberChosen.module.breakdown.strength <= 0 )
                        {
                            memberChosen.module.breakdown = null;
                        }
                        break;
                    case 2:
                        memberChosen.Action();
                        break;
                }
            }
            while ( finalAction != 0 );
        }
    }
}
