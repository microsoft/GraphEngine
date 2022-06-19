using System;

namespace Trinity.VSExtension
{
    internal static class TrinityGuids
    {
        public const string GeneralPropertyPage              = "3476EA8B-2997-4A9A-AFF0-BD8DBCEBAF34";
        public const string TrinityVSExtensionPkg            = "51A02FA7-9A83-4AA0-9236-599637C597C6";
        public const string TrinityLibraryManagerService     = "38701D8F-EA5E-480A-AAAE-8F99B3AE339A";
        public const string TrinityLibraryManager            = "7B16014D-24E5-4E3D-BA59-1A56AA420B85";
        public const string TrinityProjectFactory            = "FD8A01E6-12A1-461A-AF0E-51EBE962703F";
        public const string TrinityProjectNode               = "FAA8648F-7BC3-42FA-B862-33AF238CDED6";
        public const string TrinityProjectNodeProperties     = "6C31D573-9A6A-4F9E-BC3D-93E69C62C500";
        public const string OATrinityProjectFileItem         = "C8AF9062-0382-4119-AF36-A5BB08111A5C";

        public static readonly Guid TrinityVSExtensionCmdSet = new Guid("BB09F295-92EE-41A8-AC76-AA5D59B15CAC");
    }

    internal static class ProjectTypeGuidStrings
    {
        public const string CSharp                           = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string FSharp                           = "{F2A71F9B-5D33-465A-A702-920D77279786}";
    }

    internal static class TrinityConsts
    {
    }
}