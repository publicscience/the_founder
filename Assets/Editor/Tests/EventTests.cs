using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    internal class TestEventListener
    {
        public GameEvent gameEvent;
        public GameEvent triggeredEvent;

        public TestEventListener(GameEvent gameEvent) {
            gameEvent.EventTriggered += OnEvent;
        }

        void OnEvent(GameEvent e) {
             triggeredEvent = e;
        }
    }

	[TestFixture]
	internal class EventTests
	{
        private GameEvent gE = null;

        [SetUp]
        public void SetUp() {
            gE = new GameEvent("Some event", 1f);
        }

        [TearDown]
        public void TearDown() {
            gE = null;
        }

		[Test]
		public void EventConstructor()
		{
            Assert.IsNotNull(gE);
            Assert.AreEqual(gE.name, "Some event");
		}

		[Test]
		public void EventRolling()
		{
            List<GameEvent> candidateEvents = new List<GameEvent>() {
                gE
            };

            List<GameEvent> happeningEvents = GameEvent.Roll(candidateEvents);

            Assert.AreEqual(happeningEvents[0].name, "Some event");
		}

        [Test]
        public void EffectEvents()
        {
            gE.companyEffects.Add(new StatBuff("Cash", 1000f));

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener(gE);

            // Trigger the event.
            gE.Trigger();

            // Check if the event was captured.
            Assert.AreEqual(eL.triggeredEvent, gE);
            Assert.AreEqual(eL.triggeredEvent.companyEffects[0].value, 1000f);
        }
    }
}
