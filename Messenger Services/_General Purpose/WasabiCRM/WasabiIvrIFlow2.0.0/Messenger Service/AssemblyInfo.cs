// Copyright © IFM Infomaster. All rights reserved.

#region Namespaces

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

#region Assembly Attributes

[assembly: AssemblyCompany("Base Digitale Platform S.p.A.")]
[assembly: AssemblyCopyright("Copyright © 2024-26 Base Digitale Platform S.p.A.")]

[assembly: AssemblyTitle("WasabiIvrIFlow.CService")]
[assembly: AssemblyProduct("WasabiIvrIFlow.CService")]

[assembly: AssemblyVersion("2.0.0.28")]

[assembly: AssemblyFileVersion("2.0.0.28")]
[assembly: AssemblyInformationalVersion(AssemblyInfo.ProductVersion)]

#region Build Configuration

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

#endregion

#region COM Interoperability

// Leave ComVisible(false) at assembly level.
// If you need to access a type in this assembly from COM,
// set the ComVisible attribute to true on that type:
[assembly: ComVisible(true)]
[assembly: Guid("63F1716A-D629-42C5-917A-4421E44A32F1")]

#endregion

#region Code Integration

// Framework libraries and components
// should set it to True:
[assembly: CLSCompliant(true)]
[assembly: AssemblyDescription("WasabiIvrIFlow.CService")]

#endregion

#endregion

static class AssemblyInfo {

    public const String Company         = "IFM Infomaster";
    public const String Copyright       = "© IFM Infomaster. All rights reserved.";

    public const String Title           = "WasabiIvrBot";
    public const String Product         = "Phones Messenger";

    // Assembly version. Does not appear in file properties.
    // The CLR uses these version numbers to resolve references:
    public const String Version         = "2.0.2.0";

    // File version for Properties and Setup programs.
    public const String FileVersion     = Version;

    // Appear as 'Product Version' in Properties dialog.
    public const String ProductVersion  = Version;

}
