using Barotrauma;

namespace ReactorFix;

public partial class Plugin : IAssemblyPlugin
{
    public void Initialize()
    {
        Log.Info( "Reactor Fix Initialized!" );
    }


    public void OnLoadCompleted()
    {

    }

    public void PreInitPatching()
    {
        // Unused
    }

    public void Dispose()
    {
        // Clean up if necessary
    }
}