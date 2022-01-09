using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrich.UtilityScripts
{
    public class EventCodes
    {
        public const byte DeactivateAllRingsEventCode = 1;
        public const byte ActivateAllRingsEventCode = 2;
        public const byte ActivateNew500RingEventCode = 3;

        public const byte DeactivateAllItemBoxesEventCode = 4;
        public const byte ActivateAllItemBoxesEventCode = 5;

        public const byte AddPlayerGotEliminatedToUIEventCode = 6;
        public const byte AddPlayerRespawnedToUIEventCode = 7;
    }
}
