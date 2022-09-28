using System;
using System.Collections.Generic;
using Ink.Parsed;

namespace Ink {
    public class PluginManager {
        private readonly List<IPlugin> _plugins;

        public PluginManager(List<string> pluginNames) {
            _plugins = new List<IPlugin>();

            // TODO: Make these plugin names DLL filenames, and load up their assemblies
            foreach (var pluginName in pluginNames)
                //if (pluginName == "ChoiceListPlugin") {
                //    _plugins.Add (new InkPlugin.ChoiceListPlugin ());
                //}else  
            {
                throw new Exception("Plugin not found");
            }
        }

        public void PostParse(Story parsedStory) {
            foreach (var plugin in _plugins) plugin.PostParse(parsedStory);
        }

        public void PostExport(Story parsedStory, Runtime.Story runtimeStory) {
            foreach (var plugin in _plugins) plugin.PostExport(parsedStory, runtimeStory);
        }
    }
}