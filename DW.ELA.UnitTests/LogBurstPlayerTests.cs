﻿using Controller;
using DW.ELA.Interfaces;
using DW.ELA.Utility.Json;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DW.ELA.UnitTests
{
    [TestFixture]
    public class LogBurstPlayerTests
    {
        [Test]
        public void ShouldPlayEvents()
        {
            var directoryProvider = new TestDirectoryProvider();
            Directory.Delete(directoryProvider.Directory, true);
            var events = new ConcurrentBag<LogEvent>();
            CollectionAssert.IsEmpty(events);

            string testFile1 = Path.Combine(directoryProvider.Directory, "testFile1.log");
            string testFile2 = Path.Combine(directoryProvider.Directory, "testFile2.log");

            File.WriteAllText(testFile1, EventsAsJson.Skip(5).First());
            File.WriteAllText(testFile2, EventsAsJson.Skip(5).First());

            var burstPlayer1 = new LogBurstPlayer(directoryProvider.Directory, 1);
            burstPlayer1.Subscribe(events.Add);
            burstPlayer1.Play();
            CollectionAssert.IsNotEmpty(events);
            Assert.AreEqual(1, events.Count);

            var burstPlayer2 = new LogBurstPlayer(directoryProvider.Directory, 100);
            burstPlayer2.Subscribe(events.Add);
            burstPlayer2.Play();
            CollectionAssert.IsNotEmpty(events);
            Assert.AreEqual(3, events.Count);
        }

        private static IEnumerable<string> EventsAsJson => TestEventSource.LogEvents.Select(Serialize.ToJson);

        private class TestDirectoryProvider : ILogDirectoryNameProvider
        {
            public string Directory
            {
                get
                {
                    var dir = Path.Combine(Path.GetTempPath(), "ELA-TEST1");
                    System.IO.Directory.CreateDirectory(dir);
                    return dir;
                }
            }
        }
    }

}
