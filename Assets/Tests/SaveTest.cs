using System.IO;
using Core;
using NUnit.Framework;
using Zenject;

namespace Tests
{
    public class SaveTest
    {
        private DiContainer _container;
        private string _testFilePath;

        [SetUp]
        public void SetUp()
        {
            _container = new DiContainer();

            _container.Bind<ISaveSystem>()
                .To<SaveSystem>()
                .AsSingle();

            _container.Bind<IJsonSerializer>()
                .To<JsonSerializer>()
                .AsSingle();

            _container.Bind<ILogger>()
                .To<Logger>()
                .AsSingle();
        }

        [Test]
        public void SaveData_WorkingCorrectly()
        {
            var saveSystem = _container.Resolve<ISaveSystem>();

            var playerStats = new PlayerStats()
            {
                Health = 100f
            };

            var gd = new GameData
            {
                GDName = "FileTest",
                PlayerStats = playerStats
            };

            _testFilePath = saveSystem.GetPath(gd.GDName);

            saveSystem.Save(gd);

            Assert.That(File.Exists(_testFilePath), $"File not found at path: {_testFilePath}");

            var contents = File.ReadAllText(_testFilePath);
            Assert.IsTrue(contents.Contains("100"), "File content does not contain expected health value");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }
}