using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace NOP_Actors
{
    class PremisseIsClosed : UntypedActor
    {
        private IActorRef _ConditionCloseGateActorRef;

        private void SetConditionCloseGateActorRef(IActorRef ConditionCloseGateActorRef)
        {
            _ConditionCloseGateActorRef = ConditionCloseGateActorRef;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.ConditionCloseGateRef) SetConditionCloseGateActorRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case PremiseAction.MakeTrue:
                    if (!_ConditionCloseGateActorRef.IsNobody()) _ConditionCloseGateActorRef.Tell(ConditionAction.SendTrue);
                    break;

                case PremiseAction.MakeFalse:
                    if (!_ConditionCloseGateActorRef.IsNobody()) _ConditionCloseGateActorRef.Tell(ConditionAction.SendFalse);
                    break;
            }
        }
    }

    class PremisseIsOpen : UntypedActor
    {
        private IActorRef _ConditionOpenGateActorRef;

        private void SetConditionOpenGateActorRef(IActorRef ConditionOpenGateActorRef)
        {
            _ConditionOpenGateActorRef = ConditionOpenGateActorRef;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.ConditionOpenGateRef) SetConditionOpenGateActorRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case PremiseAction.MakeTrue:
                    if (!_ConditionOpenGateActorRef.IsNobody()) _ConditionOpenGateActorRef.Tell(ConditionAction.SendTrue);
                    break;

                case PremiseAction.MakeFalse:
                    if (!_ConditionOpenGateActorRef.IsNobody()) _ConditionOpenGateActorRef.Tell(ConditionAction.SendFalse);
                    break;
            }
        }
    }

    class PremisseChangeState : UntypedActor
    {
        private IActorRef _ConditionCloseGateActorRef;
        private IActorRef _ConditionOpenGateActorRef;
        private IActorRef _ConditionStateChangedRef;

        private void SetConditionCloseGateActorRef(IActorRef ConditionCloseGateActorRef)
        {
            _ConditionCloseGateActorRef = ConditionCloseGateActorRef;
        }

        private void SetConditionOpenGateActorRef(IActorRef ConditionOpenGateActorRef)
        {
            _ConditionOpenGateActorRef = ConditionOpenGateActorRef;
        }

        private void SetConditionStateChangedRef(IActorRef ConditionStateChangedRef)
        {
            _ConditionStateChangedRef = ConditionStateChangedRef;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.ConditionCloseGateRef) SetConditionCloseGateActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.ConditionOpenGateRef) SetConditionOpenGateActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.ConditionStateChangedRef) SetConditionStateChangedRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case PremiseAction.MakeTrue:
                    if (!_ConditionCloseGateActorRef.IsNobody()) _ConditionCloseGateActorRef.Tell(ConditionAction.SendClock);
                    if (!_ConditionOpenGateActorRef.IsNobody()) _ConditionOpenGateActorRef.Tell(ConditionAction.SendClock);
                    if (!_ConditionStateChangedRef.IsNobody()) _ConditionStateChangedRef.Tell(ConditionAction.SendFalse);
                    break;

                case PremiseAction.MakeFalse:
                    //Don't do anything, work only on the rising edge of the condition
                    break;
            }
        }
    }
}
