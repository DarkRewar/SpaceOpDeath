using System;

namespace SpaceOpDeath.Utils
{
    class SpaceOpDeathException: Exception
    {
        public SpaceOpDeathException( string message )
            :base(message)
        {
        }
    }
}
