using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace NOP_Actors
{
    public static class GlobalVar
    {
        public const int CRC_32_LOAD_K = 3000;
        public static int global_crc_count = 0;
    }

    enum ActorRefType
    {
        FBEGateRef,
        FBERemoteControlRef,
        PremisseIsClosedRef,
        PremisseIsOpenRef,
        PremisseChangeStateRef,
        ConditionCloseGateRef,
        ConditionOpenGateRef,
        ConditionStateChangedRef
    };

    enum PremiseAction
    {
        MakeTrue,
        MakeFalse
    };

    enum ConditionAction
    {
        SendTrue,
        SendFalse,
        SendClock
    };

    enum GateState
    {
        Open,
        Closed
    };

    enum RemoteControlState
    {
        ButtonPress,
        ChangeComplete,
        Off
    }

    enum GateAction
    {
        Open,
        Close
    };

    class ActorReference
    {
        public ActorRefType ActorRefID;
        public IActorRef Ref;

        public ActorReference(ActorRefType ActorRefIDIn, IActorRef RefIn)
        {
            ActorRefID = ActorRefIDIn;
            Ref = RefIn;
        }

    }
}
