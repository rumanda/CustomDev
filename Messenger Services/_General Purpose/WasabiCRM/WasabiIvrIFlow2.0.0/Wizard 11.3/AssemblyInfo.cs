
// Copyright © 2024 Base Digitale Platform S.p.A.

#region Namespaces

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

#region Assembly Attributes

[assembly: AssemblyCompany("Base Digitale Platform S.p.A.")]
[assembly: AssemblyCopyright("© Base Digitale Platform 2024-2026")]

[assembly: AssemblyTitle("WasabiIvrIFlowWizard")]
[assembly: AssemblyProduct("WasabiIvrIFlowWizard")]

[assembly: AssemblyVersion("11.3.3.1")]

[assembly: AssemblyFileVersion("11.3.3.1")]
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
[assembly: ComVisible(false)]
[assembly: Guid("BFA811CB-8734-4312-9BDC-33F32479CE16")]

#endregion

#region Code Integration

// Framework libraries and components should set it to True:
[assembly: CLSCompliant(true)]
[assembly: AssemblyDescription("Wizard Blocchetto IvrBot Wasabi")]
[assembly: AssemblyTrademark("Base Digitale Platform  S.p.A.")]

#endregion

#endregion

static class AssemblyInfo {

    public const String Company         = "Base Digitale Platform S.p.A.";
    public const String Copyright       = "Copyright © 2024 Base Digitale Platform S.p.A.";

    public const String Title           = "Ivr IFlow Wasabi Wizard";
    public const String Product         = "Phones Messenger";

    // Assembly version. Does not appear in file properties.
    // The CLR uses these version numbers to resolve references:
    public const String Version         = "11.3.0.0";

    // File version for Properties and Setup programs.
    public const String FileVersion     = Version;

    // Appear as 'Product Version' in Properties dialog.
    public const String ProductVersion  = Version;

}
