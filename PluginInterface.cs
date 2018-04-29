using System;
using System.Collections.Generic;
using System.IO;

namespace MLLE
{
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
