namespace Mandarin.PluginSystem {
    public interface IPlugin {

        // Register stuff, make stuff ready for other plugins
        void Init(Messenger msg);

        // All plugins are ready
        void Ready(Messenger msg);
    }
}
