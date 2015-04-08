using UnityEngine;
using UnityEditor;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class CompanyTests
	{
        private GameObject gameObj;
        private GameData gd;
        private GameManager gm;

        private Company c;
        private Worker worker;
        private Perk perk;
        private Location startLoc;

        private List<ProductType> pts;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            c = new Company("Foo Inc").Init();

            startLoc = ScriptableObject.CreateInstance<Location>();
            startLoc.cost = 0;
            c.ExpandToLocation(startLoc);

            pts = new List<ProductType>() {
                ProductType.Load("Social Network"),
                ProductType.Load("Virtual Reality")
            };

            worker = ScriptableObject.CreateInstance<Worker>();
            worker.Init("Franklin");

            perk = (Perk)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestPerk.asset", typeof(Perk)));
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            worker = null;
            c = null;
        }

		[Test]
		public void CompanyConstructor() {
            Assert.IsNotNull(c);

            // Creates a name.
            Assert.AreEqual(c.name, "Foo Inc");
		}

		[Test]
		public void ManageWorkers() {
          Assert.AreEqual(c.workers.Count, 0);

          worker.creativity.baseValue = 20;

          c.baseSizeLimit = 0;
          c.HireWorker(worker);
          Assert.AreEqual(c.workers.Count, 0);

          c.cash.baseValue = 2000;
          c.baseSizeLimit = 10;
          c.HireWorker(worker);
          Assert.AreEqual(c.workers.Count, 1);

          worker.salary = 2000;
          c.FireWorker(worker);
          Assert.AreEqual(c.workers.Count, 0);
          Assert.AreEqual(worker.salary, 0);
      }


		[Test]
		public void AggregateWorkerStats() {
            Founder founder = ScriptableObject.CreateInstance<Founder>();
            founder.Init("Jobs");
            founder.charisma.baseValue = 10;
            founder.creativity.baseValue = 20;
            founder.cleverness.baseValue = 30;
            founder.happiness.baseValue = 40;
            founder.productivity.baseValue = 50;
            c.founders.Add(founder);

            worker.charisma.baseValue = 1;
            worker.creativity.baseValue = 2;
            worker.cleverness.baseValue = 3;
            worker.happiness.baseValue = 4;
            worker.productivity.baseValue = 5;
            c.HireWorker(worker);

            Assert.AreEqual(c.AggregateWorkerStat("Charisma"), 11);
            Assert.AreEqual(c.AggregateWorkerStat("Creativity"), 22);
            Assert.AreEqual(c.AggregateWorkerStat("Cleverness"), 33);
            Assert.AreEqual(c.AggregateWorkerStat("Happiness"), 44);
            Assert.AreEqual(c.AggregateWorkerStat("Productivity"), 55);
        }

		[Test]
        public void ExpandToVertical() {
            c.cash.baseValue = 0;
            Vertical vert = ScriptableObject.CreateInstance<Vertical>();
            vert.cost = 2000;

            Assert.IsFalse(c.ExpandToVertical(vert));

            c.cash.baseValue = 2000;
            Assert.IsTrue(c.ExpandToVertical(vert));
            Assert.IsTrue(c.verticals.Contains(vert));
        }

		[Test]
        public void ExpandToLocation() {
            c.cash.baseValue = 0;
            Location loc = ScriptableObject.CreateInstance<Location>();
            loc.cost = 2000;
            loc.market = MarketManager.Market.Antarctica;

            EffectSet es = new EffectSet();
            es.cash = 5000;
            loc.effects = es;

            Assert.IsFalse(c.ExpandToLocation(loc));

            c.cash.baseValue = 2000;
            Assert.IsTrue(c.ExpandToLocation(loc));
            Assert.IsTrue(c.locations.Select(l => l.name).Contains(loc.name));
            Assert.IsTrue(c.markets.Contains(loc.market));
            Assert.AreEqual(c.cash.value, 5000);
        }

		[Test]
		public void PayMonthly() {
            // For this test to work, startCash has to be enough for all these purchases.
            // Otherwise the purchases don't go through.
            float startCash = 200000;
            c.cash.baseValue = startCash;

            Location loc = ScriptableObject.CreateInstance<Location>();
            loc.cost = 100;
            c.ExpandToLocation(loc);

            worker.salary = 500;

            // Location rent is calculated twice because it's paid on purchase, and then again as monthly rent.
            float paid = worker.hiringFee + (worker.monthlyPay * GameManager.Instance.wageMultiplier) + loc.cost + (loc.cost/1000 * GameManager.Instance.costMultiplier);

            c.HireWorker(worker);

            c.PayMonthly();
            Assert.AreEqual(c.cash.baseValue, startCash - paid);
        }

        [Test]
        public void ActiveEffects() {
            //Assert.IsTrue(c.BuyItem(item));

            //int last = c.activeEffects.Count - 1;
            //Assert.IsTrue(c.activeEffects[last].Equals(item.effects));
        }


        // ===============================================
        // Product Management ============================
        // ===============================================

        [Test]
        public void StartNewProduct() {
            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            Assert.AreEqual(c.developingProduct, p);
            Assert.AreEqual(c.products.Count, 1);

            // Creating a new product should not apply existing effects.
            //Assert.AreEqual(p.design.value, 0);
        }

		[Test]
		public void HarvestProduct() {
            c.cash.baseValue = 2000;

            c.StartNewProduct(pts, 10, 10, 10);
            Product p = c.products[0];
            p.Launch(c);
            Assert.AreEqual(c.activeProducts[0], p);

            c.HarvestProducts(0.1f);

            Assert.IsTrue(c.cash.baseValue > 2000);
        }

        [Test]
        public void ShutdownProduct() {
            c.cash.baseValue = 2000;

            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            p.Launch(c);
            Assert.AreEqual(c.activeProducts[0], p);

            c.ShutdownProduct(p);

            Assert.AreEqual(p.state, Product.State.RETIRED);
            Assert.AreEqual(p.design.value, 0);
        }

        // ===============================================
        // Perk Management ===============================
        // ===============================================

		[Test]
		public void BuyPerk_CanAfford() {
            c.cash.baseValue = perk.cost;
            float startCash = c.cash.value;
            float startResearch = c.research.value;


            Assert.IsTrue(c.BuyPerk(perk));

            Assert.AreEqual(c.cash.baseValue, startCash - perk.cost);
            Assert.AreEqual(c.perks.Count, 1);
            Assert.AreEqual(c.perks[0].upgradeLevel, 0);
            Assert.AreEqual(c.research.value, startResearch + perk.effects.research.value);
        }

		[Test]
		public void UpgradePerk_CanAfford() {
            c.cash.baseValue = perk.cost;
            Assert.IsTrue(c.BuyPerk(perk));

            c.cash.baseValue = perk.next.cost;
            float startResearch = c.research.value;
            Assert.IsTrue(c.UpgradePerk(perk));
            Assert.AreEqual(c.perks.Count, 1);
            Assert.AreEqual(c.perks[0].upgradeLevel, 1);
            Assert.AreEqual(c.research.value, startResearch + perk.effects.research.value);
        }

        [Test]
        public void OpinionEvents() {
            c.opinion.baseValue   = 200;
            c.publicity.baseValue = 100;
            gd.forgettingRate     = 10;

            EffectSet es = new EffectSet();
            es.opinionEvent = new OpinionEvent(100, 400);
            es.Apply(c);

            Assert.AreEqual(c.opinion.value, 300);
            Assert.AreEqual(c.publicity.value, 500);

            c.ForgetOpinionEvents();

            Assert.AreEqual(c.opinion.value, 290);
            Assert.AreEqual(c.publicity.value, 500);
        }

        [Test]
        public void MiniCompanyAcquisition() {
            AICompany aic = ScriptableObject.CreateInstance<AICompany>();
            AICompany.all = new List<AICompany>() { aic };

            Assert.IsFalse(aic.disabled);

            MiniCompany mc = ScriptableObject.CreateInstance<MiniCompany>();
            mc.baseCost = 10000;
            mc.aiCompany = aic;
            mc.revenue = 1;

            EffectSet es = new EffectSet();
            es.cash = 5000;
            mc.effects = es;

            c.cash.baseValue = 0;
            Assert.IsFalse(c.BuyCompany(mc));

            // Cost is affected by health of the economy;
            float econMult = gm.economyMultiplier;
            Assert.AreEqual(mc.cost, mc.baseCost * econMult, 0.1);

            c.cash.baseValue = 10000 * econMult;
            Assert.IsTrue(c.BuyCompany(mc));
            Assert.IsTrue(c.companies.Contains(mc));
            Assert.IsTrue(aic.disabled);
            Assert.AreEqual(c.cash.value, 5000, 0.1f);

            c.HarvestCompanies();
            Assert.AreEqual(c.cash.value, 5001, 0.1f);
        }
    }
}
