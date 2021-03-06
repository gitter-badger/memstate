namespace Memstate.Test.Models
{
    using System;
    using Memstate.Models;
    using Xunit;

    public class KeyValueStoreProxyTests
    {
        private readonly IKeyValueStore<int> _keyValueStore;

        public KeyValueStoreProxyTests()
        {
            MemstateSettings config = new MemstateSettings();
            config.StorageProvider = typeof(InMemoryStorageProvider).FullName;
            IKeyValueStore<int> model = new KeyValueStore<int>();
            var engine = new EngineBuilder(config).Build(model);
            var client = new LocalClient<IKeyValueStore<int>>(engine);
            _keyValueStore = client.GetDispatchProxy();
        }

        [Fact]
        public void Set_stores_value()
        {
            _keyValueStore.Set("KEY", 1);
            var node = _keyValueStore.Get("KEY");
            Assert.Equal(node.Value, 1);
        }

        [Fact]
        public void Set_new_key_yields_correct_version()
        {
            _keyValueStore.Set("KEY", 1);
            var node = _keyValueStore.Get("KEY");
            Assert.Equal(node.Version, 1);
        }

        [Fact]
        public void Update_bumps_version()
        {
            _keyValueStore.Set("KEY", 1);
            _keyValueStore.Set("KEY", 2);
            var node = _keyValueStore.Get("KEY");
            Assert.Equal(node.Version, 2);
        }

        [Fact]
        public void Remove_throws_when_key_not_exists()
        {
            Assert.Throws<InvalidOperationException>(() => _keyValueStore.Remove("KEY"));
        }
    }
}