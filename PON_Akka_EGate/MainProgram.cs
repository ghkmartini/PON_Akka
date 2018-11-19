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
            int i = 4;
            while((i--) > 0) FBERemoteControlActor.Tell(RemoteControlState.ButtonPress);

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
}
