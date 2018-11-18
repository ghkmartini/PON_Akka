//#define DEBUG_GATE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace NOP_Actors
{
    class Program
    {
        public static ActorSystem NOPActorSystem;


        static void Main(string[] args)
        {
            //Starts the watch timer 
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Start the actor system
            NOPActorSystem = ActorSystem.Create("NOPActorSystem");

            //Build up all actors
            IActorRef FBEGateActor               = NOPActorSystem.ActorOf(Props.Create(() => new FBEGate()),               "FBEGateActor");
            IActorRef FBERemoteControlActor      = NOPActorSystem.ActorOf(Props.Create(() => new FBERemoteControl()),      "FBERemoteControlActor");
            IActorRef PremisseIsClosedActor      = NOPActorSystem.ActorOf(Props.Create(() => new PremisseIsClosed()),      "PremisseIsClosedActor");
            IActorRef PremisseIsOpenActor        = NOPActorSystem.ActorOf(Props.Create(() => new PremisseIsOpen()),        "PremisseIsOpenActor");
            IActorRef PremisseChangeStateActor   = NOPActorSystem.ActorOf(Props.Create(() => new PremisseChangeState()),   "PremisseChangeStateActor");
            IActorRef ConditionCloseGateActor    = NOPActorSystem.ActorOf(Props.Create(() => new ConditionCloseGate()),    "ConditionCloseGateActor");
            IActorRef ConditionOpenGateActor     = NOPActorSystem.ActorOf(Props.Create(() => new ConditionOpenGate()),     "ConditionOpenGateActor");
            IActorRef ConditionStateChangedActor = NOPActorSystem.ActorOf(Props.Create(() => new ConditionStateChanged()), "ConditionStateChangedActor");

            //Set all actor references
            var FBEGateActorTask1               = FBEGateActor.Ask(new ActorReference(ActorRefType.PremisseIsClosedRef, PremisseIsClosedActor));
            var FBEGateActorTask2               = FBEGateActor.Ask(new ActorReference(ActorRefType.PremisseIsOpenRef, PremisseIsOpenActor));
            var FBERemoteControlActorTask1      = FBERemoteControlActor.Ask(new ActorReference(ActorRefType.PremisseChangeStateRef, PremisseChangeStateActor));
            var PremisseIsClosedActorTask1      = PremisseIsClosedActor.Ask(new ActorReference(ActorRefType.ConditionCloseGateRef, ConditionCloseGateActor));
            var PremisseIsOpenActorTask1        = PremisseIsOpenActor.Ask(new ActorReference(ActorRefType.ConditionOpenGateRef, ConditionOpenGateActor));
            var PremisseChangeStateActorTask1   = PremisseChangeStateActor.Ask(new ActorReference(ActorRefType.ConditionCloseGateRef, ConditionCloseGateActor));
            var PremisseChangeStateActorTask2   = PremisseChangeStateActor.Ask(new ActorReference(ActorRefType.ConditionOpenGateRef, ConditionOpenGateActor));
            var PremisseChangeStateActorTask3   = PremisseChangeStateActor.Ask(new ActorReference(ActorRefType.ConditionStateChangedRef, ConditionStateChangedActor));
            var ConditionCloseGateActorTask1    = ConditionCloseGateActor.Ask(new ActorReference(ActorRefType.FBEGateRef, FBEGateActor));
            var ConditionCloseGateActorTask2    = ConditionCloseGateActor.Ask(new ActorReference(ActorRefType.FBERemoteControlRef, FBERemoteControlActor));
            var ConditionOpenGateActorTask1     = ConditionOpenGateActor.Ask(new ActorReference(ActorRefType.FBEGateRef, FBEGateActor));
            var ConditionOpenGateActorTask2     = ConditionOpenGateActor.Ask(new ActorReference(ActorRefType.FBERemoteControlRef, FBERemoteControlActor));
            var ConditionStateChangedActorTask1 = ConditionStateChangedActor.Ask(new ActorReference(ActorRefType.FBERemoteControlRef, FBERemoteControlActor));

            //Wait for all references to be set
            FBEGateActorTask1.Wait();
            FBEGateActorTask1.Wait();
            FBEGateActorTask2.Wait();
            FBERemoteControlActorTask1.Wait();
            PremisseIsClosedActorTask1.Wait();
            PremisseIsOpenActorTask1.Wait();
            PremisseChangeStateActorTask1.Wait();
            PremisseChangeStateActorTask2.Wait();
            PremisseChangeStateActorTask3.Wait();
            ConditionCloseGateActorTask1.Wait();
            ConditionCloseGateActorTask2.Wait();
            ConditionOpenGateActorTask1.Wait();
            ConditionStateChangedActorTask1.Wait();

            //ActionTriggers
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);
            FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);

            //Tells to shut down the actor system
            FBERemoteControlActor.Tell(RemoteControlState.End);

            //Blocks the main thread from exiting until the actor system is shut down
            NOPActorSystem.WhenTerminated.Wait();

            //Stop the watch timer
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            //Wait for a key press to finish the application
            Console.Write("Time elapsed [ms]:");
            Console.WriteLine(elapsedMs);
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }
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
        Off,
        End
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
            if(_PremiseIsClosedActorRef.IsNobody() || _PremiseIsOpenActorRef.IsNobody() )
                return;

            if(attributeGateState != oldAttributeGateState)
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
            if(message is ActorReference)
            {
                var temp = message as ActorReference;
                if (temp.ActorRefID == ActorRefType.PremisseIsClosedRef) SetPremiseIsClosedActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.PremisseIsOpenRef)   SetPremiseIsOpenActorRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch(message)
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
        private int  attributeRemoteControlState;
        private int  oldAttributeRemoteControlState;
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
                if(attributeRemoteControlState == (int) RemoteControlState.ButtonPress)
                {
                    _PremiseChangeStateActorRef.Tell(PremiseAction.MakeTrue);
                }
                else if(attributeRemoteControlState == (int) RemoteControlState.Off)
                {
                    _PremiseChangeStateActorRef.Tell(PremiseAction.MakeFalse);
                }
                oldAttributeRemoteControlState = attributeRemoteControlState;
            }
        }

        private void CheckChangeCompleteLatch()
        {
            if(changeCompleteLatch && remoteControlOffLatch)
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
                    if(remoteControlActions.Count() == 1) ButtonPressEvent();
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
                    if(!_ConditionOpenGateActorRef.IsNobody()) _ConditionOpenGateActorRef.Tell(ConditionAction.SendTrue);
                    break;

                case PremiseAction.MakeFalse:
                    if(!_ConditionOpenGateActorRef.IsNobody()) _ConditionOpenGateActorRef.Tell(ConditionAction.SendFalse);
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
                if (temp.ActorRefID == ActorRefType.ConditionCloseGateRef)    SetConditionCloseGateActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.ConditionOpenGateRef)     SetConditionOpenGateActorRef(temp.Ref);
                if (temp.ActorRefID == ActorRefType.ConditionStateChangedRef) SetConditionStateChangedRef(temp.Ref);

                Sender.Tell(true);
            }
            else switch (message)
            {
                case PremiseAction.MakeTrue:
                    if(!_ConditionCloseGateActorRef.IsNobody()) _ConditionCloseGateActorRef.Tell(ConditionAction.SendClock);
                    if(!_ConditionOpenGateActorRef.IsNobody())  _ConditionOpenGateActorRef.Tell(ConditionAction.SendClock);
                    if(!_ConditionStateChangedRef.IsNobody())   _ConditionStateChangedRef.Tell(ConditionAction.SendFalse);
                    break;

                case PremiseAction.MakeFalse:
                    //Don't do anything, work only on the rising edge of the condition
                    break;
            }
        }
    }

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
