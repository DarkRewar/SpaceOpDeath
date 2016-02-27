using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceOpDeath.Breakdowns;
using SpaceOpDeath.Members;

namespace SpaceOpDeath.Ships
{
    class Module
    {
        public string name;
        public Breakdown breakdown;
        private List<Member> _members;
        private List<Module> _modulesLinked;
        public Ship ship
        {
            get;
            set;
        }

        public Module()
        {
            _members = new List<Member>();
            _modulesLinked = new List<Module>();
        }

        public Module( string name )
            :this()
        {
            this.name = name;
        }

        public List<Member> GetMembers()
        {
            return _members;
        }

        public Module AddMember( Member member )
        {
            _members.Add( member );
            return this;
        }

        public Module RemoveMember( Member member )
        {
            _members.Remove( member );
            return this;
        }

        public List<Module> GetModulesLinked()
        {
            return _modulesLinked;
        }

        public Module LinkModule( Module module )
        {
            _modulesLinked.Add( module );

            if ( !module.GetModulesLinked().Contains( this ) )
            {
                module.LinkModule( this );
            }

            return this;
        }

        public Module UnlinkModule( Module module )
        {
            _modulesLinked.Remove( module );

            if ( module.GetModulesLinked().Contains( this ) )
            {
                module.UnlinkModule( this );
            }
            return this;
        }
        
        public Module UnlinkModule( int index )
        {
            _modulesLinked.RemoveAt( index );
            return this;
        }

        public Module UnlinkAllModules()
        {
            foreach( Module m in _modulesLinked )
            {
                _modulesLinked.Remove( m );
            }

            return this;
        }

        public Module Repair( int pointsToRepair )
        {
            breakdown.SubStrength( pointsToRepair );
            return this;
        }

        public override string ToString()
        {
            string str = "Le module " + name + " est lié à " + _modulesLinked.Count + " modules.\n";
            foreach( Module m in _modulesLinked )
            {
                str += "Le module " + name + " est lié au module : " + m.name + ".\n";
            }

            foreach( Member m in _members )
            {
                str += "Le module " + name + " contient un " + m.GetMemberType() + ".\n";
            }

            return str;
        }
    }
}
