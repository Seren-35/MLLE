using System;
using System.Drawing;

namespace MLLE
{
    public class ToolContext
    {
        public Image Image { get; set; }
        public EventHandler OnSelect { get; set; }
        public EventHandler OnUse { get; set; }
    }

    public class PluginSerializedData : EventArgs
    {
        public uint Identifier { get; set; }
        public uint Version { get; set; }
        public byte[] Content { get; set; }
    }

    public interface IPluginHost
    {
        event EventHandler<PluginSerializedData> OnLevelSave;
        event EventHandler<PluginSerializedData> OnLevelLoad;

        void AddTool(ToolContext context);
    }

    public interface IPlugin
    {
        void Initialize(IPluginHost host);
        void Dispose(IPluginHost host);
    }

    public interface IPluginMetadata
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
    }
}
