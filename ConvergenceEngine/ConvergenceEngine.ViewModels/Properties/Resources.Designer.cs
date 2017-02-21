using System.Resources;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace ConvergenceEngine.ViewModels.Properties {

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class Resources {

        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() { }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if ((resourceMan == null)) {
                    ResourceManager temp =
                        new ResourceManager("ConvergenceEngine.ViewModels.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get { return resourceCulture; }
            set { resourceCulture = value; }
        }
    }
}