using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace NOP_Actors
{
    class ConditionCloseGate : UntypedActor
    {
        private bool isGateClosed;
        private IActorRef _FBEGateActorRef;
        private IActorRef _FBERemoteControlRef;

        private void SetFBERemoteControl(IActorRef FBERemoteControl)
        {
            _FBERemoteControlRef = FBERemoteControl;
        }

        private void SetFBEGateActorRef(IActorRef FBEGateActorRef)
        {
            _FBEGateActorRef = FBEGateActorRef;
        }

        public ConditionCloseGate()
        {
            isGateClosed = true;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.FBEGateRef) SetFBEGateActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.FBERemoteControlRef) SetFBERemoteControl(temp.Ref);
                Sender.Tell(true);
            }
            else switch (message)
            {
                case ConditionAction.SendClock:
                    if (!_FBEGateActorRef.IsNobody() && isGateClosed == false) _FBEGateActorRef.Tell(GateAction.Close);
                    break;

                case ConditionAction.SendTrue:
                    if (!_FBERemoteControlRef.IsNobody()) _FBERemoteControlRef.Tell(RemoteControlState.ChangeComplete);
                    isGateClosed = true;
                    break;

                case ConditionAction.SendFalse:
                    isGateClosed = false;
                    break;
            }
        }
    }

    class ConditionOpenGate : UntypedActor
    {
        private bool isGateOpen;
        private IActorRef _FBEGateActorRef;
        private IActorRef _FBERemoteControlRef;

        private void SetFBERemoteControl(IActorRef FBERemoteControl)
        {
            _FBERemoteControlRef = FBERemoteControl;
        }

        private void SetFBEGateActorRef(IActorRef FBEGateActorRef)
        {
            _FBEGateActorRef = FBEGateActorRef;
        }

        public ConditionOpenGate()
        {
            isGateOpen = false;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.FBEGateRef) SetFBEGateActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.FBERemoteControlRef) SetFBERemoteControl(temp.Ref);
                Sender.Tell(true);
            }
            else switch (message)
            {
                case ConditionAction.SendClock:
                    if (!_FBEGateActorRef.IsNobody() && isGateOpen == false) _FBEGateActorRef.Tell(GateAction.Open);
                    break;

                case ConditionAction.SendTrue:
                    isGateOpen = true;
                    if (!_FBERemoteControlRef.IsNobody()) _FBERemoteControlRef.Tell(RemoteControlState.ChangeComplete);
                    break;

                case ConditionAction.SendFalse:
                    isGateOpen = false;
                    break;
            }
        }
    }

    class ConditionStateChanged : UntypedActor
    {
        private IActorRef _FBERemoteControlRef;

        private void SetFBERemoteControl(IActorRef FBERemoteControl)
        {
            _FBERemoteControlRef = FBERemoteControl;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.FBERemoteControlRef) SetFBERemoteControl(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case ConditionAction.SendFalse:
                    if (!_FBERemoteControlRef.IsNobody()) _FBERemoteControlRef.Tell(RemoteControlState.Off);
                    break;
            }
        }
    }
}
