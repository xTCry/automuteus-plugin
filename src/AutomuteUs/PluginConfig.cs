using Config.Net;

namespace Impostor.Plugins.AutomuteUs
{
    static class PluginConfig {
        public static IAutomuteUsConfig config = new ConfigurationBuilder<IAutomuteUsConfig>().UseJsonFile("./automuteus.json").Build();
    }

    public interface IAutomuteUsConfig
    {
        [Option(Alias = "Host", DefaultValue = "http://localhost:8123")]
        public string Host { get; }

        /**
		 * TODO: change ConnectCode to SecretKey
		 */
        [Option(Alias = "ConnectCode", DefaultValue = "")]
        public string ConnectCode { get; }
	}
}
