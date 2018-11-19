using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace NOP_Actors
{
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
