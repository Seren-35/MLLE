using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;

namespace MLLE
{
    public class PluginHost : IPluginHost, IDisposable
    {
        [ImportMany]
        private IEnumerable<IPlugin> plugins;

        public delegate void AddToolDelegate(ToolStripItem item, bool usable);
        public AddToolDelegate AddToolCallback { get; set; }

        public event EventHandler<PluginSerializedData> OnLevelSave;
        public event EventHandler<PluginSerializedData> OnLevelLoad;

        public bool LoadAll()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PluginHost).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog("plugins"));
            var container = new CompositionContainer(catalog);
            try
            {
                container.ComposeParts(this);
            }
            catch
            {
                return false;
            }
            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.Initialize(this);
                }
                catch (Exception exception)
                {
                    try
                    {
                        plugin.Dispose(this);
                    }
                    catch { }
                    // TODO: Inform user of plugin failure
                }
            }
            return true;
        }

        public IEnumerable<PluginSerializedData> LevelSave()
        {
            var result = new List<PluginSerializedData>();
            if (OnLevelSave != null) {
                foreach (EventHandler<PluginSerializedData> subscription in OnLevelSave.GetInvocationList())
                {
                    try
                    {
                        var args = new PluginSerializedData();
                        subscription(this, args);
                        if (args.Identifier != 0 && args.Content != null)
                        {
                            result.Add(args);
                        }
                    }
                    catch (Exception exception)
                    {
                        // TODO: Inform user of plugin failure
                    }
                }
            }
            return result;
        }
        
        public void LevelLoad(PluginSerializedData data)
        {
            // TODO: Handle the situation when no plugin understands the data (modify to dispatch?)
            if (OnLevelLoad != null)
            {
                foreach (EventHandler<PluginSerializedData> subscription in OnLevelLoad.GetInvocationList())
                {
                    try
                    {
                        subscription(this, data);
                    }
                    catch (Exception exception)
                    {
                        // TODO: Inform user of plugin failure
                    }
                }
            }
        }

        public void Dispose()
        {
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    try
                    {
                        plugin.Dispose(this);
                    }
                    catch { }
                }
                plugins = null;
            }
        }

        private EventHandler Wrap(EventHandler callback)
        {
            if (callback == null)
                return null;
            return delegate (object sender, EventArgs e) {
                try
                {
                    callback(sender, e);
                }
                catch (Exception exception)
                {
                    // TODO: Inform user of plugin failure
                }
            };
        }

        public void AddTool(ToolContext context)
        {
            var item = new ToolStripButton(null, context.Image, Wrap(context.OnSelect));
            AddToolCallback(item, context.OnUse != null);
        }
    }
}
