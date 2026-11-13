using NUnit.Framework;
using UnityEngine;

namespace DeadAir.Tests
{
    public sealed class RadioBatteryTests
    {
        private GameObject equipment;
        private BatteryComponent battery;
        private RadioController radio;

        [SetUp]
        public void SetUp()
        {
            equipment = new GameObject("TestEquipment");
            battery = equipment.AddComponent<BatteryComponent>();
            radio = equipment.AddComponent<RadioController>();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(equipment);

        [Test]
        public void OffPreservesChargeOnDrainsAndOffCanBeChosen()
        {
            float full = battery.Charge;
            radio.Tick(1f);
            Assert.That(battery.Charge, Is.EqualTo(full));
            radio.Toggle();
            radio.Tick(1f);
            Assert.That(battery.Charge, Is.LessThan(full));
            radio.Toggle();
            float remaining = battery.Charge;
            radio.Tick(100f);
            Assert.That(battery.Charge, Is.EqualTo(remaining));
        }

        [Test]
        public void ExhaustionImmediatelySwitchesOffAndPreventsRestart()
        {
            int notifications = 0;
            radio.StateChanged += _ => notifications++;
            radio.SetPower(true);
            radio.Tick(10000f);
            Assert.That(battery.Charge, Is.Zero);
            Assert.That(radio.IsOn, Is.False);
            radio.Toggle();
            Assert.That(radio.IsOn, Is.False);
            Assert.That(notifications, Is.EqualTo(2));
        }

        [Test]
        public void ReplacementRestoresFullChargeWithoutPoweringOn()
        {
            battery.Consume(float.PositiveInfinity);
            battery.ReplaceBattery();
            Assert.That(battery.Charge, Is.EqualTo(battery.Capacity));
            Assert.That(radio.IsOn, Is.False);
            radio.Toggle();
            Assert.That(radio.IsOn, Is.True);
        }

        [Test]
        public void InvalidConsumptionDoesNotAddChargeOrDuplicateDepletion()
        {
            int depletions = 0;
            battery.Depleted += () => depletions++;
            battery.Consume(-5);
            battery.Consume(float.NaN);
            Assert.That(battery.Charge, Is.EqualTo(battery.Capacity));
            battery.Consume(10000);
            battery.Consume(10000);
            Assert.That(depletions, Is.EqualTo(1));
        }
    }
}
