//#define DEBUG_GATE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace NOP_Actors
{
    class FBEGate : UntypedActor
    {
        private int attributeGateState;
        private int oldAttributeGateState;
        private IActorRef _PremiseIsClosedActorRef;
        private IActorRef _PremiseIsOpenActorRef;

        private void MethodCloseGate()
        {
            #if DEBUG_GATE
                Console.WriteLine("Gate Closed\n");
            #endif
            attributeGateState = (int)GateState.Closed;
            CheckChange();
        }

        private void MethodOpenGate()
        {
            #if DEBUG_GATE
                Console.WriteLine("Gate Open\n");
            #endif
            attributeGateState = (int)GateState.Open;
            CheckChange();
        }

        private void CheckChange()
        {
            if (_PremiseIsClosedActorRef.IsNobody() || _PremiseIsOpenActorRef.IsNobody())
                return;

            if (attributeGateState != oldAttributeGateState)
            {
                if (attributeGateState == (int)GateState.Closed)
                {
                    _PremiseIsClosedActorRef.Tell(PremiseAction.MakeTrue);
                    _PremiseIsOpenActorRef.Tell(PremiseAction.MakeFalse);
                }
                else if (attributeGateState == (int)GateState.Open)
                {
                    _PremiseIsClosedActorRef.Tell(PremiseAction.MakeFalse);
                    _PremiseIsOpenActorRef.Tell(PremiseAction.MakeTrue);
                }
                oldAttributeGateState = attributeGateState;
            }
        }

        public FBEGate()
        {
            attributeGateState = (int)GateState.Closed;
            oldAttributeGateState = attributeGateState;
        }

        private void SetPremiseIsClosedActorRef(IActorRef PremiseIsClosedActorRef)
        {
            _PremiseIsClosedActorRef = PremiseIsClosedActorRef;
        }

        private void SetPremiseIsOpenActorRef(IActorRef PremiseIsOpenActorRef)
        {
            _PremiseIsOpenActorRef = PremiseIsOpenActorRef;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.PremisseIsClosedRef) SetPremiseIsClosedActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.PremisseIsOpenRef) SetPremiseIsOpenActorRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case GateAction.Close:
                    MethodCloseGate();
                    break;

                case GateAction.Open:
                    MethodOpenGate();
                    break;
            }
        }
    }

    class FBERemoteControl : UntypedActor
    {
        private int attributeRemoteControlState;
        private int oldAttributeRemoteControlState;
        private bool changeCompleteLatch;
        private bool remoteControlOffLatch;
        private IActorRef _PremiseChangeStateActorRef;
        Queue<RemoteControlState> remoteControlActions;

        private void MethodClearEvent()
        {
            attributeRemoteControlState = (int)RemoteControlState.Off;
            CheckChange();
        }

        private void ButtonPressEvent()
        {
            attributeRemoteControlState = (int)RemoteControlState.ButtonPress;
            CheckChange();
        }

        private void CheckChange()
        {
            if (_PremiseChangeStateActorRef.IsNobody()) return;

            if (attributeRemoteControlState != oldAttributeRemoteControlState)
            {
                if (attributeRemoteControlState == (int)RemoteControlState.ButtonPress)
                {
                    _PremiseChangeStateActorRef.Tell(PremiseAction.MakeTrue);
                }
                else if (attributeRemoteControlState == (int)RemoteControlState.Off)
                {
                    _PremiseChangeStateActorRef.Tell(PremiseAction.MakeFalse);
                }
                oldAttributeRemoteControlState = attributeRemoteControlState;
            }
        }

        private void CheckChangeCompleteLatch()
        {
            if (changeCompleteLatch && remoteControlOffLatch)
            {
                changeCompleteLatch = false;
                remoteControlOffLatch = false;

                remoteControlActions.Dequeue();
                MethodClearEvent();
                if (remoteControlActions.Count() > 0) ButtonPressEvent();
            }
        }

        public FBERemoteControl()
        {
            attributeRemoteControlState = (int)RemoteControlState.Off;
            oldAttributeRemoteControlState = attributeRemoteControlState;

            changeCompleteLatch = false;
            remoteControlOffLatch = false;

            remoteControlActions = new Queue<RemoteControlState>();
        }

        private void SetPremiseChangeStateActorRef(IActorRef PremiseChangeStateActorRef)
        {
            _PremiseChangeStateActorRef = PremiseChangeStateActorRef;
        }

        protected override void OnReceive(object message)
        {
            if (message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.PremisseChangeStateRef) SetPremiseChangeStateActorRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case RemoteControlState.ButtonPress:
                    remoteControlActions.Enqueue(RemoteControlState.ButtonPress);
                    if (remoteControlActions.Count() == 1) ButtonPressEvent();
                    break;

                case RemoteControlState.ChangeComplete:
                    changeCompleteLatch = true;
                    CheckChangeCompleteLatch();
                    break;

                case RemoteControlState.Off:
                    remoteControlOffLatch = true;
                    CheckChangeCompleteLatch();
                    break;

                case RemoteControlState.End:
                    Context.System.Terminate();
                    break;
            }
        }
    }
}
